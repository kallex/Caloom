using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Microsoft.WindowsAzure;
using TheBall;

namespace WebInterface
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            string connStr;
            connStr = InstanceConfiguration.AzureStorageConnectionString;
            StorageSupport.InitializeWithConnectionString(connStr);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //if(Request.Path == "default.htm")
            //    Response.RedirectPermanent("anon/default/oip-anon-landing-page.phtml", true);
            //if(Request.Path.ToLower().StartsWith("/theball") == false)
            //    Response.Redirect("/theballanon/oip-layouts/oip-edit-default-layout-jeroen.html", true);
            if (!Request.IsLocal && !Request.IsSecureConnection)
            {
                bool isWebSocket = Request.Path.StartsWith("/websocket/");
                bool isIndexAspx = Request.Path.StartsWith("index.aspx");
                // TODO: Line below is a hack, that's assuming www.prefix
                bool isWww = Request.Url.DnsSafeHost.StartsWith("www.") ||
                             Request.Url.DnsSafeHost.StartsWith("teaching.") ||
                             Request.Url.DnsSafeHost.StartsWith("ptt.") ||
                             Request.Url.DnsSafeHost.StartsWith("7lk.") ||
                             Request.Url.DnsSafeHost.StartsWith("globalimpact.") ||
                             Request.Url.DnsSafeHost.StartsWith("newglobal.");
                if (isWebSocket == false && isIndexAspx == false && isWww == false)
                {
                    string redirectUrl = Request.Url.ToString().Replace("http:", "https:");
                    Response.Redirect(redirectUrl, true);
                }
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            var ctx = HttpContext.Current;
            var request = ctx.Request;
            var authorization = request.Headers["Authorization"];
            if (authorization != null && authorization.StartsWith("DeviceAES:"))
            {
                string[] parts = authorization.Split(':');
                string trustID = parts[2];
                ctx.User = new GenericPrincipal(new GenericIdentity(trustID), new string[] { "DeviceAES"});
            } else
                AuthenticationSupport.SetUserFromCookieIfExists(HttpContext.Current);
        }

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}