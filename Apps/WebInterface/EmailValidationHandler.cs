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
    public class EmailValidationHandler : IHttpHandler
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

        public void ProcessRequest(HttpContext context)
        {
            return;
            string user = context.User.Identity.Name;
            bool isAuthenticated = String.IsNullOrEmpty(user) == false;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            if(request.Path.StartsWith("/anon/") || isAuthenticated == false)
            {
                return;
            }

            context.Response.Write("<p>Not implemented</p>");
            return;
            // Get the file name.

            string fileName = context.Request.Path.Replace("/blobproxy/", string.Empty);

            // Get the blob from blob storage.
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var blobStorage = storageAccount.CreateCloudBlobClient();
            string blobContainerName = "";
            string blobAddress = blobContainerName + "/" + fileName;
            CloudBlob blob = blobStorage.GetBlobReference(blobAddress);

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

        #endregion
    }
}
