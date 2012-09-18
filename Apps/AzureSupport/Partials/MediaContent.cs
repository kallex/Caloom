using System;
using System.IO;
using System.Runtime.Serialization;
using System.Web;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class MediaContent
    {
        public string ContentUrl
        {
            get { return RenderWebSupport.GetUrlFromRelativeLocation(RelativeLocation); }
        }

        [DataMember]
        public string OriginalFileName { get; set; }

        [DataMember]
        public string FileExt { get; set; }

        [DataMember]
        public int ContentLength { get; set; }

        public void SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
        {
            if(ID != contentObjectID)
                return;
            HttpPostedFile postedContent = (HttpPostedFile) mediaContent;
            FileExt = Path.GetExtension(postedContent.FileName);
            ContentLength = postedContent.ContentLength;
            string locationFileName = ID + FileExt;
            SetLocationAsOwnerContent(containerOwner, locationFileName);
            StorageSupport.CurrActiveContainer.UploadBlobStream(RelativeLocation, postedContent.InputStream, StorageSupport.InformationType_GenericContentValue);
        }
    }
}