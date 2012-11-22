using System;

namespace AaltoGlobalImpact.OIP
{
    public static class UpdateLoginGroupPermissionsImplementation
    {
        public static TBRAccountRoot GetTarget_AccountRoot(string accountID)
        {
            return TBRAccountRoot.RetrieveFromDefaultLocation(accountID);
        }

        public static TBRLoginGroupRoot[] GetTarget_LoginGroupRoots(TBRAccountRoot accountRoot)
        {
            //throw new NotImplementedException();
            return null;
        }

        public static void ExecuteMethod_SynchronizeLoginGroupRoots(TBRAccountRoot accountRoot, TBRLoginGroupRoot[] loginGroupRoots)
        {
            //throw new NotImplementedException();
        }
    }
}