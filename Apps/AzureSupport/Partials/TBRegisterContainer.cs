using System;

namespace AaltoGlobalImpact.OIP
{
    partial class TBRegisterContainer
    {
        public static TBRegisterContainer CreateWithLoginProviders(string returnUrl, string title, string subtitle)
        {
            TBRegisterContainer registerContainer = TBRegisterContainer.CreateDefault();
            registerContainer.ReturnUrl = returnUrl;
            registerContainer.Header.Title = "Sign in";
            registerContainer.Header.SubTitle = "... or register";
            registerContainer.addLoginProviders(returnUrl);
            return registerContainer;
        }

        private void addLoginProviders(string returnUrl)
        {
            LoginProvider google = LoginProvider.CreateDefault();
            google.ProviderName = "Google";
            google.ProviderIconClass = "icon-oip-google";
            google.ProviderType = "openid";
            google.ProviderUrl = "https://www.google.com/accounts/o8/id";
            google.ReturnUrl = returnUrl;

            LoginProvider yahoo = LoginProvider.CreateDefault();
            yahoo.ProviderName = "Yahoo";
            yahoo.ProviderIconClass = "icon-oip-yahoo";
            yahoo.ProviderType = "openid";
            yahoo.ProviderUrl = "https://me.yahoo.com";
            yahoo.ReturnUrl = returnUrl;

            LoginProviderCollection.CollectionContent.Add(google);
            LoginProviderCollection.CollectionContent.Add(yahoo);
        }
    }
}