using System.Drawing.Imaging;
using System.IO;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class LinkToContent : IBeforeStoreHandler
    {
        public void PerformBeforeStoreUpdate()
        {
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
                        FileName = "AutoFetch.jpeg",
                        FileContent = jpegData
                    };
                this.SetMediaContent(InformationContext.CurrentOwner, ImageData.ID, mediaContent);
            }
        }
    }
}