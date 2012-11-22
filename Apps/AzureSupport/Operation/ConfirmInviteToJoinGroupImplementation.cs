using System;
using System.Linq;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    public static class ConfirmInviteToJoinGroupImplementation
    {
        public static TBRGroupRoot GetTarget_GroupRoot(string groupID)
        {
            return TBRGroupRoot.RetrieveFromDefaultLocation(groupID);
        }

        public static string GetTarget_AccountID(string memberEmailAddress)
        {
            string emailRootID = TBREmailRoot.GetIDFromEmailAddress(memberEmailAddress);
            TBREmailRoot emailRoot = TBREmailRoot.RetrieveFromDefaultLocation(emailRootID);
            return emailRoot.Account.ID;
        }

        public static RefreshAccountGroupMembershipsParameters RefreshAccountAndGroupContainers_GetParameters(TBRGroupRoot groupRoot, string accountID)
        {
            return new RefreshAccountGroupMembershipsParameters() {AccountID = accountID, GroupRoot = groupRoot};
        }

        public static void ExecuteMethod_ConfirmPendingInvitationToGroupRoot(string memberEmailAddress, TBRGroupRoot groupRoot)
        {
            var groupRole =
                groupRoot.Group.Roles.CollectionContent.First(role => role.Email.EmailAddress == memberEmailAddress);
            groupRole.RoleStatus = TBCollaboratorRole.RoleStatusMemberValue;
        }

        public static void ExecuteMethod_StoreObjects(TBRGroupRoot groupRoot)
        {
            groupRoot.StoreInformation();
        }
    }
}