using System;
using System.IO;
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

        public static TBEmailValidation GetTarget_MergeAccountEmailConfirmation(string currentAccountId, string emailAddress, string redirectUrlAfterValidation, string accountToMergeToId)
        {
            if (currentAccountId == accountToMergeToId)
                return null;
            TBEmailValidation emailValidation = new TBEmailValidation
                {
                    AccountID = currentAccountId,
                    Email = emailAddress,
                    ValidUntil = DateTime.UtcNow.AddMinutes(5),
                    MergeAccountsConfirmation = new TBMergeAccountConfirmation
                        {
                            AccountToBeMergedID = currentAccountId,
                            AccountToMergeToID = accountToMergeToId
                        },
                    RedirectUrlAfterValidation = redirectUrlAfterValidation
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
            if(mergeAccountEmailConfirmation != null)
                EmailSupport.SendMergeAccountsConfirmationEmail(mergeAccountEmailConfirmation);
        }

        public static void ExecuteMethod_ValidateExistingEmail(string emailAddress)
        {
            string emailRootID = TBREmailRoot.GetIDFromEmailAddress(emailAddress);
            TBREmailRoot emailRoot = TBREmailRoot.RetrieveFromDefaultLocation(emailRootID);
            if(emailRoot == null)
                throw new InvalidDataException("Email address for merge does not exist in the system");
        }

    }
}