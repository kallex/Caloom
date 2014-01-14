using TheBall.CORE;

namespace AaltoGlobalImpact.OIP
{
    partial class BinaryFile : IAdditionalFormatProvider
    {
        partial void DoPostDeleteExecute(IContainerOwner owner)
        {
            if(Data != null)
                Data.ClearCurrentContent(owner);
        }

        AdditionalFormatContent[] IAdditionalFormatProvider.GetAdditionalContentToStore(string masterBlobETag)
        {
            return this.GetFormattedContentToStore(masterBlobETag, AdditionalFormatSupport.WebUIFormatExtensions);
        }

        string[] IAdditionalFormatProvider.GetAdditionalFormatExtensions()
        {
            return this.GetFormatExtensions(AdditionalFormatSupport.WebUIFormatExtensions);
        }

    }
}