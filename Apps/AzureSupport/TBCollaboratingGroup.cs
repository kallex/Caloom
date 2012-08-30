using System;

namespace AaltoGlobalImpact.OIP
{
    partial class TBCollaboratingGroup : IContainerOwner
    {
        public string ContainerName
        {
            get { return "grp"; }
        }

        public string LocationPrefix
        {
            get { return ID; }
        }
    }
}