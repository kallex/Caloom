using System;
using System.Linq;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    public static class UpdateAccountRootGroupMembershipImplementation
    {
        public static TBRAccountRoot GetTarget_AccountRoot(string accountID)
        {
            return TBRAccountRoot.RetrieveFromDefaultLocation(accountID);
        }

        public static void ExecuteMethod_UpdateAccountRootGroupMemberships(TBRGroupRoot groupRoot, TBRAccountRoot accountRoot)
        {
            string[] accountEmailAddresses =
                accountRoot.Account.Emails.CollectionContent.Select(email => email.EmailAddress).ToArray();
            var accountRoles =
                groupRoot.Group.Roles.CollectionContent.Where(
                    role => accountEmailAddresses.Contains(role.Email.EmailAddress)).ToArray();
            accountRoot.Account.GroupRoleCollection.CollectionContent.RemoveAll(
                currRole => currRole.GroupID == groupRoot.Group.ID);
            var acctCollaborationRoles = accountRoles.
                Select(grpRole =>
                           {
                               TBAccountCollaborationGroup acctGroup = TBAccountCollaborationGroup.CreateDefault();
                               acctGroup.GroupID = groupRoot.Group.ID;
                               acctGroup.GroupRole = grpRole.Role;
                               acctGroup.RoleStatus = grpRole.RoleStatus;
                               return acctGroup;
                           });
            accountRoot.Account.GroupRoleCollection.CollectionContent.AddRange(acctCollaborationRoles);
        }

        public static void ExecuteMethod_StoreObjects(TBRAccountRoot accountRoot)
        {
            accountRoot.StoreInformation();
        }
    }
}