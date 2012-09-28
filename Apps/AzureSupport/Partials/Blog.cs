using System.IO;
using System.Runtime.Serialization.Json;

namespace AaltoGlobalImpact.OIP
{
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
}