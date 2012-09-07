using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure.StorageClient;

namespace TheBall
{
    public static class WorkerSupport
    {
        class BlobCopyItem
        {
            public CloudBlob SourceBlob;
            public CloudBlob TargetBlob;
        }

        public enum SyncOperationType
        {
            Copy,
            Delete,
        }

        public delegate bool PerformCustomOperation(CloudBlob source, CloudBlob target, SyncOperationType operationType);

        public static void ExecuteSubscription(Subscription subscription)
        {
            if (String.IsNullOrEmpty(subscription.SubscriptionType))
                return;
            if (subscription.SubscriptionType == SubscribeSupport.SubscribeType_WebPageToSource)
            {
                CloudBlob cloudBlob =
                    StorageSupport.CurrActiveContainer.GetBlockBlobReference(subscription.SubscriberRelativeLocation);
                RenderWebSupport.RefreshContent(cloudBlob, true);
            }
            else
                throw new InvalidDataException(String.Format(
                    "Unsupported subscription type {0} for object: {1} by {2}", subscription.SubscriptionType,
                    subscription.TargetRelativeLocation, subscription.SubscriberRelativeLocation));
        }

        public static int WebContentSync(string sourceContainerName, string sourcePathRoot, string targetContainerName, string targetPathRoot, PerformCustomOperation customHandler = null)
        {
            //requestOptions.BlobListingDetails = BlobListingDetails.Metadata;
            string sourceSearchRoot = sourceContainerName + "/" + sourcePathRoot;
            string targetSearchRoot = targetContainerName + "/" + targetPathRoot;
            CloudBlobContainer targetContainer = StorageSupport.CurrBlobClient.GetContainerReference(targetContainerName);
            BlobRequestOptions requestOptions = new BlobRequestOptions
                                                    {
                                                        UseFlatBlobListing = true,
                                                        BlobListingDetails = BlobListingDetails.Metadata,
                                                    };
            var sourceBlobList = StorageSupport.CurrBlobClient.ListBlobsWithPrefix(sourceSearchRoot, requestOptions).
                OfType<CloudBlob>().OrderBy(blob => blob.Name).ToArray();
            var targetBlobList = StorageSupport.CurrBlobClient.ListBlobsWithPrefix(targetSearchRoot, requestOptions).
                OfType<CloudBlob>().OrderBy(blob => blob.Name).ToArray();
            List<CloudBlob> targetBlobsToDelete;
            List<BlobCopyItem> blobCopyList;
            int sourcePathLen = sourcePathRoot.Length;
            int targetPathLen = targetPathRoot.Length;
            CompareSourceToTarget(sourceBlobList, targetBlobList, sourcePathLen, targetPathLen, 
                out blobCopyList, out targetBlobsToDelete);
            foreach(var blobToDelete in targetBlobsToDelete)
            {
                bool handled = false;
                if(customHandler != null)
                {
                    handled = customHandler(null, blobToDelete, SyncOperationType.Delete);
                }
                if(handled == false)
                    blobToDelete.DeleteIfExists();
            }
            foreach(var blobCopyItem in blobCopyList)
            {
                CloudBlob targetBlob;
                if (blobCopyItem.TargetBlob == null)
                {
                    string targetBlobName = blobCopyItem.SourceBlob.Name.Replace(sourcePathRoot, targetPathRoot);
                    targetBlob = targetContainer.GetBlobReference(targetBlobName);
                }
                else
                    targetBlob = blobCopyItem.TargetBlob;
                bool handled = false;
                Console.WriteLine("Processing sync: " + blobCopyItem.SourceBlob.Name + " => " + targetBlob.Name);
                if(customHandler != null)
                {
                    handled = customHandler(blobCopyItem.SourceBlob, targetBlob, SyncOperationType.Copy);
                }
                if(handled == false)
                    targetBlob.CopyFromBlob(blobCopyItem.SourceBlob);
            }
            return targetBlobsToDelete.Count + blobCopyList.Count;
        }

        private static void CompareSourceToTarget(CloudBlob[] sourceBlobs, CloudBlob[] targetBlobs, int sourcePathLen, int targetPathLen, out List<BlobCopyItem> blobCopyList, out List<CloudBlob> targetBlobsToDelete)
        {
            blobCopyList = new List<BlobCopyItem>();
            targetBlobsToDelete = new List<CloudBlob>();
            int currTargetIX = 0;
            int currSourceIX = 0;
            while(currSourceIX < sourceBlobs.Length && currTargetIX < targetBlobs.Length)
            {
                CloudBlob currSourceItem = sourceBlobs[currSourceIX];
                CloudBlob currTargetItem = targetBlobs[currTargetIX];
                string sourceCompareName = currSourceItem.Name.Substring(sourcePathLen).ToLower();
                string targetCompareName = currTargetItem.Name.Substring(targetPathLen).ToLower();
                int compareResult = String.Compare(sourceCompareName, targetCompareName);
                bool namesMatch = compareResult == 0;
                bool sourceButNoTarget = compareResult < 0;
                if (namesMatch)
                {
                    // Compare blob contents
                    bool sourceIsTemplate = currSourceItem.GetBlobInformationType() ==
                                            StorageSupport.InformationType_WebTemplateValue;
                    bool targetIsTemplate = currTargetItem.GetBlobInformationType() ==
                                            StorageSupport.InformationType_WebTemplateValue;
                    if ((sourceIsTemplate && !targetIsTemplate) || currSourceItem.Properties.ContentMD5 != currTargetItem.Properties.ContentMD5)
                        blobCopyList.Add(new BlobCopyItem
                                             {
                                                 SourceBlob = currSourceItem,
                                                 TargetBlob = currTargetItem
                                             });
                    currSourceIX++;
                    currTargetIX++;
                }
                else if (sourceButNoTarget)
                {
                    blobCopyList.Add(new BlobCopyItem
                                         {
                                             SourceBlob = currSourceItem,
                                             TargetBlob = null
                                         });
                    currSourceIX++;
                }
                else // target but no source
                {
                    targetBlobsToDelete.Add(currTargetItem);
                    currTargetIX++;
                }
            }
            // if the target and source ixs are matched exit here
            if (currSourceIX == sourceBlobs.Length && currTargetIX == targetBlobs.Length)
                return;

            // Delete targets blobs that weren't enumerated before the sources did end
            while(currTargetIX < targetBlobs.Length)
                targetBlobsToDelete.Add(targetBlobs[currTargetIX++]);

            // Copy the source blobs that weren't enumerated before the targets did end
            while(currSourceIX < sourceBlobs.Length)
                blobCopyList.Add(new BlobCopyItem
                                     {
                                         SourceBlob = sourceBlobs[currSourceIX++],
                                         TargetBlob = null
                                     });
        }

        private static int counter = 0;
        public static void ProcessMessage(QueueEnvelope envelope)
        {
            if (envelope.UpdateWebContentOperation != null)
                ProcessUpdateWebContent(envelope.UpdateWebContentOperation);
            if (envelope.SubscriberNotification != null)
                WorkerSupport.ExecuteSubscription(envelope.SubscriberNotification);
            counter++;
            if (counter >= 1000)
            {
                QueueSupport.ReportStatistics("Processed " + counter + " messages...");
                counter = 0;
            }

        }

        public static void ProcessUpdateWebContent(UpdateWebContentOperation operation)
        {
            string sourceContainerName = operation.SourceContainerName;
            string sourcePathRoot = operation.SourcePathRoot;
            string targetContainerName = operation.TargetContainerName;
            string targetPathRoot = operation.TargetPathRoot;
            bool renderWhileSync = operation.RenderWhileSync;
            WorkerSupport.WebContentSync(sourceContainerName, sourcePathRoot, targetContainerName, targetPathRoot,
                                         renderWhileSync
                                             ? (WorkerSupport.PerformCustomOperation)RenderWebSupport.RenderingSyncHandler
                                             : (WorkerSupport.PerformCustomOperation)RenderWebSupport.CopyAsIsSyncHandler);
        }


    }
}