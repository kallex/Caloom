

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        public const string SubscribeType_MasterCollectionToContainerUpdate = "MasterCollectionToContainerUpdate";

        public static void AddSubscriptionToObject(string targetLocation, string subscriberLocation, string subscriptionType, string targetTypeName = null, string subscriberTypeName = null)
        {
            if(targetLocation == subscriberLocation)
                throw new InvalidDataException("Self-circular subscription targeting self attempted: " + targetLocation);
            var sub = GetSubscriptionToObject(targetLocation, subscriberLocation, subscriptionType, targetTypeName:targetTypeName, subscriberTypeName:subscriberTypeName);
            SubscriptionCollection subscriptionCollection = GetSubscriptions(targetLocation);
            if(subscriptionCollection == null)
            {
                subscriptionCollection = new SubscriptionCollection();
                subscriptionCollection.SetRelativeLocationAsMetadataTo(targetLocation);
            }
            var alreadyExists =
                subscriptionCollection.CollectionContent.FirstOrDefault(
                    existing => existing.SubscriberRelativeLocation == sub.SubscriberRelativeLocation
                                && existing.SubscriptionType == sub.SubscriptionType);
            if(alreadyExists != null)
            {
                // If the values match, don't save when there are no changes
                if (alreadyExists.SubscriberInformationObjectType == sub.SubscriberInformationObjectType &&
                    alreadyExists.TargetInformationObjectType == sub.TargetInformationObjectType)
                    return;
                // ... otherwise update the values 
                alreadyExists.SubscriberInformationObjectType = sub.SubscriberInformationObjectType;
                alreadyExists.TargetInformationObjectType = sub.TargetInformationObjectType;
            } else
            {
                subscriptionCollection.CollectionContent.Add(sub);
            }
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

        public static void GetSubcriptionList(string startTargetLocation, List<Subscription> result, List<string> currTargetStack)
        {
            if(currTargetStack.Contains(startTargetLocation))
            {
                bool circular = true;
            }
            currTargetStack.Add(startTargetLocation);
            SubscriptionCollection subscriptionCollection = GetSubscriptions(startTargetLocation);
            string targetParentLocation = GetParentDirectoryTarget(startTargetLocation);
            SubscriptionCollection parentSubscriptionCollection = GetSubscriptions(targetParentLocation);
            if (subscriptionCollection == null && parentSubscriptionCollection == null)
                return;
            List<Subscription> thisLevelCombined = new List<Subscription>();
            if(subscriptionCollection != null)
            {
                thisLevelCombined.AddRange(subscriptionCollection.CollectionContent);
            }
            if(parentSubscriptionCollection != null)
            {
                thisLevelCombined.AddRange(parentSubscriptionCollection.CollectionContent);
            }
            result.AddRange(thisLevelCombined);
            foreach(var subscription in thisLevelCombined)
            {
                List<string> independentTargetStack = new List<string>(currTargetStack);
                GetSubcriptionList(subscription.SubscriberRelativeLocation, result, independentTargetStack);
            }
            int distinctCount = result.Select(sub => sub.SubscriberRelativeLocation).Distinct().Count();
            Debug.WriteLine("Count: " + result.Count + " Distinctive: " + distinctCount);
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

        public static void SetCollectionSubscriptionToMaster(IInformationObject containerObject, string masterCollectionLocation, Type collectionType)
        {
            AddSubscriptionToObject(masterCollectionLocation, containerObject.RelativeLocation, SubscribeType_MasterCollectionToContainerUpdate, collectionType.FullName,
                containerObject.GetType().FullName);
        }
    }
}