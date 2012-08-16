using System;
using System.Web;
using System.Web.Security;
using DotNetOpenAuth.OpenId.RelyingParty;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

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

        public void ProcessRequest2(HttpContext context)
        {
            //OpenIdRelyingParty openid = new OpenIdRelyingParty();
            //var authenticationResponse = openid.GetResponse();
            string user = context.User.Identity.Name;
            //write your handler implementation here.
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            // This handler is called whenever a file ending 
            // in .sample is requested. A file with that extension
            // does not need to exist.
            Response.Write("<html>");
            Response.Write("<body>");
            Response.Write("<h1>Hello from a synchronous custom HTTP handler 2.</h1>");
            if(String.IsNullOrEmpty(user) == false)
                Response.Write("<p>According to the request, you are a OpenAuth user called: " + user + "</p>");
            Response.Write("<p><a href=\"/theballanon/anonlink.txt\">Some anon data link</a></p>");
            Response.Write("<p><a href=\"/theballauth/authlink.txt\">Some auth data link</a></p>");
            Response.Write("</body>");
            Response.Write("</html>");
        }


        public void ProcessRequest(HttpContext context)
        {
            string user = context.User.Identity.Name;
            bool isAuthenticated = String.IsNullOrEmpty(user) == false;
            HttpRequest request = context.Request;
            if(request.RequestType != "GET")
            {
                string reqType = request.RequestType;
            }
            HttpResponse response = context.Response;
            if(request.Path.StartsWith("/theballanon/"))
            {
                ProcessAnonymousRequest(request, response);
                return;
            }
            
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
