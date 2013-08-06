using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using TheBall;
using TheBall.CORE;

namespace TheBall.CORE
{
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

}

namespace AaltoGlobalImpact.OIP
{

    public static class ExtIInformationObject
    {
        public static void SetObjectContent(this IInformationObject rootObject, string containerID,
                                            string containedField, string[] objectIDList)
        {
            List<IInformationObject> containerList = new List<IInformationObject>();
            rootObject.FindObjectsFromTree(containerList, iObj => iObj.ID == containerID, false);
            foreach (var iObj in containerList)
            {
                var type = iObj.GetType();
                var prop = type.GetProperty(containedField);
                if (prop == null)
                    throw new InvalidDataException(String.Format("No property {0} found in type {1}", containedField, type.Name));
                IInformationObject containedObject = (IInformationObject) prop.GetValue(iObj, null);
                if (containedObject == null && objectIDList.Length == 0)
                    continue;
                if (objectIDList.Length == 0)
                {
                    prop.SetValue(iObj, null, null);
                }
                else
                {
                    VirtualOwner owner = VirtualOwner.FigureOwner(rootObject.RelativeLocation);
                    Type contentType = prop.PropertyType;
                    string contentDomain = contentType.Namespace;
                    string contentTypeName = contentType.Name;
                    bool isCollectionType = typeof(IInformationCollection).IsAssignableFrom(contentType);
                    if (isCollectionType)
                    {
                        if (containedObject == null)
                        {
                            containedObject = (IInformationObject) Activator.CreateInstance(contentType);
                            prop.SetValue(iObj, containedObject, null);
                        }
                        dynamic dynObj = containedObject;
                        object listObject = dynObj.CollectionContent;
                        // Note the below works for List<T>, that we know the type is of ;-)
                        Type collectionItemType = listObject.GetType().GetGenericArguments()[0];
                        // This is assuming collections are referring within same domain only
                        contentTypeName = collectionItemType.Name;
                        IList contentList = (IList) listObject;
                        IEnumerable<IInformationObject> contentEnum = (IEnumerable<IInformationObject>) listObject;
                        List<IInformationObject> objectsToRemove = new List<IInformationObject>();
                        foreach (IInformationObject existingObject in contentList)
                        {
                            if(objectIDList.Contains(existingObject.ID) == false)
                                objectsToRemove.Add(existingObject);
                        }
                        objectsToRemove.ForEach(obj => contentList.Remove(obj));
                        foreach (string contentObjectID in objectIDList)
                        {
                            if (contentEnum.Any(item => item.ID == contentObjectID))
                                continue;
                            IInformationObject contentObject =
                                StorageSupport.RetrieveInformationObjectFromDefaultLocation(contentDomain, contentTypeName, contentObjectID,
                                                                                            owner);
                            if (contentObject == null)
                                continue;
                            contentList.Add(contentObject);
                        }
                    }
                    else
                    {
                        if(objectIDList.Length > 1)
                            throw new InvalidDataException("Object link name " + containedField + " of type " + contentTypeName + " does not allow multiple values");
                        string contentObjectID = objectIDList[0];
                        IInformationObject contentObject =
                            StorageSupport.RetrieveInformationObjectFromDefaultLocation(contentDomain, contentTypeName, contentObjectID,
                                                                                        owner);
                        prop.SetValue(iObj, contentObject, null);
                    }
                    //RetrieveInformationObjectFromDefaultLocation()
                }
            }
        }

        public static void SetMediaContent(this IInformationObject rootObject, IContainerOwner containerOwner,
                                           string containerID, string containedField,
                                           object mediaContent)
        {
            List<IInformationObject> containerList = new List<IInformationObject>();
            rootObject.FindObjectsFromTree(containerList, iObj => iObj.ID == containerID, false);
            foreach (var iObj in containerList)
            {
                var type = iObj.GetType();
                var prop = type.GetProperty(containedField);
                if(prop == null)
                    throw new InvalidDataException(String.Format("No property {0} found in type {1}", containedField, type.Name));
                MediaContent propValue = (MediaContent) prop.GetValue(iObj, null);
                if (propValue == null && mediaContent == null)
                    continue;
                if (propValue != null && mediaContent == null)
                {
                    propValue.ClearCurrentContent(containerOwner);
                    prop.SetValue(iObj, null, null);
                    continue;
                }
                if (propValue == null && mediaContent != null)
                {
                    propValue = MediaContent.CreateDefault();
                    prop.SetValue(iObj, propValue, null);
                }
                propValue.SetMediaContent(containerOwner, propValue.ID, mediaContent);
            }
        }
    }

}

