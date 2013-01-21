 

namespace TheBall.CORE { 
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


/*
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
        string ETag { get; set;  }
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
	*/

		public static class DomainInformationSupport
		{
            public static void EnsureMasterCollections(this IContainerOwner owner)
            {
                {
                    var masterCollection = InvoiceCollection.GetMasterCollectionInstance(owner);
                    if(masterCollection == null)
                    {
                        masterCollection = InvoiceCollection.CreateDefault();
                        masterCollection.RelativeLocation =
                            InvoiceCollection.GetMasterCollectionLocation(owner);
                        StorageSupport.StoreInformation(masterCollection, owner);
                    }
					IInformationCollection collection = masterCollection;
					collection.SubscribeToContentSource();
                }
            }

            public static void RefreshMasterCollections(this IContainerOwner owner)
            {
                {
                    IInformationCollection masterCollection = InvoiceCollection.GetMasterCollectionInstance(owner);
                    if (masterCollection == null)
                        throw new InvalidDataException("Master collection InvoiceCollection missing for owner");
                    masterCollection.RefreshContent();
                    StorageSupport.StoreInformation((IInformationObject) masterCollection, owner);
                }
            }
		}
			[DataContract]
			public partial class InvoiceFiscalExportSummary : IInformationObject 
			{
				public InvoiceFiscalExportSummary()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceFiscalExportSummary";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceFiscalExportSummary/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(InvoiceFiscalExportSummary), null, owner);
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.CORE", "InvoiceFiscalExportSummary", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static InvoiceFiscalExportSummary RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveInvoiceFiscalExportSummary(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceFiscalExportSummary");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(InvoiceFiscalExportSummary), null, owner);
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


                public static InvoiceFiscalExportSummary RetrieveInvoiceFiscalExportSummary(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (InvoiceFiscalExportSummary) StorageSupport.RetrieveInformation(relativeLocation, typeof(InvoiceFiscalExportSummary), null, owner);
                    return result;
                }

				public static InvoiceFiscalExportSummary RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = InvoiceFiscalExportSummary.RetrieveInvoiceFiscalExportSummary("Content/TheBall.CORE/InvoiceFiscalExportSummary/" + contentName, containerOwner);
					var result = InvoiceFiscalExportSummary.RetrieveInvoiceFiscalExportSummary("TheBall.CORE/InvoiceFiscalExportSummary/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.CORE/InvoiceFiscalExportSummary/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.CORE/InvoiceFiscalExportSummary/" + contentName);
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

				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
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
					CopyContentFrom((InvoiceFiscalExportSummary) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceFiscalExportSummary));
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

				public static InvoiceFiscalExportSummary DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceFiscalExportSummary));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceFiscalExportSummary) serializer.ReadObject(xmlReader);
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
					return Path.Combine("TheBall.CORE", "InvoiceFiscalExportSummary", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceFiscalExportSummary", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceFiscalExportSummary customDemoObject);



				public static InvoiceFiscalExportSummary CreateDefault()
				{
					var result = new InvoiceFiscalExportSummary();
					result.ExportedInvoices = InvoiceCollection.CreateDefault();
					return result;
				}

				public static InvoiceFiscalExportSummary CreateDemoDefault()
				{
					InvoiceFiscalExportSummary customDemo = null;
					InvoiceFiscalExportSummary.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceFiscalExportSummary();
					result.ExportedInvoices = InvoiceCollection.CreateDemoDefault();
				
					return result;
				}


				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(masterInstance is InvoiceCollection) {
						CollectionUpdateImplementation.Update_InvoiceFiscalExportSummary_ExportedInvoices(this, localCollection:ExportedInvoices, masterCollection:(InvoiceCollection) masterInstance);
					} else if(ExportedInvoices != null) {
						((IInformationObject) ExportedInvoices).UpdateCollections(masterInstance);
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
						IInformationObject item = ExportedInvoices;
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
						var item = ExportedInvoices;
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
						var item = (IInformationObject) ExportedInvoices;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get {
						if(FiscalInclusiveStartDate != _unmodified_FiscalInclusiveStartDate)
							return true;
						if(FiscalInclusiveEndDate != _unmodified_FiscalInclusiveEndDate)
							return true;
						if(ExportedInvoices != _unmodified_ExportedInvoices)
							return true;
						{
							IInformationObject item = (IInformationObject) ExportedInvoices;
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
					if(ExportedInvoices != null) {
						if(ExportedInvoices.ID == replacingObject.ID)
							ExportedInvoices = (InvoiceCollection) replacingObject;
						else {
							IInformationObject iObject = ExportedInvoices;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(InvoiceFiscalExportSummary sourceObject)
				{
					FiscalInclusiveStartDate = sourceObject.FiscalInclusiveStartDate;
					FiscalInclusiveEndDate = sourceObject.FiscalInclusiveEndDate;
					ExportedInvoices = sourceObject.ExportedInvoices;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_FiscalInclusiveStartDate = FiscalInclusiveStartDate;
					_unmodified_FiscalInclusiveEndDate = FiscalInclusiveEndDate;
				
					_unmodified_ExportedInvoices = ExportedInvoices;
					if(ExportedInvoices != null)
						((IInformationObject) ExportedInvoices).SetInstanceTreeValuesAsUnmodified();

				
				}




				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "FiscalInclusiveStartDate":
							FiscalInclusiveStartDate = DateTime.Parse(value);
							break;
						case "FiscalInclusiveEndDate":
							FiscalInclusiveEndDate = DateTime.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public DateTime FiscalInclusiveStartDate { get; set; }
			private DateTime _unmodified_FiscalInclusiveStartDate;
			[DataMember]
			public DateTime FiscalInclusiveEndDate { get; set; }
			private DateTime _unmodified_FiscalInclusiveEndDate;
			[DataMember]
			public InvoiceCollection ExportedInvoices { get; set; }
			private InvoiceCollection _unmodified_ExportedInvoices;
			
			}
			[DataContract]
			public partial class InvoiceSummaryContainer : IInformationObject 
			{
				public InvoiceSummaryContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceSummaryContainer";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceSummaryContainer/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(InvoiceSummaryContainer), null, owner);
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.CORE", "InvoiceSummaryContainer", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static InvoiceSummaryContainer RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveInvoiceSummaryContainer(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceSummaryContainer");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(InvoiceSummaryContainer), null, owner);
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


                public static InvoiceSummaryContainer RetrieveInvoiceSummaryContainer(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (InvoiceSummaryContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(InvoiceSummaryContainer), null, owner);
                    return result;
                }

				public static InvoiceSummaryContainer RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = InvoiceSummaryContainer.RetrieveInvoiceSummaryContainer("Content/TheBall.CORE/InvoiceSummaryContainer/" + contentName, containerOwner);
					var result = InvoiceSummaryContainer.RetrieveInvoiceSummaryContainer("TheBall.CORE/InvoiceSummaryContainer/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.CORE/InvoiceSummaryContainer/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.CORE/InvoiceSummaryContainer/" + contentName);
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

				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
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
					CopyContentFrom((InvoiceSummaryContainer) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceSummaryContainer));
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

				public static InvoiceSummaryContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceSummaryContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceSummaryContainer) serializer.ReadObject(xmlReader);
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
					return Path.Combine("TheBall.CORE", "InvoiceSummaryContainer", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceSummaryContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceSummaryContainer customDemoObject);



				public static InvoiceSummaryContainer CreateDefault()
				{
					var result = new InvoiceSummaryContainer();
					result.OpenInvoices = InvoiceCollection.CreateDefault();
					result.PredictedInvoices = InvoiceCollection.CreateDefault();
					result.PaidInvoicesActiveYear = InvoiceCollection.CreateDefault();
					result.PaidInvoicesLast12Months = InvoiceCollection.CreateDefault();
					return result;
				}

				public static InvoiceSummaryContainer CreateDemoDefault()
				{
					InvoiceSummaryContainer customDemo = null;
					InvoiceSummaryContainer.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceSummaryContainer();
					result.OpenInvoices = InvoiceCollection.CreateDemoDefault();
					result.PredictedInvoices = InvoiceCollection.CreateDemoDefault();
					result.PaidInvoicesActiveYear = InvoiceCollection.CreateDemoDefault();
					result.PaidInvoicesLast12Months = InvoiceCollection.CreateDemoDefault();
				
					return result;
				}


				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(masterInstance is InvoiceCollection) {
						CollectionUpdateImplementation.Update_InvoiceSummaryContainer_OpenInvoices(this, localCollection:OpenInvoices, masterCollection:(InvoiceCollection) masterInstance);
					} else if(OpenInvoices != null) {
						((IInformationObject) OpenInvoices).UpdateCollections(masterInstance);
					}
					if(masterInstance is InvoiceCollection) {
						CollectionUpdateImplementation.Update_InvoiceSummaryContainer_PredictedInvoices(this, localCollection:PredictedInvoices, masterCollection:(InvoiceCollection) masterInstance);
					} else if(PredictedInvoices != null) {
						((IInformationObject) PredictedInvoices).UpdateCollections(masterInstance);
					}
					if(masterInstance is InvoiceCollection) {
						CollectionUpdateImplementation.Update_InvoiceSummaryContainer_PaidInvoicesActiveYear(this, localCollection:PaidInvoicesActiveYear, masterCollection:(InvoiceCollection) masterInstance);
					} else if(PaidInvoicesActiveYear != null) {
						((IInformationObject) PaidInvoicesActiveYear).UpdateCollections(masterInstance);
					}
					if(masterInstance is InvoiceCollection) {
						CollectionUpdateImplementation.Update_InvoiceSummaryContainer_PaidInvoicesLast12Months(this, localCollection:PaidInvoicesLast12Months, masterCollection:(InvoiceCollection) masterInstance);
					} else if(PaidInvoicesLast12Months != null) {
						((IInformationObject) PaidInvoicesLast12Months).UpdateCollections(masterInstance);
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
						IInformationObject item = OpenInvoices;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = PredictedInvoices;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = PaidInvoicesActiveYear;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = PaidInvoicesLast12Months;
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
						var item = OpenInvoices;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = PredictedInvoices;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = PaidInvoicesActiveYear;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = PaidInvoicesLast12Months;
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
						var item = (IInformationObject) OpenInvoices;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) PredictedInvoices;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) PaidInvoicesActiveYear;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) PaidInvoicesLast12Months;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get {
						if(OpenInvoices != _unmodified_OpenInvoices)
							return true;
						if(PredictedInvoices != _unmodified_PredictedInvoices)
							return true;
						if(PaidInvoicesActiveYear != _unmodified_PaidInvoicesActiveYear)
							return true;
						if(PaidInvoicesLast12Months != _unmodified_PaidInvoicesLast12Months)
							return true;
						{
							IInformationObject item = (IInformationObject) OpenInvoices;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) PredictedInvoices;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) PaidInvoicesActiveYear;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) PaidInvoicesLast12Months;
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
					if(OpenInvoices != null) {
						if(OpenInvoices.ID == replacingObject.ID)
							OpenInvoices = (InvoiceCollection) replacingObject;
						else {
							IInformationObject iObject = OpenInvoices;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(PredictedInvoices != null) {
						if(PredictedInvoices.ID == replacingObject.ID)
							PredictedInvoices = (InvoiceCollection) replacingObject;
						else {
							IInformationObject iObject = PredictedInvoices;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(PaidInvoicesActiveYear != null) {
						if(PaidInvoicesActiveYear.ID == replacingObject.ID)
							PaidInvoicesActiveYear = (InvoiceCollection) replacingObject;
						else {
							IInformationObject iObject = PaidInvoicesActiveYear;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(PaidInvoicesLast12Months != null) {
						if(PaidInvoicesLast12Months.ID == replacingObject.ID)
							PaidInvoicesLast12Months = (InvoiceCollection) replacingObject;
						else {
							IInformationObject iObject = PaidInvoicesLast12Months;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(InvoiceSummaryContainer sourceObject)
				{
					OpenInvoices = sourceObject.OpenInvoices;
					PredictedInvoices = sourceObject.PredictedInvoices;
					PaidInvoicesActiveYear = sourceObject.PaidInvoicesActiveYear;
					PaidInvoicesLast12Months = sourceObject.PaidInvoicesLast12Months;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
				
					_unmodified_OpenInvoices = OpenInvoices;
					if(OpenInvoices != null)
						((IInformationObject) OpenInvoices).SetInstanceTreeValuesAsUnmodified();

					_unmodified_PredictedInvoices = PredictedInvoices;
					if(PredictedInvoices != null)
						((IInformationObject) PredictedInvoices).SetInstanceTreeValuesAsUnmodified();

					_unmodified_PaidInvoicesActiveYear = PaidInvoicesActiveYear;
					if(PaidInvoicesActiveYear != null)
						((IInformationObject) PaidInvoicesActiveYear).SetInstanceTreeValuesAsUnmodified();

					_unmodified_PaidInvoicesLast12Months = PaidInvoicesLast12Months;
					if(PaidInvoicesLast12Months != null)
						((IInformationObject) PaidInvoicesLast12Months).SetInstanceTreeValuesAsUnmodified();

				
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
			public InvoiceCollection OpenInvoices { get; set; }
			private InvoiceCollection _unmodified_OpenInvoices;
			[DataMember]
			public InvoiceCollection PredictedInvoices { get; set; }
			private InvoiceCollection _unmodified_PredictedInvoices;
			[DataMember]
			public InvoiceCollection PaidInvoicesActiveYear { get; set; }
			private InvoiceCollection _unmodified_PaidInvoicesActiveYear;
			[DataMember]
			public InvoiceCollection PaidInvoicesLast12Months { get; set; }
			private InvoiceCollection _unmodified_PaidInvoicesLast12Months;
			
			}
			[DataContract]
			public partial class InvoiceCollection : IInformationObject , IInformationCollection
			{
				public InvoiceCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(InvoiceCollection), null, owner);
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.CORE", "InvoiceCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static InvoiceCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveInvoiceCollection(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceCollection");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(InvoiceCollection), null, owner);
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


                public static InvoiceCollection RetrieveInvoiceCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (InvoiceCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(InvoiceCollection), null, owner);
                    return result;
                }

				public static InvoiceCollection RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = InvoiceCollection.RetrieveInvoiceCollection("Content/TheBall.CORE/InvoiceCollection/" + contentName, containerOwner);
					var result = InvoiceCollection.RetrieveInvoiceCollection("TheBall.CORE/InvoiceCollection/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.CORE/InvoiceCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.CORE/InvoiceCollection/" + contentName);
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

				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
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
					CopyContentFrom((InvoiceCollection) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceCollection));
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

				public static InvoiceCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceCollection) serializer.ReadObject(xmlReader);
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
					return Path.Combine("TheBall.CORE", "InvoiceCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceCollection customDemoObject);


				
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
					string dummyItemLocation = Invoice.GetRelativeLocationFromID("dummy");
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
																								 typeof(Invoice));
					CollectionContent.Clear();
					CollectionContent.AddRange(informationObjects.Select(obj => (Invoice) obj));
            
				}

				public static InvoiceCollection GetMasterCollectionInstance(IContainerOwner owner)
				{
					return InvoiceCollection.RetrieveFromOwnerContent(owner, "MasterCollection");
				}

				public void SubscribeToContentSource()
				{
					// DirectoryToCollection
					string itemDirectory = GetItemDirectory();
					SubscribeSupport.AddSubscriptionToObject(itemDirectory, RelativeLocation,
															 SubscribeSupport.SubscribeType_DirectoryToCollection, null, typeof(InvoiceCollection).FullName);
				}

				public static string GetMasterCollectionLocation(IContainerOwner owner)
				{
					return StorageSupport.GetBlobOwnerAddress(owner, "TheBall.CORE/InvoiceCollection/" + "MasterCollection");
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

				
		
				public static InvoiceCollection CreateDefault()
				{
					var result = new InvoiceCollection();
					return result;
				}

				public static InvoiceCollection CreateDemoDefault()
				{
					InvoiceCollection customDemo = null;
					InvoiceCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceCollection();
					result.CollectionContent.Add(Invoice.CreateDemoDefault());
					//result.CollectionContent.Add(Invoice.CreateDemoDefault());
					//result.CollectionContent.Add(Invoice.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<Invoice> CollectionContent = new List<Invoice>();
				private Invoice[] _unmodified_CollectionContent;

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

				public Invoice[] GetIDSelectedArray()
				{
					if (IsCollectionFiltered == false || this.OrderFilterIDList == null)
						return CollectionContent.ToArray();
					return
						this.OrderFilterIDList.Select(id => CollectionContent.FirstOrDefault(item => item.ID == id)).ToArray();
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
							CollectionContent[i] = (Invoice )replacingObject;
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

				private void CopyContentFrom(InvoiceCollection sourceObject)
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
			public partial class Invoice : IInformationObject 
			{
				public Invoice()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "Invoice";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/Invoice/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(Invoice), null, owner);
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.CORE", "Invoice", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Invoice RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveInvoice(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: Invoice");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(Invoice), null, owner);
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


                public static Invoice RetrieveInvoice(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Invoice) StorageSupport.RetrieveInformation(relativeLocation, typeof(Invoice), null, owner);
                    return result;
                }

				public static Invoice RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = Invoice.RetrieveInvoice("Content/TheBall.CORE/Invoice/" + contentName, containerOwner);
					var result = Invoice.RetrieveInvoice("TheBall.CORE/Invoice/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.CORE/Invoice/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.CORE/Invoice/" + contentName);
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

				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
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
					CopyContentFrom((Invoice) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(Invoice));
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

				public static Invoice DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Invoice));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Invoice) serializer.ReadObject(xmlReader);
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
					return Path.Combine("TheBall.CORE", "Invoice", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "Invoice", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref Invoice customDemoObject);



				public static Invoice CreateDefault()
				{
					var result = new Invoice();
					result.ReferenceToInformation = ReferenceToInformation.CreateDefault();
					result.InvoiceDetails = InvoiceDetails.CreateDefault();
					result.InvoiceUsers = InvoiceUserCollection.CreateDefault();
					return result;
				}

				public static Invoice CreateDemoDefault()
				{
					Invoice customDemo = null;
					Invoice.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new Invoice();
					result.ReferenceToInformation = ReferenceToInformation.CreateDemoDefault();
					result.InvoiceName = @"Invoice.InvoiceName";

					result.InvoiceID = @"Invoice.InvoiceID";

					result.InvoicedAmount = @"Invoice.InvoicedAmount";

					result.PaidAmount = @"Invoice.PaidAmount";

					result.FeesAndInterestAmount = @"Invoice.FeesAndInterestAmount";

					result.UnpaidAmount = @"Invoice.UnpaidAmount";

					result.InvoiceDetails = InvoiceDetails.CreateDemoDefault();
					result.InvoiceUsers = InvoiceUserCollection.CreateDemoDefault();
				
					return result;
				}


				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(InvoiceDetails != null) {
						((IInformationObject) InvoiceDetails).UpdateCollections(masterInstance);
					}

					if(InvoiceUsers != null) {
						((IInformationObject) InvoiceUsers).UpdateCollections(masterInstance);
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
						IInformationObject item = InvoiceDetails;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = InvoiceUsers;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					if(searchWithinCurrentMasterOnly == false)
					{
						{
							IInformationObject item = ReferenceToInformation;
							if(item != null)
							{
								item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
							}
						}
					}					
				}


				private object FindFromObjectTree(string objectId)
				{
					{
						var item = ReferenceToInformation;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = InvoiceDetails;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = InvoiceUsers;
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
						var item = (IInformationObject) ReferenceToInformation;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) InvoiceDetails;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) InvoiceUsers;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get {
						if(ReferenceToInformation != _unmodified_ReferenceToInformation)
							return true;
						if(InvoiceName != _unmodified_InvoiceName)
							return true;
						if(InvoiceID != _unmodified_InvoiceID)
							return true;
						if(InvoicedAmount != _unmodified_InvoicedAmount)
							return true;
						if(CreateDate != _unmodified_CreateDate)
							return true;
						if(DueDate != _unmodified_DueDate)
							return true;
						if(PaidAmount != _unmodified_PaidAmount)
							return true;
						if(FeesAndInterestAmount != _unmodified_FeesAndInterestAmount)
							return true;
						if(UnpaidAmount != _unmodified_UnpaidAmount)
							return true;
						if(InvoiceDetails != _unmodified_InvoiceDetails)
							return true;
						if(InvoiceUsers != _unmodified_InvoiceUsers)
							return true;
						{
							IInformationObject item = (IInformationObject) InvoiceDetails;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) InvoiceUsers;
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
					if(ReferenceToInformation != null) {
						if(ReferenceToInformation.ID == replacingObject.ID)
							ReferenceToInformation = (ReferenceToInformation) replacingObject;
						else {
							IInformationObject iObject = ReferenceToInformation;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(InvoiceDetails != null) {
						if(InvoiceDetails.ID == replacingObject.ID)
							InvoiceDetails = (InvoiceDetails) replacingObject;
						else {
							IInformationObject iObject = InvoiceDetails;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(InvoiceUsers != null) {
						if(InvoiceUsers.ID == replacingObject.ID)
							InvoiceUsers = (InvoiceUserCollection) replacingObject;
						else {
							IInformationObject iObject = InvoiceUsers;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(Invoice sourceObject)
				{
					ReferenceToInformation = sourceObject.ReferenceToInformation;
					InvoiceName = sourceObject.InvoiceName;
					InvoiceID = sourceObject.InvoiceID;
					InvoicedAmount = sourceObject.InvoicedAmount;
					CreateDate = sourceObject.CreateDate;
					DueDate = sourceObject.DueDate;
					PaidAmount = sourceObject.PaidAmount;
					FeesAndInterestAmount = sourceObject.FeesAndInterestAmount;
					UnpaidAmount = sourceObject.UnpaidAmount;
					InvoiceDetails = sourceObject.InvoiceDetails;
					InvoiceUsers = sourceObject.InvoiceUsers;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_InvoiceName = InvoiceName;
					_unmodified_InvoiceID = InvoiceID;
					_unmodified_InvoicedAmount = InvoicedAmount;
					_unmodified_CreateDate = CreateDate;
					_unmodified_DueDate = DueDate;
					_unmodified_PaidAmount = PaidAmount;
					_unmodified_FeesAndInterestAmount = FeesAndInterestAmount;
					_unmodified_UnpaidAmount = UnpaidAmount;
				
					_unmodified_ReferenceToInformation = ReferenceToInformation;
					if(ReferenceToInformation != null)
						((IInformationObject) ReferenceToInformation).SetInstanceTreeValuesAsUnmodified();

					_unmodified_InvoiceDetails = InvoiceDetails;
					if(InvoiceDetails != null)
						((IInformationObject) InvoiceDetails).SetInstanceTreeValuesAsUnmodified();

					_unmodified_InvoiceUsers = InvoiceUsers;
					if(InvoiceUsers != null)
						((IInformationObject) InvoiceUsers).SetInstanceTreeValuesAsUnmodified();

				
				}




				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "InvoiceName":
							InvoiceName = value;
							break;
						case "InvoiceID":
							InvoiceID = value;
							break;
						case "InvoicedAmount":
							InvoicedAmount = value;
							break;
						case "CreateDate":
							CreateDate = DateTime.Parse(value);
							break;
						case "DueDate":
							DueDate = DateTime.Parse(value);
							break;
						case "PaidAmount":
							PaidAmount = value;
							break;
						case "FeesAndInterestAmount":
							FeesAndInterestAmount = value;
							break;
						case "UnpaidAmount":
							UnpaidAmount = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public ReferenceToInformation ReferenceToInformation { get; set; }
			private ReferenceToInformation _unmodified_ReferenceToInformation;
			[DataMember]
			public string InvoiceName { get; set; }
			private string _unmodified_InvoiceName;
			[DataMember]
			public string InvoiceID { get; set; }
			private string _unmodified_InvoiceID;
			[DataMember]
			public string InvoicedAmount { get; set; }
			private string _unmodified_InvoicedAmount;
			[DataMember]
			public DateTime CreateDate { get; set; }
			private DateTime _unmodified_CreateDate;
			[DataMember]
			public DateTime DueDate { get; set; }
			private DateTime _unmodified_DueDate;
			[DataMember]
			public string PaidAmount { get; set; }
			private string _unmodified_PaidAmount;
			[DataMember]
			public string FeesAndInterestAmount { get; set; }
			private string _unmodified_FeesAndInterestAmount;
			[DataMember]
			public string UnpaidAmount { get; set; }
			private string _unmodified_UnpaidAmount;
			[DataMember]
			public InvoiceDetails InvoiceDetails { get; set; }
			private InvoiceDetails _unmodified_InvoiceDetails;
			[DataMember]
			public InvoiceUserCollection InvoiceUsers { get; set; }
			private InvoiceUserCollection _unmodified_InvoiceUsers;
			
			}
			[DataContract]
			public partial class InvoiceDetails : IInformationObject 
			{
				public InvoiceDetails()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceDetails";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceDetails/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(InvoiceDetails), null, owner);
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.CORE", "InvoiceDetails", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static InvoiceDetails RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveInvoiceDetails(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceDetails");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(InvoiceDetails), null, owner);
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


                public static InvoiceDetails RetrieveInvoiceDetails(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (InvoiceDetails) StorageSupport.RetrieveInformation(relativeLocation, typeof(InvoiceDetails), null, owner);
                    return result;
                }

				public static InvoiceDetails RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = InvoiceDetails.RetrieveInvoiceDetails("Content/TheBall.CORE/InvoiceDetails/" + contentName, containerOwner);
					var result = InvoiceDetails.RetrieveInvoiceDetails("TheBall.CORE/InvoiceDetails/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.CORE/InvoiceDetails/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.CORE/InvoiceDetails/" + contentName);
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

				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
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
					CopyContentFrom((InvoiceDetails) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceDetails));
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

				public static InvoiceDetails DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceDetails));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceDetails) serializer.ReadObject(xmlReader);
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
					return Path.Combine("TheBall.CORE", "InvoiceDetails", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceDetails", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceDetails customDemoObject);



				public static InvoiceDetails CreateDefault()
				{
					var result = new InvoiceDetails();
					return result;
				}

				public static InvoiceDetails CreateDemoDefault()
				{
					InvoiceDetails customDemo = null;
					InvoiceDetails.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceDetails();
					result.MonthlyFeesTotal = @"InvoiceDetails.MonthlyFeesTotal";

					result.OneTimeFeesTotal = @"InvoiceDetails.OneTimeFeesTotal";

					result.UsageFeesTotal = @"InvoiceDetails.UsageFeesTotal";

					result.InterestFeesTotal = @"InvoiceDetails.InterestFeesTotal";

					result.PenaltyFeesTotal = @"InvoiceDetails.PenaltyFeesTotal";

					result.TotalFeesTotal = @"InvoiceDetails.TotalFeesTotal";

				
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
						if(MonthlyFeesTotal != _unmodified_MonthlyFeesTotal)
							return true;
						if(OneTimeFeesTotal != _unmodified_OneTimeFeesTotal)
							return true;
						if(UsageFeesTotal != _unmodified_UsageFeesTotal)
							return true;
						if(InterestFeesTotal != _unmodified_InterestFeesTotal)
							return true;
						if(PenaltyFeesTotal != _unmodified_PenaltyFeesTotal)
							return true;
						if(TotalFeesTotal != _unmodified_TotalFeesTotal)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(InvoiceDetails sourceObject)
				{
					MonthlyFeesTotal = sourceObject.MonthlyFeesTotal;
					OneTimeFeesTotal = sourceObject.OneTimeFeesTotal;
					UsageFeesTotal = sourceObject.UsageFeesTotal;
					InterestFeesTotal = sourceObject.InterestFeesTotal;
					PenaltyFeesTotal = sourceObject.PenaltyFeesTotal;
					TotalFeesTotal = sourceObject.TotalFeesTotal;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_MonthlyFeesTotal = MonthlyFeesTotal;
					_unmodified_OneTimeFeesTotal = OneTimeFeesTotal;
					_unmodified_UsageFeesTotal = UsageFeesTotal;
					_unmodified_InterestFeesTotal = InterestFeesTotal;
					_unmodified_PenaltyFeesTotal = PenaltyFeesTotal;
					_unmodified_TotalFeesTotal = TotalFeesTotal;
				
				
				}




				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "MonthlyFeesTotal":
							MonthlyFeesTotal = value;
							break;
						case "OneTimeFeesTotal":
							OneTimeFeesTotal = value;
							break;
						case "UsageFeesTotal":
							UsageFeesTotal = value;
							break;
						case "InterestFeesTotal":
							InterestFeesTotal = value;
							break;
						case "PenaltyFeesTotal":
							PenaltyFeesTotal = value;
							break;
						case "TotalFeesTotal":
							TotalFeesTotal = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string MonthlyFeesTotal { get; set; }
			private string _unmodified_MonthlyFeesTotal;
			[DataMember]
			public string OneTimeFeesTotal { get; set; }
			private string _unmodified_OneTimeFeesTotal;
			[DataMember]
			public string UsageFeesTotal { get; set; }
			private string _unmodified_UsageFeesTotal;
			[DataMember]
			public string InterestFeesTotal { get; set; }
			private string _unmodified_InterestFeesTotal;
			[DataMember]
			public string PenaltyFeesTotal { get; set; }
			private string _unmodified_PenaltyFeesTotal;
			[DataMember]
			public string TotalFeesTotal { get; set; }
			private string _unmodified_TotalFeesTotal;
			
			}
			[DataContract]
			public partial class InvoiceUserCollection : IInformationObject , IInformationCollection
			{
				public InvoiceUserCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceUserCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceUserCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(InvoiceUserCollection), null, owner);
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.CORE", "InvoiceUserCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static InvoiceUserCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveInvoiceUserCollection(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceUserCollection");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(InvoiceUserCollection), null, owner);
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


                public static InvoiceUserCollection RetrieveInvoiceUserCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (InvoiceUserCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(InvoiceUserCollection), null, owner);
                    return result;
                }

				public static InvoiceUserCollection RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = InvoiceUserCollection.RetrieveInvoiceUserCollection("Content/TheBall.CORE/InvoiceUserCollection/" + contentName, containerOwner);
					var result = InvoiceUserCollection.RetrieveInvoiceUserCollection("TheBall.CORE/InvoiceUserCollection/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.CORE/InvoiceUserCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.CORE/InvoiceUserCollection/" + contentName);
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

				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
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
					CopyContentFrom((InvoiceUserCollection) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceUserCollection));
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

				public static InvoiceUserCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceUserCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceUserCollection) serializer.ReadObject(xmlReader);
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
					return Path.Combine("TheBall.CORE", "InvoiceUserCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceUserCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceUserCollection customDemoObject);


				
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
						return false;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					throw new NotSupportedException("Master collection location only supported for master collections");
					
				}

				IInformationCollection IInformationCollection.GetMasterInstance()
				{
					throw new NotSupportedException("Master collection instance only supported for master collections");
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = InvoiceUser.GetRelativeLocationFromID("dummy");
					string nonOwnerDirectoryLocation = SubscribeSupport.GetParentDirectoryTarget(dummyItemLocation);
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetBlobOwnerAddress(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public void RefreshContent()
				{
				}


				public void SubscribeToContentSource()
				{
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

				
		
				public static InvoiceUserCollection CreateDefault()
				{
					var result = new InvoiceUserCollection();
					return result;
				}

				public static InvoiceUserCollection CreateDemoDefault()
				{
					InvoiceUserCollection customDemo = null;
					InvoiceUserCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceUserCollection();
					result.CollectionContent.Add(InvoiceUser.CreateDemoDefault());
					//result.CollectionContent.Add(InvoiceUser.CreateDemoDefault());
					//result.CollectionContent.Add(InvoiceUser.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<InvoiceUser> CollectionContent = new List<InvoiceUser>();
				private InvoiceUser[] _unmodified_CollectionContent;

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

				public InvoiceUser[] GetIDSelectedArray()
				{
					if (IsCollectionFiltered == false || this.OrderFilterIDList == null)
						return CollectionContent.ToArray();
					return
						this.OrderFilterIDList.Select(id => CollectionContent.FirstOrDefault(item => item.ID == id)).ToArray();
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
							CollectionContent[i] = (InvoiceUser )replacingObject;
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

				private void CopyContentFrom(InvoiceUserCollection sourceObject)
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
			public partial class InvoiceUser : IInformationObject 
			{
				public InvoiceUser()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceUser";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceUser/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(InvoiceUser), null, owner);
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.CORE", "InvoiceUser", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static InvoiceUser RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveInvoiceUser(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceUser");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(InvoiceUser), null, owner);
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


                public static InvoiceUser RetrieveInvoiceUser(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (InvoiceUser) StorageSupport.RetrieveInformation(relativeLocation, typeof(InvoiceUser), null, owner);
                    return result;
                }

				public static InvoiceUser RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = InvoiceUser.RetrieveInvoiceUser("Content/TheBall.CORE/InvoiceUser/" + contentName, containerOwner);
					var result = InvoiceUser.RetrieveInvoiceUser("TheBall.CORE/InvoiceUser/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.CORE/InvoiceUser/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.CORE/InvoiceUser/" + contentName);
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

				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
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
					CopyContentFrom((InvoiceUser) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceUser));
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

				public static InvoiceUser DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceUser));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceUser) serializer.ReadObject(xmlReader);
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
					return Path.Combine("TheBall.CORE", "InvoiceUser", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceUser", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceUser customDemoObject);



				public static InvoiceUser CreateDefault()
				{
					var result = new InvoiceUser();
					result.InvoiceRowGroupCollection = InvoiceRowGroupCollection.CreateDefault();
					result.InvoiceEventDetailGroupCollection = InvoiceEventDetailGroupCollection.CreateDefault();
					return result;
				}

				public static InvoiceUser CreateDemoDefault()
				{
					InvoiceUser customDemo = null;
					InvoiceUser.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceUser();
					result.UserName = @"InvoiceUser.UserName";

					result.UserID = @"InvoiceUser.UserID";

					result.UserPhoneNumber = @"InvoiceUser.UserPhoneNumber";

					result.UserSubscriptionNumber = @"InvoiceUser.UserSubscriptionNumber";

					result.UserInvoiceTotalAmount = @"InvoiceUser.UserInvoiceTotalAmount";

					result.InvoiceRowGroupCollection = InvoiceRowGroupCollection.CreateDemoDefault();
					result.InvoiceEventDetailGroupCollection = InvoiceEventDetailGroupCollection.CreateDemoDefault();
				
					return result;
				}


				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(InvoiceRowGroupCollection != null) {
						((IInformationObject) InvoiceRowGroupCollection).UpdateCollections(masterInstance);
					}

					if(InvoiceEventDetailGroupCollection != null) {
						((IInformationObject) InvoiceEventDetailGroupCollection).UpdateCollections(masterInstance);
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
						IInformationObject item = InvoiceRowGroupCollection;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = InvoiceEventDetailGroupCollection;
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
						var item = InvoiceRowGroupCollection;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = InvoiceEventDetailGroupCollection;
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
						var item = (IInformationObject) InvoiceRowGroupCollection;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) InvoiceEventDetailGroupCollection;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get {
						if(UserName != _unmodified_UserName)
							return true;
						if(UserID != _unmodified_UserID)
							return true;
						if(UserPhoneNumber != _unmodified_UserPhoneNumber)
							return true;
						if(UserSubscriptionNumber != _unmodified_UserSubscriptionNumber)
							return true;
						if(UserInvoiceTotalAmount != _unmodified_UserInvoiceTotalAmount)
							return true;
						if(InvoiceRowGroupCollection != _unmodified_InvoiceRowGroupCollection)
							return true;
						if(InvoiceEventDetailGroupCollection != _unmodified_InvoiceEventDetailGroupCollection)
							return true;
						{
							IInformationObject item = (IInformationObject) InvoiceRowGroupCollection;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) InvoiceEventDetailGroupCollection;
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
					if(InvoiceRowGroupCollection != null) {
						if(InvoiceRowGroupCollection.ID == replacingObject.ID)
							InvoiceRowGroupCollection = (InvoiceRowGroupCollection) replacingObject;
						else {
							IInformationObject iObject = InvoiceRowGroupCollection;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(InvoiceEventDetailGroupCollection != null) {
						if(InvoiceEventDetailGroupCollection.ID == replacingObject.ID)
							InvoiceEventDetailGroupCollection = (InvoiceEventDetailGroupCollection) replacingObject;
						else {
							IInformationObject iObject = InvoiceEventDetailGroupCollection;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(InvoiceUser sourceObject)
				{
					UserName = sourceObject.UserName;
					UserID = sourceObject.UserID;
					UserPhoneNumber = sourceObject.UserPhoneNumber;
					UserSubscriptionNumber = sourceObject.UserSubscriptionNumber;
					UserInvoiceTotalAmount = sourceObject.UserInvoiceTotalAmount;
					InvoiceRowGroupCollection = sourceObject.InvoiceRowGroupCollection;
					InvoiceEventDetailGroupCollection = sourceObject.InvoiceEventDetailGroupCollection;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_UserName = UserName;
					_unmodified_UserID = UserID;
					_unmodified_UserPhoneNumber = UserPhoneNumber;
					_unmodified_UserSubscriptionNumber = UserSubscriptionNumber;
					_unmodified_UserInvoiceTotalAmount = UserInvoiceTotalAmount;
				
					_unmodified_InvoiceRowGroupCollection = InvoiceRowGroupCollection;
					if(InvoiceRowGroupCollection != null)
						((IInformationObject) InvoiceRowGroupCollection).SetInstanceTreeValuesAsUnmodified();

					_unmodified_InvoiceEventDetailGroupCollection = InvoiceEventDetailGroupCollection;
					if(InvoiceEventDetailGroupCollection != null)
						((IInformationObject) InvoiceEventDetailGroupCollection).SetInstanceTreeValuesAsUnmodified();

				
				}




				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "UserName":
							UserName = value;
							break;
						case "UserID":
							UserID = value;
							break;
						case "UserPhoneNumber":
							UserPhoneNumber = value;
							break;
						case "UserSubscriptionNumber":
							UserSubscriptionNumber = value;
							break;
						case "UserInvoiceTotalAmount":
							UserInvoiceTotalAmount = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string UserName { get; set; }
			private string _unmodified_UserName;
			[DataMember]
			public string UserID { get; set; }
			private string _unmodified_UserID;
			[DataMember]
			public string UserPhoneNumber { get; set; }
			private string _unmodified_UserPhoneNumber;
			[DataMember]
			public string UserSubscriptionNumber { get; set; }
			private string _unmodified_UserSubscriptionNumber;
			[DataMember]
			public string UserInvoiceTotalAmount { get; set; }
			private string _unmodified_UserInvoiceTotalAmount;
			[DataMember]
			public InvoiceRowGroupCollection InvoiceRowGroupCollection { get; set; }
			private InvoiceRowGroupCollection _unmodified_InvoiceRowGroupCollection;
			[DataMember]
			public InvoiceEventDetailGroupCollection InvoiceEventDetailGroupCollection { get; set; }
			private InvoiceEventDetailGroupCollection _unmodified_InvoiceEventDetailGroupCollection;
			
			}
			[DataContract]
			public partial class InvoiceRowGroupCollection : IInformationObject , IInformationCollection
			{
				public InvoiceRowGroupCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceRowGroupCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceRowGroupCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(InvoiceRowGroupCollection), null, owner);
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.CORE", "InvoiceRowGroupCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static InvoiceRowGroupCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveInvoiceRowGroupCollection(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceRowGroupCollection");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(InvoiceRowGroupCollection), null, owner);
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


                public static InvoiceRowGroupCollection RetrieveInvoiceRowGroupCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (InvoiceRowGroupCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(InvoiceRowGroupCollection), null, owner);
                    return result;
                }

				public static InvoiceRowGroupCollection RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = InvoiceRowGroupCollection.RetrieveInvoiceRowGroupCollection("Content/TheBall.CORE/InvoiceRowGroupCollection/" + contentName, containerOwner);
					var result = InvoiceRowGroupCollection.RetrieveInvoiceRowGroupCollection("TheBall.CORE/InvoiceRowGroupCollection/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.CORE/InvoiceRowGroupCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.CORE/InvoiceRowGroupCollection/" + contentName);
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

				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
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
					CopyContentFrom((InvoiceRowGroupCollection) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceRowGroupCollection));
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

				public static InvoiceRowGroupCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceRowGroupCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceRowGroupCollection) serializer.ReadObject(xmlReader);
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
					return Path.Combine("TheBall.CORE", "InvoiceRowGroupCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceRowGroupCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceRowGroupCollection customDemoObject);


				
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
						return false;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					throw new NotSupportedException("Master collection location only supported for master collections");
					
				}

				IInformationCollection IInformationCollection.GetMasterInstance()
				{
					throw new NotSupportedException("Master collection instance only supported for master collections");
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = InvoiceRowGroup.GetRelativeLocationFromID("dummy");
					string nonOwnerDirectoryLocation = SubscribeSupport.GetParentDirectoryTarget(dummyItemLocation);
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetBlobOwnerAddress(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public void RefreshContent()
				{
				}


				public void SubscribeToContentSource()
				{
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

				
		
				public static InvoiceRowGroupCollection CreateDefault()
				{
					var result = new InvoiceRowGroupCollection();
					return result;
				}

				public static InvoiceRowGroupCollection CreateDemoDefault()
				{
					InvoiceRowGroupCollection customDemo = null;
					InvoiceRowGroupCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceRowGroupCollection();
					result.CollectionContent.Add(InvoiceRowGroup.CreateDemoDefault());
					//result.CollectionContent.Add(InvoiceRowGroup.CreateDemoDefault());
					//result.CollectionContent.Add(InvoiceRowGroup.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<InvoiceRowGroup> CollectionContent = new List<InvoiceRowGroup>();
				private InvoiceRowGroup[] _unmodified_CollectionContent;

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

				public InvoiceRowGroup[] GetIDSelectedArray()
				{
					if (IsCollectionFiltered == false || this.OrderFilterIDList == null)
						return CollectionContent.ToArray();
					return
						this.OrderFilterIDList.Select(id => CollectionContent.FirstOrDefault(item => item.ID == id)).ToArray();
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
							CollectionContent[i] = (InvoiceRowGroup )replacingObject;
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

				private void CopyContentFrom(InvoiceRowGroupCollection sourceObject)
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
			public partial class InvoiceEventDetailGroupCollection : IInformationObject , IInformationCollection
			{
				public InvoiceEventDetailGroupCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceEventDetailGroupCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceEventDetailGroupCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(InvoiceEventDetailGroupCollection), null, owner);
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.CORE", "InvoiceEventDetailGroupCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static InvoiceEventDetailGroupCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveInvoiceEventDetailGroupCollection(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceEventDetailGroupCollection");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(InvoiceEventDetailGroupCollection), null, owner);
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


                public static InvoiceEventDetailGroupCollection RetrieveInvoiceEventDetailGroupCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (InvoiceEventDetailGroupCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(InvoiceEventDetailGroupCollection), null, owner);
                    return result;
                }

				public static InvoiceEventDetailGroupCollection RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = InvoiceEventDetailGroupCollection.RetrieveInvoiceEventDetailGroupCollection("Content/TheBall.CORE/InvoiceEventDetailGroupCollection/" + contentName, containerOwner);
					var result = InvoiceEventDetailGroupCollection.RetrieveInvoiceEventDetailGroupCollection("TheBall.CORE/InvoiceEventDetailGroupCollection/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.CORE/InvoiceEventDetailGroupCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.CORE/InvoiceEventDetailGroupCollection/" + contentName);
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

				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
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
					CopyContentFrom((InvoiceEventDetailGroupCollection) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceEventDetailGroupCollection));
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

				public static InvoiceEventDetailGroupCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceEventDetailGroupCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceEventDetailGroupCollection) serializer.ReadObject(xmlReader);
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
					return Path.Combine("TheBall.CORE", "InvoiceEventDetailGroupCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceEventDetailGroupCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceEventDetailGroupCollection customDemoObject);


				
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
						return false;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					throw new NotSupportedException("Master collection location only supported for master collections");
					
				}

				IInformationCollection IInformationCollection.GetMasterInstance()
				{
					throw new NotSupportedException("Master collection instance only supported for master collections");
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = InvoiceEventDetailGroup.GetRelativeLocationFromID("dummy");
					string nonOwnerDirectoryLocation = SubscribeSupport.GetParentDirectoryTarget(dummyItemLocation);
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetBlobOwnerAddress(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public void RefreshContent()
				{
				}


				public void SubscribeToContentSource()
				{
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

				
		
				public static InvoiceEventDetailGroupCollection CreateDefault()
				{
					var result = new InvoiceEventDetailGroupCollection();
					return result;
				}

				public static InvoiceEventDetailGroupCollection CreateDemoDefault()
				{
					InvoiceEventDetailGroupCollection customDemo = null;
					InvoiceEventDetailGroupCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceEventDetailGroupCollection();
					result.CollectionContent.Add(InvoiceEventDetailGroup.CreateDemoDefault());
					//result.CollectionContent.Add(InvoiceEventDetailGroup.CreateDemoDefault());
					//result.CollectionContent.Add(InvoiceEventDetailGroup.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<InvoiceEventDetailGroup> CollectionContent = new List<InvoiceEventDetailGroup>();
				private InvoiceEventDetailGroup[] _unmodified_CollectionContent;

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

				public InvoiceEventDetailGroup[] GetIDSelectedArray()
				{
					if (IsCollectionFiltered == false || this.OrderFilterIDList == null)
						return CollectionContent.ToArray();
					return
						this.OrderFilterIDList.Select(id => CollectionContent.FirstOrDefault(item => item.ID == id)).ToArray();
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
							CollectionContent[i] = (InvoiceEventDetailGroup )replacingObject;
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

				private void CopyContentFrom(InvoiceEventDetailGroupCollection sourceObject)
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
			public partial class InvoiceRowGroup : IInformationObject 
			{
				public InvoiceRowGroup()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceRowGroup";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceRowGroup/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(InvoiceRowGroup), null, owner);
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.CORE", "InvoiceRowGroup", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static InvoiceRowGroup RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveInvoiceRowGroup(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceRowGroup");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(InvoiceRowGroup), null, owner);
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


                public static InvoiceRowGroup RetrieveInvoiceRowGroup(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (InvoiceRowGroup) StorageSupport.RetrieveInformation(relativeLocation, typeof(InvoiceRowGroup), null, owner);
                    return result;
                }

				public static InvoiceRowGroup RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = InvoiceRowGroup.RetrieveInvoiceRowGroup("Content/TheBall.CORE/InvoiceRowGroup/" + contentName, containerOwner);
					var result = InvoiceRowGroup.RetrieveInvoiceRowGroup("TheBall.CORE/InvoiceRowGroup/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.CORE/InvoiceRowGroup/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.CORE/InvoiceRowGroup/" + contentName);
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

				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
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
					CopyContentFrom((InvoiceRowGroup) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceRowGroup));
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

				public static InvoiceRowGroup DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceRowGroup));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceRowGroup) serializer.ReadObject(xmlReader);
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
					return Path.Combine("TheBall.CORE", "InvoiceRowGroup", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceRowGroup", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceRowGroup customDemoObject);



				public static InvoiceRowGroup CreateDefault()
				{
					var result = new InvoiceRowGroup();
					result.InvoiceRowCollection = InvoiceRowCollection.CreateDefault();
					return result;
				}

				public static InvoiceRowGroup CreateDemoDefault()
				{
					InvoiceRowGroup customDemo = null;
					InvoiceRowGroup.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceRowGroup();
					result.GroupName = @"InvoiceRowGroup.GroupName";

					result.GroupTotalPriceWithoutTaxes = @"InvoiceRowGroup.GroupTotalPriceWithoutTaxes";

					result.GroupTotalTaxes = @"InvoiceRowGroup.GroupTotalTaxes";

					result.GroupTotalPriceWithTaxes = @"InvoiceRowGroup.GroupTotalPriceWithTaxes";

					result.InvoiceRowCollection = InvoiceRowCollection.CreateDemoDefault();
				
					return result;
				}


				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(InvoiceRowCollection != null) {
						((IInformationObject) InvoiceRowCollection).UpdateCollections(masterInstance);
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
						IInformationObject item = InvoiceRowCollection;
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
						var item = InvoiceRowCollection;
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
						var item = (IInformationObject) InvoiceRowCollection;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get {
						if(GroupName != _unmodified_GroupName)
							return true;
						if(GroupTotalPriceWithoutTaxes != _unmodified_GroupTotalPriceWithoutTaxes)
							return true;
						if(GroupTotalTaxes != _unmodified_GroupTotalTaxes)
							return true;
						if(GroupTotalPriceWithTaxes != _unmodified_GroupTotalPriceWithTaxes)
							return true;
						if(InvoiceRowCollection != _unmodified_InvoiceRowCollection)
							return true;
						{
							IInformationObject item = (IInformationObject) InvoiceRowCollection;
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
					if(InvoiceRowCollection != null) {
						if(InvoiceRowCollection.ID == replacingObject.ID)
							InvoiceRowCollection = (InvoiceRowCollection) replacingObject;
						else {
							IInformationObject iObject = InvoiceRowCollection;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(InvoiceRowGroup sourceObject)
				{
					GroupName = sourceObject.GroupName;
					GroupTotalPriceWithoutTaxes = sourceObject.GroupTotalPriceWithoutTaxes;
					GroupTotalTaxes = sourceObject.GroupTotalTaxes;
					GroupTotalPriceWithTaxes = sourceObject.GroupTotalPriceWithTaxes;
					InvoiceRowCollection = sourceObject.InvoiceRowCollection;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_GroupName = GroupName;
					_unmodified_GroupTotalPriceWithoutTaxes = GroupTotalPriceWithoutTaxes;
					_unmodified_GroupTotalTaxes = GroupTotalTaxes;
					_unmodified_GroupTotalPriceWithTaxes = GroupTotalPriceWithTaxes;
				
					_unmodified_InvoiceRowCollection = InvoiceRowCollection;
					if(InvoiceRowCollection != null)
						((IInformationObject) InvoiceRowCollection).SetInstanceTreeValuesAsUnmodified();

				
				}




				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "GroupName":
							GroupName = value;
							break;
						case "GroupTotalPriceWithoutTaxes":
							GroupTotalPriceWithoutTaxes = value;
							break;
						case "GroupTotalTaxes":
							GroupTotalTaxes = value;
							break;
						case "GroupTotalPriceWithTaxes":
							GroupTotalPriceWithTaxes = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string GroupName { get; set; }
			private string _unmodified_GroupName;
			[DataMember]
			public string GroupTotalPriceWithoutTaxes { get; set; }
			private string _unmodified_GroupTotalPriceWithoutTaxes;
			[DataMember]
			public string GroupTotalTaxes { get; set; }
			private string _unmodified_GroupTotalTaxes;
			[DataMember]
			public string GroupTotalPriceWithTaxes { get; set; }
			private string _unmodified_GroupTotalPriceWithTaxes;
			[DataMember]
			public InvoiceRowCollection InvoiceRowCollection { get; set; }
			private InvoiceRowCollection _unmodified_InvoiceRowCollection;
			
			}
			[DataContract]
			public partial class InvoiceEventDetailGroup : IInformationObject 
			{
				public InvoiceEventDetailGroup()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceEventDetailGroup";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceEventDetailGroup/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(InvoiceEventDetailGroup), null, owner);
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.CORE", "InvoiceEventDetailGroup", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static InvoiceEventDetailGroup RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveInvoiceEventDetailGroup(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceEventDetailGroup");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(InvoiceEventDetailGroup), null, owner);
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


                public static InvoiceEventDetailGroup RetrieveInvoiceEventDetailGroup(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (InvoiceEventDetailGroup) StorageSupport.RetrieveInformation(relativeLocation, typeof(InvoiceEventDetailGroup), null, owner);
                    return result;
                }

				public static InvoiceEventDetailGroup RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = InvoiceEventDetailGroup.RetrieveInvoiceEventDetailGroup("Content/TheBall.CORE/InvoiceEventDetailGroup/" + contentName, containerOwner);
					var result = InvoiceEventDetailGroup.RetrieveInvoiceEventDetailGroup("TheBall.CORE/InvoiceEventDetailGroup/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.CORE/InvoiceEventDetailGroup/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.CORE/InvoiceEventDetailGroup/" + contentName);
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

				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
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
					CopyContentFrom((InvoiceEventDetailGroup) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceEventDetailGroup));
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

				public static InvoiceEventDetailGroup DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceEventDetailGroup));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceEventDetailGroup) serializer.ReadObject(xmlReader);
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
					return Path.Combine("TheBall.CORE", "InvoiceEventDetailGroup", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceEventDetailGroup", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceEventDetailGroup customDemoObject);



				public static InvoiceEventDetailGroup CreateDefault()
				{
					var result = new InvoiceEventDetailGroup();
					result.InvoiceEventDetailCollection = InvoiceEventDetailCollection.CreateDefault();
					return result;
				}

				public static InvoiceEventDetailGroup CreateDemoDefault()
				{
					InvoiceEventDetailGroup customDemo = null;
					InvoiceEventDetailGroup.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceEventDetailGroup();
					result.GroupName = @"InvoiceEventDetailGroup.GroupName";

					result.InvoiceEventDetailCollection = InvoiceEventDetailCollection.CreateDemoDefault();
				
					return result;
				}


				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(InvoiceEventDetailCollection != null) {
						((IInformationObject) InvoiceEventDetailCollection).UpdateCollections(masterInstance);
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
						IInformationObject item = InvoiceEventDetailCollection;
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
						var item = InvoiceEventDetailCollection;
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
						var item = (IInformationObject) InvoiceEventDetailCollection;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get {
						if(GroupName != _unmodified_GroupName)
							return true;
						if(InvoiceEventDetailCollection != _unmodified_InvoiceEventDetailCollection)
							return true;
						{
							IInformationObject item = (IInformationObject) InvoiceEventDetailCollection;
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
					if(InvoiceEventDetailCollection != null) {
						if(InvoiceEventDetailCollection.ID == replacingObject.ID)
							InvoiceEventDetailCollection = (InvoiceEventDetailCollection) replacingObject;
						else {
							IInformationObject iObject = InvoiceEventDetailCollection;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(InvoiceEventDetailGroup sourceObject)
				{
					GroupName = sourceObject.GroupName;
					InvoiceEventDetailCollection = sourceObject.InvoiceEventDetailCollection;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_GroupName = GroupName;
				
					_unmodified_InvoiceEventDetailCollection = InvoiceEventDetailCollection;
					if(InvoiceEventDetailCollection != null)
						((IInformationObject) InvoiceEventDetailCollection).SetInstanceTreeValuesAsUnmodified();

				
				}




				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "GroupName":
							GroupName = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string GroupName { get; set; }
			private string _unmodified_GroupName;
			[DataMember]
			public InvoiceEventDetailCollection InvoiceEventDetailCollection { get; set; }
			private InvoiceEventDetailCollection _unmodified_InvoiceEventDetailCollection;
			
			}
			[DataContract]
			public partial class InvoiceEventDetailCollection : IInformationObject , IInformationCollection
			{
				public InvoiceEventDetailCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceEventDetailCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceEventDetailCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(InvoiceEventDetailCollection), null, owner);
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.CORE", "InvoiceEventDetailCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static InvoiceEventDetailCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveInvoiceEventDetailCollection(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceEventDetailCollection");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(InvoiceEventDetailCollection), null, owner);
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


                public static InvoiceEventDetailCollection RetrieveInvoiceEventDetailCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (InvoiceEventDetailCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(InvoiceEventDetailCollection), null, owner);
                    return result;
                }

				public static InvoiceEventDetailCollection RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = InvoiceEventDetailCollection.RetrieveInvoiceEventDetailCollection("Content/TheBall.CORE/InvoiceEventDetailCollection/" + contentName, containerOwner);
					var result = InvoiceEventDetailCollection.RetrieveInvoiceEventDetailCollection("TheBall.CORE/InvoiceEventDetailCollection/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.CORE/InvoiceEventDetailCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.CORE/InvoiceEventDetailCollection/" + contentName);
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

				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
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
					CopyContentFrom((InvoiceEventDetailCollection) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceEventDetailCollection));
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

				public static InvoiceEventDetailCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceEventDetailCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceEventDetailCollection) serializer.ReadObject(xmlReader);
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
					return Path.Combine("TheBall.CORE", "InvoiceEventDetailCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceEventDetailCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceEventDetailCollection customDemoObject);


				
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
						return false;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					throw new NotSupportedException("Master collection location only supported for master collections");
					
				}

				IInformationCollection IInformationCollection.GetMasterInstance()
				{
					throw new NotSupportedException("Master collection instance only supported for master collections");
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = InvoiceEventDetail.GetRelativeLocationFromID("dummy");
					string nonOwnerDirectoryLocation = SubscribeSupport.GetParentDirectoryTarget(dummyItemLocation);
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetBlobOwnerAddress(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public void RefreshContent()
				{
				}


				public void SubscribeToContentSource()
				{
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

				
		
				public static InvoiceEventDetailCollection CreateDefault()
				{
					var result = new InvoiceEventDetailCollection();
					return result;
				}

				public static InvoiceEventDetailCollection CreateDemoDefault()
				{
					InvoiceEventDetailCollection customDemo = null;
					InvoiceEventDetailCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceEventDetailCollection();
					result.CollectionContent.Add(InvoiceEventDetail.CreateDemoDefault());
					//result.CollectionContent.Add(InvoiceEventDetail.CreateDemoDefault());
					//result.CollectionContent.Add(InvoiceEventDetail.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<InvoiceEventDetail> CollectionContent = new List<InvoiceEventDetail>();
				private InvoiceEventDetail[] _unmodified_CollectionContent;

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

				public InvoiceEventDetail[] GetIDSelectedArray()
				{
					if (IsCollectionFiltered == false || this.OrderFilterIDList == null)
						return CollectionContent.ToArray();
					return
						this.OrderFilterIDList.Select(id => CollectionContent.FirstOrDefault(item => item.ID == id)).ToArray();
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
							CollectionContent[i] = (InvoiceEventDetail )replacingObject;
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

				private void CopyContentFrom(InvoiceEventDetailCollection sourceObject)
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
			public partial class InvoiceRowCollection : IInformationObject , IInformationCollection
			{
				public InvoiceRowCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceRowCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceRowCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(InvoiceRowCollection), null, owner);
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.CORE", "InvoiceRowCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static InvoiceRowCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveInvoiceRowCollection(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceRowCollection");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(InvoiceRowCollection), null, owner);
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


                public static InvoiceRowCollection RetrieveInvoiceRowCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (InvoiceRowCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(InvoiceRowCollection), null, owner);
                    return result;
                }

				public static InvoiceRowCollection RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = InvoiceRowCollection.RetrieveInvoiceRowCollection("Content/TheBall.CORE/InvoiceRowCollection/" + contentName, containerOwner);
					var result = InvoiceRowCollection.RetrieveInvoiceRowCollection("TheBall.CORE/InvoiceRowCollection/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.CORE/InvoiceRowCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.CORE/InvoiceRowCollection/" + contentName);
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

				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
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
					CopyContentFrom((InvoiceRowCollection) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceRowCollection));
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

				public static InvoiceRowCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceRowCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceRowCollection) serializer.ReadObject(xmlReader);
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
					return Path.Combine("TheBall.CORE", "InvoiceRowCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceRowCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceRowCollection customDemoObject);


				
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
						return false;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					throw new NotSupportedException("Master collection location only supported for master collections");
					
				}

				IInformationCollection IInformationCollection.GetMasterInstance()
				{
					throw new NotSupportedException("Master collection instance only supported for master collections");
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = InvoiceRow.GetRelativeLocationFromID("dummy");
					string nonOwnerDirectoryLocation = SubscribeSupport.GetParentDirectoryTarget(dummyItemLocation);
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetBlobOwnerAddress(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public void RefreshContent()
				{
				}


				public void SubscribeToContentSource()
				{
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

				
		
				public static InvoiceRowCollection CreateDefault()
				{
					var result = new InvoiceRowCollection();
					return result;
				}

				public static InvoiceRowCollection CreateDemoDefault()
				{
					InvoiceRowCollection customDemo = null;
					InvoiceRowCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceRowCollection();
					result.CollectionContent.Add(InvoiceRow.CreateDemoDefault());
					//result.CollectionContent.Add(InvoiceRow.CreateDemoDefault());
					//result.CollectionContent.Add(InvoiceRow.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<InvoiceRow> CollectionContent = new List<InvoiceRow>();
				private InvoiceRow[] _unmodified_CollectionContent;

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

				public InvoiceRow[] GetIDSelectedArray()
				{
					if (IsCollectionFiltered == false || this.OrderFilterIDList == null)
						return CollectionContent.ToArray();
					return
						this.OrderFilterIDList.Select(id => CollectionContent.FirstOrDefault(item => item.ID == id)).ToArray();
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
							CollectionContent[i] = (InvoiceRow )replacingObject;
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

				private void CopyContentFrom(InvoiceRowCollection sourceObject)
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
			public partial class InvoiceEventDetail : IInformationObject 
			{
				public InvoiceEventDetail()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceEventDetail";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceEventDetail/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(InvoiceEventDetail), null, owner);
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.CORE", "InvoiceEventDetail", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static InvoiceEventDetail RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveInvoiceEventDetail(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceEventDetail");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(InvoiceEventDetail), null, owner);
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


                public static InvoiceEventDetail RetrieveInvoiceEventDetail(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (InvoiceEventDetail) StorageSupport.RetrieveInformation(relativeLocation, typeof(InvoiceEventDetail), null, owner);
                    return result;
                }

				public static InvoiceEventDetail RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = InvoiceEventDetail.RetrieveInvoiceEventDetail("Content/TheBall.CORE/InvoiceEventDetail/" + contentName, containerOwner);
					var result = InvoiceEventDetail.RetrieveInvoiceEventDetail("TheBall.CORE/InvoiceEventDetail/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.CORE/InvoiceEventDetail/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.CORE/InvoiceEventDetail/" + contentName);
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

				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
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
					CopyContentFrom((InvoiceEventDetail) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceEventDetail));
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

				public static InvoiceEventDetail DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceEventDetail));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceEventDetail) serializer.ReadObject(xmlReader);
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
					return Path.Combine("TheBall.CORE", "InvoiceEventDetail", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceEventDetail", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceEventDetail customDemoObject);



				public static InvoiceEventDetail CreateDefault()
				{
					var result = new InvoiceEventDetail();
					return result;
				}

				public static InvoiceEventDetail CreateDemoDefault()
				{
					InvoiceEventDetail customDemo = null;
					InvoiceEventDetail.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceEventDetail();
					result.IndentMode = @"InvoiceEventDetail.IndentMode";

					result.ReceivingParty = @"InvoiceEventDetail.ReceivingParty";

					result.AmountOfUnits = @"InvoiceEventDetail.AmountOfUnits";

					result.Duration = @"InvoiceEventDetail.Duration";

					result.UnitPrice = @"InvoiceEventDetail.UnitPrice";

					result.PriceWithoutTaxes = @"InvoiceEventDetail.PriceWithoutTaxes";

					result.Taxes = @"InvoiceEventDetail.Taxes";

					result.PriceWithTaxes = @"InvoiceEventDetail.PriceWithTaxes";

				
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
						if(IndentMode != _unmodified_IndentMode)
							return true;
						if(EventStartDateTime != _unmodified_EventStartDateTime)
							return true;
						if(EventEndDateTime != _unmodified_EventEndDateTime)
							return true;
						if(ReceivingParty != _unmodified_ReceivingParty)
							return true;
						if(AmountOfUnits != _unmodified_AmountOfUnits)
							return true;
						if(Duration != _unmodified_Duration)
							return true;
						if(UnitPrice != _unmodified_UnitPrice)
							return true;
						if(PriceWithoutTaxes != _unmodified_PriceWithoutTaxes)
							return true;
						if(Taxes != _unmodified_Taxes)
							return true;
						if(PriceWithTaxes != _unmodified_PriceWithTaxes)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(InvoiceEventDetail sourceObject)
				{
					IndentMode = sourceObject.IndentMode;
					EventStartDateTime = sourceObject.EventStartDateTime;
					EventEndDateTime = sourceObject.EventEndDateTime;
					ReceivingParty = sourceObject.ReceivingParty;
					AmountOfUnits = sourceObject.AmountOfUnits;
					Duration = sourceObject.Duration;
					UnitPrice = sourceObject.UnitPrice;
					PriceWithoutTaxes = sourceObject.PriceWithoutTaxes;
					Taxes = sourceObject.Taxes;
					PriceWithTaxes = sourceObject.PriceWithTaxes;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_IndentMode = IndentMode;
					_unmodified_EventStartDateTime = EventStartDateTime;
					_unmodified_EventEndDateTime = EventEndDateTime;
					_unmodified_ReceivingParty = ReceivingParty;
					_unmodified_AmountOfUnits = AmountOfUnits;
					_unmodified_Duration = Duration;
					_unmodified_UnitPrice = UnitPrice;
					_unmodified_PriceWithoutTaxes = PriceWithoutTaxes;
					_unmodified_Taxes = Taxes;
					_unmodified_PriceWithTaxes = PriceWithTaxes;
				
				
				}




				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "IndentMode":
							IndentMode = value;
							break;
						case "EventStartDateTime":
							EventStartDateTime = DateTime.Parse(value);
							break;
						case "EventEndDateTime":
							EventEndDateTime = DateTime.Parse(value);
							break;
						case "ReceivingParty":
							ReceivingParty = value;
							break;
						case "AmountOfUnits":
							AmountOfUnits = value;
							break;
						case "Duration":
							Duration = value;
							break;
						case "UnitPrice":
							UnitPrice = value;
							break;
						case "PriceWithoutTaxes":
							PriceWithoutTaxes = value;
							break;
						case "Taxes":
							Taxes = value;
							break;
						case "PriceWithTaxes":
							PriceWithTaxes = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string IndentMode { get; set; }
			private string _unmodified_IndentMode;
			[DataMember]
			public DateTime EventStartDateTime { get; set; }
			private DateTime _unmodified_EventStartDateTime;
			[DataMember]
			public DateTime EventEndDateTime { get; set; }
			private DateTime _unmodified_EventEndDateTime;
			[DataMember]
			public string ReceivingParty { get; set; }
			private string _unmodified_ReceivingParty;
			[DataMember]
			public string AmountOfUnits { get; set; }
			private string _unmodified_AmountOfUnits;
			[DataMember]
			public string Duration { get; set; }
			private string _unmodified_Duration;
			[DataMember]
			public string UnitPrice { get; set; }
			private string _unmodified_UnitPrice;
			[DataMember]
			public string PriceWithoutTaxes { get; set; }
			private string _unmodified_PriceWithoutTaxes;
			[DataMember]
			public string Taxes { get; set; }
			private string _unmodified_Taxes;
			[DataMember]
			public string PriceWithTaxes { get; set; }
			private string _unmodified_PriceWithTaxes;
			
			}
			[DataContract]
			public partial class InvoiceRow : IInformationObject 
			{
				public InvoiceRow()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceRow";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceRow/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(InvoiceRow), null, owner);
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.CORE", "InvoiceRow", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static InvoiceRow RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveInvoiceRow(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceRow");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(InvoiceRow), null, owner);
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


                public static InvoiceRow RetrieveInvoiceRow(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (InvoiceRow) StorageSupport.RetrieveInformation(relativeLocation, typeof(InvoiceRow), null, owner);
                    return result;
                }

				public static InvoiceRow RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = InvoiceRow.RetrieveInvoiceRow("Content/TheBall.CORE/InvoiceRow/" + contentName, containerOwner);
					var result = InvoiceRow.RetrieveInvoiceRow("TheBall.CORE/InvoiceRow/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.CORE/InvoiceRow/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.CORE/InvoiceRow/" + contentName);
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

				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
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
					CopyContentFrom((InvoiceRow) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceRow));
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

				public static InvoiceRow DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceRow));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceRow) serializer.ReadObject(xmlReader);
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
					return Path.Combine("TheBall.CORE", "InvoiceRow", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceRow", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceRow customDemoObject);



				public static InvoiceRow CreateDefault()
				{
					var result = new InvoiceRow();
					result.ReferenceToInformation = ReferenceToInformation.CreateDefault();
					return result;
				}

				public static InvoiceRow CreateDemoDefault()
				{
					InvoiceRow customDemo = null;
					InvoiceRow.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceRow();
					result.ReferenceToInformation = ReferenceToInformation.CreateDemoDefault();
					result.IndentMode = @"InvoiceRow.IndentMode";

					result.AmountOfUnits = @"InvoiceRow.AmountOfUnits";

					result.Duration = @"InvoiceRow.Duration";

					result.UnitPrice = @"InvoiceRow.UnitPrice";

					result.PriceWithoutTaxes = @"InvoiceRow.PriceWithoutTaxes";

					result.Taxes = @"InvoiceRow.Taxes";

					result.PriceWithTaxes = @"InvoiceRow.PriceWithTaxes";

				
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
						{
							IInformationObject item = ReferenceToInformation;
							if(item != null)
							{
								item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
							}
						}
					}					
				}


				private object FindFromObjectTree(string objectId)
				{
					{
						var item = ReferenceToInformation;
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
						var item = (IInformationObject) ReferenceToInformation;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get {
						if(ReferenceToInformation != _unmodified_ReferenceToInformation)
							return true;
						if(IndentMode != _unmodified_IndentMode)
							return true;
						if(AmountOfUnits != _unmodified_AmountOfUnits)
							return true;
						if(Duration != _unmodified_Duration)
							return true;
						if(UnitPrice != _unmodified_UnitPrice)
							return true;
						if(PriceWithoutTaxes != _unmodified_PriceWithoutTaxes)
							return true;
						if(Taxes != _unmodified_Taxes)
							return true;
						if(PriceWithTaxes != _unmodified_PriceWithTaxes)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					if(ReferenceToInformation != null) {
						if(ReferenceToInformation.ID == replacingObject.ID)
							ReferenceToInformation = (ReferenceToInformation) replacingObject;
						else {
							IInformationObject iObject = ReferenceToInformation;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(InvoiceRow sourceObject)
				{
					ReferenceToInformation = sourceObject.ReferenceToInformation;
					IndentMode = sourceObject.IndentMode;
					AmountOfUnits = sourceObject.AmountOfUnits;
					Duration = sourceObject.Duration;
					UnitPrice = sourceObject.UnitPrice;
					PriceWithoutTaxes = sourceObject.PriceWithoutTaxes;
					Taxes = sourceObject.Taxes;
					PriceWithTaxes = sourceObject.PriceWithTaxes;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_IndentMode = IndentMode;
					_unmodified_AmountOfUnits = AmountOfUnits;
					_unmodified_Duration = Duration;
					_unmodified_UnitPrice = UnitPrice;
					_unmodified_PriceWithoutTaxes = PriceWithoutTaxes;
					_unmodified_Taxes = Taxes;
					_unmodified_PriceWithTaxes = PriceWithTaxes;
				
					_unmodified_ReferenceToInformation = ReferenceToInformation;
					if(ReferenceToInformation != null)
						((IInformationObject) ReferenceToInformation).SetInstanceTreeValuesAsUnmodified();

				
				}




				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "IndentMode":
							IndentMode = value;
							break;
						case "AmountOfUnits":
							AmountOfUnits = value;
							break;
						case "Duration":
							Duration = value;
							break;
						case "UnitPrice":
							UnitPrice = value;
							break;
						case "PriceWithoutTaxes":
							PriceWithoutTaxes = value;
							break;
						case "Taxes":
							Taxes = value;
							break;
						case "PriceWithTaxes":
							PriceWithTaxes = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public ReferenceToInformation ReferenceToInformation { get; set; }
			private ReferenceToInformation _unmodified_ReferenceToInformation;
			[DataMember]
			public string IndentMode { get; set; }
			private string _unmodified_IndentMode;
			[DataMember]
			public string AmountOfUnits { get; set; }
			private string _unmodified_AmountOfUnits;
			[DataMember]
			public string Duration { get; set; }
			private string _unmodified_Duration;
			[DataMember]
			public string UnitPrice { get; set; }
			private string _unmodified_UnitPrice;
			[DataMember]
			public string PriceWithoutTaxes { get; set; }
			private string _unmodified_PriceWithoutTaxes;
			[DataMember]
			public string Taxes { get; set; }
			private string _unmodified_Taxes;
			[DataMember]
			public string PriceWithTaxes { get; set; }
			private string _unmodified_PriceWithTaxes;
			
			}
			[DataContract]
			public partial class CategoryCollection : IInformationObject , IInformationCollection
			{
				public CategoryCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "CategoryCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/CategoryCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(CategoryCollection), null, owner);
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.CORE", "CategoryCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static CategoryCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveCategoryCollection(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: CategoryCollection");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(CategoryCollection), null, owner);
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


                public static CategoryCollection RetrieveCategoryCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (CategoryCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(CategoryCollection), null, owner);
                    return result;
                }

				public static CategoryCollection RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = CategoryCollection.RetrieveCategoryCollection("Content/TheBall.CORE/CategoryCollection/" + contentName, containerOwner);
					var result = CategoryCollection.RetrieveCategoryCollection("TheBall.CORE/CategoryCollection/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.CORE/CategoryCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.CORE/CategoryCollection/" + contentName);
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

				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
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
					CopyContentFrom((CategoryCollection) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(CategoryCollection));
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

				public static CategoryCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CategoryCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (CategoryCollection) serializer.ReadObject(xmlReader);
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
					return Path.Combine("TheBall.CORE", "CategoryCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "CategoryCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref CategoryCollection customDemoObject);


				
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
						return false;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					throw new NotSupportedException("Master collection location only supported for master collections");
					
				}

				IInformationCollection IInformationCollection.GetMasterInstance()
				{
					throw new NotSupportedException("Master collection instance only supported for master collections");
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = Category.GetRelativeLocationFromID("dummy");
					string nonOwnerDirectoryLocation = SubscribeSupport.GetParentDirectoryTarget(dummyItemLocation);
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetBlobOwnerAddress(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public void RefreshContent()
				{
				}


				public void SubscribeToContentSource()
				{
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

				
		
				public static CategoryCollection CreateDefault()
				{
					var result = new CategoryCollection();
					return result;
				}

				public static CategoryCollection CreateDemoDefault()
				{
					CategoryCollection customDemo = null;
					CategoryCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new CategoryCollection();
					result.CollectionContent.Add(Category.CreateDemoDefault());
					//result.CollectionContent.Add(Category.CreateDemoDefault());
					//result.CollectionContent.Add(Category.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<Category> CollectionContent = new List<Category>();
				private Category[] _unmodified_CollectionContent;

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

				public Category[] GetIDSelectedArray()
				{
					if (IsCollectionFiltered == false || this.OrderFilterIDList == null)
						return CollectionContent.ToArray();
					return
						this.OrderFilterIDList.Select(id => CollectionContent.FirstOrDefault(item => item.ID == id)).ToArray();
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
							CollectionContent[i] = (Category )replacingObject;
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

				private void CopyContentFrom(CategoryCollection sourceObject)
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
			public partial class Category : IInformationObject 
			{
				public Category()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "Category";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/Category/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(Category), null, owner);
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.CORE", "Category", id).Replace("\\", "/");
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
					// var result = Category.RetrieveCategory("Content/TheBall.CORE/Category/" + contentName, containerOwner);
					var result = Category.RetrieveCategory("TheBall.CORE/Category/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.CORE/Category/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.CORE/Category/" + contentName);
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

				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "Category", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "Category", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref Category customDemoObject);



				public static Category CreateDefault()
				{
					var result = new Category();
					return result;
				}

				public static Category CreateDemoDefault()
				{
					Category customDemo = null;
					Category.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new Category();
					result.CategoryName = @"Category.CategoryName";

				
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
						if(CategoryName != _unmodified_CategoryName)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(Category sourceObject)
				{
					CategoryName = sourceObject.CategoryName;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CategoryName = CategoryName;
				
				
				}




				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "CategoryName":
							CategoryName = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string CategoryName { get; set; }
			private string _unmodified_CategoryName;
			
			}
			[DataContract]
			public partial class Process : IInformationObject 
			{
				public Process()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "Process";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/Process/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(Process), null, owner);
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.CORE", "Process", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Process RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveProcess(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: Process");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(Process), null, owner);
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


                public static Process RetrieveProcess(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Process) StorageSupport.RetrieveInformation(relativeLocation, typeof(Process), null, owner);
                    return result;
                }

				public static Process RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = Process.RetrieveProcess("Content/TheBall.CORE/Process/" + contentName, containerOwner);
					var result = Process.RetrieveProcess("TheBall.CORE/Process/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.CORE/Process/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.CORE/Process/" + contentName);
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

				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
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
					CopyContentFrom((Process) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(Process));
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

				public static Process DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Process));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Process) serializer.ReadObject(xmlReader);
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
					return Path.Combine("TheBall.CORE", "Process", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "Process", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref Process customDemoObject);



				public static Process CreateDefault()
				{
					var result = new Process();
					result.CategoryCollection = CategoryCollection.CreateDefault();
					return result;
				}

				public static Process CreateDemoDefault()
				{
					Process customDemo = null;
					Process.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new Process();
					result.ProcessID = @"Process.ProcessID";

					result.ProcessName = @"Process.ProcessName";

					result.CategoryCollection = CategoryCollection.CreateDemoDefault();
				
					return result;
				}


				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(CategoryCollection != null) {
						((IInformationObject) CategoryCollection).UpdateCollections(masterInstance);
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
						IInformationObject item = CategoryCollection;
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
						var item = CategoryCollection;
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
						var item = (IInformationObject) CategoryCollection;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get {
						if(ProcessID != _unmodified_ProcessID)
							return true;
						if(ProcessName != _unmodified_ProcessName)
							return true;
						if(CategoryCollection != _unmodified_CategoryCollection)
							return true;
						{
							IInformationObject item = (IInformationObject) CategoryCollection;
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
					if(CategoryCollection != null) {
						if(CategoryCollection.ID == replacingObject.ID)
							CategoryCollection = (CategoryCollection) replacingObject;
						else {
							IInformationObject iObject = CategoryCollection;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(Process sourceObject)
				{
					ProcessID = sourceObject.ProcessID;
					ProcessName = sourceObject.ProcessName;
					CategoryCollection = sourceObject.CategoryCollection;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_ProcessID = ProcessID;
					_unmodified_ProcessName = ProcessName;
				
					_unmodified_CategoryCollection = CategoryCollection;
					if(CategoryCollection != null)
						((IInformationObject) CategoryCollection).SetInstanceTreeValuesAsUnmodified();

				
				}




				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "ProcessID":
							ProcessID = value;
							break;
						case "ProcessName":
							ProcessName = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string ProcessID { get; set; }
			private string _unmodified_ProcessID;
			[DataMember]
			public string ProcessName { get; set; }
			private string _unmodified_ProcessName;
			[DataMember]
			public CategoryCollection CategoryCollection { get; set; }
			private CategoryCollection _unmodified_CategoryCollection;
			
			}
			[DataContract]
			public partial class ReferenceToInformation : IInformationObject 
			{
				public ReferenceToInformation()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "ReferenceToInformation";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/ReferenceToInformation/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(ReferenceToInformation), null, owner);
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.CORE", "ReferenceToInformation", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static ReferenceToInformation RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveReferenceToInformation(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: ReferenceToInformation");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(ReferenceToInformation), null, owner);
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


                public static ReferenceToInformation RetrieveReferenceToInformation(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ReferenceToInformation) StorageSupport.RetrieveInformation(relativeLocation, typeof(ReferenceToInformation), null, owner);
                    return result;
                }

				public static ReferenceToInformation RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = ReferenceToInformation.RetrieveReferenceToInformation("Content/TheBall.CORE/ReferenceToInformation/" + contentName, containerOwner);
					var result = ReferenceToInformation.RetrieveReferenceToInformation("TheBall.CORE/ReferenceToInformation/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.CORE/ReferenceToInformation/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.CORE/ReferenceToInformation/" + contentName);
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

				bool IInformationObject.IsIndependentMaster { 
					get {
						return true;
					}
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
					CopyContentFrom((ReferenceToInformation) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(ReferenceToInformation));
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

				public static ReferenceToInformation DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ReferenceToInformation));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ReferenceToInformation) serializer.ReadObject(xmlReader);
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
					return Path.Combine("TheBall.CORE", "ReferenceToInformation", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "ReferenceToInformation", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref ReferenceToInformation customDemoObject);



				public static ReferenceToInformation CreateDefault()
				{
					var result = new ReferenceToInformation();
					return result;
				}

				public static ReferenceToInformation CreateDemoDefault()
				{
					ReferenceToInformation customDemo = null;
					ReferenceToInformation.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new ReferenceToInformation();
					result.Title = @"ReferenceToInformation.Title";

					result.URL = @"ReferenceToInformation.URL";

				
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
						if(Title != _unmodified_Title)
							return true;
						if(URL != _unmodified_URL)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(ReferenceToInformation sourceObject)
				{
					Title = sourceObject.Title;
					URL = sourceObject.URL;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_Title = Title;
					_unmodified_URL = URL;
				
				
				}




				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Title":
							Title = value;
							break;
						case "URL":
							URL = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Title { get; set; }
			private string _unmodified_Title;
			[DataMember]
			public string URL { get; set; }
			private string _unmodified_URL;
			
			}
			[DataContract]
			public partial class ReferenceCollection : IInformationObject , IInformationCollection
			{
				public ReferenceCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "ReferenceCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/ReferenceCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(ReferenceCollection), null, owner);
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("TheBall.CORE", "ReferenceCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static ReferenceCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveReferenceCollection(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: ReferenceCollection");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(ReferenceCollection), null, owner);
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


                public static ReferenceCollection RetrieveReferenceCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ReferenceCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(ReferenceCollection), null, owner);
                    return result;
                }

				public static ReferenceCollection RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = ReferenceCollection.RetrieveReferenceCollection("Content/TheBall.CORE/ReferenceCollection/" + contentName, containerOwner);
					var result = ReferenceCollection.RetrieveReferenceCollection("TheBall.CORE/ReferenceCollection/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "Content/TheBall.CORE/ReferenceCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetBlobOwnerAddress(containerOwner, "TheBall.CORE/ReferenceCollection/" + contentName);
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

				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
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
					CopyContentFrom((ReferenceCollection) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(ReferenceCollection));
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

				public static ReferenceCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ReferenceCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ReferenceCollection) serializer.ReadObject(xmlReader);
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
					return Path.Combine("TheBall.CORE", "ReferenceCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "ReferenceCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref ReferenceCollection customDemoObject);


				
				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
				}



				bool IInformationCollection.IsMasterCollection {
					get {
						return false;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					throw new NotSupportedException("Master collection location only supported for master collections");
					
				}

				IInformationCollection IInformationCollection.GetMasterInstance()
				{
					throw new NotSupportedException("Master collection instance only supported for master collections");
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = ReferenceToInformation.GetRelativeLocationFromID("dummy");
					string nonOwnerDirectoryLocation = SubscribeSupport.GetParentDirectoryTarget(dummyItemLocation);
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetBlobOwnerAddress(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public void RefreshContent()
				{
				}


				public void SubscribeToContentSource()
				{
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

				
		
				public static ReferenceCollection CreateDefault()
				{
					var result = new ReferenceCollection();
					return result;
				}

				public static ReferenceCollection CreateDemoDefault()
				{
					ReferenceCollection customDemo = null;
					ReferenceCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new ReferenceCollection();
					result.CollectionContent.Add(ReferenceToInformation.CreateDemoDefault());
					//result.CollectionContent.Add(ReferenceToInformation.CreateDemoDefault());
					//result.CollectionContent.Add(ReferenceToInformation.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<ReferenceToInformation> CollectionContent = new List<ReferenceToInformation>();
				private ReferenceToInformation[] _unmodified_CollectionContent;

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

				public ReferenceToInformation[] GetIDSelectedArray()
				{
					if (IsCollectionFiltered == false || this.OrderFilterIDList == null)
						return CollectionContent.ToArray();
					return
						this.OrderFilterIDList.Select(id => CollectionContent.FirstOrDefault(item => item.ID == id)).ToArray();
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
							CollectionContent[i] = (ReferenceToInformation )replacingObject;
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

				private void CopyContentFrom(ReferenceCollection sourceObject)
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
					if(searchWithinCurrentMasterOnly == false) {
						foreach(IInformationObject iObject in CollectionContent)
							iObject.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
					}
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
 } 