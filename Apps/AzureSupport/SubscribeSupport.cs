

using System.IO;
using System.Runtime.Serialization;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure.StorageClient;

namespace TheBall
{
    public static class SubscribeSupport
    {
        public static void AddSubscriptionToObject(IInformationObject target, IInformationObject subscriber, string operationName)
        {
            var sub = GetSubscriptionToObject(target, subscriber, operationName);
            SubscriptionCollection subscriptionCollection = GetSubscriptions(target);
            if(subscriptionCollection == null)
            {
                subscriptionCollection = new SubscriptionCollection();
                subscriptionCollection.SetRelativeLocationTo(target);
            }
            subscriptionCollection.CollectionContent.Add(sub);
            AzureSupport.StoreInformation(subscriptionCollection, null);
        }

        public static void AddSubscriptionToItem(IInformationObject target, string itemName, IInformationObject subscriber, string operationName)
        {
            var sub = GetSubscriptionToItem(target, itemName, subscriber, operationName);
        }

        public static Subscription GetSubscriptionToObject(IInformationObject target, IInformationObject subscriber, string operationName)
        {
            var sub = new Subscription
                          {
                              TargetObjectName = target.Name,
                              TargetItemID = target.ID,
                              SubscriberID = subscriber.ID,
                              OperationActionName = operationName
                          };
            return sub;
        }

        public static Subscription GetSubscriptionToItem(IInformationObject target, string itemName, IInformationObject subscriber, string operationName)
        {
            Subscription sub = GetSubscriptionToObject(target, subscriber, operationName);
            sub.TargetItemName = itemName;
            return sub;
        }

        public static SubscriptionCollection GetSubscriptions(IInformationObject target)
        {
            string blobPath = SubscriptionCollection.GetRelativeLocationTo(target);
            var result = AzureSupport.RetrieveInformation(blobPath, typeof(SubscriptionCollection));
            return (SubscriptionCollection) result;
        }

    }
}