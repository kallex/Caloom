using System;
using System.Collections.Generic;

namespace TheBall.CORE
{
    partial class UsageMonitorItem
    {
        public void InitializeSteppedCollections()
        {
            int totalMinutes = (int) (TimeRangeInclusiveStartExclusiveEnd.EndTime -
                                      TimeRangeInclusiveStartExclusiveEnd.StartTime).TotalMinutes;
            var collSizes = totalMinutes - StepSizeInMinutes;
            ProcessorUsages = new ProcessorUsageCollection();
            StorageTransactionUsages = new StorageTransactionUsageCollection();
            StorageUsages = new StorageUsageCollection();
            NetworkUsages = new NetworkUsageCollection();

            for (int i = 0; i < collSizes; i++)
            {
                var processorUsage = new ProcessorUsage {TimeRange = new TimeRange()};
                processorUsage.TimeRange = new TimeRange {StartTime = DateTime.MinValue, EndTime = DateTime.MinValue};
                ProcessorUsages.CollectionContent.Add(processorUsage);
                var storageTransactionUsage = new StorageTransactionUsage();
                StorageTransactionUsages.CollectionContent.Add(storageTransactionUsage);
                var networkUsage = new NetworkUsage();
                NetworkUsages.CollectionContent.Add(networkUsage);
                var storageUsage = new StorageUsage();
                StorageUsages.CollectionContent.Add(storageUsage);
            }
        }
    }
}