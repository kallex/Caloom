using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class Blog : IBeforeStoreHandler
    {
        public void PerformBeforeStoreUpdate()
        {
            if (ReferenceToInformation == null)
                ReferenceToInformation = OIP.ReferenceToInformation.CreateDefault();
            this.ReferenceToInformation.Title = this.Title;
            ReferenceToInformation.URL = RelativeLocation; // DefaultViewSupport.GetDefaultViewURL(this);
            this.LocationCollection.IsCollectionFiltered = true;
            this.CategoryCollection.IsCollectionFiltered = true;
            this.ImageGroupCollection.IsCollectionFiltered = true;
            if (Excerpt == null)
                Excerpt = "";
            if(Excerpt.Length > 200)
                Excerpt = Excerpt.Substring(0, 200);
            SetProfileImageAsFeaturedImage();
            if (Published == default(DateTime))
                Published = DateTime.UtcNow.Date;
        }

        private void SetProfileImageAsFeaturedImage()
        {
            FeaturedImage = ProfileImage;
        }
    }
#if jsondatetimefixed
    partial class Blog : IAdditionalFormatProvider
    {
        public AdditionalFormatContent[] GetAdditionalContentToStore()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(GetType());

            byte[] dataContent;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, this);
                dataContent = memoryStream.ToArray();

            }
            return new AdditionalFormatContent[]
                       {
                           new AdditionalFormatContent {Extension = "json", Content = dataContent}
                       };
        }

        public string[] GetAdditionalFormatExtensions()
        {
            return new string[] { "json" };
        }

    }
#endif
}