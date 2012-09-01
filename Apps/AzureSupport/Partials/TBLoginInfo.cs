using System;

namespace AaltoGlobalImpact.OIP
{
    partial class TBLoginInfo
    {
        public static string GetLoginIDFromLoginURL(string user)
        {
            if (user.StartsWith("https://"))
                return user.Substring(8);
            if (user.StartsWith("http://"))
                return user.Substring(7);
            throw new NotSupportedException("Not supported user name prefix: " + user);
        }
    }
}