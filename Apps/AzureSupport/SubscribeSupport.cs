

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
                subscriptionCollection.ID = target.ID;
            }
            subscriptionCollection.CollectionContent.Add(sub);
            DataContractSerializer ser = new DataContractSerializer(typeof(SubscriptionCollection));
            MemoryStream memoryStream = new MemoryStream();
            ser.WriteObject(memoryStream, subscriptionCollection);
            memoryStream.Seek(0, SeekOrigin.Begin);
            CloudBlob blob = AzureSupport.CurrSubscriberContainer.GetBlobReference(subscriptionCollection.GetBlobPath());
            blob.UploadFromStream(memoryStream);
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
            string blobPath = SubscriptionCollection.GetBlobPath(target.ID);
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