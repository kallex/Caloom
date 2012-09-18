using System;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure.StorageClient;

namespace TheBall
{
    public static class InformationSourceSupport
    {
        public static InformationSourceCollection GetInformationSources(string targetLocation)
        {
            string blobPath = InformationSourceCollection.GetRelativeLocationAsMetadataTo(targetLocation);
            var result = StorageSupport.RetrieveInformation(blobPath, typeof(InformationSourceCollection));
            return (InformationSourceCollection)result;
        }

        public static void SetInformationSources(string targetLocation, InformationSourceCollection sourceCollection)
        {
            if(sourceCollection == null)
            {
                // Delete existing
                string blobPath = InformationSourceCollection.GetRelativeLocationAsMetadataTo(targetLocation);
                CloudBlob blob = StorageSupport.CurrActiveContainer.GetBlobReference(blobPath);
                blob.DeleteWithoutFiringSubscriptions();
                return;
            }
            sourceCollection.SetRelativeLocationAsMetadataTo(targetLocation);
            StorageSupport.StoreInformation(sourceCollection);
        }

        public static void DeleteInformationSources(string targetLocation)
        {
            string blobPath = InformationSourceCollection.GetRelativeLocationAsMetadataTo(targetLocation);
            StorageSupport.DeleteBlob(blobPath);
        }
    }
}