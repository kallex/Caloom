using System;
using System.IO;
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

        private const string AuthPersonalPrefix = "/auth/personal/";
        private const string AuthGroupPrefix = "/auth/grp/";
        private const string AuthAccountPrefix = "/auth/acc/";
        private const string AuthProcPrefix = "/auth/proc/";
        private const string AuthPrefix = "/auth/";
        private const string AuthEmailValidation = "/auth/emailvalidation/";
        private int AuthGroupPrefixLen;
        private int AuthPersonalPrefixLen;
        private int AuthAccountPrefixLen;
        private int AuthProcPrefixLen;
        private int AuthPrefixLen;
        private int AuthEmailValidationLen;
        private int GuidIDLen;


        public AuthorizedBlobStorageHandler()
        {
            AuthGroupPrefixLen = AuthGroupPrefix.Length;
            AuthPersonalPrefixLen = AuthPersonalPrefix.Length;
            AuthAccountPrefixLen = AuthAccountPrefix.Length;
            AuthProcPrefixLen = AuthProcPrefix.Length;
            AuthPrefixLen = AuthPrefix.Length;
            AuthEmailValidationLen = AuthEmailValidation.Length;
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
            } else if(request.Path.StartsWith(AuthEmailValidation))
            {
                HandleEmailValidation(context);
            }
            return;
        }

        private void HandleEmailValidation(HttpContext context)
        {
            TBRLoginRoot loginRoot = GetOrCreateLoginRoot(context);
            string requestPath = context.Request.Path;
            string emailValidationID = requestPath.Substring(AuthEmailValidationLen);
            TBAccount account = loginRoot.Account;
            TBEmailValidation emailValidation = TBEmailValidation.RetrieveFromDefaultLocation(emailValidationID, account);
            if (emailValidation == null)
                return;
            if(emailValidation.ValidUntil < DateTime.Now)
            {
                // TODO: Some invalidation message + UTC time
                StorageSupport.DeleteInformationObject(emailValidation, account);
                throw new TimeoutException("Email validation expired at: " + emailValidation.ToString());
            }
            TBRAccountRoot accountRoot = TBRAccountRoot.RetrieveFromDefaultLocation(account.ID);
            TBEmail email = TBEmail.CreateDefault();
            email.EmailAddress = emailValidation.Email;
            email.ValidatedAt = DateTime.Now;
            account.Emails.CollectionContent.Add(email);
            accountRoot.Account = account;
            StorageSupport.StoreInformation(loginRoot);
            StorageSupport.StoreInformation(accountRoot);
            context.Response.Redirect("/auth/personal/oip-personal-landing-page.phtml", true);
        }

        private void HandleAccountRequest(HttpContext context)
        {
            TBRLoginRoot loginRoot = GetOrCreateLoginRoot(context);
        }

        private void HandleProcRequest(HttpContext context)
        {
            TBRLoginRoot loginRoot = GetOrCreateLoginRoot(context);
            TBAccount account = loginRoot.Account;
            string requestPath = context.Request.Path;
            string contentPath = requestPath.Substring(AuthPrefixLen);
            HandleOwnerRequest(account, context, contentPath);
        }

        private void HandleGroupRequest(HttpContext context)
        {
            string requestPath = context.Request.Path;
            string groupID = GetGroupID(context.Request.Path);
            string loginPath = GetLoginBlobName(context.User.Identity.Name);
            string loginGroupID = "g-" + groupID + "-l-" + loginPath;
            TBRLoginGroupRoot loginGroupRoot = TBRLoginGroupRoot.RetrieveFromDefaultLocation(loginGroupID);
            if(loginGroupRoot == null)
            {
                // TODO: Polite invitation request
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
            TBRLoginRoot loginRoot = GetOrCreateLoginRoot(context);

            TBAccount account = loginRoot.Account;
            bool hasRegisteredEmail = account.Emails.CollectionContent.Count > 0;
            if(hasRegisteredEmail == false)
            {
                PrepareEmailRegistrationPage(account, context);
                context.Response.Redirect("/auth/proc/tbp-layout-registeremail.phtml", true);
                return;
            }
            string requestPath = context.Request.Path;
            string contentPath = requestPath.Substring(AuthPersonalPrefixLen);
            HandleOwnerRequest(account, context, contentPath);
        }

        private void PrepareEmailRegistrationPage(TBAccount account, HttpContext context)
        {
            const string procRegisterEmailPage = "proc/tbp-layout-registeremail.phtml";
            string currPage = StorageSupport.DownloadOwnerBlobText(account, procRegisterEmailPage, true);
            if(currPage == null)
            {
                TBPRegisterEmail registerEmail = TBPRegisterEmail.CreateDefault();
                registerEmail.EmailAddress = "";
                StorageSupport.StoreInformation(registerEmail, account);
                string template = StorageSupport.CurrTemplateContainer.DownloadBlobText("theball-proc/tbp-layout-registeremail.phtml");
                string result = RenderWebSupport.RenderTemplateWithContent(template, registerEmail);
                StorageSupport.UploadOwnerBlobText(account, procRegisterEmailPage, result);
            }
        }

        private void HandleOwnerRequest(IContainerOwner containerOwner, HttpContext context, string contentPath)
        {
            if (context.Request.RequestType == "POST")
                HandleOwnerPostRequest(containerOwner, context, contentPath);
            else
                HandleOwnerGetRequest(containerOwner, context, contentPath);
        }

        private void HandleOwnerPostRequest(IContainerOwner containerOwner, HttpContext context, string contentPath)
        {
            HttpRequest request = context.Request;
            var form = request.Form;
            string objectTypeName = form["RootObjectType"];
            string objectRelativeLocation = form["RootObjectRelativeLocation"];
            string eTag = form["RootObjectETag"];
            if (eTag == null)
            {
                throw new InvalidDataException("ETag must be present in submit request for root container object");
            }
            IInformationObject rootObject = StorageSupport.RetrieveInformation(objectRelativeLocation, objectTypeName, eTag, containerOwner);
            rootObject.SetValuesToObjects(form);
            StorageSupport.StoreInformation(rootObject, containerOwner);
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
            }
            catch (Exception ex)
            {
                context.Response.Write(ex.ToString());
            }
            context.Response.End();
        }

        private TBRLoginRoot GetOrCreateLoginRoot(HttpContext context)
        {
            string user = context.User.Identity.Name;
            string loginBlobID = GetLoginBlobName(user);
            TBRLoginRoot loginRoot = TBRLoginRoot.RetrieveFromDefaultLocation(loginBlobID);
            if(loginRoot == null)
            {
                TBLoginInfo loginInfo = TBLoginInfo.CreateDefault();
                loginInfo.OpenIDUrl = user;

                TBRAccountRoot accountRoot = TBRAccountRoot.CreateDefault();
                accountRoot.Account.Logins.CollectionContent.Add(loginInfo);
                accountRoot.ID = accountRoot.Account.ID;
                accountRoot.UpdateRelativeLocationFromID();
                StorageSupport.StoreInformation(accountRoot);

                loginRoot = TBRLoginRoot.CreateDefault();
                loginRoot.ID = loginBlobID;
                loginRoot.UpdateRelativeLocationFromID();
                loginRoot.Account = accountRoot.Account;
                StorageSupport.StoreInformation(loginRoot);
            }
            HttpContext.Current.Items.Add("Account", loginRoot.Account);
            return loginRoot;
        }

        private string GetLoginBlobName(string user)
        {
            if (user.StartsWith("https://"))
                return user.Substring(8);
            if (user.StartsWith("http://"))
                return user.Substring(7);
            throw new NotSupportedException("Not supported user name prefix: " + user);
        }

        private void ProcessPost(HttpContext context)
        {
            var request = context.Request;
            string reqType = request.RequestType;
            var form = request.Form;
            string objectTypeName = form["RootObjectType"];
            string objectRelativeLocation = form["RootObjectRelativeLocation"];
            string eTag = form["RootObjectETag"];
            if(eTag == null)
            {
                throw new InvalidDataException("ETag must be present in submit request for root container object");
            }
            IInformationObject rootObject = StorageSupport.RetrieveInformation(objectRelativeLocation, objectTypeName, eTag);
            rootObject.SetValuesToObjects(form);
            StorageSupport.StoreInformation(rootObject);

            // Hard coded for demo:
            CloudBlobClient publicClient = new CloudBlobClient("http://theball.blob.core.windows.net/");
            string templatePath = "anon-webcontainer/theball-reference/example1-layout-default.html";
            CloudBlob blob = publicClient.GetBlobReference(templatePath);
            string templateContent = blob.DownloadText();
            string finalHtml = RenderWebSupport.RenderTemplateWithContent(templateContent, rootObject);
            string finalPath = "theball-reference/example1-rendered.html";
            StorageSupport.CurrAnonPublicContainer.UploadBlobText(finalPath, finalHtml);
        }

        #endregion
    }
}
