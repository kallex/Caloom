using System;
using System.IO;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    public class VirtualOwner : IContainerOwner
    {
        private string containerName;
        private string locationPrefix;

        public static VirtualOwner FigureOwner(IInformationObject ownedObject)
        {
            if(ownedObject.RelativeLocation.StartsWith("acc/") || ownedObject.RelativeLocation.StartsWith("grp/"))
                return new VirtualOwner(ownedObject.RelativeLocation.Substring(0, 3),
                    ownedObject.RelativeLocation.Substring(4, StorageSupport.GuidLength));
            throw new InvalidDataException("Cannot figure owner of: " + ownedObject.RelativeLocation);
        }

        public VirtualOwner(string containerName, string locationPrefix)
        {
            this.containerName = containerName;
            this.locationPrefix = locationPrefix;
        }

        public string ContainerName
        {
            get { return containerName; }
        }

        public string LocationPrefix
        {
            get { return locationPrefix; }
        }
    }
}