 


using DOM=TheBall.Interface;

namespace TheBall.CORE {
	public static partial class OwnerInitializer
	{
		private static void DOMAININIT_TheBall_Interface(IContainerOwner owner)
		{
			DOM.DomainInformationSupport.EnsureMasterCollections(owner);
			DOM.DomainInformationSupport.RefreshMasterCollections(owner);
		}
	}
}


namespace TheBall.Interface { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;
using TheBall.CORE;



			[DataContract]
			public partial class ConnectionCommunicationData
			{
				[DataMember]
				public string ActiveSideConnectionID { get; set; }
				[DataMember]
				public string ReceivingSideConnectionID { get; set; }
				[DataMember]
				public string ProcessRequest { get; set; }
				[DataMember]
				public string ProcessParametersString { get; set; }
				[DataMember]
				public string ProcessResultString { get; set; }
				[DataMember]
				public string[] ProcessResultArray { get; set; }
				[DataMember]
				public CategoryInfo[] CategoryCollectionData { get; set; }
				[DataMember]
				public CategoryLinkItem[] LinkItems { get; set; }
			}

			[DataContract]
			public partial class CategoryInfo
			{
				[DataMember]
				public string CategoryID { get; set; }
				[DataMember]
				public string NativeCategoryID { get; set; }
				[DataMember]
				public string NativeCategoryDomainName { get; set; }
				[DataMember]
				public string NativeCategoryObjectName { get; set; }
				[DataMember]
				public string NativeCategoryTitle { get; set; }
				[DataMember]
				public string IdentifyingCategoryName { get; set; }
				[DataMember]
				public string ParentCategoryID { get; set; }
			}

			[DataContract]
			public partial class CategoryLinkParameters
			{
				[DataMember]
				public string ConnectionID { get; set; }
				[DataMember]
				public CategoryLinkItem[] LinkItems { get; set; }
			}

			[DataContract]
			public partial class CategoryLinkItem
			{
				[DataMember]
				public string SourceCategoryID { get; set; }
				[DataMember]
				public string TargetCategoryID { get; set; }
				[DataMember]
				public string LinkingType { get; set; }
			}

		public static class DomainInformationSupport
		{
            public static void EnsureMasterCollections(IContainerOwner owner)
            {
                {
                    var masterCollection = ConnectionCollection.GetMasterCollectionInstance(owner);
                    if(masterCollection == null)
                    {
                        masterCollection = ConnectionCollection.CreateDefault();
                        masterCollection.RelativeLocation =
                            ConnectionCollection.GetMasterCollectionLocation(owner);
                        StorageSupport.StoreInformation(masterCollection, owner);
                    }
					IInformationCollection collection = masterCollection;
					collection.SubscribeToContentSource();
                }
                {
                    var masterCollection = GenericObjectCollection.GetMasterCollectionInstance(owner);
                    if(masterCollection == null)
                    {
                        masterCollection = GenericObjectCollection.CreateDefault();
                        masterCollection.RelativeLocation =
                            GenericObjectCollection.GetMasterCollectionLocation(owner);
                        StorageSupport.StoreInformation(masterCollection, owner);
                    }
					IInformationCollection collection = masterCollection;
					collection.SubscribeToContentSource();
                }
            }

            public static void RefreshMasterCollections(IContainerOwner owner)
            {
                {
                    IInformationCollection masterCollection = ConnectionCollection.GetMasterCollectionInstance(owner);
                    if (masterCollection == null)
                        throw new InvalidDataException("Master collection ConnectionCollection missing for owner");
                    masterCollection.RefreshContent();
                    StorageSupport.StoreInformation((IInformationObject) masterCollection, owner);
                }
                {
                    IInformationCollection masterCollection = GenericObjectCollection.GetMasterCollectionInstance(owner);
                    if (masterCollection == null)
                        throw new InvalidDataException("Master collection GenericObjectCollection missing for owner");
                    masterCollection.RefreshContent();
                    StorageSupport.StoreInformation((IInformationObject) masterCollection, owner);
                }
            }
		}
			[DataContract]
			[Serializable]
			public partial class ConnectionCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public ConnectionCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "ConnectionCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.Interface/ConnectionCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(ConnectionCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.Interface", "ConnectionCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static ConnectionCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveConnectionCollection(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: ConnectionCollection");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(ConnectionCollection), null, owner);
					if(master == null && initiateIfMissing)
					{
						StorageSupport.StoreInformation(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}


				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return iObject.RetrieveMaster(initiateIfMissing, out initiated);
				}


                public static ConnectionCollection RetrieveConnectionCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ConnectionCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(ConnectionCollection), null, owner);
                    return result;
                }

				public static ConnectionCollection RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = ConnectionCollection.RetrieveConnectionCollection("Content/TheBall.Interface/ConnectionCollection/" + contentName, containerOwner);
					var result = ConnectionCollection.RetrieveConnectionCollection("TheBall.Interface/ConnectionCollection/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.Interface/ConnectionCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.Interface/ConnectionCollection/" + contentName);
                }

				partial void DoInitializeDefaultSubscribers(IContainerOwner owner);

			    public void InitializeDefaultSubscribers(IContainerOwner owner)
			    {
					DoInitializeDefaultSubscribers(owner);
			    }

				partial void DoPostStoringExecute(IContainerOwner owner);

				public void PostStoringExecute(IContainerOwner owner)
				{
					DoPostStoringExecute(owner);
				}

				partial void DoPostDeleteExecute(IContainerOwner owner);

				public void PostDeleteExecute(IContainerOwner owner)
				{
					DoPostDeleteExecute(owner);
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        dynamic dyn = targetObject;
                        dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((ConnectionCollection) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
				}

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ConnectionCollection));
					using (var output = new StringWriter())
					{
						using (var writer = new XmlTextWriter(output))
						{
                            if(noFormatting == false)
						        writer.Formatting = Formatting.Indented;
							serializer.WriteObject(writer, this);
						}
						return output.GetStringBuilder().ToString();
					}
				}

				public static ConnectionCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ConnectionCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ConnectionCollection) serializer.ReadObject(xmlReader);
					}
            
				}

				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

                [DataMember]
                public Guid OwnerID { get; set; }

                [DataMember]
                public string RelativeLocation { get; set; }

                [DataMember]
                public string Name { get; set; }

                [DataMember]
                public string SemanticDomainName { get; set; }

				[DataMember]
				public string MasterETag { get; set; }

				[DataMember]
				public string GeneratedByProcessID { get; set; }

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.Interface", "ConnectionCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.Interface", "ConnectionCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref ConnectionCollection customDemoObject);


				
				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.UpdateCollections(masterInstance);
					}
				}



				bool IInformationCollection.IsMasterCollection {
					get {
						return true;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					return GetMasterCollectionLocation(owner);
					
				}

				IInformationCollection IInformationCollection.GetMasterInstance()
				{
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					return GetMasterCollectionInstance(owner);
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = Connection.GetRelativeLocationFromID("dummy");
					string nonOwnerDirectoryLocation = SubscribeSupport.GetParentDirectoryTarget(dummyItemLocation);
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public void RefreshContent()
				{
					// DirectoryToMaster
					string itemDirectory = GetItemDirectory();
					IInformationObject[] informationObjects = StorageSupport.RetrieveInformationObjects(itemDirectory,
																								 typeof(Connection));
                    Array.ForEach(informationObjects, io => io.MasterETag = io.ETag);
					CollectionContent.Clear();
					CollectionContent.AddRange(informationObjects.Select(obj => (Connection) obj));
            
				}

				public static ConnectionCollection GetMasterCollectionInstance(IContainerOwner owner)
				{
					return ConnectionCollection.RetrieveFromOwnerContent(owner, "MasterCollection");
				}

				public void SubscribeToContentSource()
				{
					// DirectoryToCollection
					string itemDirectory = GetItemDirectory();
					SubscribeSupport.AddSubscriptionToObject(itemDirectory, RelativeLocation,
															 SubscribeSupport.SubscribeType_DirectoryToCollection, null, typeof(ConnectionCollection).FullName);
				}

				public static string GetMasterCollectionLocation(IContainerOwner owner)
				{
					return StorageSupport.GetOwnerContentLocation(owner, "TheBall.Interface/ConnectionCollection/" + "MasterCollection");
				}



                public void SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }

				
		
				public static ConnectionCollection CreateDefault()
				{
					var result = new ConnectionCollection();
					return result;
				}

				/*
				public static ConnectionCollection CreateDemoDefault()
				{
					ConnectionCollection customDemo = null;
					ConnectionCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new ConnectionCollection();
					result.CollectionContent.Add(Connection.CreateDemoDefault());
					//result.CollectionContent.Add(Connection.CreateDemoDefault());
					//result.CollectionContent.Add(Connection.CreateDemoDefault());
					return result;
				}
				*/

		
				[DataMember] public List<Connection> CollectionContent = new List<Connection>();
				private Connection[] _unmodified_CollectionContent;

				[DataMember] public bool IsCollectionFiltered;
				private bool _unmodified_IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();
				private string[] _unmodified_OrderFilterIDList;

				public string SelectedIDCommaSeparated
				{
					get
					{
						string[] sourceArray;
						if (OrderFilterIDList != null)
							sourceArray = OrderFilterIDList.ToArray();
						else
							sourceArray = CollectionContent.Select(item => item.ID).ToArray();
						return String.Join(",", sourceArray);
					}
					set 
					{
						if (value == null)
							return;
						string[] valueArray = value.Split(',');
						OrderFilterIDList = new List<string>();
						OrderFilterIDList.AddRange(valueArray);
						OrderFilterIDList.RemoveAll(item => CollectionContent.Any(colItem => colItem.ID == item) == false);
					}
				}

				public Connection[] GetIDSelectedArray()
				{
					if (IsCollectionFiltered == false || this.OrderFilterIDList == null)
						return CollectionContent.ToArray();
					return
						this.OrderFilterIDList.Select(id => CollectionContent.FirstOrDefault(item => item.ID == id)).Where(item => item != null).ToArray();
				}

				public void RefreshOrderAndFilterListFromContent()
                {
                    if (OrderFilterIDList == null)
                        return;
                    OrderFilterIDList.RemoveAll(item => CollectionContent.Any(colItem => colItem.ID == item) == false);
                }

				public void ParsePropertyValue(string propertyName, string propertyValue)
				{
					switch(propertyName)
					{
						case "SelectedIDCommaSeparated":
							SelectedIDCommaSeparated = propertyValue;
							break;
						case "IsCollectionFiltered":
							IsCollectionFiltered = bool.Parse(propertyValue);
							break;
						default:
							throw new NotSupportedException("No ParsePropertyValue supported for property: " + propertyName);
					}
				}


				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					for(int i = 0; i < CollectionContent.Count; i++) // >
					{
						if(CollectionContent[i].ID == replacingObject.ID)
							CollectionContent[i] = (Connection )replacingObject;
						else { // Cannot have circular reference, so can be in else branch
							IInformationObject iObject = CollectionContent[i];
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}

				
				bool IInformationObject.IsInstanceTreeModified {
					get {
						bool collectionModified = CollectionContent.SequenceEqual(_unmodified_CollectionContent) == false;
						if(collectionModified)
							return true;
						//if((OrderFilterIDList == null && _unmodified_OrderFilterIDList != null) || _unmodified_OrderFilterIDList
						if(IsCollectionFiltered != _unmodified_IsCollectionFiltered)
							return true;
						// For non-master content
						foreach(IInformationObject item in CollectionContent)
						{
							bool itemTreeModified = item.IsInstanceTreeModified;
							if(itemTreeModified)
								return true;
						}
							
						return false;
					}
				}
				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CollectionContent = CollectionContent.ToArray();
					_unmodified_IsCollectionFiltered = IsCollectionFiltered;
					if(OrderFilterIDList == null)
						_unmodified_OrderFilterIDList = null;
					else
						_unmodified_OrderFilterIDList = OrderFilterIDList.ToArray();
					foreach(IInformationObject iObject in CollectionContent)
						iObject.SetInstanceTreeValuesAsUnmodified();
				}

				private void CopyContentFrom(ConnectionCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
				private object FindFromObjectTree(string objectId)
				{
					foreach(var item in CollectionContent)
					{
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					foreach(IInformationObject iObject in CollectionContent)
						iObject.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
				}


				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						bool doAdd = true;
						if(filterOnFalse != null)
							doAdd = filterOnFalse(iObject);
						if(doAdd) {
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
				}


			
			}
			[DataContract]
			[Serializable]
			public partial class Connection : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public Connection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "Connection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.Interface/Connection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(Connection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.Interface", "Connection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Connection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveConnection(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: Connection");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(Connection), null, owner);
					if(master == null && initiateIfMissing)
					{
						StorageSupport.StoreInformation(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}


				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return iObject.RetrieveMaster(initiateIfMissing, out initiated);
				}


                public static Connection RetrieveConnection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Connection) StorageSupport.RetrieveInformation(relativeLocation, typeof(Connection), null, owner);
                    return result;
                }

				public static Connection RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = Connection.RetrieveConnection("Content/TheBall.Interface/Connection/" + contentName, containerOwner);
					var result = Connection.RetrieveConnection("TheBall.Interface/Connection/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.Interface/Connection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.Interface/Connection/" + contentName);
                }

				partial void DoInitializeDefaultSubscribers(IContainerOwner owner);

			    public void InitializeDefaultSubscribers(IContainerOwner owner)
			    {
					DoInitializeDefaultSubscribers(owner);
			    }

				partial void DoPostStoringExecute(IContainerOwner owner);

				public void PostStoringExecute(IContainerOwner owner)
				{
					DoPostStoringExecute(owner);
				}

				partial void DoPostDeleteExecute(IContainerOwner owner);

				public void PostDeleteExecute(IContainerOwner owner)
				{
					DoPostDeleteExecute(owner);
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				void IInformationObject.SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Connection));
					using (var output = new StringWriter())
					{
						using (var writer = new XmlTextWriter(output))
						{
                            if(noFormatting == false)
						        writer.Formatting = Formatting.Indented;
							serializer.WriteObject(writer, this);
						}
						return output.GetStringBuilder().ToString();
					}
				}

				public static Connection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Connection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Connection) serializer.ReadObject(xmlReader);
					}
            
				}

				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

                [DataMember]
                public Guid OwnerID { get; set; }

                [DataMember]
                public string RelativeLocation { get; set; }

                [DataMember]
                public string Name { get; set; }

                [DataMember]
                public string SemanticDomainName { get; set; }

				[DataMember]
				public string MasterETag { get; set; }

				[DataMember]
				public string GeneratedByProcessID { get; set; }

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.Interface", "Connection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.Interface", "Connection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref Connection customDemoObject);




				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					// Remove exception if basic functionality starts to have issues
					//throw new NotImplementedException("Item level collections do not support object tree operations right now");
					if(filterOnFalse(this))
						result.Add(this);
				}

				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Object tree support not implemented for item level collection objects");


				}

			
                void IInformationObject.SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}
	

				bool IInformationObject.IsInstanceTreeModified {
					get { 
						// Remove exception if some basic functionality is broken due to it
						throw new NotImplementedException("Collection items do not support instance tree queries as of now");
					}
				}
				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					// Remove exception if some basic functionality is broken due to it
					//throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "OutputInformationID":
							OutputInformationID = value;
							break;
						case "Description":
							Description = value;
							break;
						case "DeviceID":
							DeviceID = value;
							break;
						case "IsActiveParty":
							IsActiveParty = bool.Parse(value);
							break;
						case "OtherSideConnectionID":
							OtherSideConnectionID = value;
							break;
						case "OperationNameToListPackageContents":
							OperationNameToListPackageContents = value;
							break;
						case "OperationNameToProcessReceived":
							OperationNameToProcessReceived = value;
							break;
						case "OperationNameToUpdateThisSideCategories":
							OperationNameToUpdateThisSideCategories = value;
							break;
						case "ProcessIDToListPackageContents":
							ProcessIDToListPackageContents = value;
							break;
						case "ProcessIDToProcessReceived":
							ProcessIDToProcessReceived = value;
							break;
						case "ProcessIDToUpdateThisSideCategories":
							ProcessIDToUpdateThisSideCategories = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string OutputInformationID { get; set; }
			private string _unmodified_OutputInformationID;
			[DataMember]
			public string Description { get; set; }
			private string _unmodified_Description;
			[DataMember]
			public string DeviceID { get; set; }
			private string _unmodified_DeviceID;
			[DataMember]
			public bool IsActiveParty { get; set; }
			private bool _unmodified_IsActiveParty;
			[DataMember]
			public string OtherSideConnectionID { get; set; }
			private string _unmodified_OtherSideConnectionID;
			[DataMember]
			public List< Category > ThisSideCategories = new List< Category >();
			[DataMember]
			public List< Category > OtherSideCategories = new List< Category >();
			[DataMember]
			public List< CategoryLink > CategoryLinks = new List< CategoryLink >();
			[DataMember]
			public List< TransferPackage > IncomingPackages = new List< TransferPackage >();
			[DataMember]
			public List< TransferPackage > OutgoingPackages = new List< TransferPackage >();
			[DataMember]
			public string OperationNameToListPackageContents { get; set; }
			private string _unmodified_OperationNameToListPackageContents;
			[DataMember]
			public string OperationNameToProcessReceived { get; set; }
			private string _unmodified_OperationNameToProcessReceived;
			[DataMember]
			public string OperationNameToUpdateThisSideCategories { get; set; }
			private string _unmodified_OperationNameToUpdateThisSideCategories;
			[DataMember]
			public string ProcessIDToListPackageContents { get; set; }
			private string _unmodified_ProcessIDToListPackageContents;
			[DataMember]
			public string ProcessIDToProcessReceived { get; set; }
			private string _unmodified_ProcessIDToProcessReceived;
			[DataMember]
			public string ProcessIDToUpdateThisSideCategories { get; set; }
			private string _unmodified_ProcessIDToUpdateThisSideCategories;
			
			}
			[DataContract]
			[Serializable]
			public partial class TransferPackage : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public TransferPackage()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "TransferPackage";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.Interface/TransferPackage/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(TransferPackage), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.Interface", "TransferPackage", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static TransferPackage RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTransferPackage(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: TransferPackage");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(TransferPackage), null, owner);
					if(master == null && initiateIfMissing)
					{
						StorageSupport.StoreInformation(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}


				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return iObject.RetrieveMaster(initiateIfMissing, out initiated);
				}


                public static TransferPackage RetrieveTransferPackage(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (TransferPackage) StorageSupport.RetrieveInformation(relativeLocation, typeof(TransferPackage), null, owner);
                    return result;
                }

				public static TransferPackage RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = TransferPackage.RetrieveTransferPackage("Content/TheBall.Interface/TransferPackage/" + contentName, containerOwner);
					var result = TransferPackage.RetrieveTransferPackage("TheBall.Interface/TransferPackage/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.Interface/TransferPackage/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.Interface/TransferPackage/" + contentName);
                }

				partial void DoInitializeDefaultSubscribers(IContainerOwner owner);

			    public void InitializeDefaultSubscribers(IContainerOwner owner)
			    {
					DoInitializeDefaultSubscribers(owner);
			    }

				partial void DoPostStoringExecute(IContainerOwner owner);

				public void PostStoringExecute(IContainerOwner owner)
				{
					DoPostStoringExecute(owner);
				}

				partial void DoPostDeleteExecute(IContainerOwner owner);

				public void PostDeleteExecute(IContainerOwner owner)
				{
					DoPostDeleteExecute(owner);
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				void IInformationObject.SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TransferPackage));
					using (var output = new StringWriter())
					{
						using (var writer = new XmlTextWriter(output))
						{
                            if(noFormatting == false)
						        writer.Formatting = Formatting.Indented;
							serializer.WriteObject(writer, this);
						}
						return output.GetStringBuilder().ToString();
					}
				}

				public static TransferPackage DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TransferPackage));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TransferPackage) serializer.ReadObject(xmlReader);
					}
            
				}

				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

                [DataMember]
                public Guid OwnerID { get; set; }

                [DataMember]
                public string RelativeLocation { get; set; }

                [DataMember]
                public string Name { get; set; }

                [DataMember]
                public string SemanticDomainName { get; set; }

				[DataMember]
				public string MasterETag { get; set; }

				[DataMember]
				public string GeneratedByProcessID { get; set; }

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.Interface", "TransferPackage", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.Interface", "TransferPackage", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref TransferPackage customDemoObject);




				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					// Remove exception if basic functionality starts to have issues
					//throw new NotImplementedException("Item level collections do not support object tree operations right now");
					if(filterOnFalse(this))
						result.Add(this);
				}

				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Object tree support not implemented for item level collection objects");


				}

			
                void IInformationObject.SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}
	

				bool IInformationObject.IsInstanceTreeModified {
					get { 
						// Remove exception if some basic functionality is broken due to it
						throw new NotImplementedException("Collection items do not support instance tree queries as of now");
					}
				}
				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					// Remove exception if some basic functionality is broken due to it
					//throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "ConnectionID":
							ConnectionID = value;
							break;
						case "PackageDirection":
							PackageDirection = value;
							break;
						case "PackageType":
							PackageType = value;
							break;
						case "IsProcessed":
							IsProcessed = bool.Parse(value);
							break;
						case "PackageContentBlobs":
							throw new NotImplementedException("Parsing collection types is not implemented for item collections");
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string ConnectionID { get; set; }
			private string _unmodified_ConnectionID;
			[DataMember]
			public string PackageDirection { get; set; }
			private string _unmodified_PackageDirection;
			[DataMember]
			public string PackageType { get; set; }
			private string _unmodified_PackageType;
			[DataMember]
			public bool IsProcessed { get; set; }
			private bool _unmodified_IsProcessed;
			[DataMember]
			public List< string > PackageContentBlobs = new List< string >();
			
			}
			[DataContract]
			[Serializable]
			public partial class CategoryLink : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public CategoryLink()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "CategoryLink";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.Interface/CategoryLink/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(CategoryLink), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.Interface", "CategoryLink", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static CategoryLink RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveCategoryLink(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: CategoryLink");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(CategoryLink), null, owner);
					if(master == null && initiateIfMissing)
					{
						StorageSupport.StoreInformation(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}


				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return iObject.RetrieveMaster(initiateIfMissing, out initiated);
				}


                public static CategoryLink RetrieveCategoryLink(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (CategoryLink) StorageSupport.RetrieveInformation(relativeLocation, typeof(CategoryLink), null, owner);
                    return result;
                }

				public static CategoryLink RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = CategoryLink.RetrieveCategoryLink("Content/TheBall.Interface/CategoryLink/" + contentName, containerOwner);
					var result = CategoryLink.RetrieveCategoryLink("TheBall.Interface/CategoryLink/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.Interface/CategoryLink/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.Interface/CategoryLink/" + contentName);
                }

				partial void DoInitializeDefaultSubscribers(IContainerOwner owner);

			    public void InitializeDefaultSubscribers(IContainerOwner owner)
			    {
					DoInitializeDefaultSubscribers(owner);
			    }

				partial void DoPostStoringExecute(IContainerOwner owner);

				public void PostStoringExecute(IContainerOwner owner)
				{
					DoPostStoringExecute(owner);
				}

				partial void DoPostDeleteExecute(IContainerOwner owner);

				public void PostDeleteExecute(IContainerOwner owner)
				{
					DoPostDeleteExecute(owner);
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        dynamic dyn = targetObject;
                        dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((CategoryLink) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
				}

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CategoryLink));
					using (var output = new StringWriter())
					{
						using (var writer = new XmlTextWriter(output))
						{
                            if(noFormatting == false)
						        writer.Formatting = Formatting.Indented;
							serializer.WriteObject(writer, this);
						}
						return output.GetStringBuilder().ToString();
					}
				}

				public static CategoryLink DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CategoryLink));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (CategoryLink) serializer.ReadObject(xmlReader);
					}
            
				}

				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

                [DataMember]
                public Guid OwnerID { get; set; }

                [DataMember]
                public string RelativeLocation { get; set; }

                [DataMember]
                public string Name { get; set; }

                [DataMember]
                public string SemanticDomainName { get; set; }

				[DataMember]
				public string MasterETag { get; set; }

				[DataMember]
				public string GeneratedByProcessID { get; set; }

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.Interface", "CategoryLink", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.Interface", "CategoryLink", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref CategoryLink customDemoObject);



				public static CategoryLink CreateDefault()
				{
					var result = new CategoryLink();
					return result;
				}
				/*
				public static CategoryLink CreateDemoDefault()
				{
					CategoryLink customDemo = null;
					CategoryLink.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new CategoryLink();
					result.SourceCategoryID = @"CategoryLink.SourceCategoryID";

					result.TargetCategoryID = @"CategoryLink.TargetCategoryID";

					result.LinkingType = @"CategoryLink.LinkingType";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public void SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(SourceCategoryID != _unmodified_SourceCategoryID)
							return true;
						if(TargetCategoryID != _unmodified_TargetCategoryID)
							return true;
						if(LinkingType != _unmodified_LinkingType)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(CategoryLink sourceObject)
				{
					SourceCategoryID = sourceObject.SourceCategoryID;
					TargetCategoryID = sourceObject.TargetCategoryID;
					LinkingType = sourceObject.LinkingType;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_SourceCategoryID = SourceCategoryID;
					_unmodified_TargetCategoryID = TargetCategoryID;
					_unmodified_LinkingType = LinkingType;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "SourceCategoryID":
							SourceCategoryID = value;
							break;
						case "TargetCategoryID":
							TargetCategoryID = value;
							break;
						case "LinkingType":
							LinkingType = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string SourceCategoryID { get; set; }
			private string _unmodified_SourceCategoryID;
			[DataMember]
			public string TargetCategoryID { get; set; }
			private string _unmodified_TargetCategoryID;
			[DataMember]
			public string LinkingType { get; set; }
			private string _unmodified_LinkingType;
			
			}
			[DataContract]
			[Serializable]
			public partial class Category : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public Category()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "Category";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.Interface/Category/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(Category), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.Interface", "Category", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Category RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveCategory(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: Category");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(Category), null, owner);
					if(master == null && initiateIfMissing)
					{
						StorageSupport.StoreInformation(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}


				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return iObject.RetrieveMaster(initiateIfMissing, out initiated);
				}


                public static Category RetrieveCategory(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Category) StorageSupport.RetrieveInformation(relativeLocation, typeof(Category), null, owner);
                    return result;
                }

				public static Category RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = Category.RetrieveCategory("Content/TheBall.Interface/Category/" + contentName, containerOwner);
					var result = Category.RetrieveCategory("TheBall.Interface/Category/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.Interface/Category/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.Interface/Category/" + contentName);
                }

				partial void DoInitializeDefaultSubscribers(IContainerOwner owner);

			    public void InitializeDefaultSubscribers(IContainerOwner owner)
			    {
					DoInitializeDefaultSubscribers(owner);
			    }

				partial void DoPostStoringExecute(IContainerOwner owner);

				public void PostStoringExecute(IContainerOwner owner)
				{
					DoPostStoringExecute(owner);
				}

				partial void DoPostDeleteExecute(IContainerOwner owner);

				public void PostDeleteExecute(IContainerOwner owner)
				{
					DoPostDeleteExecute(owner);
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        dynamic dyn = targetObject;
                        dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((Category) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
				}

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Category));
					using (var output = new StringWriter())
					{
						using (var writer = new XmlTextWriter(output))
						{
                            if(noFormatting == false)
						        writer.Formatting = Formatting.Indented;
							serializer.WriteObject(writer, this);
						}
						return output.GetStringBuilder().ToString();
					}
				}

				public static Category DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Category));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Category) serializer.ReadObject(xmlReader);
					}
            
				}

				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

                [DataMember]
                public Guid OwnerID { get; set; }

                [DataMember]
                public string RelativeLocation { get; set; }

                [DataMember]
                public string Name { get; set; }

                [DataMember]
                public string SemanticDomainName { get; set; }

				[DataMember]
				public string MasterETag { get; set; }

				[DataMember]
				public string GeneratedByProcessID { get; set; }

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.Interface", "Category", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.Interface", "Category", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref Category customDemoObject);



				public static Category CreateDefault()
				{
					var result = new Category();
					return result;
				}
				/*
				public static Category CreateDemoDefault()
				{
					Category customDemo = null;
					Category.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new Category();
					result.NativeCategoryID = @"Category.NativeCategoryID";

					result.NativeCategoryDomainName = @"Category.NativeCategoryDomainName";

					result.NativeCategoryObjectName = @"Category.NativeCategoryObjectName";

					result.NativeCategoryTitle = @"Category.NativeCategoryTitle";

					result.IdentifyingCategoryName = @"Category.IdentifyingCategoryName";

					result.ParentCategoryID = @"Category.ParentCategoryID";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public void SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(NativeCategoryID != _unmodified_NativeCategoryID)
							return true;
						if(NativeCategoryDomainName != _unmodified_NativeCategoryDomainName)
							return true;
						if(NativeCategoryObjectName != _unmodified_NativeCategoryObjectName)
							return true;
						if(NativeCategoryTitle != _unmodified_NativeCategoryTitle)
							return true;
						if(IdentifyingCategoryName != _unmodified_IdentifyingCategoryName)
							return true;
						if(ParentCategoryID != _unmodified_ParentCategoryID)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(Category sourceObject)
				{
					NativeCategoryID = sourceObject.NativeCategoryID;
					NativeCategoryDomainName = sourceObject.NativeCategoryDomainName;
					NativeCategoryObjectName = sourceObject.NativeCategoryObjectName;
					NativeCategoryTitle = sourceObject.NativeCategoryTitle;
					IdentifyingCategoryName = sourceObject.IdentifyingCategoryName;
					ParentCategoryID = sourceObject.ParentCategoryID;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_NativeCategoryID = NativeCategoryID;
					_unmodified_NativeCategoryDomainName = NativeCategoryDomainName;
					_unmodified_NativeCategoryObjectName = NativeCategoryObjectName;
					_unmodified_NativeCategoryTitle = NativeCategoryTitle;
					_unmodified_IdentifyingCategoryName = IdentifyingCategoryName;
					_unmodified_ParentCategoryID = ParentCategoryID;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "NativeCategoryID":
							NativeCategoryID = value;
							break;
						case "NativeCategoryDomainName":
							NativeCategoryDomainName = value;
							break;
						case "NativeCategoryObjectName":
							NativeCategoryObjectName = value;
							break;
						case "NativeCategoryTitle":
							NativeCategoryTitle = value;
							break;
						case "IdentifyingCategoryName":
							IdentifyingCategoryName = value;
							break;
						case "ParentCategoryID":
							ParentCategoryID = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string NativeCategoryID { get; set; }
			private string _unmodified_NativeCategoryID;
			[DataMember]
			public string NativeCategoryDomainName { get; set; }
			private string _unmodified_NativeCategoryDomainName;
			[DataMember]
			public string NativeCategoryObjectName { get; set; }
			private string _unmodified_NativeCategoryObjectName;
			[DataMember]
			public string NativeCategoryTitle { get; set; }
			private string _unmodified_NativeCategoryTitle;
			[DataMember]
			public string IdentifyingCategoryName { get; set; }
			private string _unmodified_IdentifyingCategoryName;
			[DataMember]
			public string ParentCategoryID { get; set; }
			private string _unmodified_ParentCategoryID;
			
			}
			[DataContract]
			[Serializable]
			public partial class StatusSummary : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.JSON;
					}
				}

				public StatusSummary()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "StatusSummary";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.Interface/StatusSummary/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(StatusSummary), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.Interface", "StatusSummary", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static StatusSummary RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveStatusSummary(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: StatusSummary");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(StatusSummary), null, owner);
					if(master == null && initiateIfMissing)
					{
						StorageSupport.StoreInformation(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}


				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return iObject.RetrieveMaster(initiateIfMissing, out initiated);
				}


                public static StatusSummary RetrieveStatusSummary(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (StatusSummary) StorageSupport.RetrieveInformation(relativeLocation, typeof(StatusSummary), null, owner);
                    return result;
                }

				public static StatusSummary RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = StatusSummary.RetrieveStatusSummary("Content/TheBall.Interface/StatusSummary/" + contentName, containerOwner);
					var result = StatusSummary.RetrieveStatusSummary("TheBall.Interface/StatusSummary/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.Interface/StatusSummary/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.Interface/StatusSummary/" + contentName);
                }

				partial void DoInitializeDefaultSubscribers(IContainerOwner owner);

			    public void InitializeDefaultSubscribers(IContainerOwner owner)
			    {
					DoInitializeDefaultSubscribers(owner);
			    }

				partial void DoPostStoringExecute(IContainerOwner owner);

				public void PostStoringExecute(IContainerOwner owner)
				{
					DoPostStoringExecute(owner);
				}

				partial void DoPostDeleteExecute(IContainerOwner owner);

				public void PostDeleteExecute(IContainerOwner owner)
				{
					DoPostDeleteExecute(owner);
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				void IInformationObject.SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(StatusSummary));
					using (var output = new StringWriter())
					{
						using (var writer = new XmlTextWriter(output))
						{
                            if(noFormatting == false)
						        writer.Formatting = Formatting.Indented;
							serializer.WriteObject(writer, this);
						}
						return output.GetStringBuilder().ToString();
					}
				}

				public static StatusSummary DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(StatusSummary));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (StatusSummary) serializer.ReadObject(xmlReader);
					}
            
				}

				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

                [DataMember]
                public Guid OwnerID { get; set; }

                [DataMember]
                public string RelativeLocation { get; set; }

                [DataMember]
                public string Name { get; set; }

                [DataMember]
                public string SemanticDomainName { get; set; }

				[DataMember]
				public string MasterETag { get; set; }

				[DataMember]
				public string GeneratedByProcessID { get; set; }

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.Interface", "StatusSummary", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.Interface", "StatusSummary", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref StatusSummary customDemoObject);




				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					// Remove exception if basic functionality starts to have issues
					//throw new NotImplementedException("Item level collections do not support object tree operations right now");
					if(filterOnFalse(this))
						result.Add(this);
				}

				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Object tree support not implemented for item level collection objects");


				}

			
                void IInformationObject.SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}
	

				bool IInformationObject.IsInstanceTreeModified {
					get { 
						// Remove exception if some basic functionality is broken due to it
						throw new NotImplementedException("Collection items do not support instance tree queries as of now");
					}
				}
				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					// Remove exception if some basic functionality is broken due to it
					//throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "ChangeItemTrackingList":
							throw new NotImplementedException("Parsing collection types is not implemented for item collections");
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public List< OperationExecutionItem > PendingOperations = new List< OperationExecutionItem >();
			[DataMember]
			public List< OperationExecutionItem > ExecutingOperations = new List< OperationExecutionItem >();
			[DataMember]
			public List< OperationExecutionItem > RecentCompletedOperations = new List< OperationExecutionItem >();
			[DataMember]
			public List< string > ChangeItemTrackingList = new List< string >();
			
			}
			[DataContract]
			[Serializable]
			public partial class InformationChangeItem : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.Binary;
					}
				}

				public InformationChangeItem()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "InformationChangeItem";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.Interface/InformationChangeItem/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(InformationChangeItem), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.Interface", "InformationChangeItem", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static InformationChangeItem RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveInformationChangeItem(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InformationChangeItem");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(InformationChangeItem), null, owner);
					if(master == null && initiateIfMissing)
					{
						StorageSupport.StoreInformation(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}


				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return iObject.RetrieveMaster(initiateIfMissing, out initiated);
				}


                public static InformationChangeItem RetrieveInformationChangeItem(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (InformationChangeItem) StorageSupport.RetrieveInformation(relativeLocation, typeof(InformationChangeItem), null, owner);
                    return result;
                }

				public static InformationChangeItem RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = InformationChangeItem.RetrieveInformationChangeItem("Content/TheBall.Interface/InformationChangeItem/" + contentName, containerOwner);
					var result = InformationChangeItem.RetrieveInformationChangeItem("TheBall.Interface/InformationChangeItem/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.Interface/InformationChangeItem/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.Interface/InformationChangeItem/" + contentName);
                }

				partial void DoInitializeDefaultSubscribers(IContainerOwner owner);

			    public void InitializeDefaultSubscribers(IContainerOwner owner)
			    {
					DoInitializeDefaultSubscribers(owner);
			    }

				partial void DoPostStoringExecute(IContainerOwner owner);

				public void PostStoringExecute(IContainerOwner owner)
				{
					DoPostStoringExecute(owner);
				}

				partial void DoPostDeleteExecute(IContainerOwner owner);

				public void PostDeleteExecute(IContainerOwner owner)
				{
					DoPostDeleteExecute(owner);
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				void IInformationObject.SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InformationChangeItem));
					using (var output = new StringWriter())
					{
						using (var writer = new XmlTextWriter(output))
						{
                            if(noFormatting == false)
						        writer.Formatting = Formatting.Indented;
							serializer.WriteObject(writer, this);
						}
						return output.GetStringBuilder().ToString();
					}
				}

				public static InformationChangeItem DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InformationChangeItem));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InformationChangeItem) serializer.ReadObject(xmlReader);
					}
            
				}

				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

                [DataMember]
                public Guid OwnerID { get; set; }

                [DataMember]
                public string RelativeLocation { get; set; }

                [DataMember]
                public string Name { get; set; }

                [DataMember]
                public string SemanticDomainName { get; set; }

				[DataMember]
				public string MasterETag { get; set; }

				[DataMember]
				public string GeneratedByProcessID { get; set; }

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.Interface", "InformationChangeItem", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.Interface", "InformationChangeItem", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InformationChangeItem customDemoObject);




				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					// Remove exception if basic functionality starts to have issues
					//throw new NotImplementedException("Item level collections do not support object tree operations right now");
					if(filterOnFalse(this))
						result.Add(this);
				}

				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Object tree support not implemented for item level collection objects");


				}

			
                void IInformationObject.SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}
	

				bool IInformationObject.IsInstanceTreeModified {
					get { 
						// Remove exception if some basic functionality is broken due to it
						throw new NotImplementedException("Collection items do not support instance tree queries as of now");
					}
				}
				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					// Remove exception if some basic functionality is broken due to it
					//throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "StartTimeUTC":
							StartTimeUTC = DateTime.Parse(value);
							break;
						case "EndTimeUTC":
							EndTimeUTC = DateTime.Parse(value);
							break;
						case "ChangedObjectIDList":
							throw new NotImplementedException("Parsing collection types is not implemented for item collections");
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public DateTime StartTimeUTC { get; set; }
			private DateTime _unmodified_StartTimeUTC;
			[DataMember]
			public DateTime EndTimeUTC { get; set; }
			private DateTime _unmodified_EndTimeUTC;
			[DataMember]
			public List< string > ChangedObjectIDList = new List< string >();
			
			}
			[DataContract]
			[Serializable]
			public partial class OperationExecutionItem : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public OperationExecutionItem()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "OperationExecutionItem";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.Interface/OperationExecutionItem/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(OperationExecutionItem), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.Interface", "OperationExecutionItem", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static OperationExecutionItem RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveOperationExecutionItem(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: OperationExecutionItem");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(OperationExecutionItem), null, owner);
					if(master == null && initiateIfMissing)
					{
						StorageSupport.StoreInformation(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}


				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return iObject.RetrieveMaster(initiateIfMissing, out initiated);
				}


                public static OperationExecutionItem RetrieveOperationExecutionItem(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (OperationExecutionItem) StorageSupport.RetrieveInformation(relativeLocation, typeof(OperationExecutionItem), null, owner);
                    return result;
                }

				public static OperationExecutionItem RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = OperationExecutionItem.RetrieveOperationExecutionItem("Content/TheBall.Interface/OperationExecutionItem/" + contentName, containerOwner);
					var result = OperationExecutionItem.RetrieveOperationExecutionItem("TheBall.Interface/OperationExecutionItem/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.Interface/OperationExecutionItem/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.Interface/OperationExecutionItem/" + contentName);
                }

				partial void DoInitializeDefaultSubscribers(IContainerOwner owner);

			    public void InitializeDefaultSubscribers(IContainerOwner owner)
			    {
					DoInitializeDefaultSubscribers(owner);
			    }

				partial void DoPostStoringExecute(IContainerOwner owner);

				public void PostStoringExecute(IContainerOwner owner)
				{
					DoPostStoringExecute(owner);
				}

				partial void DoPostDeleteExecute(IContainerOwner owner);

				public void PostDeleteExecute(IContainerOwner owner)
				{
					DoPostDeleteExecute(owner);
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        dynamic dyn = targetObject;
                        dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((OperationExecutionItem) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
				}

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(OperationExecutionItem));
					using (var output = new StringWriter())
					{
						using (var writer = new XmlTextWriter(output))
						{
                            if(noFormatting == false)
						        writer.Formatting = Formatting.Indented;
							serializer.WriteObject(writer, this);
						}
						return output.GetStringBuilder().ToString();
					}
				}

				public static OperationExecutionItem DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(OperationExecutionItem));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (OperationExecutionItem) serializer.ReadObject(xmlReader);
					}
            
				}

				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

                [DataMember]
                public Guid OwnerID { get; set; }

                [DataMember]
                public string RelativeLocation { get; set; }

                [DataMember]
                public string Name { get; set; }

                [DataMember]
                public string SemanticDomainName { get; set; }

				[DataMember]
				public string MasterETag { get; set; }

				[DataMember]
				public string GeneratedByProcessID { get; set; }

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.Interface", "OperationExecutionItem", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.Interface", "OperationExecutionItem", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref OperationExecutionItem customDemoObject);



				public static OperationExecutionItem CreateDefault()
				{
					var result = new OperationExecutionItem();
					return result;
				}
				/*
				public static OperationExecutionItem CreateDemoDefault()
				{
					OperationExecutionItem customDemo = null;
					OperationExecutionItem.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new OperationExecutionItem();
					result.OperationName = @"OperationExecutionItem.OperationName";

					result.OperationDomain = @"OperationExecutionItem.OperationDomain";

					result.OperationID = @"OperationExecutionItem.OperationID";

					result.CallerProvidedInfo = @"OperationExecutionItem.CallerProvidedInfo";

					result.ExecutionStatus = @"OperationExecutionItem.ExecutionStatus";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public void SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(OperationName != _unmodified_OperationName)
							return true;
						if(OperationDomain != _unmodified_OperationDomain)
							return true;
						if(OperationID != _unmodified_OperationID)
							return true;
						if(CallerProvidedInfo != _unmodified_CallerProvidedInfo)
							return true;
						if(CreationTime != _unmodified_CreationTime)
							return true;
						if(ExecutionBeginTime != _unmodified_ExecutionBeginTime)
							return true;
						if(ExecutionCompletedTime != _unmodified_ExecutionCompletedTime)
							return true;
						if(ExecutionStatus != _unmodified_ExecutionStatus)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(OperationExecutionItem sourceObject)
				{
					OperationName = sourceObject.OperationName;
					OperationDomain = sourceObject.OperationDomain;
					OperationID = sourceObject.OperationID;
					CallerProvidedInfo = sourceObject.CallerProvidedInfo;
					CreationTime = sourceObject.CreationTime;
					ExecutionBeginTime = sourceObject.ExecutionBeginTime;
					ExecutionCompletedTime = sourceObject.ExecutionCompletedTime;
					ExecutionStatus = sourceObject.ExecutionStatus;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_OperationName = OperationName;
					_unmodified_OperationDomain = OperationDomain;
					_unmodified_OperationID = OperationID;
					_unmodified_CallerProvidedInfo = CallerProvidedInfo;
					_unmodified_CreationTime = CreationTime;
					_unmodified_ExecutionBeginTime = ExecutionBeginTime;
					_unmodified_ExecutionCompletedTime = ExecutionCompletedTime;
					_unmodified_ExecutionStatus = ExecutionStatus;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "OperationName":
							OperationName = value;
							break;
						case "OperationDomain":
							OperationDomain = value;
							break;
						case "OperationID":
							OperationID = value;
							break;
						case "CallerProvidedInfo":
							CallerProvidedInfo = value;
							break;
						case "CreationTime":
							CreationTime = DateTime.Parse(value);
							break;
						case "ExecutionBeginTime":
							ExecutionBeginTime = DateTime.Parse(value);
							break;
						case "ExecutionCompletedTime":
							ExecutionCompletedTime = DateTime.Parse(value);
							break;
						case "ExecutionStatus":
							ExecutionStatus = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string OperationName { get; set; }
			private string _unmodified_OperationName;
			[DataMember]
			public string OperationDomain { get; set; }
			private string _unmodified_OperationDomain;
			[DataMember]
			public string OperationID { get; set; }
			private string _unmodified_OperationID;
			[DataMember]
			public string CallerProvidedInfo { get; set; }
			private string _unmodified_CallerProvidedInfo;
			[DataMember]
			public DateTime CreationTime { get; set; }
			private DateTime _unmodified_CreationTime;
			[DataMember]
			public DateTime ExecutionBeginTime { get; set; }
			private DateTime _unmodified_ExecutionBeginTime;
			[DataMember]
			public DateTime ExecutionCompletedTime { get; set; }
			private DateTime _unmodified_ExecutionCompletedTime;
			[DataMember]
			public string ExecutionStatus { get; set; }
			private string _unmodified_ExecutionStatus;
			
			}
			[DataContract]
			[Serializable]
			public partial class GenericObjectCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public GenericObjectCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "GenericObjectCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.Interface/GenericObjectCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(GenericObjectCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.Interface", "GenericObjectCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static GenericObjectCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveGenericObjectCollection(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: GenericObjectCollection");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(GenericObjectCollection), null, owner);
					if(master == null && initiateIfMissing)
					{
						StorageSupport.StoreInformation(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}


				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return iObject.RetrieveMaster(initiateIfMissing, out initiated);
				}


                public static GenericObjectCollection RetrieveGenericObjectCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (GenericObjectCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(GenericObjectCollection), null, owner);
                    return result;
                }

				public static GenericObjectCollection RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = GenericObjectCollection.RetrieveGenericObjectCollection("Content/TheBall.Interface/GenericObjectCollection/" + contentName, containerOwner);
					var result = GenericObjectCollection.RetrieveGenericObjectCollection("TheBall.Interface/GenericObjectCollection/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.Interface/GenericObjectCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.Interface/GenericObjectCollection/" + contentName);
                }

				partial void DoInitializeDefaultSubscribers(IContainerOwner owner);

			    public void InitializeDefaultSubscribers(IContainerOwner owner)
			    {
					DoInitializeDefaultSubscribers(owner);
			    }

				partial void DoPostStoringExecute(IContainerOwner owner);

				public void PostStoringExecute(IContainerOwner owner)
				{
					DoPostStoringExecute(owner);
				}

				partial void DoPostDeleteExecute(IContainerOwner owner);

				public void PostDeleteExecute(IContainerOwner owner)
				{
					DoPostDeleteExecute(owner);
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        dynamic dyn = targetObject;
                        dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((GenericObjectCollection) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
				}

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GenericObjectCollection));
					using (var output = new StringWriter())
					{
						using (var writer = new XmlTextWriter(output))
						{
                            if(noFormatting == false)
						        writer.Formatting = Formatting.Indented;
							serializer.WriteObject(writer, this);
						}
						return output.GetStringBuilder().ToString();
					}
				}

				public static GenericObjectCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GenericObjectCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (GenericObjectCollection) serializer.ReadObject(xmlReader);
					}
            
				}

				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

                [DataMember]
                public Guid OwnerID { get; set; }

                [DataMember]
                public string RelativeLocation { get; set; }

                [DataMember]
                public string Name { get; set; }

                [DataMember]
                public string SemanticDomainName { get; set; }

				[DataMember]
				public string MasterETag { get; set; }

				[DataMember]
				public string GeneratedByProcessID { get; set; }

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.Interface", "GenericObjectCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.Interface", "GenericObjectCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref GenericObjectCollection customDemoObject);


				
				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.UpdateCollections(masterInstance);
					}
				}



				bool IInformationCollection.IsMasterCollection {
					get {
						return true;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					return GetMasterCollectionLocation(owner);
					
				}

				IInformationCollection IInformationCollection.GetMasterInstance()
				{
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					return GetMasterCollectionInstance(owner);
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = GenericCollectionableObject.GetRelativeLocationFromID("dummy");
					string nonOwnerDirectoryLocation = SubscribeSupport.GetParentDirectoryTarget(dummyItemLocation);
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public void RefreshContent()
				{
					// DirectoryToMaster
					string itemDirectory = GetItemDirectory();
					IInformationObject[] informationObjects = StorageSupport.RetrieveInformationObjects(itemDirectory,
																								 typeof(GenericCollectionableObject));
                    Array.ForEach(informationObjects, io => io.MasterETag = io.ETag);
					CollectionContent.Clear();
					CollectionContent.AddRange(informationObjects.Select(obj => (GenericCollectionableObject) obj));
            
				}

				public static GenericObjectCollection GetMasterCollectionInstance(IContainerOwner owner)
				{
					return GenericObjectCollection.RetrieveFromOwnerContent(owner, "MasterCollection");
				}

				public void SubscribeToContentSource()
				{
					// DirectoryToCollection
					string itemDirectory = GetItemDirectory();
					SubscribeSupport.AddSubscriptionToObject(itemDirectory, RelativeLocation,
															 SubscribeSupport.SubscribeType_DirectoryToCollection, null, typeof(GenericObjectCollection).FullName);
				}

				public static string GetMasterCollectionLocation(IContainerOwner owner)
				{
					return StorageSupport.GetOwnerContentLocation(owner, "TheBall.Interface/GenericObjectCollection/" + "MasterCollection");
				}



                public void SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }

				
		
				public static GenericObjectCollection CreateDefault()
				{
					var result = new GenericObjectCollection();
					return result;
				}

				/*
				public static GenericObjectCollection CreateDemoDefault()
				{
					GenericObjectCollection customDemo = null;
					GenericObjectCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new GenericObjectCollection();
					result.CollectionContent.Add(GenericCollectionableObject.CreateDemoDefault());
					//result.CollectionContent.Add(GenericCollectionableObject.CreateDemoDefault());
					//result.CollectionContent.Add(GenericCollectionableObject.CreateDemoDefault());
					return result;
				}
				*/

		
				[DataMember] public List<GenericCollectionableObject> CollectionContent = new List<GenericCollectionableObject>();
				private GenericCollectionableObject[] _unmodified_CollectionContent;

				[DataMember] public bool IsCollectionFiltered;
				private bool _unmodified_IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();
				private string[] _unmodified_OrderFilterIDList;

				public string SelectedIDCommaSeparated
				{
					get
					{
						string[] sourceArray;
						if (OrderFilterIDList != null)
							sourceArray = OrderFilterIDList.ToArray();
						else
							sourceArray = CollectionContent.Select(item => item.ID).ToArray();
						return String.Join(",", sourceArray);
					}
					set 
					{
						if (value == null)
							return;
						string[] valueArray = value.Split(',');
						OrderFilterIDList = new List<string>();
						OrderFilterIDList.AddRange(valueArray);
						OrderFilterIDList.RemoveAll(item => CollectionContent.Any(colItem => colItem.ID == item) == false);
					}
				}

				public GenericCollectionableObject[] GetIDSelectedArray()
				{
					if (IsCollectionFiltered == false || this.OrderFilterIDList == null)
						return CollectionContent.ToArray();
					return
						this.OrderFilterIDList.Select(id => CollectionContent.FirstOrDefault(item => item.ID == id)).Where(item => item != null).ToArray();
				}

				public void RefreshOrderAndFilterListFromContent()
                {
                    if (OrderFilterIDList == null)
                        return;
                    OrderFilterIDList.RemoveAll(item => CollectionContent.Any(colItem => colItem.ID == item) == false);
                }

				public void ParsePropertyValue(string propertyName, string propertyValue)
				{
					switch(propertyName)
					{
						case "SelectedIDCommaSeparated":
							SelectedIDCommaSeparated = propertyValue;
							break;
						case "IsCollectionFiltered":
							IsCollectionFiltered = bool.Parse(propertyValue);
							break;
						default:
							throw new NotSupportedException("No ParsePropertyValue supported for property: " + propertyName);
					}
				}


				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					for(int i = 0; i < CollectionContent.Count; i++) // >
					{
						if(CollectionContent[i].ID == replacingObject.ID)
							CollectionContent[i] = (GenericCollectionableObject )replacingObject;
						else { // Cannot have circular reference, so can be in else branch
							IInformationObject iObject = CollectionContent[i];
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}

				
				bool IInformationObject.IsInstanceTreeModified {
					get {
						bool collectionModified = CollectionContent.SequenceEqual(_unmodified_CollectionContent) == false;
						if(collectionModified)
							return true;
						//if((OrderFilterIDList == null && _unmodified_OrderFilterIDList != null) || _unmodified_OrderFilterIDList
						if(IsCollectionFiltered != _unmodified_IsCollectionFiltered)
							return true;
						// For non-master content
						foreach(IInformationObject item in CollectionContent)
						{
							bool itemTreeModified = item.IsInstanceTreeModified;
							if(itemTreeModified)
								return true;
						}
							
						return false;
					}
				}
				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CollectionContent = CollectionContent.ToArray();
					_unmodified_IsCollectionFiltered = IsCollectionFiltered;
					if(OrderFilterIDList == null)
						_unmodified_OrderFilterIDList = null;
					else
						_unmodified_OrderFilterIDList = OrderFilterIDList.ToArray();
					foreach(IInformationObject iObject in CollectionContent)
						iObject.SetInstanceTreeValuesAsUnmodified();
				}

				private void CopyContentFrom(GenericObjectCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
				private object FindFromObjectTree(string objectId)
				{
					foreach(var item in CollectionContent)
					{
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					foreach(IInformationObject iObject in CollectionContent)
						iObject.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
				}


				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						bool doAdd = true;
						if(filterOnFalse != null)
							doAdd = filterOnFalse(iObject);
						if(doAdd) {
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
				}


			
			}
			[DataContract]
			[Serializable]
			public partial class GenericCollectionableObject : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.JSON;
					}
				}

				public GenericCollectionableObject()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "GenericCollectionableObject";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.Interface/GenericCollectionableObject/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(GenericCollectionableObject), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.Interface", "GenericCollectionableObject", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static GenericCollectionableObject RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveGenericCollectionableObject(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: GenericCollectionableObject");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(GenericCollectionableObject), null, owner);
					if(master == null && initiateIfMissing)
					{
						StorageSupport.StoreInformation(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}


				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return iObject.RetrieveMaster(initiateIfMissing, out initiated);
				}


                public static GenericCollectionableObject RetrieveGenericCollectionableObject(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (GenericCollectionableObject) StorageSupport.RetrieveInformation(relativeLocation, typeof(GenericCollectionableObject), null, owner);
                    return result;
                }

				public static GenericCollectionableObject RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = GenericCollectionableObject.RetrieveGenericCollectionableObject("Content/TheBall.Interface/GenericCollectionableObject/" + contentName, containerOwner);
					var result = GenericCollectionableObject.RetrieveGenericCollectionableObject("TheBall.Interface/GenericCollectionableObject/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.Interface/GenericCollectionableObject/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.Interface/GenericCollectionableObject/" + contentName);
                }

				partial void DoInitializeDefaultSubscribers(IContainerOwner owner);

			    public void InitializeDefaultSubscribers(IContainerOwner owner)
			    {
					DoInitializeDefaultSubscribers(owner);
			    }

				partial void DoPostStoringExecute(IContainerOwner owner);

				public void PostStoringExecute(IContainerOwner owner)
				{
					DoPostStoringExecute(owner);
				}

				partial void DoPostDeleteExecute(IContainerOwner owner);

				public void PostDeleteExecute(IContainerOwner owner)
				{
					DoPostDeleteExecute(owner);
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        dynamic dyn = targetObject;
                        dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((GenericCollectionableObject) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
				}

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GenericCollectionableObject));
					using (var output = new StringWriter())
					{
						using (var writer = new XmlTextWriter(output))
						{
                            if(noFormatting == false)
						        writer.Formatting = Formatting.Indented;
							serializer.WriteObject(writer, this);
						}
						return output.GetStringBuilder().ToString();
					}
				}

				public static GenericCollectionableObject DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GenericCollectionableObject));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (GenericCollectionableObject) serializer.ReadObject(xmlReader);
					}
            
				}

				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

                [DataMember]
                public Guid OwnerID { get; set; }

                [DataMember]
                public string RelativeLocation { get; set; }

                [DataMember]
                public string Name { get; set; }

                [DataMember]
                public string SemanticDomainName { get; set; }

				[DataMember]
				public string MasterETag { get; set; }

				[DataMember]
				public string GeneratedByProcessID { get; set; }

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.Interface", "GenericCollectionableObject", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.Interface", "GenericCollectionableObject", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref GenericCollectionableObject customDemoObject);



				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(ValueObject != null) {
						((IInformationObject) ValueObject).UpdateCollections(masterInstance);
					}

				}

                public void SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					{ // Scoping block for variable name reusability
						IInformationObject item = ValueObject;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					{
						var item = ValueObject;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					{
						var item = (IInformationObject) ValueObject;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(ValueObject != _unmodified_ValueObject)
							return true;
						{
							IInformationObject item = (IInformationObject) ValueObject;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					if(ValueObject != null) {
						if(ValueObject.ID == replacingObject.ID)
							ValueObject = (GenericObject) replacingObject;
						else {
							IInformationObject iObject = ValueObject;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(GenericCollectionableObject sourceObject)
				{
					ValueObject = sourceObject.ValueObject;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
				
					_unmodified_ValueObject = ValueObject;
					if(ValueObject != null)
						((IInformationObject) ValueObject).SetInstanceTreeValuesAsUnmodified();

				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public GenericObject ValueObject { get; set; }
			private GenericObject _unmodified_ValueObject;
			
			}
			[DataContract]
			[Serializable]
			public partial class GenericObject : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.JSON;
					}
				}

				public GenericObject()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "GenericObject";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.Interface/GenericObject/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(GenericObject), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.Interface", "GenericObject", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static GenericObject RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveGenericObject(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: GenericObject");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(GenericObject), null, owner);
					if(master == null && initiateIfMissing)
					{
						StorageSupport.StoreInformation(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}


				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return iObject.RetrieveMaster(initiateIfMissing, out initiated);
				}


                public static GenericObject RetrieveGenericObject(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (GenericObject) StorageSupport.RetrieveInformation(relativeLocation, typeof(GenericObject), null, owner);
                    return result;
                }

				public static GenericObject RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = GenericObject.RetrieveGenericObject("Content/TheBall.Interface/GenericObject/" + contentName, containerOwner);
					var result = GenericObject.RetrieveGenericObject("TheBall.Interface/GenericObject/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.Interface/GenericObject/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.Interface/GenericObject/" + contentName);
                }

				partial void DoInitializeDefaultSubscribers(IContainerOwner owner);

			    public void InitializeDefaultSubscribers(IContainerOwner owner)
			    {
					DoInitializeDefaultSubscribers(owner);
			    }

				partial void DoPostStoringExecute(IContainerOwner owner);

				public void PostStoringExecute(IContainerOwner owner)
				{
					DoPostStoringExecute(owner);
				}

				partial void DoPostDeleteExecute(IContainerOwner owner);

				public void PostDeleteExecute(IContainerOwner owner)
				{
					DoPostDeleteExecute(owner);
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				void IInformationObject.SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GenericObject));
					using (var output = new StringWriter())
					{
						using (var writer = new XmlTextWriter(output))
						{
                            if(noFormatting == false)
						        writer.Formatting = Formatting.Indented;
							serializer.WriteObject(writer, this);
						}
						return output.GetStringBuilder().ToString();
					}
				}

				public static GenericObject DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GenericObject));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (GenericObject) serializer.ReadObject(xmlReader);
					}
            
				}

				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

                [DataMember]
                public Guid OwnerID { get; set; }

                [DataMember]
                public string RelativeLocation { get; set; }

                [DataMember]
                public string Name { get; set; }

                [DataMember]
                public string SemanticDomainName { get; set; }

				[DataMember]
				public string MasterETag { get; set; }

				[DataMember]
				public string GeneratedByProcessID { get; set; }

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.Interface", "GenericObject", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.Interface", "GenericObject", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref GenericObject customDemoObject);




				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					// Remove exception if basic functionality starts to have issues
					//throw new NotImplementedException("Item level collections do not support object tree operations right now");
					if(filterOnFalse(this))
						result.Add(this);
				}

				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Object tree support not implemented for item level collection objects");


				}

			
                void IInformationObject.SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}
	

				bool IInformationObject.IsInstanceTreeModified {
					get { 
						// Remove exception if some basic functionality is broken due to it
						throw new NotImplementedException("Collection items do not support instance tree queries as of now");
					}
				}
				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					// Remove exception if some basic functionality is broken due to it
					//throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "IncludeInCollection":
							IncludeInCollection = bool.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public List< GenericValue > Values = new List< GenericValue >();
			[DataMember]
			public bool IncludeInCollection { get; set; }
			private bool _unmodified_IncludeInCollection;
			
			}
			[DataContract]
			[Serializable]
			public partial class GenericValue : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.JSON;
					}
				}

				public GenericValue()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "GenericValue";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.Interface/GenericValue/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(GenericValue), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.Interface", "GenericValue", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static GenericValue RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveGenericValue(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: GenericValue");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(GenericValue), null, owner);
					if(master == null && initiateIfMissing)
					{
						StorageSupport.StoreInformation(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}


				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return iObject.RetrieveMaster(initiateIfMissing, out initiated);
				}


                public static GenericValue RetrieveGenericValue(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (GenericValue) StorageSupport.RetrieveInformation(relativeLocation, typeof(GenericValue), null, owner);
                    return result;
                }

				public static GenericValue RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = GenericValue.RetrieveGenericValue("Content/TheBall.Interface/GenericValue/" + contentName, containerOwner);
					var result = GenericValue.RetrieveGenericValue("TheBall.Interface/GenericValue/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.Interface/GenericValue/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.Interface/GenericValue/" + contentName);
                }

				partial void DoInitializeDefaultSubscribers(IContainerOwner owner);

			    public void InitializeDefaultSubscribers(IContainerOwner owner)
			    {
					DoInitializeDefaultSubscribers(owner);
			    }

				partial void DoPostStoringExecute(IContainerOwner owner);

				public void PostStoringExecute(IContainerOwner owner)
				{
					DoPostStoringExecute(owner);
				}

				partial void DoPostDeleteExecute(IContainerOwner owner);

				public void PostDeleteExecute(IContainerOwner owner)
				{
					DoPostDeleteExecute(owner);
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				void IInformationObject.SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GenericValue));
					using (var output = new StringWriter())
					{
						using (var writer = new XmlTextWriter(output))
						{
                            if(noFormatting == false)
						        writer.Formatting = Formatting.Indented;
							serializer.WriteObject(writer, this);
						}
						return output.GetStringBuilder().ToString();
					}
				}

				public static GenericValue DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GenericValue));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (GenericValue) serializer.ReadObject(xmlReader);
					}
            
				}

				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

                [DataMember]
                public Guid OwnerID { get; set; }

                [DataMember]
                public string RelativeLocation { get; set; }

                [DataMember]
                public string Name { get; set; }

                [DataMember]
                public string SemanticDomainName { get; set; }

				[DataMember]
				public string MasterETag { get; set; }

				[DataMember]
				public string GeneratedByProcessID { get; set; }

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.Interface", "GenericValue", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.Interface", "GenericValue", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref GenericValue customDemoObject);




				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					// Remove exception if basic functionality starts to have issues
					//throw new NotImplementedException("Item level collections do not support object tree operations right now");
					if(filterOnFalse(this))
						result.Add(this);
				}

				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Object tree support not implemented for item level collection objects");


				}

			
                void IInformationObject.SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}
	

				bool IInformationObject.IsInstanceTreeModified {
					get { 
						// Remove exception if some basic functionality is broken due to it
						throw new NotImplementedException("Collection items do not support instance tree queries as of now");
					}
				}
				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					// Remove exception if some basic functionality is broken due to it
					//throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "ValueName":
							ValueName = value;
							break;
						case "String":
							String = value;
							break;
						case "StringArray":
							throw new NotImplementedException("Parsing collection types is not implemented for item collections");
							break;
						case "Number":
							Number = double.Parse(value);
							break;
						case "NumberArray":
							throw new NotImplementedException("Parsing collection types is not implemented for item collections");
							break;
						case "Boolean":
							Boolean = bool.Parse(value);
							break;
						case "BooleanArray":
							throw new NotImplementedException("Parsing collection types is not implemented for item collections");
							break;
						case "DateTime":
							DateTime = DateTime.Parse(value);
							break;
						case "DateTimeArray":
							throw new NotImplementedException("Parsing collection types is not implemented for item collections");
							break;
						case "IndexingInfo":
							IndexingInfo = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string ValueName { get; set; }
			private string _unmodified_ValueName;
			[DataMember]
			public string String { get; set; }
			private string _unmodified_String;
			[DataMember]
			public List< string > StringArray = new List< string >();
			[DataMember]
			public double Number { get; set; }
			private double _unmodified_Number;
			[DataMember]
			public List< double > NumberArray = new List< double >();
			[DataMember]
			public bool Boolean { get; set; }
			private bool _unmodified_Boolean;
			[DataMember]
			public List< bool > BooleanArray = new List< bool >();
			[DataMember]
			public DateTime DateTime { get; set; }
			private DateTime _unmodified_DateTime;
			[DataMember]
			public List< DateTime > DateTimeArray = new List< DateTime >();
			[DataMember]
			public GenericObject Object { get; set; }
			private GenericObject _unmodified_Object;
			[DataMember]
			public List< GenericObject > ObjectArray = new List< GenericObject >();
			[DataMember]
			public string IndexingInfo { get; set; }
			private string _unmodified_IndexingInfo;
			
			}
 } 