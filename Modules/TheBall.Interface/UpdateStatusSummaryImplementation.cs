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

        public static string[] ExecuteMethod_EnsureUpdateOnStatusSummary(IContainerOwner owner, int removeExpiredEntriesSeconds, InformationChangeItem changeItem)
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
                    statusSummary.RecentChangeItemIDs.Add(changeItem.ID);
                    string removeBeforeString =
                        DateTime.UtcNow.AddSeconds(-removeExpiredEntriesSeconds).ToString("yyyy-MM-dd_HH-mm-ss");
                    var idsToRemove =
                        statusSummary.RecentChangeItemIDs.Where(
                            candidate => String.CompareOrdinal(candidate, removeBeforeString) < 0).ToArray();
                    statusSummary.RecentChangeItemIDs.RemoveAll(
                        candidate => String.CompareOrdinal(candidate, removeBeforeString) < 0);
                    statusSummary.RecentChangeItemIDs.Sort();
                    statusSummary.StoreInformation();
                    return idsToRemove;
                }
                catch (Exception ex)
                {
                    
                }
            }
            return new string[0];
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