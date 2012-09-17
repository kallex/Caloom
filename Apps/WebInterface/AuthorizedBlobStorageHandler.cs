using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Security;
using AaltoGlobalImpact.OIP;
using DotNetOpenAuth.OpenId.RelyingParty;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;

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
        private const string AuthProcPrefix = "/auth/proc/";
        private const string AuthPrefix = "/auth/";
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
            AuthProcPrefixLen = AuthProcPrefix.Length;
            AuthPrefixLen = AuthPrefix.Length;
            GuidIDLen = Guid.Empty.ToString().Length;
        }

        public void ProcessRequest(HttpContext context)
        {
            string user = context.User.Identity.Name;
            bool isAuthenticated = String.IsNullOrEmpty(user) == false;
            if (isAuthenticated == false)
            {
                return;
            }
            HttpRequest request = context.Request;
            if(request.Path.StartsWith(AuthPersonalPrefix))
            {
                HandlePersonalRequest(context);
            } else if(request.Path.StartsWith(AuthGroupPrefix))
            {
                HandleGroupRequest(context);
            } else if(request.Path.StartsWith(AuthProcPrefix))
            {
                HandleProcRequest(context);
            } else if(request.Path.StartsWith(AuthAccountPrefix))
            {
                HandleAccountRequest(context);
            } 
            return;
        }

        private void HandleAccountRequest(HttpContext context)
        {
            //TBRLoginRoot loginRoot = GetOrCreateLoginRoot(context);
        }

        private void HandleProcRequest(HttpContext context)
        {
            string loginUrl = context.User.Identity.Name;
            TBRLoginRoot loginRoot = TBRLoginRoot.GetOrCreateLoginRootWithAccount(loginUrl, false);
            TBAccount account = loginRoot.Account;
            string requestPath = context.Request.Path;
            string contentPath = requestPath.Substring(AuthPrefixLen);
            HandleOwnerRequest(account, context, contentPath);
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
            string contentPath = requestPath.Substring(AuthGroupPrefixLen + GuidIDLen + 1);
            HandleOwnerRequest(loginGroupRoot, context, contentPath);
        }

        private string GetGroupID(string path)
        {
            return path.Substring(AuthGroupPrefixLen, GuidIDLen);
        }

        private void HandlePersonalRequest(HttpContext context)
        {
            string loginUrl = WebSupport.GetLoginUrl(context);
            TBRLoginRoot loginRoot = TBRLoginRoot.GetOrCreateLoginRootWithAccount(loginUrl, true);

            TBAccount account = loginRoot.Account;
            string requestPath = context.Request.Path;
            string contentPath = requestPath.Substring(AuthPersonalPrefixLen);
            HandleOwnerRequest(account, context, contentPath);
        }

        private void HandleOwnerRequest(IContainerOwner containerOwner, HttpContext context, string contentPath)
        {
            if (context.Request.RequestType == "POST")
            {
                // Do first post, and then get to the same URL
                HandleOwnerPostRequest(containerOwner, context, contentPath);
            }
            HandleOwnerGetRequest(containerOwner, context, contentPath);
        }

        private void HandleOwnerPostRequest(IContainerOwner containerOwner, HttpContext context, string contentPath)
        {
            HttpRequest request = context.Request;
            var form = request.Form;

            string sourceNamesCommaSeparated = form["RootSourceName"];
            bool isCancelButton = form["btnCancel"] != null;
            if (isCancelButton)
                return;
            string objectFieldID = form["ObjectFieldID"];
            string objectFieldValue = form["Text_Short"];
            if (objectFieldValue == null)
                objectFieldValue = form["Text_Long"];
            if(objectFieldID != null)
            {
                form = new NameValueCollection();
                form.Set(objectFieldID, objectFieldValue);
            }

            CloudBlob webPageBlob = StorageSupport.CurrActiveContainer.GetBlob(contentPath, containerOwner);
            InformationSourceCollection sources = webPageBlob.GetBlobInformationSources();
            if(sourceNamesCommaSeparated == null)
                sourceNamesCommaSeparated = "";
            string[] sourceNames = sourceNamesCommaSeparated.Split(',');
            InformationSource[] sourceArray =
                sources.CollectionContent.Where(
                    src => src.IsInformationObjectSource && sourceNames.Contains(src.SourceName)).ToArray();
            foreach (InformationSource source in sourceArray)
            {
                string oldETag = source.SourceETag;
                IInformationObject rootObject = source.RetrieveInformationObject();
                if (oldETag != rootObject.ETag)
                {
                    RenderWebSupport.RefreshContent(webPageBlob);
                    throw new InvalidDataException("Information under editing was modified during display and save");
                }
                rootObject.SetValuesToObjects(form);
                IAddOperationProvider addOperationProvider = rootObject as IAddOperationProvider;
                if(addOperationProvider != null)
                {
                    bool storeAfterAdd = addOperationProvider.PerformAddOperation(sources);
                    if (storeAfterAdd)
                        StorageSupport.StoreInformation(rootObject, containerOwner);
                } else
                    StorageSupport.StoreInformation(rootObject, containerOwner);
            }
            RenderWebSupport.RefreshContent(webPageBlob);
            // Temporary live to pub sync below, to be removed
            //SyncTemplatesToSite(StorageSupport.CurrActiveContainer.Name,
            //    String.Format("grp/f8e1d8c6-0000-467e-b487-74be4ad099cd/{0}/", "livesite"),
            //    StorageSupport.CurrAnonPublicContainer.Name,
            //                    String.Format("grp/default/{0}/", "livepubsite"), true);

        }

        private void HandleOwnerGetRequest(IContainerOwner containerOwner, HttpContext context, string contentPath)
        {
            CloudBlob blob = StorageSupport.GetOwnerBlobReference(containerOwner, contentPath);

            // Read blob content to response.
            context.Response.Clear();
            try
            {
                blob.FetchAttributes();
                context.Response.ContentType = blob.Properties.ContentType;
                blob.DownloadToStream(context.Response.OutputStream);
            } catch(StorageClientException scEx)
            {
                if(scEx.ErrorCode == StorageErrorCode.BlobNotFound || scEx.ErrorCode == StorageErrorCode.ResourceNotFound)
                {
                    context.Response.Write("Blob not found: " + blob.Name + " (original path: " + context.Request.Path + ")");
                } else
                {
                    context.Response.Write("Error code: " + scEx.ErrorCode.ToString() + Environment.NewLine);
                    context.Response.Write(scEx.ToString());
                }
            }
            catch (Exception ex)
            {
                context.Response.Write(ex.ToString());
            }
            context.Response.End();
        }


        #endregion
    }
}
