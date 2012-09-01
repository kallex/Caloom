using System;

namespace AaltoGlobalImpact.OIP
{
    partial class TBRLoginGroupRoot : IContainerOwner
    {
        public string ContainerName
        {
            get { return "grp"; }
        }

        public string LocationPrefix
        {
            get { return GroupID; }
        }

        public static string GetLoginGroupID(string groupID, string loginRootID)
        {
            string loginGroupID = "g-" + groupID + "-l-" + loginRootID;
            return loginGroupID;
        }
    }
}