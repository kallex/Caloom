using System;
using System.Linq;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class AddressAndLocationCollection 
    {
        /*
        public string GetItemDirectory()
        {
            string dummyItemLocation = AddressAndLocation.GetRelativeLocationFromID("dummy");
            string nonOwnerDirectoryLocation = SubscribeSupport.GetParentDirectoryTarget(dummyItemLocation);
            VirtualOwner owner = VirtualOwner.FigureOwner(this);
            string ownerDirectoryLocation = StorageSupport.GetBlobOwnerAddress(owner, nonOwnerDirectoryLocation);
            return ownerDirectoryLocation;
        }

        public void RefreshContent()
        {
            // DirectoryToMaster
            string itemDirectory = GetItemDirectory();
            IInformationObject[] informationObjects = StorageSupport.RetrieveInformationObjects(itemDirectory,
                                                                                         typeof(AddressAndLocation));
            CollectionContent.Clear();
            CollectionContent.AddRange(informationObjects.Select(obj => (AddressAndLocation) obj));
            
            // CollectionToCollection
            VirtualOwner owner = VirtualOwner.FigureOwner(this);
            AddressAndLocationCollection masterCollection = AddressAndLocationCollection.GetMasterCollectionInstance(owner);
            this.RefreshContentFromMaster(masterCollection);
        }

        private void RefreshContentFromMaster(AddressAndLocationCollection masterCollection)
        {
            // TODO: Instance specific filtering
        }

        private static AddressAndLocationCollection GetMasterCollectionInstance(IContainerOwner owner)
        {
            return AddressAndLocationCollection.RetrieveFromOwnerContent(owner, "MasterCollection");
        }

        public void SubscribeToContentSource()
        {
            // DirectoryToCollection
            string itemDirectory = GetItemDirectory();
            SubscribeSupport.AddSubscriptionToObject(itemDirectory, RelativeLocation,
                                                     SubscribeSupport.SubscribeType_DirectoryToCollection);
            // CollectionToCollection
            VirtualOwner owner = VirtualOwner.FigureOwner(this);
            string masterLocation = GetMasterCollectionLocation(owner);
            SubscribeSupport.AddSubscriptionToObject(masterLocation, RelativeLocation, SubscribeSupport.SubscribeType_CollectionToCollectionUpdate);
        }

        public static string GetMasterCollectionLocation(IContainerOwner owner)
        {
            return StorageSupport.GetBlobOwnerAddress(owner, "AaltoGlobalImpact.OIP/AddressAndLocationCollection/" + "MasterCollection");
        }
         * */
    }
}