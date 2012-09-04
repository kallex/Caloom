using System;
using System.Web;

namespace WebInterface
{
    public static class WebSupport
    {
        public static string GetLoginUrl(HttpContext context)
        {
            return context.User.Identity.Name;
        }
    }
}