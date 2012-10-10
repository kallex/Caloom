using System;
using System.IO;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class AddEmailAddressInfo : IAddOperationProvider
    {
        public bool PerformAddOperation(InformationSourceCollection sources, string requesterLocation)
        {
            // TODO: Properly separate acct add email and grp invite
            if (RelativeLocation.StartsWith("acc/"))
                AddAccountEmailAddressHandling();
            else if(RelativeLocation.StartsWith("grp/"))
            {
                GroupInvitationHandling();
            }
            else
                throw new InvalidDataException("Relative location of AddEmailAddressInfo is not acc or grp context bound");
            this.EmailAddress = "";
            return false;
        }

        private void GroupInvitationHandling()
        {
            string emailRootID = TBREmailRoot.GetIDFromEmailAddress(EmailAddress);
            TBREmailRoot emailRoot = TBREmailRoot.RetrieveFromDefaultLocation(emailRootID);
            if(emailRoot == null)
                throw new NotSupportedException("Email used for group invitation is not yet registered to the system");
            VirtualOwner owner = VirtualOwner.FigureOwner(this);
            string groupID = owner.LocationPrefix;
            TBEmailValidation emailValidation = new TBEmailValidation();
            emailValidation.Email = EmailAddress;
            emailValidation.ValidUntil = DateTime.Now.AddDays(14); // Two weeks to accept the group join
            emailValidation.GroupJoinConfirmation = new TBGroupJoinConfirmation
                                                        {
                                                            GroupID = groupID
                                                        };
            StorageSupport.StoreInformation(emailValidation);
            TBRGroupRoot groupRoot = TBRGroupRoot.RetrieveFromDefaultLocation(groupID);
            groupRoot.Group.InviteToGroup(EmailAddress, TBCollaboratorRole.ViewerRoleValue);
            StorageSupport.StoreInformation(groupRoot);
            EmailSupport.SendGroupJoinEmail(emailValidation, groupRoot.Group);
        }

        private void AddAccountEmailAddressHandling()
        {
            string emailRootID = TBREmailRoot.GetIDFromEmailAddress(EmailAddress);
            TBREmailRoot emailRoot = TBREmailRoot.RetrieveFromDefaultLocation(emailRootID);
            if (emailRoot != null)
                throw new InvalidDataException("Email address '" + EmailAddress + "' is already registered to the system.");
            string accountID = StorageSupport.GetAccountIDFromLocation(this.RelativeLocation);
            VirtualOwner owner = VirtualOwner.FigureOwner(this);
            TBEmailValidation emailValidation = new TBEmailValidation();
            emailValidation.AccountID = accountID;
            emailValidation.Email = this.EmailAddress;
            emailValidation.ValidUntil = DateTime.Now.AddMinutes(30);
            StorageSupport.StoreInformation(emailValidation);
            EmailSupport.SendValidationEmail(emailValidation);
        }
    }
}