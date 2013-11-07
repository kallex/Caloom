using System.Diagnostics;
using AaltoGlobalImpact.OIP;
using TheBall.CORE;

namespace TheBall.Admin
{
    public class FixGroupMastersAndCollectionsImplementation
    {
        public static void ExecuteMethod_FixMastersAndCollections(string groupID)
        {
            Debug.WriteLine("Fixing group: " + groupID);
            TBRGroupRoot groupRoot = TBRGroupRoot.RetrieveFromDefaultLocation(groupID);
            IContainerOwner owner = groupRoot.Group;
            owner.InitializeAndConnectMastersAndCollections();
        }
    }
}