using System;
using System.Linq;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    public static class UpdateAccountRootToReferencesImplementation
    {
        public static TBRAccountRoot GetTarget_AccountRoot(string accountID)
        {
            return TBRAccountRoot.RetrieveFromDefaultLocation(accountID);
        }

        public static TBRLoginRoot[] GetTarget_AccountLogins(TBRAccountRoot accountRoot)
        {
            var loginRootIDs =
                accountRoot.Account.Logins.CollectionContent.Select(
                    login => TBLoginInfo.GetLoginIDFromLoginURL(login.OpenIDUrl)).Distinct().ToArray();
            var loginRoots = loginRootIDs.Select(loginID => TBRLoginRoot.RetrieveFromDefaultLocation(loginID)).ToArray();
            return loginRoots;
        }

        public static TBREmailRoot[] GetTarget_AccountEmails(TBRAccountRoot accountRoot)
        {
            var emailIDs = accountRoot.Account.Emails.CollectionContent.Select(email => TBREmailRoot.GetIDFromEmailAddress(email.EmailAddress)).Distinct().ToArray();
            var emailRoots =
                emailIDs.Select(emailAddress => TBREmailRoot.RetrieveFromDefaultLocation(emailAddress)).ToArray();
            return emailRoots;
        }

        public static void ExecuteMethod_UpdateAccountToLogins(TBRAccountRoot accountRoot, TBRLoginRoot[] accountLogins)
        {
            foreach(var accountLogin in accountLogins)
            {
                accountLogin.Account = accountRoot.Account;
            }
        }

        public static void ExecuteMethod_UpdateAccountToEmails(TBRAccountRoot accountRoot, TBREmailRoot[] accountEmails)
        {
            foreach(var accountEmail in accountEmails)
            {
                accountEmail.Account = accountRoot.Account;
            }
        }

        public static void ExecuteMethod_StoreObjects(TBRLoginRoot[] accountLogins, TBREmailRoot[] accountEmails)
        {
            foreach (var acctLogin in accountLogins)
                acctLogin.StoreInformation();
            foreach (var acctEmail in accountEmails)
                acctEmail.StoreInformation();
        }
    }
}