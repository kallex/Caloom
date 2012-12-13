using System;
using System.Linq;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    public static class UnlinkEmailAddressImplementation
    {
        public static string GetTarget_EmailAddress(AccountContainer accountContainer, string emailAddressID)
        {
            return
                accountContainer.AccountModule.Security.EmailCollection.CollectionContent.First(
                    email => email.ID == emailAddressID).EmailAddress;
        }

        public static TBRGroupRoot[] GetTarget_GroupRoots(TBRAccountRoot accountRoot, string emailAddress)
        {
            var groupIDs =
                accountRoot.Account.GroupRoleCollection.CollectionContent.Select(grpRole => grpRole.GroupID).ToArray();
            var groups = groupIDs.Select(groupID => TBRGroupRoot.RetrieveFromDefaultLocation(groupID)).ToArray();
            var groupsContainingEmail =
                groups.Where(
                    grp => grp.Group.Roles.CollectionContent.Any(role => role.Email.EmailAddress == emailAddress)).
                    ToArray();
            return groupsContainingEmail;
        }

        public static TBREmailRoot GetTarget_EmailRoot(string emailAddress)
        {
            var emailRootID = TBREmailRoot.GetIDFromEmailAddress(emailAddress);
            return TBREmailRoot.RetrieveFromDefaultLocation(emailRootID);
        }

        public static void ExecuteMethod_RemoveEmailFromAccountRoot(TBRAccountRoot accountRoot, string emailAddress)
        {
            accountRoot.Account.Emails.CollectionContent.RemoveAll(email => email.EmailAddress == emailAddress);
        }

        public static void ExecuteMethod_RemoveEmailFromAccountContainer(string emailAddressID, AccountContainer accountContainer)
        {
            accountContainer.AccountModule.Security.EmailCollection.CollectionContent.RemoveAll(
                email => email.ID == emailAddressID);
        }

        public static void ExecuteMethod_DeleteEmailRoot(TBREmailRoot emailRoot)
        {
            emailRoot.DeleteInformationObject();
        }

        public static void ExecuteMethod_StoreObjects(TBRAccountRoot accountRoot)
        {
            accountRoot.StoreInformation();
        }

        public static void ExecuteMethod_RemoveGroupMemberships(string emailAddress, TBRGroupRoot[] groupRoots)
        {
            foreach(var groupRoot in groupRoots)
            {
                RemoveMemberFromGroup.Execute(new RemoveMemberFromGroupParameters
                                                  {
                                                      EmailAddress = emailAddress,
                                                      GroupID = groupRoot.ID
                                                  });
            }
        }

        public static TBRAccountRoot GetTarget_AccountRootBeforeGroupRemoval(string accountID)
        {
            return TBRAccountRoot.RetrieveFromDefaultLocation(accountID);
        }

        public static TBRAccountRoot GetTarget_AccountRootAfterGroupRemoval(string accountID)
        {
            return TBRAccountRoot.RetrieveFromDefaultLocation(accountID);
        }

        public static AccountContainer GetTarget_AccountContainerAfterGroupRemoval(AccountContainer accountContainerBeforeGroupRemoval)
        {
            return AccountContainer.RetrieveAccountContainer(accountContainerBeforeGroupRemoval.RelativeLocation);
        }

        public static UpdateAccountContainerFromAccountRootParameters UpdateAccountContainer_GetParameters(string accountID)
        {
            return new UpdateAccountContainerFromAccountRootParameters {AccountID = accountID};
        }

        public static UpdateAccountRootToReferencesParameters UpdateAccountRoot_GetParameters(string accountID)
        {
            return new UpdateAccountRootToReferencesParameters {AccountID = accountID};
        }
    }
}