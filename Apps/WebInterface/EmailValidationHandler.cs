using System;
using System.IO;
using System.Security;
using System.Web;
using System.Web.Security;
using AaltoGlobalImpact.OIP;
using AzureSupport;
using DotNetOpenAuth.OpenId.RelyingParty;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;
using TheBall.CORE;

namespace WebInterface
{
    public class EmailValidationHandler : IHttpHandler
    {
        private const string AuthEmailValidation = "/emailvalidation/";
        private int AuthEmailValidationLen;


        /// <summary>
        /// You will need to configure this handler in the web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        public EmailValidationHandler()
        {
            AuthEmailValidationLen = AuthEmailValidation.Length;
        }

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            if (request.Path.StartsWith(AuthEmailValidation))
            {
                HandleEmailValidation(context);
            }        
        }

        private void HandleEmailValidation(HttpContext context)
        {
            var domainName = context.Request.Url.Host;
            string loginUrl = WebSupport.GetLoginUrl(context);
            TBRLoginRoot loginRoot = TBRLoginRoot.GetOrCreateLoginRootWithAccount(loginUrl, false, domainName);
            string requestPath = context.Request.Path;
            string emailValidationID = requestPath.Substring(AuthEmailValidationLen);
            TBAccount account = loginRoot.Account;
            TBEmailValidation emailValidation = TBEmailValidation.RetrieveFromDefaultLocation(emailValidationID);
            if (emailValidation == null)
            {
                RespondEmailValidationRecordNotExist(context);
                return;
            }
            StorageSupport.DeleteInformationObject(emailValidation);
            if (emailValidation.ValidUntil < DateTime.UtcNow)
            {
                RespondEmailValidationExpired(context, emailValidation);
                return;
            }
            if(emailValidation.GroupJoinConfirmation != null)
            {
                HandleGroupJoinConfirmation(context, account, emailValidation);
            }
            else if (emailValidation.DeviceJoinConfirmation != null)
            {
                HandleDeviceJoinConfirmation(context, account, emailValidation);
            }
            else if (emailValidation.InformationInputConfirmation != null)
            {
                HandleInputJoinConfirmation(context, account, emailValidation);
            }
            else if (emailValidation.InformationOutputConfirmation != null)
            {
                HandleOutputJoinConfirmation(context, account, emailValidation);
            }
            else if (emailValidation.MergeAccountsConfirmation != null)
            {
                HandleAccountMergeConfirmation(context, account, emailValidation);
            }
            else
            {
                HandleAccountEmailValidation(context, account, emailValidation);
            }
        }

        private void HandleAccountMergeConfirmation(HttpContext context, TBAccount account, TBEmailValidation emailValidation)
        {
            ConfirmAccountMergeFromEmail.Execute(new ConfirmAccountMergeFromEmailParameters
                {
                    CurrentAccountID = account.ID,
                    EmailConfirmation = emailValidation
                });
            string redirectUrl = emailValidation.RedirectUrlAfterValidation ?? "/auth/account/";
            context.Response.Redirect(redirectUrl, true);
        }

        private void HandleOutputJoinConfirmation(HttpContext context, TBAccount account, TBEmailValidation emailValidation)
        {
            ValidateAccountsEmailAddress(account, emailValidation);
            VirtualOwner owner;
            var outputJoinInfo = emailValidation.InformationOutputConfirmation;
            string redirectUrl;
            if (String.IsNullOrEmpty(outputJoinInfo.AccountID) == false)
            {
                owner = VirtualOwner.FigureOwner("acc/" + outputJoinInfo.AccountID);
                redirectUrl = "/auth/account/";
            }
            else
            {
                string groupID = outputJoinInfo.GroupID;
                owner = VirtualOwner.FigureOwner("grp/" + groupID);
                redirectUrl = "/auth/grp/" + groupID + "/";
            }
            SetInformationOutputValidationAndActiveStatus.Execute(
                new SetInformationOutputValidationAndActiveStatusParameters
                {
                    Owner = owner,
                    InformationOutputID = outputJoinInfo.InformationOutputID,
                    IsValidAndActive = true
                });
            context.Response.Redirect(redirectUrl, true);
        }

        private void HandleInputJoinConfirmation(HttpContext context, TBAccount account, TBEmailValidation emailValidation)
        {
            ValidateAccountsEmailAddress(account, emailValidation);
            VirtualOwner owner;
            var inputJoinInfo = emailValidation.InformationInputConfirmation;
            string redirectUrl;
            if (String.IsNullOrEmpty(inputJoinInfo.AccountID) == false)
            {
                owner = VirtualOwner.FigureOwner("acc/" + inputJoinInfo.AccountID);
                redirectUrl = "/auth/account/";
            }
            else
            {
                string groupID = inputJoinInfo.GroupID;
                owner = VirtualOwner.FigureOwner("grp/" + groupID);
                redirectUrl = "/auth/grp/" + groupID + "/";
            }
            SetInformationInputValidationAndActiveStatus.Execute(
                new SetInformationInputValidationAndActiveStatusParameters
                    {
                        Owner = owner,
                        InformationInputID = inputJoinInfo.InformationInputID,
                        IsValidAndActive = true
                    });
            context.Response.Redirect(redirectUrl, true);
        }

        private void HandleDeviceJoinConfirmation(HttpContext context, TBAccount account, TBEmailValidation emailValidation)
        {
            ValidateAccountsEmailAddress(account, emailValidation);
            VirtualOwner owner;
            var deviceJoinInfo = emailValidation.DeviceJoinConfirmation;
            string redirectUrl;
            if (String.IsNullOrEmpty(deviceJoinInfo.AccountID) == false)
            {
                owner = VirtualOwner.FigureOwner("acc/" + deviceJoinInfo.AccountID);
                redirectUrl = "/auth/account/";
            }
            else
            {
                string groupID = deviceJoinInfo.GroupID;
                owner = VirtualOwner.FigureOwner("grp/" + groupID);
                redirectUrl = "/auth/grp/" + groupID + "/";
            }
            SetDeviceMembershipValidationAndActiveStatus.Execute(new SetDeviceMembershipValidationAndActiveStatusParameters
                {
                    Owner = owner,
                    DeviceMembershipID = deviceJoinInfo.DeviceMembershipID,
                    IsValidAndActive = true
                });
            context.Response.Redirect(redirectUrl, true);
        }

        private void HandleGroupJoinConfirmation(HttpContext context, TBAccount account, TBEmailValidation emailValidation)
        {
            ValidateAccountsEmailAddress(account, emailValidation);
            string groupID = emailValidation.GroupJoinConfirmation.GroupID;
            ConfirmInviteToJoinGroup.Execute(new ConfirmInviteToJoinGroupParameters
                                                 {GroupID = groupID, MemberEmailAddress = emailValidation.Email});
            context.Response.Redirect("/auth/grp/" + groupID + "/");
        }

        private static void ValidateAccountsEmailAddress(TBAccount account, TBEmailValidation emailValidation)
        {
            if (
                account.Emails.CollectionContent.Exists(
                    candidate => candidate.EmailAddress.ToLower() == emailValidation.Email.ToLower()) == false)
                throw new SecurityException("Login account does not contain email address that was target of validation");
        }

        private void HandleAccountEmailValidation(HttpContext context, TBAccount account, TBEmailValidation emailValidation)
        {
            if (account.Emails.CollectionContent.Find(candidate => candidate.EmailAddress.ToLower() == emailValidation.Email.ToLower()) == null)
            {
                TBEmail email = TBEmail.CreateDefault();
                email.EmailAddress = emailValidation.Email;
                email.ValidatedAt = DateTime.Now;
                account.Emails.CollectionContent.Add(email);
                account.StoreAccountToRoot();
                // TODO: Move Emailroot storage to account root syncs
                string emailRootID = TBREmailRoot.GetIDFromEmailAddress(email.EmailAddress);
                TBREmailRoot emailRoot = TBREmailRoot.RetrieveFromDefaultLocation(emailRootID);
                if (emailRoot == null)
                {
                    emailRoot = TBREmailRoot.CreateDefault();
                    emailRoot.ID = emailRootID;
                    emailRoot.UpdateRelativeLocationFromID();
                }
                emailRoot.Account = account;
                StorageSupport.StoreInformation(emailRoot);

                string accountID = account.ID;
                UpdateAccountRootToReferences.Execute(new UpdateAccountRootToReferencesParameters
                                                          {
                                                              AccountID = accountID
                                                          });
                UpdateAccountContainerFromAccountRoot.Execute(new UpdateAccountContainerFromAccountRootParameters
                                                                  {
                                                                      AccountID = accountID
                                                                  });
            }

            if(String.IsNullOrEmpty(emailValidation.RedirectUrlAfterValidation) == false)
                context.Response.Redirect(emailValidation.RedirectUrlAfterValidation, true);
        }

        private void RespondEmailValidationRecordNotExist(HttpContext context)
        {
            context.Response.Redirect("/auth/account/", true);
            //context.Response.Write("Error to be replaced: email validation record does not exist.");
        }

        private void RespondEmailValidationExpired(HttpContext context, TBEmailValidation emailValidation)
        {
            context.Response.Redirect("/auth/account/", true);
            //context.Response.Write("Error to be replaced: email validation expired at: " + emailValidation.ValidUntil.ToString());
        }

        #endregion
    }
}
