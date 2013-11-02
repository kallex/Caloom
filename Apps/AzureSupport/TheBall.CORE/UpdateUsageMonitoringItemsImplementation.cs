using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.StorageClient;

namespace TheBall.CORE
{
    public class UpdateUsageMonitoringItemsImplementation
    {
        public static void ExecuteMethod_ValidateEqualSplitOfIntervalsInTimeSpan(int monitoringItemTimeSpanInMinutes, int monitoringIntervalInMinutes)
        {
            bool equalSplit = monitoringItemTimeSpanInMinutes % monitoringIntervalInMinutes == 0;
            if(!equalSplit)
                throw new ArgumentException("Monitoring item time span and interval need to divide on equal splits");
        }

        public static CloudBlockBlob[] GetTarget_CurrentMonitoringItems(IContainerOwner owner)
        {
            var cloudBlockBlobs =
                owner.ListBlobsWithPrefix("TheBall.CORE/UsageMonitorItem/").Cast<CloudBlockBlob>().ToArray();
            return cloudBlockBlobs;
        }

        public static DateTime GetTarget_EndingTimeOfNewItems(int monitoringItemTimeSpanInMinutes, DateTime startingTimeOfNewItems, CloudBlockBlob[] newResourceUsageBlobs)
        {
            DateTime endingTimeOfNewItems = startingTimeOfNewItems;
            if (newResourceUsageBlobs.Length > 0)
            {
                var lastBlobName = newResourceUsageBlobs[newResourceUsageBlobs.Length - 1].Name;
                DateTime startTime;
                DateTime endTime;
                getTimeRangeFromBlobName(lastBlobName, out startTime, out endTime);
                while (endingTimeOfNewItems.AddMinutes(monitoringItemTimeSpanInMinutes) < endTime)
                    endingTimeOfNewItems = endingTimeOfNewItems.AddMinutes(monitoringItemTimeSpanInMinutes);
            }
            return endingTimeOfNewItems;
        }

        private static void getTimeRangeFromBlobName(string blobName, out DateTime startTime, out DateTime endTime)
        {
            const string ExampleFormat = "yyyyMMddHHmmssfff_yyyyMMddHHmmssfff";
            string rangePart = blobName.Substring(blobName.Length - ExampleFormat.Length);
            string[] startEndStrings = rangePart.Split('_');
            startTime = DateTime.ParseExact(startEndStrings[0], "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
            endTime = DateTime.ParseExact(startEndStrings[1], "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        }

        public static RequestResourceUsageCollection[] GetTarget_ResourcesToIncludeInMonitoring(CloudBlockBlob[] newResourceUsageBlobs, DateTime endingTimeOfNewItems)
        {
            List<RequestResourceUsageCollection> includedItems = new List<RequestResourceUsageCollection>();
            Type type = typeof (RequestResourceUsageCollection);
            foreach (var blob in newResourceUsageBlobs)
            {
                DateTime startTime;
                DateTime endTime;
                getTimeRangeFromBlobName(blob.Name, out startTime, out endTime);
                if (startTime >= endingTimeOfNewItems)
                    break;
                RequestResourceUsageCollection resourceCollection = (RequestResourceUsageCollection)StorageSupport.RetrieveInformation(blob.Name, type);
                includedItems.Add(resourceCollection);
            }
            return includedItems.ToArray();
        }

        public static UsageMonitorItem[] GetTarget_NewMonitoringItems(IContainerOwner owner, int monitoringItemTimeSpanInMinutes, int monitoringIntervalInMinutes, DateTime startingTimeOfNewItems, DateTime endingTimeOfNewItems)
        {
            if(startingTimeOfNewItems == endingTimeOfNewItems)
                return new UsageMonitorItem[0];
            DateTime currentStartingTime = startingTimeOfNewItems;
            DateTime currentEndingTime = currentStartingTime.AddMinutes(monitoringItemTimeSpanInMinutes);
            List<UsageMonitorItem> usageMonitorItems = new List<UsageMonitorItem>();
            while (currentEndingTime <= endingTimeOfNewItems)
            {
                string currName = String.Format("{0}_{1}",
                                currentStartingTime.ToString("yyyyMMddHHmmssfff"),
                                currentEndingTime.ToString("yyyyMMddHHmmssfff"));

                var currItem = new UsageMonitorItem();
                currItem.SetLocationAsOwnerContent(owner, currName);
                currItem.TimeRangeInclusiveStartExclusiveEnd = new TimeRange
                    {
                        StartTime = currentStartingTime,
                        EndTime = currentEndingTime
                    };
                currItem.StepSizeInMinutes = monitoringIntervalInMinutes;
                //currItem.InitializeStepCollections();
                currItem.OwnerInfo = new InformationOwnerInfo
                    {
                        OwnerType = owner.ContainerName,
                        OwnerIdentifier = owner.LocationPrefix
                    };
                currentStartingTime = currentEndingTime;
                currentEndingTime = currentEndingTime.AddMinutes(monitoringItemTimeSpanInMinutes);
                currItem.InitializeSteppedCollections();
                usageMonitorItems.Add(currItem);
            }
            return usageMonitorItems.ToArray();
        }

        public static void ExecuteMethod_PopulateMonitoringItems(RequestResourceUsageCollection[] resourcesToIncludeInMonitoring, UsageMonitorItem[] newMonitoringItems)
        {
            if (resourcesToIncludeInMonitoring.Length == 0 || newMonitoringItems.Length == 0)
                return;
            int currMonitoringItemIX = 0;
            var resourcesCombined = resourcesToIncludeInMonitoring.SelectMany(coll => coll.CollectionContent).ToArray();
            var currMonitoringItem = newMonitoringItems[currMonitoringItemIX];
            foreach (var resourceItem in resourcesCombined)
            {
                DateTime resourceItemTime = resourceItem.ProcessorUsage.TimeRange.EndTime;
                while (resourceItemTime >= currMonitoringItem.TimeRangeInclusiveStartExclusiveEnd.EndTime)
                {
                    currMonitoringItemIX++;
                    if (currMonitoringItemIX == newMonitoringItems.Length)
                        return;
                    currMonitoringItem = newMonitoringItems[currMonitoringItemIX];
                }
                DateTime currMonitorStartTime = currMonitoringItem.TimeRangeInclusiveStartExclusiveEnd.StartTime;
                bool matchesTimes = currMonitorStartTime <= resourceItemTime &&
                                    currMonitoringItem.TimeRangeInclusiveStartExclusiveEnd.EndTime > resourceItemTime;
                if(matchesTimes == false)
                    throw new InvalidDataException("Invalid data processing to feed invalid data to populating items");
                var diffFromMonitorBegin = resourceItemTime - currMonitorStartTime;
                var completeMinutes = Math.Floor(diffFromMonitorBegin.TotalMinutes);
                int stepIndex = (int) (completeMinutes/currMonitoringItem.StepSizeInMinutes);
                // Add data to index entries
                addValuesToMonitorItem(resourceItem, currMonitoringItem, stepIndex);
            }
        }

        private static void addValuesToMonitorItem(RequestResourceUsage resourceItem, UsageMonitorItem currMonitoringItem, int stepIndex)
        {
            var srcProcessorUsage = resourceItem.ProcessorUsage;
            var tgtProcessorUsage = currMonitoringItem.ProcessorUsages.CollectionContent[stepIndex];
            tgtProcessorUsage.Milliseconds += srcProcessorUsage.Milliseconds;
            tgtProcessorUsage.TimeRange.EndTime = tgtProcessorUsage.TimeRange.EndTime.Add(srcProcessorUsage.TimeRange.EndTime -
                                                    srcProcessorUsage.TimeRange.StartTime);
            tgtProcessorUsage.AmountOfTicks += srcProcessorUsage.AmountOfTicks;

            var srcStorageTransactionUsage = resourceItem.StorageTransactionUsage;
            if (srcStorageTransactionUsage != null)
            {
                var tgtStorageTransactionUsage = currMonitoringItem.StorageTransactionUsages.CollectionContent[stepIndex];
                tgtStorageTransactionUsage.AmountOfTransactions += srcStorageTransactionUsage.AmountOfTransactions;
            }

            var srcNetworkUsage = resourceItem.NetworkUsage;
            var tgtNetworkUsage = currMonitoringItem.NetworkUsages.CollectionContent[stepIndex];
            tgtNetworkUsage.AmountOfBytes = srcNetworkUsage.AmountOfBytes;
        }

        public static void ExecuteMethod_StoreObjects(UsageMonitorItem[] newMonitoringItems)
        {
            foreach (var monitoringItem in newMonitoringItems)
                monitoringItem.StoreInformation();
        }

        public static DateTime GetTarget_EndingTimeOfCurrentItems(CloudBlockBlob[] currentMonitoringItems)
        {
            if (currentMonitoringItems.Length == 0)
                return DateTime.MinValue.ToUniversalTime();
            var lastBlobName = currentMonitoringItems[currentMonitoringItems.Length - 1].Name;
            DateTime startTime;
            DateTime endTime;
            getTimeRangeFromBlobName(lastBlobName, out startTime, out endTime);
            return endTime;
        }

        public static CloudBlockBlob[] GetTarget_NewResourceUsageBlobs(IContainerOwner owner, DateTime endingTimeOfCurrentItems)
        {
            var newBlobs = owner.ListBlobsWithPrefix("TheBall.CORE/RequestResourceUsageCollection/").Cast<CloudBlockBlob>()
                                .Where(blob =>
                                    {
                                        DateTime startTime;
                                        DateTime endTime;
                                        getTimeRangeFromBlobName(blob.Name, out startTime, out endTime);
                                        return startTime > endingTimeOfCurrentItems;
                                    }).ToArray();
            return newBlobs;

        }

        public static DateTime GetTarget_StartingTimeOfNewItems(int monitoringItemTimeSpanInMinutes, DateTime endingTimeOfCurrentItems, CloudBlockBlob[] newResourceUsageBlobs)
        {
            if (newResourceUsageBlobs.Length == 0)
                return endingTimeOfCurrentItems;
            DateTime newItemStartingTime = endingTimeOfCurrentItems;
            var firstBlobName = newResourceUsageBlobs[0].Name;
            DateTime blobStartTime;
            DateTime blobEndTime;
            getTimeRangeFromBlobName(firstBlobName, out blobStartTime, out blobEndTime);
            if (newItemStartingTime.Date != blobStartTime.Date)
            {
                // if new item starting time is not same date, let's seek "closest" timespan minute from beginning of day
                newItemStartingTime = blobStartTime.Date;
                while (newItemStartingTime.AddMinutes(monitoringItemTimeSpanInMinutes) < blobStartTime)
                    newItemStartingTime = newItemStartingTime.AddMinutes(monitoringItemTimeSpanInMinutes);
            }
            return newItemStartingTime;
        }
    }
}