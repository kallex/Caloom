using System;

namespace AaltoGlobalImpact.OIP
{
    public static class RefreshAccountGroupMembershipsImplementation
    {
        public static UpdateAccountRootGroupMembershipParameters UpdateAccountRoot_GetParameters(TBRGroupRoot groupRoot, string accountID)
        {
            return new UpdateAccountRootGroupMembershipParameters {AccountID = accountID, GroupRoot = groupRoot};
        }

        public static UpdateLoginGroupPermissionsParameters UpdateAccountLoginGroups_GetParameters(string accountID)
        {
            return new UpdateLoginGroupPermissionsParameters {AccountID = accountID};
        }

        public static UpdateGroupContainersGroupMembershipParameters UpdateGroupContainers_GetParameters(TBRGroupRoot groupRoot)
        {
            return new UpdateGroupContainersGroupMembershipParameters {GroupRoot = groupRoot};
        }

        public static UpdateAccountContainersGroupMembershipParameters UpdateAccountContainers_GetParameters(TBRGroupRoot groupRoot, string accountID)
        {
            return new UpdateAccountContainersGroupMembershipParameters {AccountID = accountID, GroupRoot = groupRoot};
        }

        public static UpdateAccountRootToReferencesParameters UpdateAccountRootReferences_GetParameters(string accountID)
        {
            return new UpdateAccountRootToReferencesParameters {AccountID = accountID};
        }
    }
}