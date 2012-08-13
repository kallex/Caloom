

using System.IO;
using System.Runtime.Serialization;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure.StorageClient;

namespace TheBall
{
    public static class SubscribeSupport
    {
        public static void AddSubscriptionToObject(TableServiceEntity target, TableServiceEntity subscriber, string operationName)
        {
            var sub = GetSubscriptionToObject(target, subscriber, operationName);
            SubscriptionCollection subscriptionCollection = GetSubscriptions(target);
            if(subscriptionCollection == null)
            {
                subscriptionCollection = new SubscriptionCollection();
                subscriptionCollection.PartitionKey = target.PartitionKey;
                subscriptionCollection.RowKey = target.RowKey;
            }
            subscriptionCollection.CollectionContent.Add(sub);
            DataContractSerializer ser = new DataContractSerializer(typeof(SubscriptionCollection));
            MemoryStream memoryStream = new MemoryStream();
            ser.WriteObject(memoryStream, subscriptionCollection);
            memoryStream.Seek(0, SeekOrigin.Begin);
            CloudBlob blob = AzureSupport.CurrSubscriberContainer.GetBlobReference(subscriptionCollection.GetBlobPath());
            blob.UploadFromStream(memoryStream);
        }

        public static void AddSubscriptionToItem(TableServiceEntity target, string itemName, TableServiceEntity subscriber, string operationName)
        {
            var sub = GetSubscriptionToItem(target, itemName, subscriber, operationName);
        }

        public static Subscription GetSubscriptionToObject(TableServiceEntity target, TableServiceEntity subscriber, string operationName)
        {
            var sub = new Subscription
                                   {
                                       TargetItemObjectName = target.GetType().Name,
                                       TargetItemPartitionKey = target.PartitionKey,
                                       TargetItemRowKey = target.RowKey,
                                       SubscriberPartitionKey = subscriber.PartitionKey,
                                       SubscriberRowKey = subscriber.RowKey,
                                       OperationActionName = operationName
                                   };
            return sub;
        }

        public static Subscription GetSubscriptionToItem(TableServiceEntity target, string itemName, TableServiceEntity subscriber, string operationName)
        {
            Subscription sub = GetSubscriptionToObject(target, subscriber, operationName);
            sub.TargetItemFieldName = itemName;
            return sub;
        }

        public static SubscriptionCollection GetSubscriptions(TableServiceEntity target)
        {
            string blobPath = SubscriptionCollection.GetBlobPath(target.PartitionKey, target.RowKey);
            CloudBlob blob = AzureSupport.CurrSubscriberContainer.GetBlobReference(blobPath);
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                blob.DownloadToStream(memoryStream);
            } catch(StorageClientException stEx)
            {
                if (stEx.ErrorCode == StorageErrorCode.BlobNotFound)
                    return null;
                throw;
            }
            //if (memoryStream.Length == 0)
            //    return null;
            memoryStream.Seek(0, SeekOrigin.Begin);
            DataContractSerializer serializer = new DataContractSerializer(typeof(SubscriptionCollection));
            SubscriptionCollection subscriptionCollection = (SubscriptionCollection) serializer.ReadObject(memoryStream);
            return subscriptionCollection;
        }
    }
}