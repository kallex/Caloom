using System;
using System.Collections.Generic;
using System.Linq;
using TheBall;
using TheBall.CORE;

namespace AaltoGlobalImpact.OIP
{
    public class AccountRootAndContainer
    {
        public TBRAccountRoot AccountRoot;
        public AccountContainer AccountContainer;
    }

    public static class UpdateGroupContainersGroupMembershipImplementation
    {

        public static GroupContainer GetTarget_GroupContainer(TBRGroupRoot groupRoot)
        {
            string groupID = groupRoot.Group.ID;
            VirtualOwner owner = new VirtualOwner("grp", groupID);
            var groupContainer = GroupContainer.RetrieveFromOwnerContent(owner, "default");
            if (groupContainer == null)
            {
                groupContainer = GroupContainer.CreateDefault();
                groupContainer.SetLocationAsOwnerContent(owner, "default");
                groupContainer.GroupProfile.GroupName = groupRoot.Group.Title;
                groupContainer.StoreInformation();
            }   
            return groupContainer;
        }

        public static void ExecuteMethod_StoreObjects(GroupContainer groupContainer)
        {
            groupContainer.StoreInformation();
        }

        public static void ExecuteMethod_UpdateGroupContainerMembership(TBRGroupRoot groupRoot, AccountRootAndContainer[] accountRootsAndContainers, GroupContainer groupContainer)
        {
            string groupID = groupRoot.Group.ID;
            var collaborators = accountRootsAndContainers.
                Select(acctR => new
                                    {
                                        Account = acctR.AccountRoot.Account,
                                        AccountContainer = acctR.AccountContainer,
                                        Profile = acctR.AccountContainer.AccountModule.Profile,
                                        GroupRole =
                                    acctR.AccountRoot.Account.GroupRoleCollection.CollectionContent.First(
                                        role => role.GroupID == groupID)
                                    });
            var moderators =
                collaborators.Where(coll => TBCollaboratorRole.HasModeratorRights(coll.GroupRole.GroupRole)).ToArray();
            var pendingCollaborators =
                collaborators.Where(
                    coll => TBCollaboratorRole.IsRoleStatusValidMember(coll.GroupRole.RoleStatus) == false);
            var fullCollaborators = collaborators.Where(
                coll => TBCollaboratorRole.IsRoleStatusValidMember(coll.GroupRole.RoleStatus) == true);

            groupContainer.GroupProfile.Moderators.CollectionContent.Clear();
            groupContainer.GroupProfile.Moderators.CollectionContent.
                AddRange(moderators.
                             Select(mod =>
                                        {

                                            var moderator = Moderator.CreateDefault();
                                            moderator.ModeratorName = mod.Profile.FirstName + " " + mod.Profile.LastName;
                                            return moderator;
                                        }).OrderBy(mod => mod.ModeratorName));
            groupContainer.Collaborators.CollectionContent.Clear();
            groupContainer.Collaborators.CollectionContent.AddRange(
                fullCollaborators.Select(coll =>
                                             {
                                                 Collaborator collaborator = Collaborator.CreateDefault();
                                                 collaborator.AccountID = coll.Account.ID;
                                                 collaborator.CollaboratorName = coll.Profile.FirstName + " " +
                                                                                 coll.Profile.LastName;
                                                 collaborator.Role = coll.GroupRole.GroupRole;
                                                 return collaborator;
                                             })
                );
            groupContainer.PendingCollaborators.CollectionContent.Clear();
            groupContainer.PendingCollaborators.CollectionContent.AddRange(
                pendingCollaborators.Select(coll =>
                                                {
                                                    Collaborator collaborator = Collaborator.CreateDefault();
                                                    collaborator.AccountID = coll.Account.ID;
                                                    collaborator.CollaboratorName = coll.Profile.FirstName + " " +
                                                                                    coll.Profile.LastName;
                                                    collaborator.Role = coll.GroupRole.GroupRole;
                                                    return collaborator;
                                                })
                );
        }

        public static AccountRootAndContainer[] GetTarget_AccountRootsAndContainers(TBRGroupRoot groupRoot)
        {
            var accountIDs = groupRoot.Group.Roles.CollectionContent.
                Select(role => role.Email.EmailAddress).
                Select(email =>
                {
                    string emailRootID = TBREmailRoot.GetIDFromEmailAddress(email);
                    TBREmailRoot emailRoot = TBREmailRoot.RetrieveFromDefaultLocation(emailRootID);
                    return emailRoot.Account.ID;
                }).Distinct().ToArray();
            List<AccountRootAndContainer> result = new List<AccountRootAndContainer>();
            foreach(var accountID in accountIDs)
            {
                TBRAccountRoot accountRoot = TBRAccountRoot.RetrieveFromDefaultLocation(accountID);
                VirtualOwner owner = new VirtualOwner("acc", accountID);
                var accountContainer = AccountContainer.RetrieveFromOwnerContent(owner, "default");
                AccountRootAndContainer accountRootAndContainer = new AccountRootAndContainer
                                                                      {
                                                                          AccountContainer = accountContainer,
                                                                          AccountRoot = accountRoot
                                                                      };
                result.Add(accountRootAndContainer);
            }
            return result.ToArray();
        }
    }
}