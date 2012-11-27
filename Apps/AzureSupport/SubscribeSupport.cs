

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure.StorageClient;

namespace TheBall
{
    public static class SubscribeSupport
    {
        public const string ChainRequestDirectory = "AaltoGlobalImpact.OIP/SubscriptionChainRequestContent/";
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

        public static List<SubcriptionGraphItem> GetExecutionOrder(this Dictionary<string, SubcriptionGraphItem> dictionary)
        {
            var values = dictionary.Values;
            List<SubcriptionGraphItem> executionList = new List<SubcriptionGraphItem>();
            // Reset visited flags
            foreach (var value in values)
            {
                value.IsVisited = false;
            }

            foreach(var value in values)
            {
                value.Visit(executionList);
            }
            return executionList;
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

        public static void GetSubcriptionList(string targetLocation, List<Subscription> result, Stack<string> currTargetStack, 
            Dictionary<string, SubcriptionGraphItem> lookupDictionary)
        {
            if(currTargetStack.Contains(targetLocation))
            {
                bool circular = true;
            }
            string currKey = targetLocation;
            string previousReferrerKey = null;
            if (currTargetStack.Count > 0)
                previousReferrerKey = currTargetStack.Peek();
            currTargetStack.Push(targetLocation);
            List<Subscription> thisLevelCombined = new List<Subscription>();
            if (lookupDictionary.ContainsKey(targetLocation) == false)
            {
                SubcriptionGraphItem graphItem = new SubcriptionGraphItem()
                                                        {
                                                            Key = currKey,
                                                        };
                if(previousReferrerKey != null)
                    graphItem.Targets.Add(lookupDictionary[previousReferrerKey]);
                lookupDictionary.Add(currKey, graphItem);
                SubscriptionCollection subscriptionCollection = GetSubscriptions(targetLocation);
                string targetParentLocation = GetParentDirectoryTarget(targetLocation);
                SubscriptionCollection parentSubscriptionCollection = GetSubscriptions(targetParentLocation);
                if (subscriptionCollection == null && parentSubscriptionCollection == null)
                {
                    return;
                }
                if (subscriptionCollection != null)
                {
                    thisLevelCombined.AddRange(subscriptionCollection.CollectionContent);
                }
                if (parentSubscriptionCollection != null)
                {
                    thisLevelCombined.AddRange(parentSubscriptionCollection.CollectionContent);
                }
                graphItem.SubscribersList = thisLevelCombined;
            }
            else
            {
                SubcriptionGraphItem graphItem = lookupDictionary[targetLocation];
                if(previousReferrerKey != null)
                    graphItem.AddTargetIfMissing(targetKey:previousReferrerKey, dictionary:lookupDictionary);
                thisLevelCombined = graphItem.SubscribersList;
                //thisLevelCombined = lookupDictionary[targetLocation];
            }
            if (thisLevelCombined == null || thisLevelCombined.Count == 0)
                return;
            result.AddRange(thisLevelCombined);
            foreach(var subscription in thisLevelCombined)
            {
                //List<string> independentTargetStack = new List<string>(currTargetStack);
                Stack<string> independentTargetStack = new Stack<string>(currTargetStack.Reverse());
                GetSubcriptionList(subscription.SubscriberRelativeLocation, result, independentTargetStack, lookupDictionary);
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
            var ictx = InformationContext.Current;
            if (ictx.IsExecutingSubscriptions)
                return;
            SubscriptionCollection subscriptionCollection = GetSubscriptions(targetLocation);
            string targetParentLocation = GetParentDirectoryTarget(targetLocation);
            SubscriptionCollection parentSubscriptionCollection = GetSubscriptions(targetParentLocation);
            if (subscriptionCollection == null && parentSubscriptionCollection == null)
                return;
            VirtualOwner owner = VirtualOwner.FigureOwner(targetLocation);
            OwnerSubscriptionItem subscriptionItem = new OwnerSubscriptionItem
                                                         {Owner = owner, TargetLocation = targetLocation};
            ictx.AddSubscriptionUpdateTarget(subscriptionItem);
            return;

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

        public static Subscription[] GetSubscriptionChainItemsInOrderOfExecution(string[] subscriptionTargetList)
        {
            List<Subscription> subscriptions = new List<Subscription>();
            Dictionary<string, SubcriptionGraphItem> lookupDictionary = new Dictionary<string, SubcriptionGraphItem>();
            foreach(var subTarget in subscriptionTargetList)
            {
                var subscriberStack = new Stack<string>();
                GetSubcriptionList(subTarget, subscriptions, subscriberStack, lookupDictionary);
            }
            var executionOrder = lookupDictionary.GetExecutionOrder();
            Dictionary<string, bool> alreadyExecuted = new Dictionary<string, bool>();
            var result = executionOrder.
                SelectMany(exec =>
                               {
                                   var subItems = exec.GetMySubscriptionsFromTargets();
                                   subItems = subItems.Where(subItem =>
                                                                 {
                                                                     string keyValue =
                                                                         subItem.
                                                                             SubscriptionType !=
                                                                         SubscribeType_WebPageToSource
                                                                             ? subItem.ID
                                                                             : subItem.SubscriberRelativeLocation;
                                                                     if (alreadyExecuted.ContainsKey(keyValue))
                                                                         return false;
                                                                     alreadyExecuted.Add(
                                                                         keyValue, true);
                                                                     return true;
                                                                 }).ToArray();
                                   return subItems;
                               }).ToArray();
            return result;
        }

        public const string StatusValue_Pending = "pending";
        public const string StatusValue_Executing = "executing";

        public static void AddPendingRequests(IContainerOwner owner, string[] subscriptionTargets)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");
            var requestContent = SubscriptionChainRequestContent.CreateDefault();
            DateTime submitTime = DateTime.UtcNow;
            string dateTimePrefix = submitTime.ToString("yyyyMMdd_HHmmss");
            requestContent.ID = dateTimePrefix + "_" + requestContent.ID;
            requestContent.SubscriptionTargetCollection.CollectionContent.
                AddRange(subscriptionTargets.Select(target => new SubscriptionTarget() { BlobLocation = target }));
            requestContent.SubmitTime = submitTime;
            string ownerSubscriptionLocation = GetOwnerIDStatusBasedLocation(owner, requestContent.ID, StatusValue_Pending);
            requestContent.RelativeLocation = ownerSubscriptionLocation;
            requestContent.StoreInformation();
        }

        public static bool AcquireChainLock(IContainerOwner owner, out string lockEtag)
        {
            string lockLocation = GetLockLocation(owner);
            return StorageSupport.AcquireLogicalLockByCreatingBlob(lockLocation, out lockEtag);
        }

        private static string GetLockLocation(IContainerOwner owner)
        {
            return GetOwnerIDStatusBasedLocation(owner, "lock", "system");
        }

        public static bool ReleaseChainLock(IContainerOwner owner, string lockEtag)
        {
            string lockLocation = GetLockLocation(owner);
            return StorageSupport.ReleaseLogicalLockByDeletingBlob(lockLocation, lockEtag);
        }

        private static string GetOwnerIDStatusBasedLocation(IContainerOwner owner, string id, string status)
        {
            return ChainRequestDirectory + owner.ContainerName + "/" + owner.LocationPrefix + "/" + id + "." + status;
        }

        public static IContainerOwner[] GetOwnerChainsInOrderOfSubmission()
        {
            int indexStart = ChainRequestDirectory.Length;
            int substringLength = StorageSupport.GuidLength + 4;
            string[] chainRequestList = GetChainRequestList();
            var groupedResult = chainRequestList.GroupBy(item => item.Substring(indexStart, substringLength));
            var resultsWithoutLock = groupedResult.Where(grp => grp.Any(item => IsLock(item) == false));
            return resultsWithoutLock.Select(grp => VirtualOwner.FigureOwner(grp.Key)).ToArray();
        }

        public static string[] GetChainRequestList(IContainerOwner owner)
        {
            string directory = owner != null
                                   ? ChainRequestDirectory + owner.ContainerName + "/" + owner.LocationPrefix + "/"
                                   : ChainRequestDirectory;


            var blobListing = StorageSupport.CurrActiveContainer.GetBlobListing(directory, false);
            return blobListing.Cast<CloudBlob>().Select(blob => blob.Name).Where(str => IsLock(str) == false).ToArray();
        }

        public static string[] GetChainRequestList()
        {
            return GetChainRequestList(null);
        }

        public static bool IsLock(string blobName)
        {
            return blobName.Contains("/lock.");
        }

    }
}