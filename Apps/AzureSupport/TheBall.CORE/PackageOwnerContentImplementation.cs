using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Ionic.Zip;
using Microsoft.WindowsAzure.StorageClient;

namespace TheBall.CORE
{
    public class PackageOwnerContentImplementation
    {
        public static ContentPackage GetTarget_ContentPackageObject(IContainerOwner owner, string packageType, string packageName, string description, string packageRootFolder)
        {
            ContentPackage contentPackage = new ContentPackage
                {
                    PackageName = packageName,
                    PackageType = packageType,
                    Description = description,
                    PackageRootFolder = packageRootFolder,
                    CreationTime = DateTime.UtcNow
                };
            contentPackage.SetLocationAsOwnerContent(owner, contentPackage.ID);
            return contentPackage;
        }

        public static void ExecuteMethod_StoreObject(ContentPackage contentPackageObject)
        {
            contentPackageObject.StoreInformation();
        }

        public static string[] ExecuteMethod_CreateZipPackageContent(string[] includedFolders, ContentPackage contentPackageObject, CloudBlockBlob[] archiveSourceBlobs, CloudBlockBlob archiveBlob)
        {
            IContainerOwner owner = VirtualOwner.FigureOwner(contentPackageObject);
            MemoryStream bufferStream = new MemoryStream();

            string packageRootFolder = owner.PrefixWithOwnerLocation(contentPackageObject.PackageRootFolder) + "/";
            int rootFolderPrefixLength = packageRootFolder.Length;
            foreach (var sourceBlob in archiveSourceBlobs)
            {
                string archiveName = sourceBlob.Name.Substring(rootFolderPrefixLength);

            }
            return null;
        }

        public static PackageOwnerContentReturnValue Get_ReturnValue(ContentPackage contentPackageObject)
        {
            return new PackageOwnerContentReturnValue {ContentPackage = contentPackageObject};
        }

        public static CloudBlockBlob GetTarget_ArchiveBlob(ContentPackage contentPackageObject)
        {
            string blobName = contentPackageObject.RelativeLocation + ".zip";
            IContainerOwner owner = VirtualOwner.FigureOwner(contentPackageObject);
            var blob = (CloudBlockBlob) StorageSupport.CurrActiveContainer.GetBlob(blobName, owner);
            blob.Attributes.Properties.ContentType = StorageSupport.GetMimeType(".zip");
            return blob;
        }

        public static void ExecuteMethod_CommitArchiveBlob(CloudBlockBlob archiveBlob, string[] createZipPackageContentOutput)
        {
            archiveBlob.PutBlockList(createZipPackageContentOutput);
        }

        public static CloudBlockBlob[] GetTarget_ArchiveSourceBlobs(IContainerOwner owner, string packageRootFolder, string[] includedFolders)
        {
            var acceptableFolders =
                includedFolders.Select(
                    folder => owner.PrefixWithOwnerLocation(packageRootFolder + "/" + includedFolders)).ToArray();
            var blobList = owner.ListBlobsWithPrefix(packageRootFolder)
                                .Cast<CloudBlockBlob>().Where(blob =>
                                    {
                                        bool accepted = acceptableFolders.Any(folder => blob.Name.StartsWith(folder));
                                        return accepted;
                                    }).ToArray();
            return blobList;
        }
    }
}