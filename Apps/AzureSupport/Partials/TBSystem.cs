using System;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class TBSystem : IContainerOwner
    {
        private const string SingletonRelativeLocation = "AAA-System";
        static TBSystem()
        {
            TBSystem system = (TBSystem)StorageSupport.RetrieveInformation(SingletonRelativeLocation, typeof(TBSystem));
            if(system == null)
            {
                system = CreateDefault();
                system.ID = "AAA";
                system.RelativeLocation = SingletonRelativeLocation;
                StorageSupport.StoreInformation(system);
            }
            currSystem = system;
        }

        private static TBSystem currSystem;
        public static TBSystem CurrSystem
        {
            get { return currSystem; }
        }


        public string ContainerName
        {
            get { return "sys"; }
        }

        public string LocationPrefix
        {
            get { return CurrSystem.ID; }
        }
    }
}