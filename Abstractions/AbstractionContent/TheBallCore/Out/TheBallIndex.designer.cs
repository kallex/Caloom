 


using DOM=TheBall.Index;

namespace TheBall.CORE {
	public static partial class OwnerInitializer
	{
		private static void DOMAININIT_TheBall_Index(IContainerOwner owner)
		{
			DOM.DomainInformationSupport.EnsureMasterCollections(owner);
			DOM.DomainInformationSupport.RefreshMasterCollections(owner);
		}
	}
}


namespace TheBall.Index { 
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
			public class UserQuery
			{
				[DataMember]
				public string QueryString { get; set; }
				[DataMember]
				public string DefaultFieldName { get; set; }
			}

			[DataContract]
			public class QueryToken
			{
				[DataMember]
				public string QueryRequestObjectDomainName { get; set; }
				[DataMember]
				public string QueryRequestObjectName { get; set; }
				[DataMember]
				public string QueryRequestObjectID { get; set; }
			}

		public static class DomainInformationSupport
		{
            public static void EnsureMasterCollections(IContainerOwner owner)
            {
            }

            public static void RefreshMasterCollections(IContainerOwner owner)
            {
            }
		}
			[DataContract]
			[Serializable]
			public partial class IndexingRequest : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public IndexingRequest()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.Index";
				    this.Name = "IndexingRequest";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.Index/IndexingRequest/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(IndexingRequest), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.Index", "IndexingRequest", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IndexingRequest RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveIndexingRequest(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: IndexingRequest");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(IndexingRequest), null, owner);
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


                public static IndexingRequest RetrieveIndexingRequest(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (IndexingRequest) StorageSupport.RetrieveInformation(relativeLocation, typeof(IndexingRequest), null, owner);
                    return result;
                }

				public static IndexingRequest RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = IndexingRequest.RetrieveIndexingRequest("Content/TheBall.Index/IndexingRequest/" + contentName, containerOwner);
					var result = IndexingRequest.RetrieveIndexingRequest("TheBall.Index/IndexingRequest/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.Index/IndexingRequest/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.Index/IndexingRequest/" + contentName);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(IndexingRequest));
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

				public static IndexingRequest DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(IndexingRequest));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (IndexingRequest) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.Index", "IndexingRequest", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.Index", "IndexingRequest", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref IndexingRequest customDemoObject);




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
						case "IndexName":
							IndexName = value;
							break;
						case "ObjectLocations":
							throw new NotImplementedException("Parsing collection types is not implemented for item collections");
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string IndexName { get; set; }
			private string _unmodified_IndexName;
			[DataMember]
			public List< string > ObjectLocations = new List< string >();
			
			}
			[DataContract]
			[Serializable]
			public partial class QueryRequest : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public QueryRequest()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.Index";
				    this.Name = "QueryRequest";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.Index/QueryRequest/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(QueryRequest), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.Index", "QueryRequest", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static QueryRequest RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveQueryRequest(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: QueryRequest");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(QueryRequest), null, owner);
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


                public static QueryRequest RetrieveQueryRequest(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (QueryRequest) StorageSupport.RetrieveInformation(relativeLocation, typeof(QueryRequest), null, owner);
                    return result;
                }

				public static QueryRequest RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = QueryRequest.RetrieveQueryRequest("Content/TheBall.Index/QueryRequest/" + contentName, containerOwner);
					var result = QueryRequest.RetrieveQueryRequest("TheBall.Index/QueryRequest/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.Index/QueryRequest/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.Index/QueryRequest/" + contentName);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(QueryRequest));
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

				public static QueryRequest DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(QueryRequest));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (QueryRequest) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.Index", "QueryRequest", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.Index", "QueryRequest", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref QueryRequest customDemoObject);




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
						case "QueryString":
							QueryString = value;
							break;
						case "DefaultFieldName":
							DefaultFieldName = value;
							break;
						case "IndexName":
							IndexName = value;
							break;
						case "IsQueryCompleted":
							IsQueryCompleted = bool.Parse(value);
							break;
						case "LastRequestTime":
							LastRequestTime = DateTime.Parse(value);
							break;
						case "LastCompletionTime":
							LastCompletionTime = DateTime.Parse(value);
							break;
						case "LastCompletionDurationMs":
							LastCompletionDurationMs = long.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string QueryString { get; set; }
			private string _unmodified_QueryString;
			[DataMember]
			public string DefaultFieldName { get; set; }
			private string _unmodified_DefaultFieldName;
			[DataMember]
			public string IndexName { get; set; }
			private string _unmodified_IndexName;
			[DataMember]
			public bool IsQueryCompleted { get; set; }
			private bool _unmodified_IsQueryCompleted;
			[DataMember]
			public DateTime LastRequestTime { get; set; }
			private DateTime _unmodified_LastRequestTime;
			[DataMember]
			public DateTime LastCompletionTime { get; set; }
			private DateTime _unmodified_LastCompletionTime;
			[DataMember]
			public long LastCompletionDurationMs { get; set; }
			private long _unmodified_LastCompletionDurationMs;
			[DataMember]
			public List< QueryResultItem > QueryResultObjects = new List< QueryResultItem >();
			
			}
			[DataContract]
			[Serializable]
			public partial class QueryResultItem : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public QueryResultItem()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.Index";
				    this.Name = "QueryResultItem";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.Index/QueryResultItem/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(QueryResultItem), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.Index", "QueryResultItem", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static QueryResultItem RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveQueryResultItem(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: QueryResultItem");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(QueryResultItem), null, owner);
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


                public static QueryResultItem RetrieveQueryResultItem(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (QueryResultItem) StorageSupport.RetrieveInformation(relativeLocation, typeof(QueryResultItem), null, owner);
                    return result;
                }

				public static QueryResultItem RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = QueryResultItem.RetrieveQueryResultItem("Content/TheBall.Index/QueryResultItem/" + contentName, containerOwner);
					var result = QueryResultItem.RetrieveQueryResultItem("TheBall.Index/QueryResultItem/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.Index/QueryResultItem/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.Index/QueryResultItem/" + contentName);
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
					CopyContentFrom((QueryResultItem) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(QueryResultItem));
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

				public static QueryResultItem DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(QueryResultItem));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (QueryResultItem) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.Index", "QueryResultItem", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.Index", "QueryResultItem", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref QueryResultItem customDemoObject);



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

						if(ObjectDomainName != _unmodified_ObjectDomainName)
							return true;
						if(ObjectName != _unmodified_ObjectName)
							return true;
						if(ObjectID != _unmodified_ObjectID)
							return true;
						if(Rank != _unmodified_Rank)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(QueryResultItem sourceObject)
				{
					ObjectDomainName = sourceObject.ObjectDomainName;
					ObjectName = sourceObject.ObjectName;
					ObjectID = sourceObject.ObjectID;
					Rank = sourceObject.Rank;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_ObjectDomainName = ObjectDomainName;
					_unmodified_ObjectName = ObjectName;
					_unmodified_ObjectID = ObjectID;
					_unmodified_Rank = Rank;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "ObjectDomainName":
							ObjectDomainName = value;
							break;
						case "ObjectName":
							ObjectName = value;
							break;
						case "ObjectID":
							ObjectID = value;
							break;
						case "Rank":
							Rank = double.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string ObjectDomainName { get; set; }
			private string _unmodified_ObjectDomainName;
			[DataMember]
			public string ObjectName { get; set; }
			private string _unmodified_ObjectName;
			[DataMember]
			public string ObjectID { get; set; }
			private string _unmodified_ObjectID;
			[DataMember]
			public double Rank { get; set; }
			private double _unmodified_Rank;
			
			}
 } 