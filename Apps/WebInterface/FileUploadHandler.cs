using System;
using System.IO;
using System.Security;
using System.Web;
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
    public class FileUploadHandler : IHttpHandler
    {
        private const string AuthGroupPrefix = "/auth/grp/";
        private const string AuthAccountPrefix = "/auth/acc/";
        private int AuthGroupPrefixLen;
        private int AuthAccountPrefixLen;
        //private const string AuthFileUpload = "/fileupload/grp/";
        //private int AuthEmailValidationLen;


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

        public FileUploadHandler()
        {
            AuthGroupPrefixLen = AuthGroupPrefix.Length;
            AuthAccountPrefixLen = AuthAccountPrefix.Length;
        }

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            WebSupport.InitializeContextStorage(context.Request);
            try
            {
                if (request.Path.StartsWith(AuthGroupPrefix))
                {
                    //HandleEmailValidation(context);
                }        
            } finally
            {
                InformationContext.ProcessAndClearCurrent();
            }
        }


        #endregion
    }
}
