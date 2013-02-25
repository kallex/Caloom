using System;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    public static class ClearAdditionalMediaFormatsImplementation
    {
        public static void ExecuteMethod_ClearImageMediaFormats(string masterRelativeLocation)
        {
            string currExtension = Path.GetExtension(masterRelativeLocation);
            int currExtensionLength = currExtension == null ? 0 : currExtension.Length;
            string masterLocationWithoutExtension = masterRelativeLocation.Substring(0,
                                                                                     masterRelativeLocation.Length -
                                                                                     currExtensionLength);
            var masterRelatedBlobs = StorageSupport.CurrActiveContainer.ListBlobsWithPrefix(masterLocationWithoutExtension + "_");
            foreach(var cloudBlob in masterRelatedBlobs.Cast<CloudBlockBlob>().Where(blob => blob.Name.EndsWith(".jpg") || blob.Name.EndsWith(".png")))
            {
                cloudBlob.DeleteWithoutFiringSubscriptions();
            }
        }
    }
}