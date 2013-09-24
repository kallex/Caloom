using System;
using System.Linq;
using TheBall;
using TheBall.CORE;

namespace AaltoGlobalImpact.OIP
{
    public static class CreateGroupImplementation
    {
        public static TBRGroupRoot GetTarget_GroupRoot(string groupName)
        {
            TBRGroupRoot groupRoot = TBRGroupRoot.CreateNewWithGroup();
            groupRoot.Group.Title = groupName;
            return groupRoot;
        }

        public static TBRAccountRoot GetTarget_AccountRoot(string accountID)
        {
            return TBRAccountRoot.RetrieveFromDefaultLocation(accountID);
        }

        public static TBEmail[] GetTarget_AccountEmails(TBRAccountRoot accountRoot)
        {
            var emails =
                accountRoot.Account.Emails.CollectionContent.ToArray();
            return emails;
        }

        public static void ExecuteMethod_AddAsInitiatorToGroupRoot(TBRGroupRoot groupRoot, TBEmail[] accountEmails)
        {
            var group = groupRoot.Group;
            foreach(TBEmail email in accountEmails)
            {
                var groupRole = new TBCollaboratorRole()
                {
                    Email = email,
                    Role = TBCollaboratorRole.InitiatorRoleValue,
                    RoleStatus = TBCollaboratorRole.RoleStatusMemberValue,
                };
                //account.JoinGroup(this, groupRole);
                //account.StoreAndPropagate();
                group.Roles.CollectionContent.Add(groupRole);
            }
        }

        public static void ExecuteMethod_StoreObjects(TBRGroupRoot groupRoot)
        {
            groupRoot.StoreInformation();
        }

        public static RefreshAccountGroupMembershipsParameters RefreshAccountAndGroupContainers_GetParameters(string accountID, TBRGroupRoot groupRoot)
        {
            return new RefreshAccountGroupMembershipsParameters {AccountID = accountID, GroupRoot = groupRoot};
        }

        public static void ExecuteMethod_InitializeGroupContentAndMasters(TBRGroupRoot groupRoot)
        {
            var grp = groupRoot.Group;
            foreach (var templateName in InstanceConfiguration.DefaultGroupTemplateList)
                RenderWebSupport.RefreshGroupTemplate(grp.ID, templateName, false);
            OwnerInitializer.InitializeAndConnectMastersAndCollections(grp);
        }

        public static void ExecuteMethod_VerifyAccountEmails(TBEmail[] accountEmails)
        {
            if(accountEmails == null || accountEmails.Any(email => email.ValidatedAt != default(DateTime)) == false)
                throw new InvalidOperationException("Account without valid emails cannot create group");
        }
    }
}