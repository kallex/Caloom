using System;

namespace AaltoGlobalImpact.OIP
{
    partial class TBLoginInfo
    {
        public static string GetLoginIDFromLoginURL(string loginURL)
        {
            if (loginURL.StartsWith("https://"))
                return loginURL.Substring(8);
            if (loginURL.StartsWith("http://"))
                return loginURL.Substring(7);
            throw new NotSupportedException("Not supported user name prefix: " + loginURL);
        }
    }
}