using System;
using System.IO;
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
            var initiatorRights = accountToBeMerged.Account.GroupRoleCollection.CollectionContent
                .Where(grpRole => TBCollaboratorRole.HasInitiatorRights(grpRole.GroupRole))
                .ToArray();
            return initiatorRights;
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
                // Don't delete account login roots - as they're going to be merged later on
                //loginRoot.DeleteInformationObject();
            }
        }

        public static void ExecuteMethod_AddLoginsToPrimaryAccount(TBRAccountRoot primaryAccountToStay, TBLoginInfo[] loginAccessToBeMerged)
        {
            primaryAccountToStay.Account.Logins.CollectionContent.AddRange(loginAccessToBeMerged);
        }

        public static void ExecuteMethod_AddEmailAddressesToPrimaryAccount(TBRAccountRoot primaryAccountToStay, TBEmail[] emailAddressesToBeMerged)
        {
            primaryAccountToStay.Account.Emails.CollectionContent.AddRange(emailAddressesToBeMerged);
            foreach (var email in emailAddressesToBeMerged)
            {
                string emailRootID = TBREmailRoot.GetIDFromEmailAddress(email.EmailAddress);
                TBREmailRoot emailRoot = TBREmailRoot.RetrieveFromDefaultLocation(emailRootID);
                if (emailRoot == null)
                {
                    emailRoot = TBREmailRoot.CreateDefault();
                    emailRoot.ID = emailRootID;
                    emailRoot.UpdateRelativeLocationFromID();
                }
                emailRoot.Account = primaryAccountToStay.Account;
                StorageSupport.StoreInformation(emailRoot);
            }
        }

        public static void ExecuteMethod_StorePrimaryAccount(TBRAccountRoot primaryAccountToStay)
        {
            primaryAccountToStay.StoreInformation();
        }

        public static void ExecuteMethod_CallRefreshAccountRootToReferences(string primaryAccountToStayId)
        {
            UpdateAccountRootToReferences.Execute(new UpdateAccountRootToReferencesParameters {AccountID = primaryAccountToStayId});
        }

        public static void ExecuteMethod_AddPrimaryAccountToAllGroupsWhereItsMissing(TBRAccountRoot primaryAccountToStay, TBAccountCollaborationGroup[] groupAccessToBeMerged)
        {
            string memberEmailAddress = primaryAccountToStay.Account.Emails.CollectionContent.FirstOrDefault().EmailAddress;
            foreach (var groupAccess in groupAccessToBeMerged)
            {
                TBRGroupRoot groupRoot = TBRGroupRoot.RetrieveFromDefaultLocation(groupAccess.GroupID);
                TBCollaboratorRole role =
                    groupRoot.Group.Roles.CollectionContent.FirstOrDefault(
                        candidate => candidate.Email.EmailAddress == memberEmailAddress);
                if (role != null)
                {
                    continue;
                }
                else
                {
                    role = TBCollaboratorRole.CreateDefault();
                    role.Email.EmailAddress = memberEmailAddress;
                    role.Role = TBCollaboratorRole.CollaboratorRoleValue;
                    role.SetRoleAsMember();
                    groupRoot.Group.Roles.CollectionContent.Add(role);
                }
                groupRoot.StoreInformation();
            }
        }

        public static void ExecuteMethod_TransferGroupInitiatorRights(string primaryAccountToStayId, string accountToBeMergedAndDestroyedId, TBAccountCollaborationGroup[] groupInitiatorAccessToBeTransfered)
        {
            foreach (var groupAccess in groupInitiatorAccessToBeTransfered)
            {
                var groupID = groupAccess.GroupID;
                TransferGroupInitiator.Execute(new TransferGroupInitiatorParameters
                    {
                        GroupID = groupID,
                        NewInitiatorAccountID = primaryAccountToStayId,
                        OldInitiatorAccountID = accountToBeMergedAndDestroyedId
                    });
            }
        }

    }
}