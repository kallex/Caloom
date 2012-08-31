

using System;
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
            StorageSupport.StoreInformation(subscriptionCollection);
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
            var result = StorageSupport.RetrieveInformation(blobPath, typeof(SubscriptionCollection));
            return (SubscriptionCollection) result;
        }

        public static void NotifySubscribers(IInformationObject informationObject)
        {
            SubscriptionCollection subscriptionCollection = GetSubscriptions(informationObject);
            if (subscriptionCollection == null)
                return;
            foreach(var subscription in subscriptionCollection.CollectionContent)
            {
                QueueEnvelope envelope =
                    new QueueEnvelope
                        {
                            SubscriberUpdateOperation =
                                new SubscriberUpdateOperation
                                    {
                                        OperationName = subscription.OperationActionName,
                                        SubscriberOwnerID = StorageSupport.ActiveOwnerID.ToString(),
                                        TargetOwnerID = StorageSupport.ActiveOwnerID.ToString(),
                                        OperationParameters =
                                            new SubscriberInput
                                                {
                                                    InformationObjectName = subscription.TargetObjectName,
                                                    InformationItemName = subscription.TargetItemName,
                                                    InputRelativeLocation = subscription.TargetRelativeLocation,
                                                    SubscriberRelativeLocation = subscription.SubscriberRelativeLocation,
                                                }
                                    }
                        };
                QueueSupport.PutToDefaultQueue(envelope);
            }
        }
    }
}