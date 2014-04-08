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
                else if (FileExt == ".png" || FileExt == ".bmp")
                    additionalFormatFileExt = ".png";
                else if (FileExt == ".gif")
                    additionalFormatFileExt = ".gif";
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
            MediaFileData mediaFileData = mediaContent as MediaFileData;
            if(mediaFileData == null)
                throw new NotSupportedException("Not supported mediaContent object in SetMediaContent");
            if (mediaFileData.HttpFile != null)
            {
                HttpPostedFile postedContent = mediaFileData.HttpFile;
                FileExt = Path.GetExtension(postedContent.FileName);
                ContentLength = postedContent.ContentLength;
                string locationFileName = ID + FileExt;
                SetLocationAsOwnerContent(containerOwner, locationFileName);
                postedContent.InputStream.Seek(0, SeekOrigin.Begin);
                StorageSupport.CurrActiveContainer.UploadBlobStream(RelativeLocation, postedContent.InputStream);
            }
            else
            {
                FileExt = Path.GetExtension(mediaFileData.FileName);
                ContentLength = mediaFileData.FileContent.Length;
                string locationFileName = ID + FileExt;
                SetLocationAsOwnerContent(containerOwner, locationFileName);
                StorageSupport.CurrActiveContainer.UploadBlobBinary(RelativeLocation, mediaFileData.FileContent);
            }
            UpdateAdditionalMediaFormats();
        }

        public void SetMediaContent(string contentFileName, byte[] contentData)
        {
            var owner = InformationContext.CurrentOwner;
            ClearCurrentContent(owner);
            FileExt = Path.GetExtension(contentFileName);
            ContentLength = contentData.Length;
            string locationFileName = ID + FileExt;
            SetLocationAsOwnerContent(owner, locationFileName);
            StorageSupport.CurrActiveContainer.UploadBlobBinary(RelativeLocation, contentData);
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

        public string GetMD5FromStorage()
        {
            try
            {
                var owner = InformationContext.CurrentOwner;
                CloudBlob mainBlob = StorageSupport.CurrActiveContainer.GetBlob(RelativeLocation, owner);
                mainBlob.FetchAttributes();
                return mainBlob.Properties.ContentMD5;
            }
            catch
            {
                return null;
            }
        }

        public static string CalculateComparableMD5(byte[] data)
        {
            var md5Check = System.Security.Cryptography.MD5.Create();
            md5Check.TransformBlock(data, 0, data.Length, null, 0);
            md5Check.TransformFinalBlock(new byte[0], 0, 0);
            byte[] hashBytes = md5Check.Hash;
            string hashVal = Convert.ToBase64String(hashBytes);
            return hashVal;
        }

        public byte[] GetContentData()
        {
            var owner = InformationContext.CurrentOwner;
            var blob = StorageSupport.CurrActiveContainer.GetBlob(RelativeLocation, owner);
            try
            {
                byte[] result = blob.DownloadByteArray();
                return result;
            }
            catch
            {
                return null;
            }
        }

    }
}