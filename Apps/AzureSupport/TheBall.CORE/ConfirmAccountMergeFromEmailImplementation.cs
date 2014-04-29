using System.Security;
using AaltoGlobalImpact.OIP;

namespace TheBall.CORE
{
    public class ConfirmAccountMergeFromEmailImplementation
    {
        public static TBMergeAccountConfirmation GetTarget_MergeAccountConfirmation(TBEmailValidation emailConfirmation)
        {
            return emailConfirmation.MergeAccountsConfirmation;
        }

        public static void ExecuteMethod_ValidateCurrentAccountAsMergingActor(string currentAccountId, TBMergeAccountConfirmation mergeAccountConfirmation)
        {
            if(currentAccountId != mergeAccountConfirmation.AccountToBeMergedID)
                throw new SecurityException("Current requesting account for merge does not equal to actual account to be merged by IDs");
        }

        public static void ExecuteMethod_PerformAccountMerge(TBMergeAccountConfirmation mergeAccountConfirmation)
        {
            MergeAccountsDestructively.Execute(new MergeAccountsDestructivelyParameters
                {
                    AccountToBeMergedAndDestroyedID = mergeAccountConfirmation.AccountToBeMergedID,
                    PrimaryAccountToStayID = mergeAccountConfirmation.AccountToMergeToID
                });
        }

    }
}