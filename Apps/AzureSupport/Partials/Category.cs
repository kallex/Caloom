using System.IO;
using System.Runtime.Serialization.Json;

namespace AaltoGlobalImpact.OIP
{
    partial class CategoryCollection : IAdditionalFormatProvider
    {
        AdditionalFormatContent[] IAdditionalFormatProvider.GetAdditionalContentToStore()
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

        string[] IAdditionalFormatProvider.GetAdditionalFormatExtensions()
        {
            return new string[] { "json" };
        }
    }

    partial class Category : IBeforeStoreHandler
    {
        public void PerformBeforeStoreUpdate()
        {
            if (ReferenceToInformation == null)
                ReferenceToInformation = OIP.ReferenceToInformation.CreateDefault();
            ReferenceToInformation.Title = this.CategoryName;
            ReferenceToInformation.URL = DefaultViewSupport.GetDefaultViewURL(this);
        }
    }

    partial class Category : IAdditionalFormatProvider
    {
        AdditionalFormatContent[] IAdditionalFormatProvider.GetAdditionalContentToStore()
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

        string[] IAdditionalFormatProvider.GetAdditionalFormatExtensions()
        {
            return new string[] { "json" };
        }

    }

}