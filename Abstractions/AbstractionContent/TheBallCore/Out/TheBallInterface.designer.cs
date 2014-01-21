 


using DOM=TheBall.Infrastructure;

namespace TheBall.CORE {
	public static partial class OwnerInitializer
	{
		private static void DOMAININIT_TheBall_Infrastructure(IContainerOwner owner)
		{
			DOM.DomainInformationSupport.EnsureMasterCollections(owner);
			DOM.DomainInformationSupport.RefreshMasterCollections(owner);
		}
	}
}


namespace TheBall.Infrastructure { 
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
				    this.SemanticDomainName = "TheBall.Infrastructure";
				    this.Name = "StatusSummary";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.Infrastructure/StatusSummary/";
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
                    return Path.Combine("TheBall.Infrastructure", "StatusSummary", id).Replace("\\", "/");
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
					// var result = StatusSummary.RetrieveStatusSummary("Content/TheBall.Infrastructure/StatusSummary/" + contentName, containerOwner);
					var result = StatusSummary.RetrieveStatusSummary("TheBall.Infrastructure/StatusSummary/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.Infrastructure/StatusSummary/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.Infrastructure/StatusSummary/" + contentName);
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.Infrastructure", "StatusSummary", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.Infrastructure", "StatusSummary", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref StatusSummary customDemoObject);




				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					// Remove exception if basic functionality starts to have issues
					throw new NotImplementedException("Item level collections do not support object tree operations right now");
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
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
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
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public List< OperationExecutionItem > PendingOperations = new List< OperationExecutionItem >();
			
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
				    this.SemanticDomainName = "TheBall.Infrastructure";
				    this.Name = "OperationExecutionItem";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.Infrastructure/OperationExecutionItem/";
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
                    return Path.Combine("TheBall.Infrastructure", "OperationExecutionItem", id).Replace("\\", "/");
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
					// var result = OperationExecutionItem.RetrieveOperationExecutionItem("Content/TheBall.Infrastructure/OperationExecutionItem/" + contentName, containerOwner);
					var result = OperationExecutionItem.RetrieveOperationExecutionItem("TheBall.Infrastructure/OperationExecutionItem/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.Infrastructure/OperationExecutionItem/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.Infrastructure/OperationExecutionItem/" + contentName);
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.Infrastructure", "OperationExecutionItem", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.Infrastructure", "OperationExecutionItem", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref OperationExecutionItem customDemoObject);



				public static OperationExecutionItem CreateDefault()
				{
					var result = new OperationExecutionItem();
					return result;
				}

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
 } 