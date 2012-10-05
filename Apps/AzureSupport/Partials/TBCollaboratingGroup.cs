using System;
using System.IO;
using System.Linq;
using System.Security;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class TBCollaboratingGroup : IContainerOwner
    {
        public string ContainerName
        {
            get { return "grp"; }
        }

        public string LocationPrefix
        {
            get { return ID; }
        }


        public void JoinToGroup(string emailAddress, string joinAsRole)
        {
            if (this.Roles.CollectionContent.Find(member => member.Email.EmailAddress == emailAddress) != null)
                return;
            TBAccount account = TBAccount.GetAccountFromEmail(emailAddress);
            TBEmail email = account.GetAccountEmail(emailAddress);
            var groupRole = new TBCollaboratorRole()
                                {
                                    Email = email,
                                    Role = joinAsRole,
                                    RoleStatus = TBCollaboratorRole.RoleStatusMemberValue,
                                };
            account.JoinGroup(this, groupRole);
            account.StoreAndPropagate();
            this.Roles.CollectionContent.Add(groupRole);
        }

        public bool InviteToGroup(string invitedEmailAddress, string invitedAsRole)
        {
            string role = InformationContext.Current.CurrentGroupRole;
            if (TBCollaboratorRole.HasModeratorRights(role) == false)
            {
                throw new UnauthorizedAccessException("Current role " + role + " does not have authorization to invite members to the group");
            }
            TBAccount account = TBAccount.GetAccountFromEmail(invitedEmailAddress);
            TBCollaboratorRole invited = TBCollaboratorRole.CreateDefault();
            invited.Email.EmailAddress = invitedEmailAddress;
            invited.Role = invitedAsRole;
            invited.SetRoleAsInvited();
            account.JoinGroup(this, invited);
            account.StoreAndPropagate();
            this.Roles.CollectionContent.Add(invited);
            return true;
        }

        public void ConfirmJoining(string invitedEmailAddress)
        {
            var role = GetRoleFromEmailAddress(invitedEmailAddress);
            role.SetRoleAsMember();
            TBAccount account = TBAccount.GetAccountFromEmail(invitedEmailAddress);
            var acctGroupRole = account.GroupRoleCollection.CollectionContent.First(grpRole => grpRole.GroupID == ID);
            acctGroupRole.GroupRole = role.Role;
            acctGroupRole.RoleStatus = TBCollaboratorRole.RoleStatusMemberValue;
            account.StoreAndPropagate();
        }

        public TBCollaboratorRole GetRoleFromAccount(TBAccount account)
        {
            var accountRole = Roles.CollectionContent.FirstOrDefault(
                role => account.Emails.CollectionContent.Exists(email => email.EmailAddress == role.Email.EmailAddress));
            if (accountRole == null)
                throw new InvalidDataException(String.Format("No role in group {0} found for requested account {1}",
                                                             Title, account.ID));
            return accountRole;
        }

        public TBCollaboratorRole GetRoleFromEmailAddress(string emailAddress)
        {
            var member = Roles.CollectionContent.FirstOrDefault(role => role.Email.EmailAddress == emailAddress);
            if (member == null)
                throw new InvalidDataException(String.Format("Role in group {0} not found for email address {1}", this.Title, emailAddress));
            return member;
        }

        public bool AssignRole(string memberEmailAddress, string memberRole)
        {
            string role = InformationContext.Current.CurrentGroupRole;
            if (TBCollaboratorRole.HasInitiatorRights(role) == false)
            {
                throw new UnauthorizedAccessException("Current role " + role +
                                                      " is not allowed to assign/change the roles of group members");
            }
            TBCollaboratorRole member = GetRoleFromEmailAddress(memberEmailAddress);
            member.Role = memberRole;
            member.SetRoleAsMember();
            TBAccount account = TBAccount.GetAccountFromEmail(memberEmailAddress);
            var accountRole = account.GroupRoleCollection.CollectionContent.FirstOrDefault(candidate => candidate.GroupID == ID);
            accountRole.GroupRole = member.Role;
            account.StoreAndPropagate();
            return true;
        }

        public void LeaveGroup(TBCollaboratorRole actionInitiator, bool lastPersonLeaving = false)
        {
            string role = InformationContext.Current.CurrentGroupRole;
            if (TBCollaboratorRole.HasInitiatorRights(role) && lastPersonLeaving == false)
                return;
            TBAccount account = TBAccount.GetAccountFromEmail(actionInitiator.Email.EmailAddress);
            account.GroupRoleCollection.CollectionContent.RemoveAll(candidate => candidate.GroupID == ID);
            account.StoreAndPropagate();
            this.Roles.CollectionContent.RemoveAll(candidate => candidate.Email.EmailAddress == actionInitiator.Email.EmailAddress);
        }

        public void DestroyGroup(TBCollaboratorRole actionInitiator)
        {
            string role = InformationContext.Current.CurrentGroupRole;
            if (TBCollaboratorRole.HasInitiatorRights(role) == false)
            {
                return;
            }
            LeaveGroup(actionInitiator, lastPersonLeaving: true);
            StorageSupport.DeleteEntireOwner(this);
        }
    }
}