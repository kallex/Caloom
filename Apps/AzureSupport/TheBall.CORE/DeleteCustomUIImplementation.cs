using System;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure.StorageClient;

namespace TheBall.CORE
{
    public class DeleteCustomUIImplementation
    {
        public static GroupContainer GetTarget_GroupContainer(IContainerOwner owner)
        {
            return GroupContainer.RetrieveFromOwnerContent(owner, "default");
        }

        public static string GetTarget_CustomUIFolder(IContainerOwner owner, string customUiName)
        {
            return StorageSupport.GetOwnerRootAddress(owner) + "customui_" + customUiName + "/";
        }

        public static void ExecuteMethod_RemoveCustomUIName(string customUiName, GroupContainer groupContainer)
        {
            var profile = groupContainer.GroupProfile;
            if (profile.CustomUICollection == null)
                profile.CustomUICollection = new ShortTextCollection();
            var customUICollection = groupContainer.GroupProfile.CustomUICollection;
            customUICollection.CollectionContent.RemoveAll(customUIEntry => customUIEntry.Content == customUiName);

        }

        public static void ExecuteMethod_RemoveCustomUIContents(string customUiFolder)
        {
            var blobListing = StorageSupport.CurrActiveContainer.GetBlobListing(customUiFolder);
            foreach (CloudBlockBlob blob in blobListing)
            {
                blob.DeleteIfExists();
            }
        }

        public static void ExecuteMethod_StoreObject(GroupContainer groupContainer)
        {
            groupContainer.StoreInformation();
        }
    }
}