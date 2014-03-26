using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Web;
using System.Web.Helpers;
using System.Web.Security;
using AaltoGlobalImpact.OIP;
using AzureSupport;
using DotNetOpenAuth.OpenId.RelyingParty;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;
using TheBall.CORE;

namespace WebInterface
{
    public class AuthorizedBlobStorageHandler : IHttpHandler
    {
        /// <summary>
        /// You will need to configure this handler in the web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        private const string AuthPersonalPrefix = "/auth/account/";
        private const string AuthGroupPrefix = "/auth/grp/";
        private const string AuthAccountPrefix = "/auth/acc/";
        private const string AuthPrefix = "/auth/";
        private const string AboutPrefix = "/about/";
        private int AuthGroupPrefixLen;
        private int AuthPersonalPrefixLen;
        private int AuthAccountPrefixLen;
        private int AuthProcPrefixLen;
        private int AuthPrefixLen;
        private int GuidIDLen;


        public AuthorizedBlobStorageHandler()
        {
            AuthGroupPrefixLen = AuthGroupPrefix.Length;
            AuthPersonalPrefixLen = AuthPersonalPrefix.Length;
            AuthAccountPrefixLen = AuthAccountPrefix.Length;
            AuthPrefixLen = AuthPrefix.Length;
            GuidIDLen = Guid.Empty.ToString().Length;
        }

        public void ProcessRequest(HttpContext context)
        {
            string user = context.User.Identity.Name;
            var userIdentity = context.User;
            bool isAuthenticated = String.IsNullOrEmpty(user) == false;
            var request = context.Request;
            var response = context.Response;
            if (request.Path.StartsWith(AboutPrefix))
            {
                HandleAboutGetRequest(context, request.Path);
                return;
            }

            if (isAuthenticated == false)
            {
                return;
            }
            try
            {
                if (userIdentity.IsInRole("DeviceAES"))
                {
                    HandleEncryptedDeviceRequest(context);
                }
                else if (request.Path.StartsWith(AuthPersonalPrefix))
                {
                    HandlePersonalRequest(context);
                }
                else if (request.Path.StartsWith(AuthGroupPrefix))
                {
                    HandleGroupRequest(context);
                }
                else if (request.Path.StartsWith(AuthAccountPrefix))
                {
                    HandleAccountRequest(context);
                }
            }
            catch (SecurityException securityException)
            {
                response.StatusCode = 403;
                response.Write(securityException.ToString());
            }
        }

        private void HandleEncryptedDeviceRequest(HttpContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var authorization = request.Headers["Authorization"];
            var authTokens = authorization.Split(':');
            IContainerOwner owner = null;
            if (request.Path.StartsWith("/auth/grp/"))
            {
                string groupID = GetGroupID(context.Request.Path);
                owner = new VirtualOwner("grp", groupID);
            }
            else
                throw new NotSupportedException("Account device requests not yet supported");
            string ivStr = authTokens[1];
            string trustID = authTokens[2];
            string contentName = authTokens[3];
            DeviceMembership deviceMembership = DeviceMembership.RetrieveFromOwnerContent(owner, trustID);
            if(deviceMembership == null)
                throw new InvalidDataException("Device membership not found");
            if(deviceMembership.IsValidatedAndActive == false)
                throw new SecurityException("Device membership not valid and active");
            InformationContext.Current.Owner = owner;
            InformationContext.Current.ExecutingForDevice = deviceMembership;
            if (request.RequestType == "GET")
            {
                string contentPath = request.Path.Substring(AuthGroupPrefixLen + GuidIDLen + 1);
                if(contentPath.Contains("TheBall.CORE"))
                    throw new SecurityException("Invalid request location");
                var blob = StorageSupport.GetOwnerBlobReference(owner, contentPath);

                AesManaged aes = new AesManaged();
                aes.KeySize = 256;
                aes.GenerateIV();
                aes.Key = deviceMembership.ActiveSymmetricAESKey;
                var ivBase64 = Convert.ToBase64String(aes.IV);
                response.Headers.Add("IV", ivBase64);
                var responseStream = response.OutputStream;
                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                var cryptoStream = new CryptoStream(responseStream, encryptor, CryptoStreamMode.Write);
                blob.DownloadToStream(cryptoStream);
                cryptoStream.Close();
            } else if (request.RequestType == "POST")
            {
                if (contentName.StartsWith(DeviceSupport.OperationPrefixStr))
                {
                    string operationName = contentName.Substring(DeviceSupport.OperationPrefixStr.Length);
                    var reqStream = request.GetBufferedInputStream();
                    AesManaged decAES = new AesManaged
                        {
                            KeySize = 256,
                            IV = Convert.FromBase64String(ivStr),
                            Key = deviceMembership.ActiveSymmetricAESKey
                        };
                    var reqDecryptor = decAES.CreateDecryptor(decAES.Key, decAES.IV);

                    AesManaged encAES = new AesManaged
                        {
                            KeySize = 256, 
                            Key = deviceMembership.ActiveSymmetricAESKey
                        };
                    encAES.GenerateIV();
                    var respivBase64 = Convert.ToBase64String(encAES.IV);
                    response.Headers.Add("IV", respivBase64);
                    var responseStream = response.OutputStream;
                    var respEncryptor = encAES.CreateEncryptor(encAES.Key, encAES.IV);


                    using (CryptoStream reqDecryptStream = new CryptoStream(reqStream, reqDecryptor, CryptoStreamMode.Read),
                            respEncryptedStream = new CryptoStream(responseStream, respEncryptor, CryptoStreamMode.Write))
                    {
                        OperationSupport.ExecuteOperation(operationName, 
                            new Tuple<string, object>("InputStream", reqDecryptStream),
                            new Tuple<string, object>("OutputStream", respEncryptedStream));
                    }
                }
                else
                {
                    string contentRoot = deviceMembership.RelativeLocation + "_Input";
                    string blobName = contentRoot + "/" + contentName;
                    var blob = StorageSupport.GetOwnerBlobReference(owner, blobName);
                    if (blob.Name != blobName)
                        throw new InvalidDataException("Invalid content name");
                    var reqStream = request.GetBufferedInputStream();
                    AesManaged aes = new AesManaged();
                    aes.KeySize = 256;
                    aes.IV = Convert.FromBase64String(ivStr);
                    aes.Key = deviceMembership.ActiveSymmetricAESKey;
                    var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    CryptoStream cryptoStream = new CryptoStream(reqStream, decryptor, CryptoStreamMode.Read);
                    blob.UploadFromStream(cryptoStream);
                }
                response.StatusCode = 200;
                response.End();
            }
            else
                throw new NotSupportedException("Device request type not supported: " + request.RequestType);

        }

        private static TBRegisterContainer GetRegistrationInfo(string returnUrl)
        {
            TBRegisterContainer registerContainer = TBRegisterContainer.CreateWithLoginProviders(returnUrl, title: "Sign in", subtitle: "... or register", absoluteLoginUrl:null);
            return registerContainer;
        }

        private static string GetBlobPath(HttpRequest request)
        {
            string contentPath = request.Path;
            if (contentPath.StartsWith(AboutPrefix) == false)
                throw new NotSupportedException("Content path for other than about/ is not supported");
            return contentPath.Substring(1);
        }

        private void HandleAccountRequest(HttpContext context)
        {
            //TBRLoginRoot loginRoot = GetOrCreateLoginRoot(context);
        }

        private void HandleGroupRequest(HttpContext context)
        {
            string requestPath = context.Request.Path;
            string groupID = GetGroupID(context.Request.Path);
            string loginUrl = WebSupport.GetLoginUrl(context);
            string loginRootID = TBLoginInfo.GetLoginIDFromLoginURL(loginUrl);
            string loginGroupID = TBRLoginGroupRoot.GetLoginGroupID(groupID, loginRootID);
            TBRLoginGroupRoot loginGroupRoot = TBRLoginGroupRoot.RetrieveFromDefaultLocation(loginGroupID);
            if(loginGroupRoot == null)
            {
                // TODO: Polite invitation request
                throw new SecurityException("No access to requested group: TODO - Polite landing page for the group");
                return;
            }
            InformationContext.Current.CurrentGroupRole = loginGroupRoot.Role;
            var contentPath = GetGroupContentPath(requestPath);
            HandleOwnerRequest(loginGroupRoot, context, contentPath, loginGroupRoot.Role);
        }

        private string GetGroupContentPath(string requestPath)
        {
            string contentPath = requestPath.Substring(AuthGroupPrefixLen + GuidIDLen + 1);
            return contentPath;
        }

        private string GetGroupID(string path)
        {
            return path.Substring(AuthGroupPrefixLen, GuidIDLen);
        }

        private void HandlePersonalRequest(HttpContext context)
        {
            string loginUrl = WebSupport.GetLoginUrl(context);
            TBRLoginRoot loginRoot = TBRLoginRoot.GetOrCreateLoginRootWithAccount(loginUrl, true);
            bool doDelete = false;
            if(doDelete)
            {
                loginRoot.DeleteInformationObject();
                return;
            }
            TBAccount account = loginRoot.Account;
            string requestPath = context.Request.Path;
            var contentPath = GetAccountContentPath(requestPath);
            HandleOwnerRequest(account, context, contentPath, TBCollaboratorRole.CollaboratorRoleValue);
        }

        private string GetAccountContentPath(string requestPath)
        {
            string contentPath = requestPath.Substring(AuthPersonalPrefixLen);
            return contentPath;
        }

        private void HandleOwnerRequest(IContainerOwner containerOwner, HttpContext context, string contentPath, string role)
        {
            InformationContext.Current.Owner = containerOwner;
            if (context.Request.RequestType == "POST")
            {
                // Do first post, and then get to the same URL
                if (TBCollaboratorRole.HasCollaboratorRights(role) == false)
                    throw new SecurityException("Role '" + role + "' is not authorized to do changing POST requests to web interface");
                bool redirectAfter = HandleOwnerPostRequest(containerOwner, context, contentPath);
                if(redirectAfter)
                    context.Response.Redirect(context.Request.Url.ToString(), true);
                return;
            }
            HandleOwnerGetRequest(containerOwner, context, contentPath);
        }

        private bool HandleOwnerPostRequest(IContainerOwner containerOwner, HttpContext context, string contentPath)
        {
            validateThatOwnerPostComesFromSameReferrer(context);
            HttpRequest request = context.Request;
            var form = request.Unvalidated().Form;

            bool isAjaxDataRequest = request.ContentType.StartsWith("application/json"); // form.Get("AjaxObjectInfo") != null;
            if (isAjaxDataRequest)
            {
                // Various data deserialization tests - options need to be properly set
                // strong type radically faster 151ms over 25sec with flexible type - something ill
                //throw new NotSupportedException("Not supported as-is, implementation for serialization available, not finished");
//                var stream = request.GetBufferedInputStream();
  //              var dataX = JSONSupport.GetObjectFromStream<List<ParentToChildren> >(stream);
//                var streamReader = new StreamReader(request.GetBufferedInputStream());
//                string data = streamReader.ReadToEnd();
//                var jsonData = JSONSupport.GetJsonFromStream(data);
//                HandlerOwnerAjaxDataPOST(containerOwner, request);
                //SetCategoryHierarchyAndOrderInNodeSummary.Execute();
                string operationName = request.Params["operation"];
                
                Type operationType = TypeSupport.GetTypeByName(operationName);
                var method = operationType.GetMethod("Execute", BindingFlags.Public | BindingFlags.Static);
                method.Invoke(null, null);
                context.Response.End();
                return false;
            }

            bool isClientTemplateRequest = form.Get("ContentSourceInfo") != null ||
                form.Get("ExecuteOperation") != null || form.Get("ExecuteAdminOperation") != null;
            if(isClientTemplateRequest)
            {
                HandleOwnerClientTemplatePOST(containerOwner, request);
                return true;
            }

            throw new NotSupportedException("Old legacy update no longer supported");

#if superseded
            string sourceNamesCommaSeparated = form["RootSourceName"];
            bool isCancelButton = form["btnCancel"] != null;
            if (isCancelButton)
                return true;
            string actionName = form["RootSourceAction"];
            string objectFieldID = form["ObjectFieldID"];

            CloudBlob webPageBlob = StorageSupport.CurrActiveContainer.GetBlob(contentPath, containerOwner);
            InformationSourceCollection sources = webPageBlob.GetBlobInformationSources();
            if (sources == null)
                throw new InvalidDataException("Postback to page with no information sources defined - where there should be");
            if (sourceNamesCommaSeparated == null)
                sourceNamesCommaSeparated = "";
            sourceNamesCommaSeparated += ",";
            string[] sourceNames = sourceNamesCommaSeparated.Split(',').Distinct().ToArray();
            
            if(objectFieldID != null && actionName.StartsWith("cmd") == false && actionName != "Save" && actionName.Contains(",") == false)
            {
                var result = PerformWebAction.Execute(new PerformWebActionParameters
                                                          {
                                                              CommandName = actionName,
                                                              FormSourceNames = sourceNames,
                                                              FormSubmitContent = form,
                                                              InformationSources = sources,
                                                              Owner = containerOwner,
                                                              TargetObjectID = objectFieldID
                                                          });
                if(result.RenderPageAfterOperation)
                    RenderWebSupport.RefreshPHTMLContent(webPageBlob);
                return true;
            }

            string inContextEditFieldID = form["InContextEditFieldID"];

            if (inContextEditFieldID != null)
            {
                string objectFieldValue = form["Text_Short"];
                if (objectFieldValue == null)
                    objectFieldValue = form["Text_Long"];
                form = new NameValueCollection();
                form.Set(inContextEditFieldID, objectFieldValue);
            }

            InformationSource[] sourceArray =
                sources.CollectionContent.Where(
                    src => src.IsDynamic || (src.IsInformationObjectSource && sourceNames.Contains(src.SourceName)) ).ToArray();
            foreach (InformationSource source in sourceArray)
            {
                string oldETag = source.SourceETag;
                IInformationObject rootObject = source.RetrieveInformationObject();
                /* Temporarily removed all the version checks - last save wins! 
                if (oldETag != rootObject.ETag)
                {
                    RenderWebSupport.RefreshPHTMLContent(webPageBlob);
                    throw new InvalidDataException("Information under editing was modified during display and save");
                }
                 * */
                // TODO: Proprely validate against only the object under the editing was changed (or its tree below)
                rootObject.SetValuesToObjects(form);
                IAddOperationProvider addOperationProvider = rootObject as IAddOperationProvider;
                bool skipNormalStoreAfterAdd = false;
                if(addOperationProvider != null)
                {
                    skipNormalStoreAfterAdd = addOperationProvider.PerformAddOperation(actionName, sources, contentPath, request.Files);
                }
                if(skipNormalStoreAfterAdd == false) {
                    // If not add operation, set media content to stored object
                    foreach (string contentID in request.Files.AllKeys)
                    {
                        HttpPostedFile postedFile = request.Files[contentID];
                        if (String.IsNullOrWhiteSpace(postedFile.FileName))
                            continue;
                        rootObject.SetMediaContent(containerOwner, contentID, postedFile);
                    }
                    rootObject.StoreInformationMasterFirst(containerOwner, false);
                }
            }
            RenderWebSupport.RefreshPHTMLContent(webPageBlob);
            // Temporary live to pub sync below, to be removed
            //SyncTemplatesToSite(StorageSupport.CurrActiveContainer.Name,
            //    String.Format("grp/f8e1d8c6-0000-467e-b487-74be4ad099cd/{0}/", "livesite"),
            //    StorageSupport.CurrAnonPublicContainer.Name,
            //                    String.Format("grp/default/{0}/", "livepubsite"), true);

            return true;
#endif
        }

        private void validateThatOwnerPostComesFromSameReferrer(HttpContext context)
        {
            var request = context.Request;
            var requestUrl = request.Url;
            var referrerUrl = request.UrlReferrer;
            if(referrerUrl == null || requestUrl.AbsolutePath != referrerUrl.AbsolutePath)
                throw new SecurityException("UrlReferrer mismatch or missing - potential cause is (un)intentionally malicious web template.");
        }

        private void HandleOwnerClientTemplatePOST(IContainerOwner containerOwner, HttpRequest request)
        {
            var form = request.Form;
            ModifyInformationSupport.ExecuteOwnerWebPOST(containerOwner, form, request.Files);

        }

        private void HandlerOwnerAjaxDataPOST(IContainerOwner containerOwner, NameValueCollection form)
        {
            throw new NotImplementedException();
        }

        private void HandleAboutGetRequest(HttpContext context, string contentPath)
        {
            var response = context.Response;
            var request = context.Request;
            string blobPath = GetBlobPath(request);
            CloudBlob blob = StorageSupport.CurrActiveContainer.GetBlobReference(blobPath);  //publicClient.GetBlobReference(blobPath);
            response.Clear();
            try
            {
                HandleBlobRequestWithCacheSupport(context, blob, response);
            }
            catch (StorageClientException scEx)
            {
                if (scEx.ErrorCode == StorageErrorCode.BlobNotFound || scEx.ErrorCode == StorageErrorCode.ResourceNotFound || scEx.ErrorCode == StorageErrorCode.BadRequest)
                {
                    response.Write("Blob not found or bad request: " + blob.Name + " (original path: " + request.Path + ")");
                    response.StatusCode = (int)scEx.StatusCode;
                }
                else
                {
                    response.Write("Errorcode: " + scEx.ErrorCode.ToString() + Environment.NewLine);
                    response.Write(scEx.ToString());
                    response.StatusCode = (int)scEx.StatusCode;
                }
            }
            finally
            {
                response.End();
            }

        }

        private void HandleOwnerGetRequest(IContainerOwner containerOwner, HttpContext context, string contentPath)
        {
            if(String.IsNullOrEmpty(contentPath) == false && contentPath.EndsWith("/") == false)
                validateThatOwnerGetComesFromSameReferer(containerOwner, context.Request, contentPath);
            if ((context.Request.Url.Host == "localhost" || context.Request.Url.Host == "localdev") && 
                (contentPath.Contains("groupmanagement/") || 
                contentPath.Contains("wwwsite/") || 
                contentPath.Contains("webview/") ||
                (contentPath.Contains("webui/") && containerOwner is TBAccount) ||
                (contentPath.Contains("foundation-one/") && containerOwner is TBAccount) ||
                contentPath.Contains("categoriesandcontent/")))
            {
                HandleFileSystemGetRequest(containerOwner, context, contentPath);
                return;
            }
            if (String.IsNullOrEmpty(contentPath) || contentPath.EndsWith("/"))
            {
                CloudBlob redirectBlob = StorageSupport.GetOwnerBlobReference(containerOwner, contentPath +
                                                                      InstanceConfiguration.RedirectFromFolderFileName);
                string redirectToUrl = null;
                try
                {
                    redirectToUrl = redirectBlob.DownloadText();
                }
                catch
                {
                    
                }
                if (redirectToUrl == null)
                {
                    if (containerOwner.IsAccountContainer())
                        redirectToUrl = InstanceConfiguration.AccountDefaultRedirect;
                    else
                        redirectToUrl = InstanceConfiguration.GroupDefaultRedirect;
                }
                context.Response.Redirect(redirectToUrl, true);
                return;
            }
            CloudBlob blob = StorageSupport.GetOwnerBlobReference(containerOwner, contentPath);
            var response = context.Response;
            // Read blob content to response.
            response.Clear();
            try
            {
                HandleBlobRequestWithCacheSupport(context, blob, response);
            } catch(StorageClientException scEx)
            {
                if(scEx.ErrorCode == StorageErrorCode.BlobNotFound || scEx.ErrorCode == StorageErrorCode.ResourceNotFound || scEx.ErrorCode == StorageErrorCode.BadRequest)
                {
                    response.Write("Blob not found or bad request: " + blob.Name + " (original path: " + context.Request.Path + ")");
                    response.StatusCode = (int)scEx.StatusCode;
                } else
                {
                    response.Write("Error code: " + scEx.ErrorCode.ToString() + Environment.NewLine);
                    response.Write(scEx.ToString());
                    response.StatusCode = (int)scEx.StatusCode;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                string errMsg = ex.ToString();
                response.Write(errMsg);
            }
            response.End();
        }

        private static void HandleBlobRequestWithCacheSupport(HttpContext context, CloudBlob blob, HttpResponse response)
        {
            // Set the cache request properties as IIS will include them regardless for now
            // even when we wouldn't want them on 304 response...
            response.Cache.SetMaxAge(TimeSpan.FromMinutes(0));
            response.Cache.SetCacheability(HttpCacheability.Private);
            var request = context.Request;
            blob.FetchAttributes();
            string ifNoneMatch = request.Headers["If-None-Match"];
            string ifModifiedSince = request.Headers["If-Modified-Since"];
            if (ifNoneMatch != null)
            {
                if (ifNoneMatch == blob.Properties.ETag)
                {
                    response.ClearContent();
                    response.StatusCode = 304;
                }
            }
            else if (ifModifiedSince != null)
            {
                DateTime ifModifiedSinceValue;
                if (DateTime.TryParse(ifModifiedSince, out ifModifiedSinceValue))
                {
                    ifModifiedSinceValue = ifModifiedSinceValue.ToUniversalTime();
                    if (blob.Properties.LastModifiedUtc <= ifModifiedSinceValue)
                    {
                        response.ClearContent();
                        response.StatusCode = 304;
                    }
                }
            }
            else
            {
                response.ContentType = StorageSupport.GetMimeType(blob.Name);
                //response.Cache.SetETag(blob.Properties.ETag);
                response.Headers.Add("ETag", blob.Properties.ETag);
                response.Cache.SetLastModified(blob.Properties.LastModifiedUtc);
                blob.DownloadToStream(response.OutputStream);
            }
        }

        private void validateThatOwnerGetComesFromSameReferer(IContainerOwner containerOwner, HttpRequest request, string contentPath)
        {
            bool isGroupRequest = containerOwner.IsGroupContainer();
            string requestGroupID = isGroupRequest ? containerOwner.LocationPrefix : null;
            bool isAccountRequest = !isGroupRequest;
            var urlReferrer = request.UrlReferrer;
            string[] groupTemplates = InstanceConfiguration.DefaultGroupTemplateList;
            string[] accountTemplates = InstanceConfiguration.DefaultAccountTemplateList;
            var refererPath = urlReferrer != null ? urlReferrer.AbsolutePath : "";
            bool refererIsAccount = refererPath.StartsWith("/auth/account/");
            bool refererIsGroup = refererPath.StartsWith("/auth/grp/");

            if (isGroupRequest)
            {
                bool defaultMatch = groupTemplates.Any(contentPath.StartsWith);
                if (defaultMatch && (refererIsAccount == false && refererIsGroup == false))
                    return;
            }
            else
            {
                bool defaultMatch = accountTemplates.Any(contentPath.StartsWith);
                if (defaultMatch && (refererIsAccount == false && refererIsGroup == false))
                    return;
            }
            if (urlReferrer == null)
            {
                if (contentPath.StartsWith("customui_") || contentPath.StartsWith("foundation-one"))
                    return;
                throw new SecurityException("Url referer required for non-default template requests, that target other than customui_ folder");
            }
            if (refererIsAccount && isAccountRequest)
                return;
            if (refererPath.StartsWith("/about/"))
                return;
            string refererOwnerPath = refererIsAccount
                                          ? GetAccountContentPath(refererPath)
                                          : GetGroupContentPath(refererPath);
            // Accept account and group referers of default templates
            if (refererIsAccount && accountTemplates.Any(refererOwnerPath.StartsWith))
                return;
            if (refererIsGroup && groupTemplates.Any(refererOwnerPath.StartsWith))
                return;
            // Custom referers
            if (refererIsAccount)
            {
                throw new SecurityException("Non-default account referer accessing non-account content");
            } 
            else // Referer is group
            {
                if(isAccountRequest)
                    throw new SecurityException("Non-default group referer accessing account content");
                string refererGroupID = GetGroupID(refererPath);
                if(refererGroupID != requestGroupID)
                    throw new SecurityException("Non-default group referer accessing other group content");
            }
        }

        private void HandleFileSystemGetRequest(IContainerOwner containerOwner, HttpContext context, string contentPath)
        {
            var response = context.Response;
            string contentType = StorageSupport.GetMimeType(Path.GetExtension(contentPath));
            response.ContentType = contentType;
            string prefixStrippedContent = contentPath; //.Substring(AuthGroupPrefixLen + GuidIDLen + 1);
            string LocalWebRootFolder = @"C:\Users\kalle\WebstormProjects\OIPTemplates\UI\groupmanagement\";
            string LocalWebCatConFolder = @"C:\Users\kalle\WebstormProjects\OIPTemplates\UI\categoriesandcontent\";
            //string LocalWwwSiteFolder = @"C:\Users\kalle\WebstormProjects\CustomerWww\EarthhouseWww\UI\earthhouse\";
            //string LocalWwwSiteFolder = @"C:\Users\kalle\WebstormProjects\CustomerWww\FOIPWww\UI\foip\";
            string LocalWwwSiteFolder = @"C:\Users\kalle\WebstormProjects\OIPTemplates\UI\webpresence_welearnit\"; 
            string LocalSchoolsAccountFolder = @"C:\Users\kalle\WebstormProjects\CaloomSchools\UI\account\";
            string LocalFoundationOneAccountFolder = @"C:\Users\kalle\WebstormProjects\OIPTemplates\UI\foundation-one\";
            string fileName;
            if (prefixStrippedContent.Contains("groupmanagement/"))
                fileName = prefixStrippedContent.Replace("groupmanagement/", LocalWebRootFolder);
            else if (prefixStrippedContent.Contains("webui/"))
                fileName = prefixStrippedContent.Replace("webui/", LocalSchoolsAccountFolder);
            else if (prefixStrippedContent.Contains("foundation-one/"))
                fileName = prefixStrippedContent.Replace("foundation-one/", LocalFoundationOneAccountFolder);
            else if (prefixStrippedContent.Contains("categoriesandcontent/"))
                fileName = prefixStrippedContent.Replace("categoriesandcontent/", LocalWebCatConFolder);
            else if (prefixStrippedContent.Contains("wwwsite/"))
                fileName = prefixStrippedContent.Replace("wwwsite/", LocalWwwSiteFolder);
            else
                fileName = prefixStrippedContent.Replace("webview/", LocalWwwSiteFolder);
            if (File.Exists(fileName))
            {
                var lastModified = File.GetLastWriteTimeUtc(fileName);
                lastModified = lastModified.AddTicks(-(lastModified.Ticks % TimeSpan.TicksPerSecond)); ;
                //response.Headers.Add("ETag", blob.Properties.ETag);
                //response.Headers.Add("Last-Modified", blob.Properties.LastModifiedUtc.ToString("R"));
                string ifModifiedSince = context.Request.Headers["If-Modified-Since"];
                if (ifModifiedSince != null)
                {
                    DateTime ifModifiedSinceValue;
                    if (DateTime.TryParse(ifModifiedSince, out ifModifiedSinceValue))
                    {
                        ifModifiedSinceValue = ifModifiedSinceValue.ToUniversalTime();
                        if (lastModified <= ifModifiedSinceValue)
                        {
                            response.ClearContent();
                            response.ClearHeaders();
                            response.StatusCode = 304;
                            return;
                        }
                    }
                }
                response.Cache.SetMaxAge(TimeSpan.FromMinutes(0));
                response.Cache.SetLastModified(lastModified);
                response.Cache.SetCacheability(HttpCacheability.Private);
                var fileStream = File.OpenRead(fileName);
                fileStream.CopyTo(context.Response.OutputStream);
                fileStream.Close();
            }
            else
            {
                response.StatusCode = 404;
            }
            response.End();

        }

        #endregion
    }

}
