using System;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    public static class UpdateAccountContainerFromAccountRootImplementation
    {
        public static TBRAccountRoot GetTarget_AccountRoot(string accountID)
        {
            return TBRAccountRoot.RetrieveFromDefaultLocation(accountID);
        }

        public static AccountContainer GetTarget_AccountContainer(TBRAccountRoot accountRoot)
        {
            TBAccount account = accountRoot.Account;
            AccountContainer accountContainer = AccountContainer.RetrieveFromOwnerContent(account, "default");
            if (accountContainer == null)
            {
                accountContainer = AccountContainer.CreateDefault();
                accountContainer.SetLocationAsOwnerContent(account, "default");
            }
            return accountContainer;
        }

        public static void ExecuteMethod_UpdateAccountContainerLogins(TBRAccountRoot accountRoot, AccountContainer accountContainer)
        {
            accountContainer.AccountModule.Security.LoginInfoCollection = accountRoot.Account.Logins;
        }

        public static void ExecuteMethod_UpdateAccountContainerEmails(TBRAccountRoot accountRoot, AccountContainer accountContainer)
        {
            accountContainer.AccountModule.Security.EmailCollection = accountRoot.Account.Emails;
        }

        public static void ExecuteMethod_StoreObjects(AccountContainer accountContainer)
        {
            accountContainer.StoreInformation();
        }
    }
}