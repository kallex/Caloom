using System;
using System.IO;
using AaltoGlobalImpact.OIP;

namespace TheBall.CORE
{
    public class BeginAccountEmailAddressRegistrationImplementation
    {
        public static void ExecuteMethod_ValidateUnexistingEmail(string emailAddress)
        {
            string emailRootID = TBREmailRoot.GetIDFromEmailAddress(emailAddress);
            TBREmailRoot emailRoot = TBREmailRoot.RetrieveFromDefaultLocation(emailRootID);
            if (emailRoot != null)
                throw new InvalidDataException("Email address '" + emailAddress + "' is already registered to the system.");
        }

        public static TBEmailValidation GetTarget_EmailValidation(string accountID, string emailAddress)
        {
            TBEmailValidation emailValidation = new TBEmailValidation();
            emailValidation.AccountID = accountID;
            emailValidation.Email = emailAddress;
            emailValidation.ValidUntil = DateTime.UtcNow.AddMinutes(30);
            return emailValidation;
        }

        public static void ExecuteMethod_StoreObject(TBEmailValidation emailValidation)
        {
            emailValidation.StoreInformation();
        }

        public static void ExecuteMethod_SendEmailConfirmation(TBEmailValidation emailValidation)
        {
            EmailSupport.SendValidationEmail(emailValidation);
        }
    }
}