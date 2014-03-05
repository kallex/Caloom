using System;
using System.IO;
using System.Runtime.Serialization;
using AaltoGlobalImpact.OIP;

namespace TheBall.CORE
{
    public class InitiateImportedGroupWithUnchangedIDImplementation
    {
        public static IContainerOwner GetTarget_GroupAsOwner(string groupId)
        {
            return VirtualOwner.FigureOwner("grp/" + groupId);
        }

        public static GroupContainer GetTarget_GroupContainer(IContainerOwner groupAsOwner)
        {
            return GroupContainer.RetrieveFromOwnerContent(groupAsOwner, "default");
        }

        public static void ExecuteMethod_ValidateGroupContainerID(string groupId, GroupContainer groupContainer)
        {
            if(groupContainer == null)
                throw new ArgumentNullException("groupContainer");
            if(groupContainer.RelativeLocation != "grp/" + groupId + "/AaltoGlobalImpact.OIP/GroupContainer/default")
                throw new InvalidDataException("Mismatch in group container location relative to group ID");
        }

        public static TBRGroupRoot GetTarget_GroupRoot(string groupID)
        {
            TBRGroupRoot groupRoot = new TBRGroupRoot();
            groupRoot.ID = groupID;
            groupRoot.UpdateRelativeLocationFromID();
            groupRoot.Group = new TBCollaboratingGroup();
            groupRoot.Group.ID = groupID;
            return groupRoot;
        }

        public static void ExecuteMethod_SetGroupInitiatorAccess(TBRGroupRoot groupRoot, GroupContainer groupContainer)
        {

        }

        public static void ExecuteMethod_StoreObjects(TBRGroupRoot groupRoot, GroupContainer groupContainer)
        {
            groupRoot.StoreInformation();
            groupContainer.StoreInformation();
        }

        public static void ExecuteMethod_FixContentTypesAndMetadataOfBlobs(IContainerOwner groupAsOwner)
        {
            throw new System.NotImplementedException();
        }

        public static void ExecuteMethod_FixRelativeLocationsOfInformationObjects(IContainerOwner groupAsOwner)
        {
            throw new NotImplementedException();
        }
    }
}