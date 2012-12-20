using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using TheBall;
using TheBall.CORE;

namespace TheBall.CORE
{
    public interface IInformationCollection
    {
        string GetItemDirectory();
        void RefreshContent();
        void SubscribeToContentSource();
        bool IsMasterCollection { get; }
        string GetMasterLocation();
        IInformationCollection GetMasterInstance();
    }

    public interface IInformationObject
    {
        Guid OwnerID { get; set; }
        string ID { get; set; }
        string ETag { get; set; }
        string MasterETag { get; set; }
        string RelativeLocation { get; set; }
        string SemanticDomainName { get; set; }
        string Name { get; set; }
        bool IsIndependentMaster { get; }
        void InitializeDefaultSubscribers(IContainerOwner owner);
        void SetValuesToObjects(NameValueCollection form);
        void PostStoringExecute(IContainerOwner owner);
        void PostDeleteExecute(IContainerOwner owner);
        void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName);
        string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName);
        void SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent);
        void ReplaceObjectInTree(IInformationObject replacingObject);
        Dictionary<string, List<IInformationObject>> CollectMasterObjects(Predicate<IInformationObject> filterOnFalse = null);
        void CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse = null);
        IInformationObject RetrieveMaster(bool initiateIfMissing);
        IInformationObject RetrieveMaster(bool initiateIfMissing, out bool initiated);
        bool IsInstanceTreeModified { get; }
        void SetInstanceTreeValuesAsUnmodified();
        void UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceInstance);
        void FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly);
        void UpdateCollections(IInformationCollection masterInstance);
    }

    public class VirtualOwner : IContainerOwner
    {
        private string containerName;
        private string locationPrefix;

        public static VirtualOwner FigureOwner(IInformationObject ownedObject)
        {
            string relativeLocation = ownedObject.RelativeLocation;
            return FigureOwner(relativeLocation);
        }

        public static VirtualOwner FigureOwner(string relativeLocation)
        {
            if (relativeLocation.StartsWith("acc/") || relativeLocation.StartsWith("grp/"))
                return new VirtualOwner(relativeLocation.Substring(0, 3),
                    relativeLocation.Substring(4, StorageSupport.GuidLength));
            throw new InvalidDataException("Cannot figure owner of: " + relativeLocation);
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