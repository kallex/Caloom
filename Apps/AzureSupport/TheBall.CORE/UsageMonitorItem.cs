using System;
using System.Collections.Generic;
using System.IO;

namespace TheBall.CORE
{
    partial class UsageMonitorItem
    {
        public static UsageMonitorItem Create(DateTime startTime, DateTime endTime, int stepSizeInMinutes)
        {
            UsageMonitorItem item = new UsageMonitorItem
                {
                    TimeRangeInclusiveStartExclusiveEnd = new TimeRange
                        {
                            StartTime = startTime,
                            EndTime = endTime
                        },
                    StepSizeInMinutes = stepSizeInMinutes
                };
            item.InitializeSteppedCollections();
            return item;
        }

        public void InitializeSteppedCollections()
        {
            int totalMinutes = (int) (TimeRangeInclusiveStartExclusiveEnd.EndTime -
                                      TimeRangeInclusiveStartExclusiveEnd.StartTime).TotalMinutes;
            var collSizes = totalMinutes / StepSizeInMinutes;
            ProcessorUsages = new ProcessorUsageCollection();
            StorageTransactionUsages = new StorageTransactionUsageCollection();
            StorageUsages = new StorageUsageCollection();
            NetworkUsages = new NetworkUsageCollection();

            for (int i = 0; i < collSizes; i++)
            {
                var processorUsage = new ProcessorUsage();
                processorUsage.TimeRange = new TimeRange { StartTime = DateTime.MinValue.ToUniversalTime(), EndTime = DateTime.MinValue.ToUniversalTime() };
                ProcessorUsages.CollectionContent.Add(processorUsage);
                var storageTransactionUsage = new StorageTransactionUsage();
                StorageTransactionUsages.CollectionContent.Add(storageTransactionUsage);
                var networkUsage = new NetworkUsage();
                NetworkUsages.CollectionContent.Add(networkUsage);
                var storageUsage = new StorageUsage();
                storageUsage.SnapshotTime = DateTime.MinValue.ToUniversalTime();
                StorageUsages.CollectionContent.Add(storageUsage);
            }
        }

        public void AggregateValuesFrom(UsageMonitorItem[] sourceItems)
        {
            foreach (var item in sourceItems)
            {
                DateTime sourceStart = item.TimeRangeInclusiveStartExclusiveEnd.StartTime;
                DateTime sourceEnd = item.TimeRangeInclusiveStartExclusiveEnd.EndTime;
                DateTime targetStart = TimeRangeInclusiveStartExclusiveEnd.StartTime;
                DateTime targetEnd = TimeRangeInclusiveStartExclusiveEnd.EndTime;
                if(sourceStart < targetStart || sourceEnd > targetEnd)
                    throw new InvalidDataException("The aggregation target needs to contain full source item date range");
                for (int sourceIX = 0; sourceIX < item.ProcessorUsages.CollectionContent.Count; sourceIX++)
                {
                    DateTime currSourceStart = sourceStart.AddMinutes(item.StepSizeInMinutes*sourceIX);
                    var timeDelta = currSourceStart - targetStart;
                    int targetIX = (int) (timeDelta.TotalMinutes/StepSizeInMinutes);
                    addProcessorUsageToTarget(ProcessorUsages.CollectionContent[targetIX],
                                              item.ProcessorUsages.CollectionContent[sourceIX]);
                    addStorageTransactionUsageToTraget(StorageTransactionUsages.CollectionContent[targetIX],
                                                       item.StorageTransactionUsages.CollectionContent[sourceIX]);
                    addNetworkUsageToTarget(NetworkUsages.CollectionContent[targetIX],
                                            item.NetworkUsages.CollectionContent[sourceIX]);
                    setStorageUsageToTarget(StorageUsages.CollectionContent[targetIX],
                                            item.StorageUsages.CollectionContent[sourceIX]);
                }
            }
        }

        private void setStorageUsageToTarget(StorageUsage target, StorageUsage source)
        {
            target.AmountOfUnits = source.AmountOfUnits;
            target.SnapshotTime = source.SnapshotTime;
            if (target.SnapshotTime == DateTime.MinValue)
                target.SnapshotTime = DateTime.MinValue.ToUniversalTime();
        }

        private void addNetworkUsageToTarget(NetworkUsage target, NetworkUsage source)
        {
            target.AmountOfBytes += source.AmountOfBytes;
        }

        private void addStorageTransactionUsageToTraget(StorageTransactionUsage target, StorageTransactionUsage source)
        {
            target.AmountOfTransactions += source.AmountOfTransactions;
        }

        private void addProcessorUsageToTarget(ProcessorUsage target, ProcessorUsage source)
        {
            target.TimeRange.EndTime = target.TimeRange.EndTime.AddTicks(source.TimeRange.EndTime.Ticks);
            target.Milliseconds += source.Milliseconds;
        }
    }
}