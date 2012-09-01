using System.Web;

namespace AaltoGlobalImpact.OIP
{
    partial class TBREmailRoot
    {
        public static string GetIDFromEmailAddress(string emailAddress)
        {
            return HttpUtility.UrlEncode(emailAddress);
        }
    }
}