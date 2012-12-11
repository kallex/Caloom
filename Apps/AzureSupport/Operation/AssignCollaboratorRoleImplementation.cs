using System;
using System.Linq;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    public static class AssignCollaboratorRoleImplementation
    {
        public static TBRGroupRoot GetTarget_GroupRoot(string groupID)
        {
            return TBRGroupRoot.RetrieveFromDefaultLocation(groupID);
        }

        public static Collaborator GetTarget_Collaborator(GroupContainer groupContainer, string collaboratorID)
        {
            var searchSequence =
                groupContainer.Collaborators.CollectionContent.Union(
                    groupContainer.PendingCollaborators.CollectionContent);
            return 
                searchSequence.FirstOrDefault(collaborator => collaborator.ID == collaboratorID);
        }

        public static string GetTarget_AccountID(Collaborator collaborator)
        {
            return collaborator.AccountID;
        }

        public static TBCollaboratorRole GetTarget_TBCollaboratorRole(TBRGroupRoot groupRoot, string emailAddress)
        {
            return groupRoot.Group.Roles.CollectionContent.FirstOrDefault(collRole => collRole.Email.EmailAddress == emailAddress);
        }

        public static void ExecuteMethod_AssignCollaboratorRole(string roleToAssign, TBCollaboratorRole tbCollaboratorRole)
        {
            tbCollaboratorRole.Role = roleToAssign;
        }

        public static void ExecuteMethod_StoreObjects(TBRGroupRoot groupRoot)
        {
            groupRoot.StoreInformation();
        }

        public static RefreshAccountGroupMembershipsParameters RefreshAccountAndGroupContainers_GetParameters(TBRGroupRoot groupRoot, string accountID)
        {
            return new RefreshAccountGroupMembershipsParameters {AccountID = accountID, GroupRoot = groupRoot};
        }

        public static string GetTarget_EmailAddress(TBRGroupRoot groupRoot, TBRAccountRoot accountRoot)
        {
            var emailAddresses =
                accountRoot.Account.Emails.CollectionContent.Select(email => email.EmailAddress).ToArray();
            var emailAddress = emailAddresses.First(
                email => groupRoot.Group.Roles.CollectionContent.Any(role => role.Email.EmailAddress == email));
            return emailAddress;
        }

        public static TBRAccountRoot GetTarget_AccountRoot(string accountID)
        {
            return TBRAccountRoot.RetrieveFromDefaultLocation(accountID);
        }
    }
}