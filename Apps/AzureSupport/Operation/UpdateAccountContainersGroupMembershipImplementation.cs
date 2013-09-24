using System;
using System.Linq;
using TheBall;
using TheBall.CORE;

namespace AaltoGlobalImpact.OIP
{
    public static class UpdateAccountContainersGroupMembershipImplementation
    {
        public static AccountContainer GetTarget_AccountContainer(string accountID)
        {
            VirtualOwner owner = new VirtualOwner("acc", accountID);
            var accountContainer = AccountContainer.RetrieveFromOwnerContent(owner, "default");
            if (accountContainer == null)
            {
                accountContainer = AccountContainer.CreateDefault();
                accountContainer.SetLocationAsOwnerContent(owner, "default");
                accountContainer.StoreInformation();
            }
            return accountContainer;
        }

        public static GroupSummaryContainer GetTarget_GroupSummaryContainer(string accountID)
        {
            VirtualOwner owner = new VirtualOwner("acc", accountID);
            var groupSummaryContainer = GroupSummaryContainer.RetrieveFromOwnerContent(owner, "default");
            if (groupSummaryContainer == null)
            {
                groupSummaryContainer = GroupSummaryContainer.CreateDefault();
                groupSummaryContainer.SetLocationAsOwnerContent(owner, "default");
                groupSummaryContainer.StoreInformation();
            }
            return groupSummaryContainer;
        }

        public static void ExecuteMethod_UpdateAccountContainerMemberships(TBRGroupRoot groupRoot, Group currGroup, GroupSummaryContainer groupSummaryContainer, TBRAccountRoot accountRoot, AccountContainer accountContainer)
        {
            string currRootId = groupRoot.Group.ID;
            string currReferenceUrlPrefix = String.Format("/auth/grp/{0}/", currRootId);
            var currRoles = accountContainer.AccountModule.Roles;
            currRoles.MemberInGroups.CollectionContent.RemoveAll(
                refToInfo => refToInfo.URL.StartsWith(currReferenceUrlPrefix));
            currRoles.ModeratorInGroups.CollectionContent.RemoveAll(
                refToInfo => refToInfo.URL.StartsWith(currReferenceUrlPrefix));


            foreach (var acctRole in accountRoot.Account.GroupRoleCollection.CollectionContent.Where(role => role.GroupID == currRootId))
            {
                ReferenceToInformation reference = ReferenceToInformation.CreateDefault();
                reference.URL = string.Format("/auth/grp/{0}/website/oip-group/oip-layout-groups-edit.phtml",
                                              currRootId);
                reference.Title = currGroup.GroupName + " - " + acctRole.GroupRole;
                switch (acctRole.GroupRole.ToLower())
                {
                    case "initiator":
                    case "moderator":
                        currRoles.ModeratorInGroups.CollectionContent.Add(reference);
                        break;
                    case "collaborator":
                    case "viewer":
                        currRoles.MemberInGroups.CollectionContent.Add(reference);
                        break;
                }
            }
            currRoles.ModeratorInGroups.CollectionContent.Sort(ReferenceToInformation.CompareByReferenceTitle);
            currRoles.MemberInGroups.CollectionContent.Sort(ReferenceToInformation.CompareByReferenceTitle);
            // TODO: Update account summary
            //accountContainer.AccountSummary.GroupSummary
        }

        public static void ExecuteMethod_UpdateGroupSummaryContainerMemberships(TBRGroupRoot groupRoot, Group currGroup, TBRAccountRoot accountRoot, GroupSummaryContainer groupSummaryContainer)
        {
            string currRootId = groupRoot.Group.ID;
            string currGroupID = currGroup.ID;
            bool isMember =
                accountRoot.Account.GroupRoleCollection.CollectionContent.Exists(
                    grp => grp.GroupID == currRootId);
            groupSummaryContainer.GroupCollection.CollectionContent.RemoveAll(
                grp => grp.ID == currGroupID);
            if(isMember)
            {
                groupSummaryContainer.GroupCollection.CollectionContent.Add(currGroup);
                currGroup.UpdateReferenceToInformation(currRootId);
            }
            groupSummaryContainer.GroupCollection.CollectionContent.Sort(Group.CompareByGroupName);
        }

        public static void ExecuteMethod_StoreObjects(AccountContainer accountContainer, GroupSummaryContainer groupSummaryContainer)
        {
            accountContainer.StoreInformation();
            groupSummaryContainer.StoreInformation();
        }

        public static TBRAccountRoot GetTarget_AccountRoot(string accountID)
        {
            return TBRAccountRoot.RetrieveFromDefaultLocation(accountID);
        }

        public static GroupContainer GetTarget_GroupContainer(TBRGroupRoot groupRoot)
        {
            VirtualOwner groupOwner = new VirtualOwner("grp", groupRoot.Group.ID);
            var groupContainer = GroupContainer.RetrieveFromOwnerContent(groupOwner, "default");
            if (groupContainer == null)
            {
                groupContainer = GroupContainer.CreateDefault();
                groupContainer.SetLocationAsOwnerContent(groupOwner, "default");
                groupContainer.StoreInformation();
            }
            return groupContainer;
        }

        public static Group GetTarget_Group(GroupContainer groupContainer)
        {
            return groupContainer.GroupProfile;
        }
    }
}