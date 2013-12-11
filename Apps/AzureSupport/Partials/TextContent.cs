using System;

namespace AaltoGlobalImpact.OIP
{
    partial class TextContent : IAdditionalFormatProvider, IBeforeStoreHandler
    {
        AdditionalFormatContent[] IAdditionalFormatProvider.GetAdditionalContentToStore(string masterBlobETag)
        {
            return this.GetFormattedContentToStore(masterBlobETag, AdditionalFormatSupport.WebUIFormatExtensions);
        }

        string[] IAdditionalFormatProvider.GetAdditionalFormatExtensions()
        {
            return this.GetFormatExtensions(AdditionalFormatSupport.WebUIFormatExtensions);
        }

        void IBeforeStoreHandler.PerformBeforeStoreUpdate()
        {
            /* Don't need to modify from default time */
            /*
            if (Published == default(DateTime))
                Published = DateTime.UtcNow.Date;
             */
        }
    }
}