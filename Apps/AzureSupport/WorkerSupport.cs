using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
                try
                {
                    CloudBlob cloudBlob =
                        StorageSupport.CurrActiveContainer.GetBlockBlobReference(subscription.SubscriberRelativeLocation);
                    RenderWebSupport.RefreshContent(cloudBlob, true);
                } catch(Exception ex)
                {
                    // TODO: Detect and remove the missing subscribers
                    ErrorSupport.ReportException(ex);
                }
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
                    blobToDelete.DeleteWithoutFiringSubscriptions();
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
        public static void ProcessMessage(QueueEnvelope envelope, bool reportEnvelopeError = true)
        {
            try
            {
                if (envelope.SingleOperation != null)
                    ProcessSingleOperation(envelope.SingleOperation);
                if (envelope.OrderDependentOperationSequence != null)
                {
                    envelope.OrderDependentOperationSequence.CollectionContent.ForEach(ProcessSingleOperation);
                }
            }
            catch (Exception ex)
            {
                if (reportEnvelopeError)
                    ErrorSupport.ReportEnvelopeWithException(envelope, ex);
                throw;
            } finally
            {
                InformationContext.ProcessAndClearCurrent();
            }

            counter++;
            if (counter >= 1000)
            {
                QueueSupport.ReportStatistics("Processed " + counter + " messages...");
                counter = 0;
            }

        }

        private static void ProcessSingleOperation(OperationRequest operationRequest)
        {
            if (operationRequest.UpdateWebContentOperation != null)
                ProcessUpdateWebContent(operationRequest.UpdateWebContentOperation);
            if (operationRequest.SubscriberNotification != null)
                WorkerSupport.ExecuteSubscription(operationRequest.SubscriberNotification);
            if (operationRequest.DeleteEntireOwner != null)
            {
                VirtualOwner virtualOwner = new VirtualOwner(operationRequest.DeleteEntireOwner.ContainerName,
                    operationRequest.DeleteEntireOwner.LocationPrefix);
                DeleteEntireOwner(virtualOwner);
            }
            if(operationRequest.DeleteOwnerContent != null)
            {
                VirtualOwner virtualOwner = new VirtualOwner(operationRequest.DeleteOwnerContent.ContainerName,
                    operationRequest.DeleteOwnerContent.LocationPrefix);
                DeleteOwnerContent(virtualOwner);
            }
        }

        private static void DeleteOwnerContent(VirtualOwner containerOwner)
        {
            StorageSupport.DeleteContentsFromOwner(containerOwner);
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


        public static void DeleteEntireOwner(IContainerOwner containerOwner)
        {
            StorageSupport.DeleteEntireOwner(containerOwner);
        }

        public static Task GetFirstCompleted(Task[] tasks, out int availableIx)
        {
            Task currArrayTask = null;
            for (availableIx = 0; availableIx < tasks.Length; availableIx++)
            {
                currArrayTask = tasks[availableIx];
                if (currArrayTask.IsCompleted)
                    break;
            }
            if (currArrayTask == null)
                throw new NotSupportedException("Cannot find completed task in array when there is supposed to be one");
            return currArrayTask;
        }


    }
}