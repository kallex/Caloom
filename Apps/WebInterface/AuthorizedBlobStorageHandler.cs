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

        public void ProcessRequest(HttpContext context)
        {
            string user = context.User.Identity.Name;
            bool isAuthenticated = String.IsNullOrEmpty(user) == false;
            HttpRequest request = context.Request;
            if(request.RequestType != "GET")
            {
                ProcessPost(context);
                return;
            }
            HttpResponse response = context.Response;
            if(request.Path.StartsWith("/anon/") || isAuthenticated == false)
            {
                return;
            }
            
            // Get the file name.

            context.Response.Write("<p>Not implemented</p>");
            return;
            string fileName = context.Request.Path;

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
