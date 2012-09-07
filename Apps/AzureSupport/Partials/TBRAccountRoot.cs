using System;
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
        public static TBRAccountRoot CreateAndStoreNewAccount()
        {
            TBRAccountRoot accountRoot = TBRAccountRoot.CreateDefault();
            accountRoot.ID = accountRoot.Account.ID;
            accountRoot.UpdateRelativeLocationFromID();
            StorageSupport.StoreInformation(accountRoot);
            return accountRoot;
        }

        public static TBRAccountRoot GetOwningAccountRoot(IInformationObject informationObject)
        {
            string accountID = StorageSupport.GetAccountIDFromLocation(informationObject.RelativeLocation);
            return TBRAccountRoot.RetrieveFromDefaultLocation(accountID);
        }
    }
}