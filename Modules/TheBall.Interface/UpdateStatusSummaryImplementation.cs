using System;
using System.Linq;
using TheBall.CORE;

namespace TheBall.Interface
{
    public class UpdateStatusSummaryImplementation
    {
        public static InformationChangeItem GetTarget_ChangeItem(IContainerOwner owner, DateTime startTime, DateTime endTime, string[] changedIdList)
        {
            startTime = startTime.ToUniversalTime();
            endTime = endTime.ToUniversalTime();
            string id = string.Format("{0}_{1}_{2}",
                                      startTime.ToString("yyyy-MM-dd_HH-mm-ss"),
                                      endTime.ToString("yyyy-MM-dd_HH-mm-ss"),
                                      Guid.NewGuid().ToString());
            InformationChangeItem changeItem = new InformationChangeItem();
            changeItem.ID = id;
            changeItem.SetLocationAsOwnerContent(owner, id);
            changeItem.StartTimeUTC = startTime;
            changeItem.EndTimeUTC = endTime;
            changeItem.ChangedObjectIDList.AddRange(changedIdList);
            return changeItem;
        }

        public static void ExecuteMethod_StoreObject(InformationChangeItem changeItem)
        {
            changeItem.StoreInformation();
        }

        public static void ExecuteMethod_EnsureUpdateOnStatusSummary(IContainerOwner owner, DateTime updateTime, string[] changedIdList, int removeExpiredEntriesSeconds)
        {
            int retryCount = 10;
            while (retryCount-- >= 0)
            {
                try
                {
                    var statusSummary = StatusSummary.RetrieveFromOwnerContent(owner, "default");
                    if (statusSummary == null)
                    {
                        statusSummary = new StatusSummary();
                        statusSummary.SetLocationAsOwnerContent(owner, "default");
                    }
                    string latestTimestampEntry = statusSummary.ChangeItemTrackingList.FirstOrDefault();
                    long currentTimestampTicks = updateTime.ToUniversalTime().Ticks;
                    if (latestTimestampEntry != null)
                    {
                        long latestTimestampTicks = Convert.ToInt64(latestTimestampEntry.Substring(2));
                        if (currentTimestampTicks <= latestTimestampTicks)
                            currentTimestampTicks = latestTimestampTicks + 1;
                    }
                    string currentTimestampEntry = "T:" + currentTimestampTicks;
                    var timestampedList = statusSummary.ChangeItemTrackingList;
                    // Remove possible older entries of new IDs
                    timestampedList.RemoveAll(changedIdList.Contains);
                    // Add Timestamp and new IDs
                    timestampedList.Insert(0, currentTimestampEntry);
                    timestampedList.InsertRange(1, changedIdList);
                    var removeOlderThanTicks = currentTimestampTicks -
                                               TimeSpan.FromSeconds(removeExpiredEntriesSeconds).Ticks;
                    int firstBlockToRemoveIX = timestampedList.FindIndex(candidate =>
                        {
                            if (candidate.StartsWith("T:"))
                            {
                                long candidateTicks = Convert.ToInt64(candidate.Substring(2));
                                return candidateTicks < removeOlderThanTicks;
                            }
                            return false;
                        });
                    if (firstBlockToRemoveIX > -1)
                    {
                        timestampedList.RemoveRange(firstBlockToRemoveIX, timestampedList.Count - firstBlockToRemoveIX);
                    }
                    statusSummary.StoreInformation();
                    return; // Break from while loop
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        public static void ExecuteMethod_RemoveExpiredEntries(IContainerOwner owner, string[] ensureUpdateOnStatusSummaryOutput)
        {
            foreach (string changeItemID in ensureUpdateOnStatusSummaryOutput)
            {
                string relativeLocationFromOwner = InformationChangeItem.GetRelativeLocationFromID(changeItemID);
                var blob = StorageSupport.GetOwnerBlobReference(owner, relativeLocationFromOwner);
                blob.DeleteWithoutFiringSubscriptions();
                var jsonBlob = StorageSupport.GetOwnerBlobReference(owner, relativeLocationFromOwner + ".json");
                jsonBlob.DeleteWithoutFiringSubscriptions();
            }
        }
    }
}