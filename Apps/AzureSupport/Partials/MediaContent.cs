using System;
using System.IO;
using System.Runtime.Serialization;
using System.Web;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;
using TheBall.CORE;
using System.Linq;

namespace AaltoGlobalImpact.OIP
{
    partial class MediaContent
    {
        public string ContentUrl
        {
            get { return RenderWebSupport.GetUrlFromRelativeLocation(RelativeLocation); }
        }

        public string ContentUrlBase
        {
            get
            {
                return RenderWebSupport.GetUrlFromRelativeLocation(
                    RenderWebSupport.GetLocationWithoutExtension(RelativeLocation));
            }
        }

        [DataMember]
        public string OriginalFileName { get; set; }

        [DataMember]
        public string FileExt { get; set; }

        [DataMember]
        public string AdditionalFormatFileExt
        {
            get
            {
                string additionalFormatFileExt;
                if (FileExt == ".jpg" || FileExt == ".jpeg")
                    additionalFormatFileExt = ".jpg";
                else if (FileExt == ".png" || FileExt == ".gif" || FileExt == ".bmp")
                    additionalFormatFileExt = ".png";
                else
                    additionalFormatFileExt = null;
                return additionalFormatFileExt;
            }
            set
            {
                // Empty body to please the (de)serialization
            }
        }

        [DataMember]
        public int ContentLength { get; set; }

        public void SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
        {
            if(ID != contentObjectID)
                return;
            ClearCurrentContent(containerOwner);
            HttpPostedFile postedContent = (HttpPostedFile) mediaContent;
            FileExt = Path.GetExtension(postedContent.FileName);
            ContentLength = postedContent.ContentLength;
            string locationFileName = ID + FileExt;
            SetLocationAsOwnerContent(containerOwner, locationFileName);
            postedContent.InputStream.Seek(0, SeekOrigin.Begin);
            StorageSupport.CurrActiveContainer.UploadBlobStream(RelativeLocation, postedContent.InputStream, StorageSupport.InformationType_GenericContentValue);
            UpdateAdditionalMediaFormats();
        }

        public void UpdateAdditionalMediaFormats()
        {
            RemoveAdditionalMediaFormats();
            CreateAdditionalMediaFormats();
        }

        public void CreateAdditionalMediaFormats()
        {
            if (FileExt == ".jpg" || FileExt == ".jpeg" || FileExt == ".png" || FileExt == ".gif" || FileExt == ".bmp")
                OIP.CreateAdditionalMediaFormats.Execute(new CreateAdditionalMediaFormatsParameters { MasterRelativeLocation = RelativeLocation });
        }

        public void RemoveAdditionalMediaFormats()
        {
            ClearAdditionalMediaFormats.Execute(new ClearAdditionalMediaFormatsParameters { MasterRelativeLocation = RelativeLocation });
        }

        public void ClearCurrentContent(IContainerOwner containerOwner)
        {
            CloudBlob blob = StorageSupport.CurrActiveContainer.GetBlob(RelativeLocation, containerOwner);
            blob.DeleteWithoutFiringSubscriptions();
            RemoveAdditionalMediaFormats();
        }
    }
}