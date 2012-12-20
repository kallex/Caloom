using System;
using System.Diagnostics;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;
using TheBall.CORE;

namespace AaltoGlobalImpact.OIP
{
    [DebuggerDisplay("Source: {SourceName} Type: {SourceType} IO: {SourceInformationObjectType} ETag: {SourceETag} Mod: {SourceLastModified}")]
    partial class InformationSource
    {
        public bool IsInformationObjectSource
        {
            get { return SourceInformationObjectType != null; }
        }

        public bool IsWebTemplateSource
        {
            get { return SourceType == StorageSupport.InformationType_WebTemplateValue || SourceType == StorageSupport.InformationType_RuntimeWebTemplateValue; }
        }

        public static InformationSource FromBlob(CloudBlob blob)
        {
            InformationSource source = CreateDefault();
            source.SourceLocation = blob.Name;
            source.SourceETag = blob.Properties.ETag;
            source.SourceName = "";
            source.SourceType = blob.GetBlobInformationType();
            source.SourceInformationObjectType = blob.GetBlobInformationObjectType();
            source.SourceETag = blob.Properties.ETag;
            source.SourceMD5 = blob.Properties.ContentMD5;
            source.SourceLastModified = blob.Properties.LastModifiedUtc;
            return source;
        }

        public IInformationObject RetrieveInformationObject()
        {
            if (String.IsNullOrEmpty(SourceInformationObjectType))
                return null;
            CloudBlob blob;
            IInformationObject content = StorageSupport.RetrieveInformationWithBlob(SourceLocation, SourceInformationObjectType, out blob);
            SourceETag = content.ETag;
            SourceMD5 = blob.Properties.ContentMD5;
            SourceLastModified = blob.Properties.LastModifiedUtc;
            return content;
        }

        public void SetBlobValuesToSource(CloudBlob blob)
        {
            SourceLocation = blob.Name;
            SourceETag = blob.Properties.ETag;
            SourceType = blob.GetBlobInformationType();
            SourceMD5 = blob.Properties.ContentMD5;
            SourceLastModified = blob.Properties.LastModifiedUtc;
        }

        public void SetInformationObjectValuesToSource(string sourceName, string informationObjectType)
        {
            SourceName = sourceName;
            SourceInformationObjectType = informationObjectType;
        }

        public bool HasSourceChanged()
        {
            CloudBlob blob = StorageSupport.CurrActiveContainer.GetBlob(SourceLocation);
            bool isChanged = blob.Properties.ContentMD5 != SourceMD5;
            return isChanged;
        }

        public static InformationSource GetAsDefaultSource(IInformationObject informationObject)
        {
            CloudBlob blob = StorageSupport.GetInformationObjectBlobWithProperties(informationObject);
            InformationSource informationSource = FromBlob(blob);
            return informationSource;
        }
    }
}