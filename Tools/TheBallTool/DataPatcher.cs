using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AaltoGlobalImpact.OIP;
using TheBall;

namespace TheBallTool
{
    public static class DataPatcher
    {
        public static void SetAllInvitedViewerMembersAsFullCollaborators()
        {
            var accountIDs = TBRAccountRoot.GetAllAccountIDs();
            foreach(var acctID in accountIDs)
            {
                TBRAccountRoot accountRoot = TBRAccountRoot.RetrieveFromDefaultLocation(acctID);
                TBAccount account = accountRoot.Account;
                foreach(var grpRole in account.GroupRoleCollection.CollectionContent)
                {
                    if (TBCollaboratorRole.IsRoleStatusValidMember(grpRole.RoleStatus) == false)
                        grpRole.RoleStatus = TBCollaboratorRole.RoleStatusMemberValue;
                    if (grpRole.GroupRole == TBCollaboratorRole.ViewerRoleValue)
                        grpRole.GroupRole = TBCollaboratorRole.CollaboratorRoleValue;
                }
                account.StoreAndPropagate();
            }

        }

        public static void UpdateReferenceInformationsInAllAcountsAndGroups()
        {
            var ownerLocations = GetAllOwnerLocations();
            int totalCount = ownerLocations.Length;
            int currIX = 0;
            foreach(var ownerLocation in ownerLocations)
            {
                Console.WriteLine("Updating number " + (++currIX) + " out of " + totalCount);
                VirtualOwner owner = VirtualOwner.FigureOwner(ownerLocation);
                var informationObjects = StorageSupport.CurrActiveContainer.
                    GetInformationObjects(ownerLocation,
                                          iObj =>
                                          iObj is Blog ||
                                          iObj is Activity ||
                                          iObj is AddressAndLocation);
                foreach (var iObj in informationObjects)
                {
                    try
                    {
                        StorageSupport.StoreInformationMasterFirst(iObj, owner, true);
                        InformationContext.ProcessAndClearCurrent();
                        InformationContext.Current.InitializeCloudStorageAccess(Properties.Settings.Default.CurrentActiveContainerName);
                    } catch(Exception ex)
                    {
                        bool letThrow = false;
                        if (letThrow)
                            throw;
                    }
                }

            }
        }

        public static void EnsureAndRefreshMasterCollections()
        {
            var accountIDs = TBRAccountRoot.GetAllAccountIDs();
            foreach (string accountID in accountIDs)
            {
                string acctLocation = "acc/" + accountID + "/";
                VirtualOwner owner = VirtualOwner.FigureOwner(acctLocation);
                owner.EnsureMasterCollections();
                owner.RefreshMasterCollections();
            }
            var groupIDs = TBRGroupRoot.GetAllGroupIDs();
            foreach (string groupID in groupIDs)
            {
                string grpLocation = "grp/" + groupID + "/";
                VirtualOwner owner = VirtualOwner.FigureOwner(grpLocation);
                owner.EnsureMasterCollections();
                owner.RefreshMasterCollections();
            }
        }

        public static string[] GetAllOwnerLocations()
        {
            var accountIDs = TBRAccountRoot.GetAllAccountIDs();
            var accountLocs = accountIDs.Select(accID => "acc/" + accID + "/");
            var groupLocs = GetAllGroupLocations();
            return accountLocs.Union(groupLocs).ToArray();
        }

        public static string[] GetAllGroupLocations()
        {
            var groupIDs = TBRGroupRoot.GetAllGroupIDs();
            var groupLocs = groupIDs.Select(grpID => "grp/" + grpID + "/");
            return groupLocs.ToArray();
        }



        public static string[] GetAllAccountLocations()
        {
            var accountIDs = TBRAccountRoot.GetAllAccountIDs();
            var accountLocs = accountIDs.Select(accID => "acc/" + accID + "/");
            return accountLocs.ToArray();
        }

        public static void ReconnectGroupsMastersAndCollections()
        {
            var groupLocs = GetAllGroupLocations();
            foreach(var grpLoc in groupLocs)
                ReconnectMastersAndCollections(grpLoc);
        }

        public static void ReconnectAccountsMastersAndCollections()
        {
            var acctLocs = GetAllAccountLocations();
            foreach (var acctLoc in acctLocs)
                ReconnectMastersAndCollections(acctLoc);
        }

        private static IInformationObject[] GetAllInformationObjects(Predicate<IInformationObject> filterIfFalse)
        {
            string[] ownerLocations = GetAllOwnerLocations();
            List<IInformationObject> result = new List<IInformationObject>();
            foreach(string ownerLocation in ownerLocations)
            {
                var ownerObjects = StorageSupport.CurrActiveContainer.GetInformationObjects(ownerLocation, filterIfFalse);
                result.AddRange(ownerObjects);
            }
            return result.ToArray();
        }

        private static void ReconnectMastersAndCollections(string ownerLocation)
        {
            //string myLocalAccountID = "0c560c69-c3a7-4363-b125-ba1660d21cf4";
            //string acctLoc = "acc/" + myLocalAccountID + "/";

            VirtualOwner me = VirtualOwner.FigureOwner(ownerLocation);

            var informationObjects = StorageSupport.CurrActiveContainer.GetInformationObjects(ownerLocation,
                                                                                              nonMaster =>
                                                                                              nonMaster.
                                                                                                  IsIndependentMaster ==
                                                                                              false && (nonMaster is TBEmailValidation == false)).ToArray();
            foreach (var iObj in informationObjects)
            {
                try
                {
                    iObj.ReconnectMastersAndCollections(true);
                } catch(Exception ex)
                {
                    bool ignoreException = false;
                    if (ignoreException == false)
                        throw;
                }
            }
        }



        private static void DoCustomCleanup(string groupLoc)
        {
            var defaultBlogToDelete = StorageSupport.CurrActiveContainer.
                GetInformationObjects(groupLoc,
                                      item => item is Blog && item.RelativeLocation.EndsWith("/default")).ToArray();
            foreach (Blog blog in defaultBlogToDelete)
            {
                StorageSupport.DeleteInformationObject(blog);
            }
            InformationContext.ProcessAndClearCurrent();
            InformationContext.Current.InitializeCloudStorageAccess(Properties.Settings.Default.CurrentActiveContainerName);
        }

        private static void SyncWwwPublicFromDefaultGroup()
        {
            string publicSite = "demowww.aaltoglobalimpact.org";
            string[] folderList = new[] {"bootstrap-default", "oip-additions", "www-public"};
            foreach(string folder in folderList)
            {
                string sourceFolder = folder;
                //if (sourceFolder == "www-public")
                //    sourceFolder = "oip-public";
                var operationRequest = RenderWebSupport.SyncTemplatesToSite(StorageSupport.CurrActiveContainer.Name, "grp/9798daca-afc4-4046-a99b-d0d88bb364e0/wwwsite/" + sourceFolder,
                                                     publicSite.Replace('.', '-'), folder, true, false);
                QueueSupport.PutToOperationQueue(operationRequest);
            }
        }

        private static void RefreshAllAccounts()
        {
            var accountIDs = TBRAccountRoot.GetAllAccountIDs();
            foreach (var accountID in accountIDs)
            {
                var accountRoot = TBRAccountRoot.RetrieveFromDefaultLocation(accountID);
                accountRoot.Account.StoreAndPropagate();
            }

        }

        private static void FixGroupMastersAndCollections(string groupID)
        {
            TBRGroupRoot groupRoot = TBRGroupRoot.RetrieveFromDefaultLocation(groupID);
            groupRoot.Group.EnsureMasterCollections();
            groupRoot.Group.RefreshMasterCollections();
            groupRoot.Group.ReconnectMastersAndCollectionsForOwner();
        }

        private static void AddLegacyGroupWithInitiator(string groupID, string initiatorEmailAddress)
        {
            var groupRoot = TBRGroupRoot.CreateLegacyNewWithGroup(groupID);
            groupRoot.Group.JoinToGroup(initiatorEmailAddress, TBCollaboratorRole.InitiatorRoleValue);
            //groupRoot.Group.JoinToGroup("jeroen@caloom.com", "moderator");
            StorageSupport.StoreInformation(groupRoot);
            groupRoot.Group.EnsureMasterCollections();
            groupRoot.Group.RefreshMasterCollections();
            groupRoot.Group.ReconnectMastersAndCollectionsForOwner();
        }

        private static void RemoveBlogLocationsOnce()
        {
            var blogs = GetAllInformationObjects(io => io is Blog).Cast<Blog>().ToArray();
            foreach (var blog in blogs)
            {
                //blog.Location = null;
                //blog.StoreInformation();
            }
        }

        private static void RemoveActivityLocationsOnce()
        {
            var activities = GetAllInformationObjects(io => io is Activity).Cast<Activity>().ToArray();
            foreach (var activity in activities)
            {
                //activity.Location = null;
                //activity.StoreInformation();
            }
        }

        //private static void InitBlogAndActivityLocationCollectionsOnce()
        //{
        //    var blogsAndActivities = GetAllInformationObjects(io => io is Activity || io is Blog).ToArray();
        //    var blogs = blogsAndActivities.Where(ba => ba is Blog).Cast<Blog>().ToArray();
        //    var activities = blogsAndActivities.Where(ba => ba is Activity).Cast<Activity>().ToArray();
        //    foreach(var blog in blogs.Where(bl => bl.LocationCollection == null))
        //    {
        //        blog.LocationCollection = AddressAndLocationCollection.CreateDefault();
        //        blog.StoreInformation();
        //        blog.ReconnectMastersAndCollections(false);
        //    }
        //    foreach(var activity in activities.Where(act => act.LocationCollection == null))
        //    {
        //        activity.LocationCollection = AddressAndLocationCollection.CreateDefault();
        //        activity.StoreInformation();
        //        activity.ReconnectMastersAndCollections(false);
        //    }
        //}

        //private static void ConnectMapContainerToCollections()
        //{
        //    var mapContainers = GetAllInformationObjects(io => io is MapContainer).Cast<MapContainer>().ToArray();
        //    foreach(var mapContainer in mapContainers)
        //    {
        //        mapContainer.MarkerSourceActivities = ActivityCollection.CreateDefault();
        //        mapContainer.MarkerSourceBlogs = BlogCollection.CreateDefault();
        //        mapContainer.MarkerSourceLocations = AddressAndLocationCollection.CreateDefault();
        //        mapContainer.ReconnectMastersAndCollections(true);
        //    }
        //}

        private static void ClearEmptyLocations()
        {
            var locations =
                GetAllInformationObjects(io => io is AddressAndLocation).Cast<AddressAndLocation>().ToArray();
            foreach(var loc in locations)
            {
                if(String.IsNullOrEmpty(loc.Location.LocationName))
                {
                    try
                    {
                        StorageSupport.DeleteInformationObject(loc);
                    } finally
                    {
                        InformationContext.ProcessAndClearCurrent();
                        InformationContext.Current.InitializeCloudStorageAccess(Properties.Settings.Default.CurrentActiveContainerName);
                    }
                }
            }
        }

        private static void ReportAllSubscriptionCounts()
        {
            //var informationObjects = GetAllInformationObjects(io => SubscribeSupport.GetSubscriptions(io.RelativeLocation) != null).ToArray();
            string interestGroupLocation = "grp/" + RenderWebSupport.DefaultGroupID + "/";
            var informationObjects = StorageSupport.CurrActiveContainer.GetInformationObjects(interestGroupLocation, io => io is AddressAndLocation && SubscribeSupport.GetSubscriptions(io.RelativeLocation) != null).ToArray();

            int currMaxSubs = 0;
            int currMaxDistinct = 0;
            foreach(var iObject in informationObjects)
            {
                int subCount = GetTotalSubscriberCount(iObject, ref currMaxSubs, ref currMaxDistinct);
            }
        }

        private static int GetTotalSubscriberCount(IInformationObject informationObject, ref int CurrMaxSubs, ref int CurrMaxDistinct)
        {
            string location = informationObject.RelativeLocation;
            List<Subscription> result = new List<Subscription>();
            List<string> subscriberStack = new List<string>();
            SubscribeSupport.GetSubcriptionList(location, result, subscriberStack);
            int count = result.Count;
            int distinctCount = result.Select(sub => sub.SubscriberRelativeLocation).Distinct().Count();
            if(result.Count >= CurrMaxSubs || distinctCount >= CurrMaxDistinct)
            {
                if (count > CurrMaxSubs)
                    CurrMaxSubs = count;
                if (distinctCount > CurrMaxDistinct)
                    CurrMaxDistinct = distinctCount;
                Console.WriteLine(count + " / " + distinctCount + " : " + location);
            }
            return count;
        }

        private static void UpdateAccountAndGroups(string accountEmail)
        {
            string emailID = TBREmailRoot.GetIDFromEmailAddress(accountEmail);
            TBREmailRoot emailRoot = TBREmailRoot.RetrieveFromDefaultLocation(emailID);
            TBRAccountRoot accountRoot = TBRAccountRoot.RetrieveFromDefaultLocation(emailRoot.Account.ID);
            foreach(var groupRole in accountRoot.Account.GroupRoleCollection.CollectionContent)
            {
                TBRGroupRoot groupRoot = TBRGroupRoot.RetrieveFromDefaultLocation(groupRole.GroupID);
                RefreshAccountGroupMemberships.Execute(new RefreshAccountGroupMembershipsParameters
                {
                    AccountID = accountRoot.Account.ID,
                    GroupRoot = groupRoot
                });
                InformationContext.ProcessAndClearCurrent();
                InformationContext.Current.InitializeCloudStorageAccess(Properties.Settings.Default.CurrentActiveContainerName);
            }
        }


        public static bool DoPatching()
        {
            return false;
            Debugger.Break();
            bool skip = false;
            if (skip == false)
                throw new NotSupportedException("Skip this with debugger");
            //EnsureAndRefreshMasterCollections();
            //ReconnectAccountsMastersAndCollections();
            //ReconnectGroupsMastersAndCollections();

            //SyncWwwPublicFromDefaultGroup();
            //AddLegacyGroupWithInitiator("9798daca-afc4-4046-a99b-d0d88bb364e0", "kalle.launiala@citrus.fi");
            //FixGroupMastersAndCollections("9798daca-afc4-4046-a99b-d0d88bb364e0");
            //InitBlogAndActivityLocationCollectionsOnce();
            
            //ConnectMapContainerToCollections();
            //ClearEmptyLocations();
            //ReportAllSubscriptionCounts();

            UpdateAccountAndGroups(accountEmail: "kalle.launiala@citrus.fi");

            return true;
        }

    }
}