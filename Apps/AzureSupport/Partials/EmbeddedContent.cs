using System;

namespace AaltoGlobalImpact.OIP
{
    partial class EmbeddedContent : IAdditionalFormatProvider, IBeforeStoreHandler
    {
        AdditionalFormatContent[] IAdditionalFormatProvider.GetAdditionalContentToStore(string masterBlobETag)
        {
            return this.GetFormattedContentToStore(masterBlobETag);
        }

        string[] IAdditionalFormatProvider.GetAdditionalFormatExtensions()
        {
            return this.GetFormatExtensions(AdditionalFormatSupport.WebUIFormatExtensions);
        }

        public void PerformBeforeStoreUpdate()
        {
            if (Published == default(DateTime))
                Published = Published.ToUniversalTime();
        }
    }
}