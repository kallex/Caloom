

using System;
using System.IO;
using System.Runtime.Serialization;
using AaltoGlobalImpact.OIP;

namespace TheBall
{
    public static class SubscribeSupport
    {
        public const string SubscribeType_WebPageToSource = "webpage";
        // TODO: Create needs to be applied as of address regexp filter or something, as there is no concrete object to subscribe against
        // Real case is for object creation to master collection of those objects that are created
        //public const string SubscribeType_MasterToReferenceCreate = "MasterToReferenceCreate";
        public const string SubscribeType_MasterToReferenceUpdate = "MasterToReferenceUpdate";
        // Master to reference delete can be also special case where master is no longer available
        public const string SubscribeType_MasterToReferenceDelete = "MasterToReferenceDelete";
        public const string SubscribeType_DirectoryToCollection = "DirectoryToCollectionUpdate";
        public const string SubscribeType_CollectionToCollectionUpdate = "CollectionToCollectionUpdate";

        public static void AddSubscriptionToObject(string targetLocation, string subscriberLocation, string subscriptionType, string targetTypeName = null, string subscriberTypeName = null)
        {
            var sub = GetSubscriptionToObject(targetLocation, subscriberLocation, subscriptionType, targetTypeName:targetTypeName, subscriberTypeName:subscriberTypeName);
            SubscriptionCollection subscriptionCollection = GetSubscriptions(targetLocation);
            if(subscriptionCollection == null)
            {
                subscriptionCollection = new SubscriptionCollection();
                subscriptionCollection.SetRelativeLocationAsMetadataTo(targetLocation);
            }
            if (subscriptionCollection.CollectionContent.Exists(existing => existing.SubscriberRelativeLocation == sub.SubscriberRelativeLocation 
                && existing.SubscriptionType == sub.SubscriptionType ))
                return;
            subscriptionCollection.CollectionContent.Add(sub);
            StorageSupport.StoreInformation(subscriptionCollection);
        }

        /*
        public static void AddSubscriptionToItem(IInformationObject target, string itemName, IInformationObject subscriber, string operationName)
        {
            var sub = GetSubscriptionToItem(target, itemName, subscriber, operationName);
        }*/

        public static Subscription GetSubscriptionToObject(string targetLocation, string subscriberLocation, string subscriptionType, string targetTypeName = null, string subscriberTypeName = null)
        {
            var sub = new Subscription
                          {
                              TargetRelativeLocation = targetLocation,
                              TargetInformationObjectType = targetTypeName,
                              SubscriberRelativeLocation = subscriberLocation,
                              SubscriberInformationObjectType = subscriberTypeName,
                              SubscriptionType = subscriptionType
                          };
            return sub;
        }

        /*
        public static Subscription GetSubscriptionToItem(IInformationObject target, string itemName, IInformationObject subscriber, string operationName)
        {
            Subscription sub = GetSubscriptionToObject(target, subscriber, operationName);
            sub.TargetItemName = itemName;
            return sub;
        }

        public static SubscriptionCollection GetDirectorySubscriptions(string targetLocation)
        {
            int lastDirectorySlashIndex = targetLocation.LastIndexOf("/");
            string directoryLocation = targetLocation.Substring(0, lastDirectorySlashIndex + 1);
            string metadataBlobLocation = SubscriptionCollection.GetRelativeLocationAsMetadataTo(directoryLocation);
            var result = StorageSupport.RetrieveInformation(metadataBlobLocation, typeof(SubscriptionCollection))
            return (SubscriptionCollection) result;
        }
         * */

        public static string GetParentDirectoryTarget(string targetLocation)
        {
            int lastDirectorySlashIndex = targetLocation.LastIndexOf("/");
            string directoryLocation = targetLocation.Substring(0, lastDirectorySlashIndex + 1);
            return directoryLocation;
        }

        public static SubscriptionCollection GetSubscriptions(string targetLocation)
        {
            string blobPath = SubscriptionCollection.GetRelativeLocationAsMetadataTo(targetLocation);
            var result = StorageSupport.RetrieveInformation(blobPath, typeof(SubscriptionCollection));
            return (SubscriptionCollection) result;
        }

        public static void NotifySubscribers(string targetLocation)
        {
            SubscriptionCollection subscriptionCollection = GetSubscriptions(targetLocation);
            string targetParentLocation = GetParentDirectoryTarget(targetLocation);
            SubscriptionCollection parentSubscriptionCollection = GetSubscriptions(targetParentLocation);
            if (subscriptionCollection == null && parentSubscriptionCollection == null)
                return;
            var ictx = InformationContext.Current;

            if(subscriptionCollection != null)
            {
                foreach(var subscription in subscriptionCollection.CollectionContent)
                {
                    OperationRequest operationRequest = new OperationRequest
                                                            {
                                                                SubscriberNotification = subscription
                                                            };
                    //QueueSupport.PutToOperationQueue(operationRequest);
                    ictx.AddOperationRequestToFinalizingQueue(operationRequest);
                }
            }
            if(parentSubscriptionCollection != null)
            {
                foreach (var subscription in parentSubscriptionCollection.CollectionContent)
                {
                    OperationRequest operationRequest = new OperationRequest
                    {
                        SubscriberNotification = subscription
                    };
                    //QueueSupport.PutToOperationQueue(operationRequest);
                    ictx.AddOperationRequestToFinalizingQueue(operationRequest);
                }
            }
        }

        public static void DeleteSubscriptions(string targetLocation)
        {
            string blobPath = SubscriptionCollection.GetRelativeLocationAsMetadataTo(targetLocation);
            StorageSupport.DeleteBlob(blobPath);
        }

        public static void DeleteAfterFiringSubscriptions(string targetLocation)
        {
            NotifySubscribers(targetLocation);
            DeleteSubscriptions(targetLocation);
        }

        public static void SetReferenceSubscriptionToMaster(IInformationObject containerObject, IInformationObject referenceInstance, IInformationObject masterInstance)
        {
            AddSubscriptionToObject(masterInstance.RelativeLocation, containerObject.RelativeLocation, SubscribeType_MasterToReferenceUpdate, targetTypeName:masterInstance.GetType().FullName,
                subscriberTypeName:containerObject.GetType().FullName);
        }
    }
}