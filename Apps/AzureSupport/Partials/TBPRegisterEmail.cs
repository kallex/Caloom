using System;
using System.Web;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class TBPRegisterEmail
    {
        partial void DoPostStoringExecute(IContainerOwner owner)
        {
            return;
            if (String.IsNullOrEmpty(EmailAddress))
                return;
            HttpContext context = HttpContext.Current;
            if (context == null)
                return;
            TBAccount account = (TBAccount) context.Items["Account"];
            TBEmailValidation emailValidation = TBEmailValidation.CreateDefault();
            emailValidation.AccountID = account.ID;
            emailValidation.Email = this.EmailAddress;
            emailValidation.ValidUntil = DateTime.Now.AddMinutes(30);
            StorageSupport.StoreInformation(emailValidation, owner);
            EmailSupport.SendValidationEmail(emailValidation);
        }
    }

}