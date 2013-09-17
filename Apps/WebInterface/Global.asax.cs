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
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
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