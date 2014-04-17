using System;
using System.Linq;
using AaltoGlobalImpact.OIP;

namespace TheBall.CORE
{
    public class MergeAccountsDestructivelyImplementation
    {
        public static TBRAccountRoot GetTarget_PrimaryAccountToStay(string primaryAccountToStayId)
        {
            var accountRoot = TBRAccountRoot.RetrieveFromDefaultLocation(primaryAccountToStayId);
            return accountRoot;
        }

        public static TBRAccountRoot GetTarget_AccountToBeMerged(string accountToBeMergedAndDestroyedId)
        {
            var accountRoot = TBRAccountRoot.RetrieveFromDefaultLocation(accountToBeMergedAndDestroyedId);
            return accountRoot;
        }

        public static TBAccountCollaborationGroup[] GetTarget_GroupAccessToBeMerged(TBRAccountRoot accountToBeMerged)
        {
            var nonInitiatorRigths = accountToBeMerged.Account.GroupRoleCollection.CollectionContent
                .Where(grpRole => TBCollaboratorRole.HasInitiatorRights(grpRole.GroupRole) == false)
                .ToArray();
            return nonInitiatorRigths;
        }

        public static TBAccountCollaborationGroup[] GetTarget_GroupInitiatorAccessToBeTransfered(TBRAccountRoot accountToBeMerged)
        {
            var initiatorRigths = accountToBeMerged.Account.GroupRoleCollection.CollectionContent
                .Where(grpRole => TBCollaboratorRole.HasInitiatorRights(grpRole.GroupRole))
                .ToArray();
            return initiatorRigths;
        }


        public static TBEmail[] GetTarget_EmailAddressesToBeMerged(TBRAccountRoot accountToBeMerged)
        {
            return accountToBeMerged.Account.Emails.CollectionContent.ToArray();
        }

        public static TBLoginInfo[] GetTarget_LoginAccessToBeMerged(TBRAccountRoot accountToBeMerged)
        {
            return accountToBeMerged.Account.Logins.CollectionContent.ToArray();
        }

        public static void ExecuteMethod_RemoveAccountToBeMergedFromAllGroups(TBRAccountRoot accountToBeMerged, TBAccountCollaborationGroup[] groupAccessToBeMerged)
        {
            var accountID = accountToBeMerged.ID;
            var groupIDs = groupAccessToBeMerged.Select(colGrp => colGrp.GroupID).ToArray();
            foreach (var groupID in groupIDs)
            {
                RemoveMemberFromGroup.Execute(new RemoveMemberFromGroupParameters
                {
                    AccountID = accountID,
                    GroupID = groupID
                });
            }
        }

        public static void ExecuteMethod_TransferGroupInitiatorRights(string primaryAccountToStayId, string accountToBeMergedAndDestroyedId, TBAccountCollaborationGroup[] groupInitiatorAccessToBeTransfered)
        {
            throw new NotImplementedException();
        }

        public static void ExecuteMethod_RemoveEmailAddressesFromAccountToBeMerged(TBRAccountRoot accountToBeMerged)
        {
            var accountID = accountToBeMerged.ID;
            var emailAddresses = accountToBeMerged.Account.Emails.CollectionContent.Select(emObj => emObj.EmailAddress).ToArray();
            foreach (string emailAddress in emailAddresses)
            {
                UnregisterEmailAddress.Execute(new UnregisterEmailAddressParameters
                    {
                        AccountID = accountID,
                        EmailAddress = emailAddress
                    });
            }
        }

        public static void ExecuteMethod_RemoveLoginsFromAccountToBeMerged(TBRAccountRoot accountToBeMerged)
        {
            var loginURLs = accountToBeMerged.Account.Logins.CollectionContent.Select(login => login.OpenIDUrl).ToArray();
            var loginIDs = loginURLs.Select(loginURL => TBLoginInfo.GetLoginIDFromLoginURL(loginURL)).ToArray();
            foreach (var loginID in loginIDs)
            {
                var loginRoot = TBRLoginRoot.RetrieveFromDefaultLocation(loginID);
                loginRoot.DeleteInformationObject();
            }
        }

        public static void ExecuteMethod_AddPrimaryAccountToAllGroupsWhereItsMissing(TBRAccountRoot primaryAccountToStay, TBAccountCollaborationGroup[] groupAccessToBeMerged)
        {
            throw new NotImplementedException();
        }

    }
}