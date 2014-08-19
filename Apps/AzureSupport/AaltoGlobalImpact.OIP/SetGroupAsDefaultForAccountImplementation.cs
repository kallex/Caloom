using System;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    public class SetGroupAsDefaultForAccountImplementation
    {
        public static AccountContainer GetTarget_AccountContainer()
        {
            return AccountContainer.RetrieveFromOwnerContent(InformationContext.CurrentOwner, "default");
        }

        public static string GetTarget_RedirectFromFolderBlobName()
        {
            return StorageSupport.GetOwnerRootAddress(InformationContext.CurrentOwner) + "RedirectFromFolder.red";
        }

        public static void ExecuteMethod_SetDefaultGroupValue(string groupId, AccountContainer accountContainer)
        {
            accountContainer.AccountModule.Profile.IsSimplifiedAccount = true;
            accountContainer.AccountModule.Profile.SimplifiedAccountGroupID = groupId;
        }

        public static void ExecuteMethod_StoreObject(AccountContainer accountContainer)
        {
            accountContainer.StoreInformation(InformationContext.CurrentOwner);
        }

        public static void ExecuteMethod_SetAccountRedirectFileToGroup(string groupId, string redirectFromFolderBlobName)
        {
            var blob = StorageSupport.GetOwnerBlobReference(InformationContext.CurrentOwner, redirectFromFolderBlobName);
            blob.UploadBlobText(String.Format("/auth/grp/{0}/", groupId));
        }
    }
}