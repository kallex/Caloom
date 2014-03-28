using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.StorageClient;
using TheBall.CORE;

namespace TheBall.Interface
{
    public class SyncConnectionContentToDeviceToSendImplementation
    {
        public static string GetTarget_PackageContentListingProcessID(Connection connection)
        {
            return connection.ProcessIDToListPackageContents;
        }

        public static void ExecuteMethod_ExecuteContentListingProcess(string packageContentListingProcessId)
        {
            ExecuteProcess.Execute(new ExecuteProcessParameters {ProcessID = packageContentListingProcessId});
        }

        public static Process GetTarget_PackageContentListingProcess(string packageContentListingProcessId)
        {
            return Process.RetrieveFromOwnerContent(InformationContext.CurrentOwner, packageContentListingProcessId);
        }

        public static ContentItemLocationWithMD5[] GetTarget_ContentListingResult(Process packageContentListingProcess)
        {
            List<ContentItemLocationWithMD5> contentList = new List<ContentItemLocationWithMD5>();
            foreach (var processItem in packageContentListingProcess.ProcessItems)
            {
                var contentLocation = processItem.Outputs.First(item => item.ItemFullType == "ContentLocation").ItemValue;
                var contentMD5 = processItem.Outputs.First(item => item.ItemFullType == "ContentMD5").ItemValue;
                contentList.Add(new ContentItemLocationWithMD5
                    {
                        ContentLocation = contentLocation,
                        ContentMD5 = contentMD5
                    });
            }
            return contentList.ToArray();
        }

        public static string GetTarget_SyncTargetRootFolder(Connection connection)
        {
            if(string.IsNullOrEmpty(connection.DeviceID))
                throw new InvalidDataException("Connection requires device ID");
            var ownerTargetDeviceLocation = string.Format("TheBall.CORE/AuthenticatedAsActiveDevice/{0}/",
                                                          connection.DeviceID);
            return ownerTargetDeviceLocation;
        }

        public static void ExecuteMethod_CopyContentsToSyncRoot(ContentItemLocationWithMD5[] contentListingResult, string syncTargetRootFolder)
        {
            var blobListing = InformationContext.CurrentOwner.GetOwnerBlobListing(syncTargetRootFolder, true);
            string fullRootPath = StorageSupport.GetOwnerContentLocation(InformationContext.CurrentOwner, syncTargetRootFolder);
            int fullRootPathLength = fullRootPath.Length;
            ContentItemLocationWithMD5[] targetContents = (from CloudBlockBlob blob in blobListing
                                                               let relativeName = blob.Name.Substring(fullRootPathLength)
                                                               select new ContentItemLocationWithMD5
                                                                   {
                                                                       ContentLocation = relativeName,
                                                                       ContentMD5 = blob.Properties.ContentMD5
                                                                   }).OrderBy(item => item.ContentLocation).ToArray();
            var sourceContents = contentListingResult.OrderBy(item => item.ContentLocation).ToArray();
            foreach (var sourceContent in sourceContents)
            {
                sourceContent.ContentLocation = StorageSupport.RemoveOwnerPrefixIfExists(sourceContent.ContentLocation);
            }
            int currSourceIX = 0;
            int currTargetIX = 0;
            while (currSourceIX < sourceContents.Length || currTargetIX < targetContents.Length)
            {
                var currSource = currSourceIX < sourceContents.Length ? sourceContents[currSourceIX] : null;
                var currTarget = currTargetIX < targetContents.Length ? targetContents[currTargetIX] : null;
                string currTargetBlobLocation = null;
                string currSourceBlobLocation = null;
                if (currSource != null && currTarget != null)
                {
                    if (currSource.ContentLocation == currTarget.ContentLocation)
                    {
                        currSourceIX++;
                        currTargetIX++;
                        if (currSource.ContentMD5 == currTarget.ContentMD5)
                            continue;
                        currSourceBlobLocation = StorageSupport.GetOwnerContentLocation(InformationContext.CurrentOwner, currSource.ContentLocation);
                        currTargetBlobLocation = fullRootPath + currTarget.ContentLocation;
                    }
                    else if (System.String.CompareOrdinal(currSource.ContentLocation, currTarget.ContentLocation) < 0)
                    {
                        currSourceIX++;
                        currSourceBlobLocation = StorageSupport.GetOwnerContentLocation(InformationContext.CurrentOwner, currSource.ContentLocation);
                        currTargetBlobLocation = fullRootPath + currSource.ContentLocation;
                    }
                    else // source == null, target != null
                    {
                        currTargetIX++;
                        currTargetBlobLocation = fullRootPath + currTarget.ContentLocation;
                    }
                }
                else if (currSource != null)
                {
                    currSourceIX++;
                    currSourceBlobLocation = StorageSupport.GetOwnerContentLocation(InformationContext.CurrentOwner, currSource.ContentLocation);
                    currTargetBlobLocation = fullRootPath + currSource.ContentLocation;
                }
                else if (currTarget != null)
                    currTargetBlobLocation = fullRootPath + currTarget.ContentLocation;

                // at this stage we have either both set (that's copy) or just target set (that's delete)
                if (currSourceBlobLocation != null && currTargetBlobLocation != null)
                    copySourceToTarget(currSourceBlobLocation, currTargetBlobLocation);
                else if (currTargetBlobLocation != null)
                    deleteCurrTarget(currTargetBlobLocation);

            }
        }

        private static void deleteCurrTarget(string currTargetBlobLocation)
        {
            CloudBlockBlob blob = (CloudBlockBlob) StorageSupport.GetOwnerBlobReference(InformationContext.CurrentOwner, currTargetBlobLocation);
            blob.DeleteWithoutFiringSubscriptions();
        }

        private static void copySourceToTarget(string currSourceBlobLocation, string currTargetBlobLocation)
        {
            CloudBlockBlob targetBlob = (CloudBlockBlob) StorageSupport.GetOwnerBlobReference(InformationContext.CurrentOwner, currTargetBlobLocation);
            CloudBlockBlob sourceblob = (CloudBlockBlob) StorageSupport.GetOwnerBlobReference(InformationContext.CurrentOwner, currSourceBlobLocation);
            targetBlob.CopyFromBlob(sourceblob);
        }
    }
}