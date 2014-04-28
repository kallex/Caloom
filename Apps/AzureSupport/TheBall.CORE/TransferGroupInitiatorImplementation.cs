using System.Linq;
using AaltoGlobalImpact.OIP;

namespace TheBall.CORE
{
    public class TransferGroupInitiatorImplementation
    {
        public static void ExecuteMethod_AddNewInitiatorToGroup(string groupId, string newInitiatorAccountId)
        {
            TBRGroupRoot groupRoot = TBRGroupRoot.RetrieveFromDefaultLocation(groupId);
            TBRAccountRoot accountRoot = TBRAccountRoot.RetrieveFromDefaultLocation(newInitiatorAccountId);
            var accountEmails = accountRoot.Account.Emails.CollectionContent;
            var group = groupRoot.Group;
            foreach(TBEmail email in accountEmails)
            {
                var groupRole = new TBCollaboratorRole()
                {
                    Email = email,
                    Role = TBCollaboratorRole.InitiatorRoleValue,
                    RoleStatus = TBCollaboratorRole.RoleStatusMemberValue,
                };
                group.Roles.CollectionContent.Add(groupRole);
            }
            groupRoot.StoreInformation();
            RefreshAccountGroupMemberships.Execute(new RefreshAccountGroupMembershipsParameters
                {
                    GroupRoot = groupRoot,
                    AccountID = newInitiatorAccountId
                });
        }

        public static void ExecuteMethod_RemoveOldInitiatorFromGroup(string groupId, string oldInitiatorAccountId)
        {
            TBRGroupRoot groupRoot = TBRGroupRoot.RetrieveFromDefaultLocation(groupId);
            TBRAccountRoot accountRoot = TBRAccountRoot.RetrieveFromDefaultLocation(oldInitiatorAccountId);
            var accountEmails = accountRoot.Account.Emails.CollectionContent;
            var group = groupRoot.Group;
            foreach (TBEmail email in accountEmails)
            {
                var groupRolesToRemove = group.Roles.CollectionContent.Where(role => role.Email.EmailAddress == email.EmailAddress &&
                                                                                    role.Role == TBCollaboratorRole.InitiatorRoleValue).ToArray();
                foreach (var groupRoleToRemove in groupRolesToRemove)
                {
                    group.Roles.CollectionContent.Remove(groupRoleToRemove);
                }
            }
            groupRoot.StoreInformation();
            RefreshAccountGroupMemberships.Execute(new RefreshAccountGroupMembershipsParameters
            {
                GroupRoot = groupRoot,
                AccountID = oldInitiatorAccountId
            });

        }
    }
}