using System;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    public static class RemoveMemberFromGroupImplementation
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

        public static void ExecuteMethod_RemoveMemberFromGroup(string memberEmailAddress, TBRGroupRoot groupRoot)
        {
            groupRoot.Group.Roles.CollectionContent.RemoveAll(role => role.Email.EmailAddress == memberEmailAddress);
        }

        public static void ExecuteMethod_StoreObjects(TBRGroupRoot groupRoot)
        {
            groupRoot.StoreInformation();
        }

        public static RefreshAccountGroupMembershipsParameters RefreshAccountAndGroupContainers_GetParameters(TBRGroupRoot groupRoot, string accountID)
        {
            return new RefreshAccountGroupMembershipsParameters {AccountID = accountID, GroupRoot = groupRoot};
        }
    }
}