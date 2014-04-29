using System;
using AaltoGlobalImpact.OIP;

namespace TheBall.CORE
{
    public class InitiateAccountMergeFromEmailImplementation
    {
        public static string GetTarget_AccountToMergeToID(string emailAddress)
        {
            string emailRootID = TBREmailRoot.GetIDFromEmailAddress(emailAddress);
            TBREmailRoot emailRoot = TBREmailRoot.RetrieveFromDefaultLocation(emailRootID);
            return emailRoot.Account.ID;
        }

        public static TBEmailValidation GetTarget_MergeAccountEmailConfirmation(string currentAccountId, string emailAddress, string accountToMergeToId)
        {
            if (currentAccountId == accountToMergeToId)
                return null;
            TBEmailValidation emailValidation = new TBEmailValidation();
            emailValidation.AccountID = currentAccountId;
            emailValidation.Email = emailAddress;
            emailValidation.ValidUntil = DateTime.UtcNow.AddMinutes(5);
            emailValidation.MergeAccountsConfirmation = new TBMergeAccountConfirmation
                {
                    AccountToBeMergedID = currentAccountId,
                    AccountToMergeToID = accountToMergeToId
                };
            return emailValidation;
        }

        public static void ExecuteMethod_StoreObject(TBEmailValidation mergeAccountEmailConfirmation)
        {
            if(mergeAccountEmailConfirmation != null)
                mergeAccountEmailConfirmation.StoreInformation();
        }

        public static void ExecuteMethod_SendConfirmationEmail(TBEmailValidation mergeAccountEmailConfirmation)
        {
            EmailSupport.SendMergeAccountsConfirmationEmail(mergeAccountEmailConfirmation);
        }
    }
}