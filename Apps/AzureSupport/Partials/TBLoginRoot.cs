using System;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class TBRLoginRoot
    {
        public static TBRLoginRoot GetOrCreateLoginRootWithAccount(string loginUrl)
        {
            string loginRootID = TBLoginInfo.GetLoginIDFromLoginURL(loginUrl);
            var loginRoot = RetrieveFromDefaultLocation(loginRootID);
            if (loginRoot == null)
            {
                // Storing loginroot
                loginRoot = TBRLoginRoot.CreateDefault();
                loginRoot.ID = loginRootID;
                loginRoot.UpdateRelativeLocationFromID();
                StorageSupport.StoreInformation(loginRoot);

                // Creating login info for account and storing the account
                TBLoginInfo loginInfo = TBLoginInfo.CreateDefault();
                loginInfo.OpenIDUrl = loginUrl;

                TBRAccountRoot accountRoot = TBRAccountRoot.CreateAndStoreNewAccount();
                accountRoot.Account.Logins.CollectionContent.Add(loginInfo);
                accountRoot.Account.StoreAndPropagate();
            }
            loginRoot = RetrieveFromDefaultLocation(loginRootID);
            return loginRoot;
        }

    }
}