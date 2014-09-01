using System;
using System.Drawing.Imaging;
using System.IO;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class LinkToContent : IBeforeStoreHandler, IAdditionalFormatProvider
    {
        public void PerformBeforeStoreUpdate()
        {
            if (URL != null)
                URL = URL.Trim();
            if (ImageData == null && string.IsNullOrEmpty(URL) == false)
            {
                ImageData = new MediaContent();
                byte[] jpegData;
                var bitmap = WebsiteThumbnail.WebsiteThumbnailImageGenerator.GetWebSiteThumbnail(URL,
                                                                                                 1280, 800, 1280, 800);
                using (MemoryStream memStream = new MemoryStream())
                {
                    bitmap.Save(memStream, ImageFormat.Jpeg);
                    jpegData = memStream.ToArray();
                }
                MediaFileData mediaContent = new MediaFileData
                    {
                        FileName = "AutoFetch.jpg",
                        FileContent = jpegData
                    };
                this.SetMediaContent(InformationContext.CurrentOwner, ImageData.ID, mediaContent);
            }
            if (Published == default(DateTime))
                Published = Published.ToUniversalTime();
        }

        AdditionalFormatContent[] IAdditionalFormatProvider.GetAdditionalContentToStore(string masterBlobETag)
        {
            return this.GetFormattedContentToStore(masterBlobETag);
        }

        string[] IAdditionalFormatProvider.GetAdditionalFormatExtensions()
        {
            return this.GetFormatExtensions(AdditionalFormatSupport.WebUIFormatExtensions);
        }
    }
}