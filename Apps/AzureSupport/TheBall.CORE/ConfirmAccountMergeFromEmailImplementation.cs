using AaltoGlobalImpact.OIP;

namespace TheBall.CORE
{
    public class ConfirmAccountMergeFromEmailImplementation
    {
        public static TBMergeAccountConfirmation GetTarget_MergeAccountConfirmation(TBEmailValidation emailConfirmation)
        {
            return emailConfirmation.MergeAccountsConfirmation;
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