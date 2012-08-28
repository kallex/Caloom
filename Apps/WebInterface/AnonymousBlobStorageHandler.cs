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
    public class AnonymousBlobStorageHandler : IHttpHandler
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
            string user = context.User.Identity.Name;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            if(request.Path.StartsWith("/theballanon/"))
            {
                ProcessAnonymousRequest(request, response);
                return;
            }
        }

        private void ProcessAnonymousRequest(HttpRequest request, HttpResponse response)
        {
            CloudBlobClient publicClient = new CloudBlobClient("http://theball.blob.core.windows.net/");
            string blobPath = request.Path.Replace("/theballanon/", "anon-webcontainer/");
            CloudBlob blob = publicClient.GetBlobReference(blobPath);
            response.Clear();
            try
            {
                blob.FetchAttributes();
                response.ContentType = blob.Properties.ContentType;
                blob.DownloadToStream(response.OutputStream);
            } catch(StorageClientException scEx)
            {
                response.Write(scEx.ToString());
            } finally
            {
                response.End();
            }
        }

        #endregion
    }
}
