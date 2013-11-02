using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure.StorageClient;

namespace TheBall.CORE
{
    public class ProcessBatchOfResourceUsagesToOwnerCollectionsImplementation
    {
        private const string LockLocation = "sys/AAA/TheBall.CORE/RequestResourceUsage/0.lock";
        public static CloudBlockBlob[] GetTarget_BatchToProcess(int processBatchSize, bool processIfLess)
        {
            //options.
            //options.AccessCondition 
            //StorageSupport.CurrActiveContainer.ListBlobsSegmented()
            //    ListBlobsWithPrefix("sys/AAA/TheBall.CORE/RequestResourceUsage")
            string prefix = "sys/AAA/TheBall.CORE/RequestResourceUsage/";

            BlobRequestOptions options = new BlobRequestOptions {UseFlatBlobListing = true};
            var blobList = StorageSupport.CurrActiveContainer.ListBlobxWithPrefixSegmented(prefix, processBatchSize, null, options);
            List<CloudBlockBlob> result = new List<CloudBlockBlob>();
            foreach (var blobListItem in blobList.Results)
            {
                CloudBlockBlob blob = (CloudBlockBlob) blobListItem;
                if (blob.Name == LockLocation)
                    return null;
                result.Add(blob);
            }
            if (result.Count < processBatchSize && processIfLess == false)
                return null;
            // Acquire Lock
            string lockETag;
            bool acquiredLock = StorageSupport.AcquireLogicalLockByCreatingBlob(LockLocation, out lockETag);
            if (!acquiredLock)
                return null;
            return result.ToArray();
        }

        public static void ExecuteMethod_ProcessBatch(CloudBlockBlob[] batchToProcess)
        {
            if (batchToProcess == null)
                return;
            Dictionary<string, List<RequestResourceUsage>> ownerGroupedUsages = new Dictionary<string, List<RequestResourceUsage>>();
            Type type = typeof (RequestResourceUsage);
            int i = 0;
            foreach (var blob in batchToProcess)
            {
                i++;
                Debug.WriteLine("Reading resource " + i + ": " + blob.Name);
                RequestResourceUsage resourceUsage = (RequestResourceUsage) StorageSupport.RetrieveInformation(blob.Name, type);
                addResourceUsageToOwner(resourceUsage, ownerGroupedUsages);
            }
            storeOwnerContentsAsCollections(ownerGroupedUsages);
        }

        private static void storeOwnerContentsAsCollections(Dictionary<string, List<RequestResourceUsage>> ownerGroupedUsages)
        {
            var allKeys = ownerGroupedUsages.Keys;
            foreach (var ownerKey in allKeys)
            {
                IContainerOwner owner;
                if (ownerKey.StartsWith("sys/AAA"))
                    owner = TBSystem.CurrSystem;
                else
                    owner = VirtualOwner.FigureOwner(ownerKey);
                var ownerContent = ownerGroupedUsages[ownerKey];
                var firstRangeItem = ownerContent[0];
                var lastRangeItem = ownerContent[ownerContent.Count - 1];
                string collName = String.Format("{0}_{1}",
                                                firstRangeItem.ProcessorUsage.TimeRange.EndTime.ToString(
                                                    "yyyyMMddHHmmssfff"),
                                                lastRangeItem.ProcessorUsage.TimeRange.EndTime.ToString(
                                                    "yyyyMMddHHmmssfff"));
                var existing = RequestResourceUsageCollection.RetrieveFromOwnerContent(owner, collName);
                if (existing != null)
                    continue;
                RequestResourceUsageCollection ownerCollection = new RequestResourceUsageCollection();
                ownerCollection.SetLocationAsOwnerContent(owner, collName);
                ownerCollection.CollectionContent = ownerContent;
                ownerCollection.StoreInformation();
            }
        }

        private static void addResourceUsageToOwner(RequestResourceUsage resourceUsage, Dictionary<string, List<RequestResourceUsage>> ownerGroupedUsages)
        {
            string ownerPrefixKey = resourceUsage.OwnerInfo.OwnerType + "/" + resourceUsage.OwnerInfo.OwnerIdentifier;
            List<RequestResourceUsage> ownerContent = null;
            if (ownerGroupedUsages.ContainsKey(ownerPrefixKey))
            {
                ownerContent = ownerGroupedUsages[ownerPrefixKey];
            }
            else
            {
                ownerContent = new List<RequestResourceUsage>();
                ownerGroupedUsages.Add(ownerPrefixKey, ownerContent);
            }
            ownerContent.Add(resourceUsage);
        }

        public static void ExecuteMethod_DeleteProcessedItems(CloudBlockBlob[] batchToProcess)
        {
            if (batchToProcess == null)
                return;
            //int i = 0;
            //foreach (var blob in batchToProcess)
            //{
            //    i++;
            //    Debug.WriteLine("Deleting blob " + i + ": " + blob.Name);
            //    blob.DeleteWithoutFiringSubscriptions();
            //}
            int i = 0;
            ParallelOptions options = new ParallelOptions {MaxDegreeOfParallelism = 50};
            Parallel.ForEach(batchToProcess, options, blob =>
                {
                    i++;
                    Debug.WriteLine("Deleting blob " + i + ": " + blob.Name);
                    blob.DeleteIfExists(); // NOTE! Need to use direct delete so that the InformationContext is not bothered with Task usage
                });
        }

        public static void ExecuteMethod_ReleaseLock(CloudBlockBlob[] batchToProcess)
        {
            if (batchToProcess == null)
                return;
            // Release lock
            StorageSupport.ReleaseLogicalLockByDeletingBlob(LockLocation, null);
        }

        public static ProcessBatchOfResourceUsagesToOwnerCollectionsReturnValue Get_ReturnValue(int processBatchSize, CloudBlockBlob[] batchToProcess)
        {
            return new ProcessBatchOfResourceUsagesToOwnerCollectionsReturnValue
                {
                    ProcessedAnything = batchToProcess != null,
                    ProcessedFullCount = batchToProcess != null && processBatchSize == batchToProcess.Length
                };
        }
    }

}