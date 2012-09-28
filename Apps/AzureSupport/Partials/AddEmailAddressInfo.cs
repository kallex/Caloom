using System;
using System.IO;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class AddEmailAddressInfo : IAddOperationProvider
    {
        public bool PerformAddOperation(InformationSourceCollection sources, string requesterLocation)
        {
            string emailRootID = TBREmailRoot.GetIDFromEmailAddress(EmailAddress);
            TBREmailRoot emailRoot = TBREmailRoot.RetrieveFromDefaultLocation(emailRootID);
            if(emailRoot != null)
                throw new InvalidDataException("Email address '" + EmailAddress + "' is already registered to the system.");
            string accountID = StorageSupport.GetAccountIDFromLocation(this.RelativeLocation);
            VirtualOwner owner = VirtualOwner.FigureOwner(this);
            TBEmailValidation emailValidation = TBEmailValidation.CreateDefault();
            emailValidation.AccountID = accountID;
            emailValidation.Email = this.EmailAddress;
            emailValidation.ValidUntil = DateTime.Now.AddMinutes(30);
            StorageSupport.StoreInformation(emailValidation, owner);
            EmailSupport.SendValidationEmail(emailValidation);
            this.EmailAddress = "";
            return true;
        }
    }
}