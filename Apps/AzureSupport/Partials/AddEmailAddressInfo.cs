using System;
using System.IO;
using System.Web;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class AddEmailAddressInfo : IAddOperationProvider
    {
        public bool PerformAddOperation(string commandName, InformationSourceCollection sources, string requesterLocation, HttpFileCollection files)
        {
            // TODO: Properly separate acct add email and grp invite
            EmailAddress = EmailAddress.ToLower();
            if (RelativeLocation.StartsWith("acc/"))
                AddAccountEmailAddressHandling();
            else if(RelativeLocation.StartsWith("grp/"))
            {
                GroupInvitationHandling();
            }
            else
                throw new InvalidDataException("Relative location of AddEmailAddressInfo is not acc or grp context bound");
            this.EmailAddress = "";
            return true;
        }

        private void GroupInvitationHandling()
        {
            string emailRootID = TBREmailRoot.GetIDFromEmailAddress(EmailAddress);
            TBREmailRoot emailRoot = TBREmailRoot.RetrieveFromDefaultLocation(emailRootID);
            if(emailRoot == null)
                throw new NotSupportedException("Email used for group invitation is not yet registered to the system");
            VirtualOwner owner = VirtualOwner.FigureOwner(this);
            string groupID = owner.LocationPrefix;
            InviteMemberToGroup.Execute(new InviteMemberToGroupParameters
                                            {GroupID = groupID, MemberEmailAddress = EmailAddress});
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