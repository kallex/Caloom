using System;
using System.IO;
using System.Linq;
using AaltoGlobalImpact.OIP;

namespace TheBall.CORE
{
    public class CreateAndSendEmailValidationForDeviceJoinConfirmationImplementation
    {
        public static TBEmailValidation GetTarget_EmailValidation(TBAccount owningAccount, TBCollaboratingGroup owningGroup, DeviceMembership deviceMembership, string[] ownerEmailAddresses)
        {
            TBEmailValidation emailValidation = new TBEmailValidation();
            emailValidation.DeviceJoinConfirmation = new TBDeviceJoinConfirmation();
            if(owningAccount != null && owningGroup != null)
                throw new InvalidDataException("Both owning account and owning group cannot be defined");
            if(owningAccount == null && owningGroup == null)
                throw new InvalidDataException("Both owning account and owning group must not be null");
            if (owningAccount != null)
                emailValidation.DeviceJoinConfirmation.AccountID = owningAccount.ID;
            if (owningGroup != null)
                emailValidation.DeviceJoinConfirmation.GroupID = owningGroup.ID;
            emailValidation.DeviceJoinConfirmation.DeviceMembershipID = deviceMembership.ID;
            emailValidation.ValidUntil = DateTime.UtcNow.AddMinutes(30);
            emailValidation.Email = ownerEmailAddresses.FirstOrDefault();
            if(emailValidation.Email == null)
                throw new InvalidDataException("Owner must have at least one email address defined");
            return emailValidation;
        }

        public static string[] GetTarget_OwnerEmailAddresses(TBAccount owningAccount, TBCollaboratingGroup owningGroup)
        {
            if (owningAccount != null)
            {
                return owningAccount.Emails.CollectionContent.Select(email => email.EmailAddress).ToArray();
            }
            return owningGroup.Roles.CollectionContent.Where(role => TBCollaboratorRole.HasInitiatorRights(role.Role))
                        .Select(role => role.Email.EmailAddress).ToArray();
        }

        public static void ExecuteMethod_StoreObject(TBEmailValidation emailValidation)
        {
            emailValidation.StoreInformation();
        }

        public static void ExecuteMethod_SendEmailConfirmation(DeviceMembership deviceMembership, TBEmailValidation emailValidation, string[] ownerEmailAddresses)
        {
            EmailSupport.SendDeviceJoinEmail(emailValidation, deviceMembership, ownerEmailAddresses);
        }
    }
}