using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;

namespace AaltoGlobalImpact.OIP
{
    public static class AdditionalFormatSupport
    {
        public static string[] WebUIFormatExtensions = new string[] { "json"};

        public static AdditionalFormatContent[] GetFormattedContentToStore(this IAdditionalFormatProvider providerObject, params string[] formatExtensions)
        {
            List<AdditionalFormatContent> contentList = new List<AdditionalFormatContent>(formatExtensions.Length);
            foreach (string extension in formatExtensions)
            {
                AdditionalFormatContent content = null;
                switch (extension)
                {
                    case "json":
                        {
                            DataContractJsonSerializer serializer =
                                new DataContractJsonSerializer(providerObject.GetType());

                            byte[] dataContent;
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                serializer.WriteObject(memoryStream, providerObject);
                                dataContent = memoryStream.ToArray();

                            }
                            content = new AdditionalFormatContent {Extension = "json", Content = dataContent};
                        }
                        break;
                    default:
                        throw new NotSupportedException("Not supported extension for automatic format support: " + extension);
                }
                contentList.Add(content);
            }
            return contentList.ToArray();
        }

        public static string[] GetFormatExtensions(this IAdditionalFormatProvider providerObject, params string[] formatExtensions)
        {
            return formatExtensions;
        }

        
    }
}