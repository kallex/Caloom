using TheBall;

namespace AaltoGlobalImpact.OIP
{
    public class ClearDefaultGroupFromAccountImplementation
    {
        public static AccountContainer GetTarget_AccountContainer()
        {
            return AccountContainer.RetrieveFromOwnerContent(InformationContext.CurrentOwner, "default");
        }

        public static string GetTarget_RedirectFromFolderBlobName()
        {
            return StorageSupport.GetOwnerRootAddress(InformationContext.CurrentOwner) + "RedirectFromFolder.red";
        }

        public static void ExecuteMethod_ClearDefaultGroupValue(AccountContainer accountContainer)
        {
            accountContainer.AccountModule.Profile.SimplifiedAccountGroupID = null;
            accountContainer.AccountModule.Profile.IsSimplifiedAccount = false;
        }

        public static void ExecuteMethod_StoreObject(AccountContainer accountContainer)
        {
            accountContainer.StoreInformation(InformationContext.CurrentOwner);
        }

        public static void ExecuteMethod_RemoveAccountRedirectFile(string redirectFromFolderBlobName)
        {
            var redirectBlob = StorageSupport.GetOwnerBlobReference(InformationContext.CurrentOwner, redirectFromFolderBlobName);
            redirectBlob.DeleteWithoutFiringSubscriptions();
        }
    }
}