using System.Linq;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class TBRAccountRoot
    {
        public static string[] GetAllAccountIDs()
        {
            string blobPath = "AaltoGlobalImpact.OIP/TBRAccountRoot/";
            string searchPath = StorageSupport.CurrActiveContainer.Name + "/" + blobPath;
            int substringLen = blobPath.Length;
            var blobList = StorageSupport.CurrBlobClient.ListBlobsWithPrefix(searchPath).OfType<CloudBlob>();
            return blobList.Select(blob => blob.Name.Substring(substringLen)).ToArray();
        }
    }
}