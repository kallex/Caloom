using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    public static class UpdateLoginGroupPermissionsImplementation
    {
        private const string SearchPrefix = "AaltoGlobalImpact.OIP/TBRLoginGroupRoot/";
        private static readonly int LoginStartIndex;
        private const string HttpsPrefix = "https://";
        private static readonly int HttpsPrefixLength;
        static UpdateLoginGroupPermissionsImplementation()
        {
            LoginStartIndex = SearchPrefix.Length + "g-".Length + Guid.Empty.ToString().Length + "-l-".Length;
            HttpsPrefixLength = HttpsPrefix.Length;
        }

        public static TBRAccountRoot GetTarget_AccountRoot(string accountID)
        {
            return TBRAccountRoot.RetrieveFromDefaultLocation(accountID);
        }

        public static TBRLoginGroupRoot[] GetTarget_LoginGroupRoots(TBRAccountRoot accountRoot)
        {
            var openIDUrlsWithoutHttps =
                accountRoot.Account.Logins.CollectionContent.Select(login => login.OpenIDUrl.Substring(HttpsPrefixLength)).OrderBy(str => str).
                    ToArray();
            var blobList = StorageSupport.CurrActiveContainer.ListBlobsWithPrefix(SearchPrefix,
                                                                                  new BlobRequestOptions()
                                                                                      {UseFlatBlobListing = true});
            List<CloudBlockBlob> foundBlobs = new List<CloudBlockBlob>(); 
            foreach(CloudBlockBlob blob in blobList)
            {
                string loginPartName = blob.Name.Substring(LoginStartIndex);
                int foundIndex = Array.BinarySearch(openIDUrlsWithoutHttps, loginPartName);
                if(foundIndex >= 0)
                    foundBlobs.Add(blob);
            }
            return foundBlobs.Select(blob => TBRLoginGroupRoot.RetrieveTBRLoginGroupRoot(blob.Name)).ToArray();
        }

        public static void ExecuteMethod_SynchronizeLoginGroupRoots(TBRAccountRoot accountRoot, TBRLoginGroupRoot[] loginGroupRoots)
        {
            //throw new NotImplementedException();
            //TBRLoginGroupRoot loginGroupRoot = TBRLoginGroupRoot.CreateDefault();
            //string loginGroupID = TBRLoginGroupRoot.GetLoginGroupID(groupRoleItem.GroupID, loginRootID);
            var accountLogins = accountRoot.Account.Logins.CollectionContent;
            var accountGroupRoles = accountRoot.Account.GroupRoleCollection.CollectionContent;
            List<TBRLoginGroupRoot> currentLoginGroupRoots = new List<TBRLoginGroupRoot>();
            foreach (var groupRoleItem in accountGroupRoles.Where(agr => TBCollaboratorRole.IsRoleStatusValidMember(agr.RoleStatus)))
            {
                foreach (var loginItem in accountLogins)
                {
                    string loginRootID = TBLoginInfo.GetLoginIDFromLoginURL(loginItem.OpenIDUrl);
                    string loginGroupID = TBRLoginGroupRoot.GetLoginGroupID(groupRoleItem.GroupID, loginRootID);
                    TBRLoginGroupRoot loginGroupRoot = loginGroupRoots.FirstOrDefault(lgr => lgr.ID == loginGroupID);
                    if (loginGroupRoot == null)
                    {
                        loginGroupRoot = TBRLoginGroupRoot.CreateDefault();
                        loginGroupRoot.ID = loginGroupID;
                        loginGroupRoot.UpdateRelativeLocationFromID();
                    }
                    loginGroupRoot.GroupID = groupRoleItem.GroupID;
                    loginGroupRoot.Role = groupRoleItem.GroupRole;
                    if(currentLoginGroupRoots.Any(lgr => lgr.ID == loginGroupID) == false)
                        currentLoginGroupRoots.Add(loginGroupRoot);
                }
            }
            var loginRootsToDelete = loginGroupRoots.
                Where(lgr => currentLoginGroupRoots.Exists(currLgr => currLgr.ID == lgr.ID) == false);
            foreach(var loginRootToDelete in loginRootsToDelete)
            {
                loginRootToDelete.DeleteInformationObject();
            }
            foreach (var currLoginRoot in currentLoginGroupRoots)
                currLoginRoot.StoreInformation();
            
        }
    }
}