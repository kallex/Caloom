using System;
using System.Web;
using System.Web.Security;
using DotNetOpenAuth.OpenId.RelyingParty;

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
            if(String.IsNullOrEmpty(null) == false)
                Response.Write("<p>According to the request, you are a OpenAuth user called: " + user + "</p>");
            Response.Write("<p><a href=\"/theballanon/anonlink.txt\">Some anon data link</a></p>");
            Response.Write("<p><a href=\"/theballauth/authlink.txt\">Some auth data link</a></p>");
            Response.Write("</body>");
            Response.Write("</html>");
        }

        #endregion
    }
}
