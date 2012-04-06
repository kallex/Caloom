using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace CaloomMVC
{
    class StoreData
    {
        public ClaimsResponse ProfileFields;
        public string FriendlyLoginName;
    }

    public partial class OpenIDLogin : System.Web.UI.Page
    {
        // https://www.google.com/accounts/o8/id

        private StoreData Database = new StoreData();
        protected void Page_Load(object sender, EventArgs e)
        {
            openIdBox.Focus(); OpenIdRelyingParty openid = new OpenIdRelyingParty(); var response = openid.GetResponse(); if (response != null)
            {
                switch (response.Status)
                {
                    case AuthenticationStatus.Authenticated:
                        // This is where you would look for any OpenID extension responses included                                
                        // in the authentication assertion.                                
                        var claimsResponse = response.GetExtension<ClaimsResponse>();
                        Database.ProfileFields = claimsResponse;
                        // Store off the "friendly" username to display -- NOT for username lookup                                
                        Database.FriendlyLoginName = response.FriendlyIdentifierForDisplay;
                        UriIdentifier uriId = (UriIdentifier) response.ClaimedIdentifier;
                        // Use FormsAuthentication to tell ASP.NET that the user is now logged in,                                
                        // with the OpenID Claimed Identifier as their username.                                
                        FormsAuthentication.RedirectFromLoginPage(response.ClaimedIdentifier, false);
                        break;
                    case AuthenticationStatus.Canceled:
                        this.loginCanceledLabel.Visible = true;
                        break;
                    case AuthenticationStatus.Failed:
                        this.loginFailedLabel.Visible = true;
                        break;
                }
            }
        }


        protected void openidValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // This catches common typos that result in an invalid OpenID Identifier.        
            args.IsValid = Identifier.IsValid(args.Value);
        }


        protected void loginButton_Click(object sender, EventArgs e)
        {
            if (!this.Page.IsValid)
            {
                return;
                // don't login if custom validation failed.        
            }
            try
            {
                using (OpenIdRelyingParty openid = new OpenIdRelyingParty())
                {
                    IAuthenticationRequest request = openid.CreateRequest(this.openIdBox.Text);
                    // This is where you would add any OpenID extensions you wanted                        
                    // to include in the authentication request.                        
                    request.AddExtension(new ClaimsRequest()
                                             {
                                                 //Country = DemandLevel.Require,
                                                 Email = DemandLevel.Require,
                                                 //Gender = DemandLevel.Require,
                                                 //PostalCode = DemandLevel.Require,
                                                 //TimeZone = DemandLevel.Require,
                                             });
                    // Send your visitor to their Provider for authentication.                       
                    request.RedirectToProvider();
                }
            }
            catch (ProtocolException ex)
            {
                // The user probably entered an Identifier that                
                // was not a valid OpenID endpoint.                
                this.openidValidator.Text = ex.Message;
                this.openidValidator.IsValid = false;
            }
        }

    }
}