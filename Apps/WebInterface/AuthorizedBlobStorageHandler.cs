using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
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
            bool isAuthenticated = String.IsNullOrEmpty(user) == false;
            var request = context.Request;
            var response = context.Response;
            WebSupport.InitializeContextStorage(context.Request);
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
                if (request.Path.StartsWith(AuthPersonalPrefix))
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
                
            } finally
            {
                InformationContext.ProcessAndClearCurrent();
            }
        }

        private static void ProcessDynamicRegisterRequest(HttpRequest request, HttpResponse response)
        {
            string blobPath = GetBlobPath(request);
            CloudBlob blob = StorageSupport.CurrActiveContainer.GetBlobReference(blobPath);
            response.Clear();
            try
            {
                string template = blob.DownloadText();
                string returnUrl = request.Params["ReturnUrl"];
                TBRegisterContainer registerContainer = GetRegistrationInfo(returnUrl);
                string responseContent = RenderWebSupport.RenderTemplateWithContent(template, registerContainer);
                response.ContentType = blob.Properties.ContentType;
                response.Write(responseContent);
            }
            catch (StorageClientException scEx)
            {
                response.Write(scEx.ToString());
                response.StatusCode = (int)scEx.StatusCode;
            }
            finally
            {
                response.End();
            }
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
            string contentPath = requestPath.Substring(AuthGroupPrefixLen + GuidIDLen + 1);
            HandleOwnerRequest(loginGroupRoot, context, contentPath, loginGroupRoot.Role);
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
            string contentPath = requestPath.Substring(AuthPersonalPrefixLen);
            HandleOwnerRequest(account, context, contentPath, TBCollaboratorRole.CollaboratorRoleValue);
        }

        private void HandleOwnerRequest(IContainerOwner containerOwner, HttpContext context, string contentPath, string role)
        {
            InformationContext.Current.Owner = containerOwner;
            if (context.Request.RequestType == "POST")
            {
                // Do first post, and then get to the same URL
                if (TBCollaboratorRole.HasCollaboratorRights(role) == false)
                    throw new SecurityException("Role '" + role + "' is not authorized to do changing POST requests to web interface");
                HandleOwnerPostRequest(containerOwner, context, contentPath);
                context.Response.Redirect(context.Request.Url.ToString(), true);
                return;
            }
            HandleOwnerGetRequest(containerOwner, context, contentPath);
        }

        private void HandleOwnerPostRequest(IContainerOwner containerOwner, HttpContext context, string contentPath)
        {
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
                //SetCategoryHierarchy.Execute();
                string operationName = request.Params["operation"];
                
                Type operationType = TypeSupport.GetTypeByName(operationName);
                var method = operationType.GetMethod("Execute", BindingFlags.Public | BindingFlags.Static);
                method.Invoke(null, null);
                return;
            }

            bool isClientTemplateRequest = form.Get("ContentSourceInfo") != null ||
                form.Get("ExecuteOperation") != null;
            if(isClientTemplateRequest)
            {
                HandleOwnerClientTemplatePOST(containerOwner, request);
                return;
            }

            string sourceNamesCommaSeparated = form["RootSourceName"];
            bool isCancelButton = form["btnCancel"] != null;
            if (isCancelButton)
                return;
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
                return;
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
                blob.FetchAttributes();
                response.ContentType = blob.Properties.ContentType;
                response.Headers.Add("ETag", blob.Properties.ETag);
                blob.DownloadToStream(response.OutputStream);
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
            if (context.Request.Url.Host == "localhost" && 
                (contentPath.Contains("oipcms/") || 
                contentPath.Contains("wwwsite/") || 
                (contentPath.Contains("webui/") && containerOwner is TBAccount) ||
                contentPath.Contains("categoriesandcontent/")))
            {
                HandleFileSystemGetRequest(containerOwner, context, contentPath);
                return;
            }
            if (String.IsNullOrEmpty(contentPath))
            {
                CloudBlob redirectBlob = StorageSupport.GetOwnerBlobReference(containerOwner,
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
                blob.FetchAttributes();
                response.ContentType = blob.Properties.ContentType;
                response.Headers.Add("ETag", blob.Properties.ETag);
                blob.DownloadToStream(response.OutputStream);
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
                response.Write(ex.ToString());
            }
            response.End();
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
            string LocalWwwSiteFolder = @"C:\Users\kalle\WebstormProjects\CustomerWww\FOIPWww\UI\foip\";
            string LocalSchoolsAccountFolder = @"C:\Users\kalle\WebstormProjects\CaloomSchools\UI\account\";
            string fileName;
            if (prefixStrippedContent.Contains("oipcms/"))
                fileName = prefixStrippedContent.Replace("oipcms/", LocalWebRootFolder);
            else if (prefixStrippedContent.Contains("webui/"))
                fileName = prefixStrippedContent.Replace("webui/", LocalSchoolsAccountFolder);
            else if (prefixStrippedContent.Contains("categoriesandcontent/"))
                fileName = prefixStrippedContent.Replace("categoriesandcontent/", LocalWebCatConFolder);
            else
                fileName = prefixStrippedContent.Replace("wwwsite/", LocalWwwSiteFolder);
            if (File.Exists(fileName))
            {
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
