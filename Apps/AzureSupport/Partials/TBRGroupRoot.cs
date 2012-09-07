using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class TBRGroupRoot
    {
        public static string[] GetAllGroupIDs()
        {
            string blobPath = "AaltoGlobalImpact.OIP/TBRGroupRoot/";
            string searchPath = StorageSupport.CurrActiveContainer.Name + "/" + blobPath;
            int substringLen = blobPath.Length;
            var blobList = StorageSupport.CurrBlobClient.ListBlobsWithPrefix(searchPath).OfType<CloudBlob>();
            return blobList.Select(blob => blob.Name.Substring(substringLen)).ToArray();
        }

        public static TBRGroupRoot CreateNewWithGroup()
        {
            TBRGroupRoot groupRoot = TBRGroupRoot.CreateDefault();
            groupRoot.Group.ID = groupRoot.ID;
            return groupRoot;
        }
    }
}