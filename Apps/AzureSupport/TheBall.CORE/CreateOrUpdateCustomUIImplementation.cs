using System;
using System.IO;
using System.IO.Compression;
using AaltoGlobalImpact.OIP;
using System.Linq;
using Microsoft.WindowsAzure.StorageClient;

namespace TheBall.CORE
{
    public class CreateOrUpdateCustomUIImplementation
    {
        public static GroupContainer GetTarget_GroupContainer(IContainerOwner owner)
        {
            return GroupContainer.RetrieveFromOwnerContent(owner, "default");
        }

        public static string GetTarget_CustomUIFolder(IContainerOwner owner, string customUiName)
        {
            if(string.IsNullOrWhiteSpace(customUiName))
                throw new ArgumentException("Invalid custom UI name", customUiName);
            return StorageSupport.GetOwnerRootAddress(owner) + "customui_" + customUiName + "/";
        }

        public static void ExecuteMethod_SetCustomUIName(string customUiName, GroupContainer groupContainer)
        {
            var profile = groupContainer.GroupProfile;
            if (profile.CustomUICollection == null)
                profile.CustomUICollection = new ShortTextCollection();
            var customUICollection = groupContainer.GroupProfile.CustomUICollection;
            bool hasName = customUICollection.CollectionContent.Count(customUI => customUI.Content == customUiName) > 0;
            if (hasName)
                return;
            customUICollection.CollectionContent.Add(new ShortTextObject
                {
                    Content = customUiName
                });
            customUICollection.CollectionContent.Sort((x, y) => String.Compare(x.Content, y.Content));
        }

        public static void ExecuteMethod_CopyUIContentsFromZipArchive(Stream zipArchiveStream, string customUiFolder)
        {
            var blobListing = StorageSupport.CurrActiveContainer.GetBlobListing(customUiFolder);
            foreach (CloudBlockBlob blob in blobListing)
            {
                blob.DeleteIfExists();
            }
            ZipArchive zipArchive = new ZipArchive(zipArchiveStream, ZipArchiveMode.Read);
            foreach (var zipEntry in zipArchive.Entries)
            {
                string blobFullName = customUiFolder + zipEntry.FullName;
                var entryStream = zipEntry.Open();
                StorageSupport.CurrActiveContainer.UploadBlobStream(blobFullName, entryStream);
            }
        }

        public static void ExecuteMethod_StoreObject(GroupContainer groupContainer)
        {
            groupContainer.StoreInformation();
        }

        public static void ExecuteMethod_ValidateCustomUIName(string customUiName)
        {
            bool hasInvalidCharacters = customUiName.Any(ch => char.IsLetterOrDigit(ch) == false);
            if(hasInvalidCharacters)
                throw new InvalidDataException("Custom UI name is invalid (contains non-alphanumeric character(s)): " + customUiName);
        }
    }
}