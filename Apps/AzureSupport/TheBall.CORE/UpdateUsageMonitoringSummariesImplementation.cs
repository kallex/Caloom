using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.StorageClient;

namespace TheBall.CORE
{
    public class UpdateUsageMonitoringSummariesImplementation
    {
        public static UsageMonitorItem[] GetTarget_SourceItems(IContainerOwner owner, int amountOfDays)
        {
            string filterPrefix = "TheBall.CORE/UsageMonitorItem/";
            DateTime today = DateTime.UtcNow.Date;
            List<UsageMonitorItem> result = new List<UsageMonitorItem>();
            Type type = typeof (UsageMonitorItem);
            for (DateTime fromDate = today.AddDays(-amountOfDays); fromDate <= today; fromDate = fromDate.AddDays(1))
            {
                string dateStr = fromDate.ToString("yyyyMMdd");
                var dayBlobs = owner.ListBlobsWithPrefix(filterPrefix + dateStr).Cast<CloudBlockBlob>().ToArray();
                foreach (var blob in dayBlobs)
                {
                    UsageMonitorItem item = (UsageMonitorItem) StorageSupport.RetrieveInformation(blob.Name, type);
                    result.Add(item);
                }
            }
            return result.ToArray();
        }

        public static void ExecuteMethod_CreateUsageMonitoringSummaries(IContainerOwner owner, UsageMonitorItem[] sourceItems)
        {
            var groupedByDay =
                sourceItems.OrderBy(item => item.RelativeLocation)
                           .GroupBy(item => item.TimeRangeInclusiveStartExclusiveEnd.StartTime.Date);
            foreach (var dayList in groupedByDay)
            {
                //UsageMonitorItemCollection hourlyCollection = new UsageMonitorItemCollection();
                //hourlyCollection.
                string hourlySummaryName = "Hourly Summary of " + dayList.Key.ToShortDateString();
                DateTime startTime = dayList.Key;
                DateTime endTime = startTime.AddDays(1);
                UsageSummary hourlySummary = new UsageSummary
                    {
                        SummaryName = hourlySummaryName,
                        SummaryMonitoringItem = UsageMonitorItem.Create(startTime, endTime, 60)
                    };
                hourlySummary.SummaryMonitoringItem.AggregateValuesFrom(dayList.ToArray());
                string prefixName = startTime.ToString("yyyyMMdd");
                hourlySummary.SetLocationAsOwnerContent(owner, prefixName + "_Hourly");
                hourlySummary.StoreInformation(null, true);
                string detailedSummaryName = "Detailed (5 min) Summary of " + dayList.Key.ToShortDateString();
                UsageSummary detailedSummary = new UsageSummary
                    {
                        SummaryName = detailedSummaryName,
                        SummaryMonitoringItem = UsageMonitorItem.Create(startTime, endTime, 5)
                    };
                detailedSummary.SummaryMonitoringItem.AggregateValuesFrom(dayList.ToArray());
                detailedSummary.SetLocationAsOwnerContent(owner, prefixName + "_Detailed");
                detailedSummary.StoreInformation(null, true);
            }
        }
    }
}