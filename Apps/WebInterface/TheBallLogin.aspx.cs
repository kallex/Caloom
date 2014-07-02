using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetOpenAuth.AspNet.Clients;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;
using TheBall;

namespace WebInterface
{
    public partial class TheBallLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Params["ssokey"] != null)
            {
                handleWilmaLogin();
                return;
            }
            if(Request.Params["SignOut"] != null)
            {
                AuthenticationSupport.ClearAuthenticationCookie(Response);
                Response.Redirect("/", true);
                return;
            }
            openIdBox.Focus();         
            OpenIdRelyingParty openid = new OpenIdRelyingParty();        
            var response = openid.GetResponse();
            ClaimsResponse profileFields;
            string friendlyName;
            string idprovider = Request.Params["idprovider"];
            string idProviderUrl = Request.Params["idProviderUrl"];
            if (response != null)
            {
                switch (response.Status)
                {
                    case AuthenticationStatus.Authenticated:
                        // This is where you would look for any OpenID extension responses included
                        // in the authentication assertion.                                
                        var claimsResponse = response.GetExtension<ClaimsResponse>();
                        profileFields = claimsResponse;
                        // Store off the "friendly" username to display -- NOT for username lookup                                
                        friendlyName = response.FriendlyIdentifierForDisplay;
                        // Use FormsAuthentication to tell ASP.NET that the user is now logged in,                                
                        // with the OpenID Claimed Identifier as their username. 
                        string userName = response.ClaimedIdentifier.ToString();
                        //FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, userName,
                        //                                                                 DateTime.Now,
                        //                                                                 DateTime.Now.AddDays(10),
                        //                                                                 true, "user custom data");

                        AuthenticationSupport.SetAuthenticationCookie(Response, userName);
                        //FormsAuthentication.RedirectFromLoginPage(response.ClaimedIdentifier, false);
                        //string redirectUrl = FormsAuthentication.GetRedirectUrl(userName, true);
                        string redirectUrl = Request.Params["ReturnUrl"];
                        if (redirectUrl == null)
                            redirectUrl = FormsAuthentication.DefaultUrl;
                        Response.Redirect(redirectUrl, true);
                        break;
                    case AuthenticationStatus.Canceled:
                        this.loginCanceledLabel.Visible = true;
                        break;
                    case AuthenticationStatus.Failed:
                        this.loginFailedLabel.Visible = true;
                        break;
                }
            } else
            {
                if(String.IsNullOrEmpty(idProviderUrl) == false)
                {
                    CreateOpenIDRequestAndRedirect(idProviderUrl);
                    return;
                }
                if (idprovider != null)
                {
                    switch (idprovider)
                    {
                        case "google":
                            PerformGoogleLogin();
                            return;
                        case "yahoo":
                            PerformYahooLogin();
                            return;
                        case "aol":
                            PerformAOLLogin();
                            return;
                        case "wordpress":
                            openIdBox.Text = "http://ENTER-YOUR-BLOG-NAME-HERE.wordpress.com";
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void handleOAuth2()
        {
        }

        private void handleWilmaLogin()
        {
            string ssokey = Request.Params["ssokey"];
            string query = Request.Params["query"];
            string logout = Request.Params["logout"];
            string nonce = Request.Params["nonce"];
            string h = Request.Params["h"];
            if(nonce.Length < 16 || nonce.Length > 40)
                throw new SecurityException("Invalid login parameters");
            string hashSourceStr = string.Format("ssokey={0}&query={1}&logout={2}&nonce={3}",
                ssokey, query, logout, nonce
                /*
                HttpUtility.UrlEncode(ssokey),
                HttpUtility.UrlEncode(query),
                HttpUtility.UrlEncode(logout),
                HttpUtility.UrlEncode(nonce)*/
                );
            var hashSourceBin = Encoding.UTF8.GetBytes(hashSourceStr);
            throw new NotImplementedException("Wilma login functional, pre-shared secret needs to be config/non-source code implemented");
            HMACSHA1 hmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes("INSERTYOURSHAREDSECRETHERE"));
            var hashValue = hmacsha1.ComputeHash(hashSourceBin);
            string hashValueStr = Convert.ToBase64String(hashValue);
            if(hashValueStr != h)
                throw new SecurityException("Invalid hash value");

            // Login verification done, then call to Wilma for user results
            RNGCryptoServiceProvider rngCrypto = new RNGCryptoServiceProvider();
            var cryptoData = new byte[16];
            rngCrypto.GetBytes(cryptoData);
            string myLogout = "";
            string myNonce = Convert.ToBase64String(cryptoData);
            string myHSource = string.Format("ssokey={0}&logout={1}&nonce={2}",
                ssokey, myLogout, myNonce);
            var myHHash = hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(myHSource));
            string myH = Convert.ToBase64String(myHHash);
            string wilmaRequestUrl = String.Format("{0}?ssokey={1}&logout={2}&nonce={3}&h={4}",
                query,
                ssokey,
                HttpUtility.UrlEncode(myLogout),
                HttpUtility.UrlEncode(myNonce), 
                HttpUtility.UrlEncode(myH));
            HttpWebRequest wilmaRequest = WebRequest.CreateHttp(wilmaRequestUrl);
            Debug.WriteLine(myNonce);
            Debug.WriteLine(myNonce.Length);
            var response = wilmaRequest.GetResponse();
            var stream = response.GetResponseStream();
            string content = null;
            var isoEnc = Encoding.GetEncoding("ISO-8859-1");
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) {
            /*
            using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("ISO-8859-1")))
            {
            var dataBuffer = new byte[100*1024];
            using(BinaryReader bReader = new BinaryReader(stream)) {
                var byteCount = bReader.Read(dataBuffer, 0, dataBuffer.Length);
                var convertedContent = Encoding.Convert(Encoding.ASCII, isoEnc,
                    dataBuffer, 0, byteCount);
                content = isoEnc.GetString(convertedContent);
             */
                content = reader.ReadToEnd();
                content = content.Replace("ä", "??").Replace("ö", "??");
                int hPosition = content.LastIndexOf("\r\nh=") + 2;
                var contentWithoutH = content.Substring(0, hPosition);
                //var verifyH = hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(contentWithoutH));
                var verifyH = hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(contentWithoutH));
                var verifyH2 = hmacsha1.ComputeHash(isoEnc.GetBytes(contentWithoutH));
                string verifyHTxt = Convert.ToBase64String(verifyH);
                string verifyH2Txt = Convert.ToBase64String(verifyH2);
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
            string openIdUrl = this.openIdBox.Text;
            CreateOpenIDRequestAndRedirect(openIdUrl);
        }

        private void CreateOpenIDRequestAndRedirect(string openIdUrl)
        {
            try
            {
                using (OpenIdRelyingParty openid = new OpenIdRelyingParty())
                {
                    IAuthenticationRequest request = openid.CreateRequest(openIdUrl);
                    // This is where you would add any OpenID extensions you wanted                        
                    // to include in the authentication request.                        
                    request.AddExtension(new ClaimsRequest
                                             {
                                                 //Country = DemandLevel.Request,
                                                 //Email = DemandLevel.Request,
                                                 //Gender = DemandLevel.Require,
                                                 //PostalCode = DemandLevel.Require,
                                                 //TimeZone = DemandLevel.Require,
                                             }); // Send your visitor to their Provider for authentication.
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

        protected void bGoogleLogin_Click(object sender, EventArgs e)
        {
            PerformGoogleLogin();
        }



        private void PerformAOLLogin()
        {
            CreateOpenIDRequestAndRedirect("https://www.aol.com");
        }


        private void PerformGoogleLogin()
        {
            CreateOpenIDRequestAndRedirect("https://www.google.com/accounts/o8/id");
        }

        private void PerformYahooLogin()
        {
            CreateOpenIDRequestAndRedirect("https://me.yahoo.com");
        }


    }
}