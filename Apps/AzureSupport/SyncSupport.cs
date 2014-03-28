using System;
using System.Linq;
using Microsoft.WindowsAzure.StorageClient;
using TheBall.CORE;

namespace TheBall
{
    public static class SyncSupport
    {

        public static void SynchronizeSourceListToTargetFolder(ContentItemLocationWithMD5[] sourceContentList, string syncTargetRootFolder, 
            CopySourceToTargetMethod copySourceToTarget = null, DeleteObsoleteTargetMethod deleteObsoleteTarget = null)
        {
            if (copySourceToTarget == null)
                copySourceToTarget = CopySourceToTarget;
            if (deleteObsoleteTarget == null)
                deleteObsoleteTarget = DeleteObsoleteTarget;

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
            var sourceContents = sourceContentList.OrderBy(item => item.ContentLocation).ToArray();
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
                    else if (String.CompareOrdinal(currSource.ContentLocation, currTarget.ContentLocation) < 0)
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
                    deleteObsoleteTarget(currTargetBlobLocation);

            }
        }

        public delegate void DeleteObsoleteTargetMethod(string currTargetBlobLocation);

        public delegate void CopySourceToTargetMethod(string currSourceBlobLocation, string currTargetBlobLocation);

        public static void DeleteObsoleteTarget(string currTargetBlobLocation)
        {
            CloudBlockBlob blob = (CloudBlockBlob)StorageSupport.GetOwnerBlobReference(InformationContext.CurrentOwner, currTargetBlobLocation);
            blob.DeleteWithoutFiringSubscriptions();
        }

        public static void CopySourceToTarget(string currSourceBlobLocation, string currTargetBlobLocation)
        {
            CloudBlockBlob targetBlob = (CloudBlockBlob)StorageSupport.GetOwnerBlobReference(InformationContext.CurrentOwner, currTargetBlobLocation);
            CloudBlockBlob sourceblob = (CloudBlockBlob)StorageSupport.GetOwnerBlobReference(InformationContext.CurrentOwner, currSourceBlobLocation);
            targetBlob.CopyFromBlob(sourceblob);
        }
    }
}