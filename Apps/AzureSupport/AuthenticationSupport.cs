using System;
using System.Security.Principal;
using System.Web;

namespace TheBall
{
    public static class AuthenticationSupport
    {
        private const string AuthCookieName = "TheBall_AUTH";
        private const int TimeoutSeconds = 600;
        public static void SetAuthenticationCookie(HttpResponse response, string validUserName)
        {
            string authString = EncryptionSupport.EncryptStringToBase64(validUserName);
            if(response.Cookies[AuthCookieName] != null)
                response.Cookies.Remove(AuthCookieName);
            HttpCookie cookie = new HttpCookie(AuthCookieName, authString);
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
                    string userName = EncryptionSupport.DecryptStringFromBase64(encCookie.Value);
                    context.User = new GenericPrincipal(new GenericIdentity(userName, "theball"), new string[0]);
                    // Reset cookie time to be again timeout from this request
                    encCookie.Expires = DateTime.Now.AddSeconds(TimeoutSeconds);
                    context.Response.Cookies.Set(encCookie);
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