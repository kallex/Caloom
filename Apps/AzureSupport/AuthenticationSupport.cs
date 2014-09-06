using System;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Web;
using AzureSupport;

namespace TheBall
{
    public static class AuthenticationSupport
    {
        private const string AuthCookieName = "TheBall_AUTH";
        private const string AuthStrSeparator = ">TheBall<";
        private const int TimeoutSeconds = 1800;
        //private const int TimeoutSeconds = 10;
        public static void SetAuthenticationCookie(HttpResponse response, string validUserName)
        {
            string cookieValue = DateTime.UtcNow.Ticks + AuthStrSeparator + validUserName;
            string authString = EncryptionSupport.EncryptStringToBase64(cookieValue);
            if(response.Cookies[AuthCookieName] != null)
                response.Cookies.Remove(AuthCookieName);
            HttpCookie cookie = new HttpCookie(AuthCookieName, authString);
            // Session limit from browser
            //cookie.Expires = DateTime.UtcNow.AddSeconds(TimeoutSeconds); 
            cookie.HttpOnly = false;
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static void SetUserFromCookieIfExists(HttpContext context)
        {
            var request = HttpContext.Current.Request;
            var encCookie = request.Cookies[AuthCookieName];
            if (encCookie != null)
            {
                try
                {
                    string cookieValue = EncryptionSupport.DecryptStringFromBase64(encCookie.Value);
                    var valueSplit = cookieValue.Split(new[] {AuthStrSeparator}, StringSplitOptions.None);
                    long ticks = long.Parse(valueSplit[0]);
                    DateTime cookieTime = new DateTime(ticks);
                    DateTime utcNow = DateTime.UtcNow;
                    if(cookieTime.AddSeconds(TimeoutSeconds) < utcNow)
                        throw new SecurityException("Cookie expired");
                    string userName = valueSplit[1];
                    context.User = new GenericPrincipal(new GenericIdentity(userName, "theball"), new string[0]);
                    // Reset cookie time to be again timeout from this request
                    //encCookie.Expires = DateTime.Now.AddSeconds(TimeoutSeconds);
                    //context.Response.Cookies.Set(encCookie);
                    SetAuthenticationCookie(context.Response, userName);
                } catch
                {
                    ClearAuthenticationCookie(context.Response);
                }
            }
            
        }

        public static void ClearAuthenticationCookie(HttpResponse response)
        {
            HttpCookie cookie = new HttpCookie(AuthCookieName);
            cookie.Expires = DateTime.Today.AddDays(-1);
            if(response.Cookies[AuthCookieName] != null)
                response.Cookies.Set(cookie);
            else
                response.Cookies.Add(cookie);
        }
    }
}