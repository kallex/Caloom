using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using TheBall.CORE;

namespace AaltoGlobalImpact.OIP
{
    public static class AdditionalFormatSupport
    {
        public static string[] WebUIFormatExtensions = new string[] { "json"};

        public static AdditionalFormatContent[] GetFormattedContentToStore(this IAdditionalFormatProvider providerObject, string masterBlobETag, params string[] formatExtensions)
        {
            List<AdditionalFormatContent> contentList = new List<AdditionalFormatContent>(formatExtensions.Length);
            IInformationObject iObj = providerObject as IInformationObject;
            string oldMasterETag = null;
            if (iObj != null)
            {
                oldMasterETag = iObj.MasterETag;
            }
            try
            {
                iObj.MasterETag = masterBlobETag;
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
                            throw new NotSupportedException("Not supported extension for automatic format support: " +
                                                            extension);
                    }
                    contentList.Add(content);
                }
            }
            finally
            {
                if (iObj != null)
                    iObj.MasterETag = oldMasterETag;
            }

            return contentList.ToArray();
        }

        public static string[] GetFormatExtensions(this IAdditionalFormatProvider providerObject, params string[] formatExtensions)
        {
            return formatExtensions;
        }

        
    }
}