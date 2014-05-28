 


using DOM=Caloom.CORE;

namespace TheBall.CORE {
	public static partial class OwnerInitializer
	{
		private static void DOMAININIT_Caloom_CORE(IContainerOwner owner)
		{
			DOM.DomainInformationSupport.EnsureMasterCollections(owner);
			DOM.DomainInformationSupport.RefreshMasterCollections(owner);
		}
	}
}


namespace Caloom.CORE { 
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



namespace INT { 
		 } 		public static class DomainInformationSupport
		{
            public static void EnsureMasterCollections(IContainerOwner owner)
            {
                {
                    var masterCollection = ProductUsageCollection.GetMasterCollectionInstance(owner);
                    if(masterCollection == null)
                    {
                        masterCollection = ProductUsageCollection.CreateDefault();
                        masterCollection.RelativeLocation =
                            ProductUsageCollection.GetMasterCollectionLocation(owner);
                        StorageSupport.StoreInformation(masterCollection, owner);
                    }
					IInformationCollection collection = masterCollection;
					collection.SubscribeToContentSource();
                }
                {
                    var masterCollection = ProductCollection.GetMasterCollectionInstance(owner);
                    if(masterCollection == null)
                    {
                        masterCollection = ProductCollection.CreateDefault();
                        masterCollection.RelativeLocation =
                            ProductCollection.GetMasterCollectionLocation(owner);
                        StorageSupport.StoreInformation(masterCollection, owner);
                    }
					IInformationCollection collection = masterCollection;
					collection.SubscribeToContentSource();
                }
            }

            public static void RefreshMasterCollections(IContainerOwner owner)
            {
                {
                    IInformationCollection masterCollection = ProductUsageCollection.GetMasterCollectionInstance(owner);
                    if (masterCollection == null)
                        throw new InvalidDataException("Master collection ProductUsageCollection missing for owner");
                    masterCollection.RefreshContent();
                    StorageSupport.StoreInformation((IInformationObject) masterCollection, owner);
                }
                {
                    IInformationCollection masterCollection = ProductCollection.GetMasterCollectionInstance(owner);
                    if (masterCollection == null)
                        throw new InvalidDataException("Master collection ProductCollection missing for owner");
                    masterCollection.RefreshContent();
                    StorageSupport.StoreInformation((IInformationObject) masterCollection, owner);
                }
            }
		}
			[DataContract]
			[Serializable]
			public partial class Who : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public Who()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "Caloom.CORE";
				    this.Name = "Who";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "Caloom.CORE/Who/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(Who), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("Caloom.CORE", "Who", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Who RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveWho(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: Who");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(Who), null, owner);
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


                public static Who RetrieveWho(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Who) StorageSupport.RetrieveInformation(relativeLocation, typeof(Who), null, owner);
                    return result;
                }

				public static Who RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = Who.RetrieveWho("Content/Caloom.CORE/Who/" + contentName, containerOwner);
					var result = Who.RetrieveWho("Caloom.CORE/Who/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/Caloom.CORE/Who/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Caloom.CORE/Who/" + contentName);
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
					CopyContentFrom((Who) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(Who));
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

				public static Who DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Who));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Who) serializer.ReadObject(xmlReader);
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
					return Path.Combine("Caloom.CORE", "Who", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "Caloom.CORE", "Who", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref Who customDemoObject);



				public static Who CreateDefault()
				{
					var result = new Who();
					return result;
				}
				/*
				public static Who CreateDemoDefault()
				{
					Who customDemo = null;
					Who.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new Who();
					result.ImageBaseUrl = @"Who.ImageBaseUrl";

					result.Title = @"Who.Title";

					result.Excerpt = @"Who.Excerpt
Who.Excerpt
Who.Excerpt
Who.Excerpt
Who.Excerpt
";

					result.Description = @"Who.Description
Who.Description
Who.Description
Who.Description
Who.Description
";

				
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

						if(ImageBaseUrl != _unmodified_ImageBaseUrl)
							return true;
						if(Title != _unmodified_Title)
							return true;
						if(Excerpt != _unmodified_Excerpt)
							return true;
						if(Description != _unmodified_Description)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(Who sourceObject)
				{
					ImageBaseUrl = sourceObject.ImageBaseUrl;
					Title = sourceObject.Title;
					Excerpt = sourceObject.Excerpt;
					Description = sourceObject.Description;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_ImageBaseUrl = ImageBaseUrl;
					_unmodified_Title = Title;
					_unmodified_Excerpt = Excerpt;
					_unmodified_Description = Description;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "ImageBaseUrl":
							ImageBaseUrl = value;
							break;
						case "Title":
							Title = value;
							break;
						case "Excerpt":
							Excerpt = value;
							break;
						case "Description":
							Description = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string ImageBaseUrl { get; set; }
			private string _unmodified_ImageBaseUrl;
			[DataMember]
			public string Title { get; set; }
			private string _unmodified_Title;
			[DataMember]
			public string Excerpt { get; set; }
			private string _unmodified_Excerpt;
			[DataMember]
			public string Description { get; set; }
			private string _unmodified_Description;
			
			}
			[DataContract]
			[Serializable]
			public partial class ProductForWhom : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public ProductForWhom()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "Caloom.CORE";
				    this.Name = "ProductForWhom";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "Caloom.CORE/ProductForWhom/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(ProductForWhom), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("Caloom.CORE", "ProductForWhom", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static ProductForWhom RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveProductForWhom(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: ProductForWhom");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(ProductForWhom), null, owner);
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


                public static ProductForWhom RetrieveProductForWhom(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ProductForWhom) StorageSupport.RetrieveInformation(relativeLocation, typeof(ProductForWhom), null, owner);
                    return result;
                }

				public static ProductForWhom RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = ProductForWhom.RetrieveProductForWhom("Content/Caloom.CORE/ProductForWhom/" + contentName, containerOwner);
					var result = ProductForWhom.RetrieveProductForWhom("Caloom.CORE/ProductForWhom/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/Caloom.CORE/ProductForWhom/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Caloom.CORE/ProductForWhom/" + contentName);
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
					CopyContentFrom((ProductForWhom) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(ProductForWhom));
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

				public static ProductForWhom DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ProductForWhom));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ProductForWhom) serializer.ReadObject(xmlReader);
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
					return Path.Combine("Caloom.CORE", "ProductForWhom", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "Caloom.CORE", "ProductForWhom", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref ProductForWhom customDemoObject);



				public static ProductForWhom CreateDefault()
				{
					var result = new ProductForWhom();
					result.Product = Product.CreateDefault();
					result.Who = Who.CreateDefault();
					return result;
				}
				/*
				public static ProductForWhom CreateDemoDefault()
				{
					ProductForWhom customDemo = null;
					ProductForWhom.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new ProductForWhom();
					result.ImageBaseUrl = @"ProductForWhom.ImageBaseUrl";

					result.Title = @"ProductForWhom.Title";

					result.Excerpt = @"ProductForWhom.Excerpt
ProductForWhom.Excerpt
ProductForWhom.Excerpt
ProductForWhom.Excerpt
ProductForWhom.Excerpt
";

					result.Description = @"ProductForWhom.Description
ProductForWhom.Description
ProductForWhom.Description
ProductForWhom.Description
ProductForWhom.Description
";

					result.Product = Product.CreateDemoDefault();
					result.Who = Who.CreateDemoDefault();
				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(Product != null) {
						((IInformationObject) Product).UpdateCollections(masterInstance);
					}

					if(Who != null) {
						((IInformationObject) Who).UpdateCollections(masterInstance);
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
						IInformationObject item = Product;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = Who;
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
						var item = Product;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = Who;
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
						var item = (IInformationObject) Product;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) Who;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(ImageBaseUrl != _unmodified_ImageBaseUrl)
							return true;
						if(Title != _unmodified_Title)
							return true;
						if(Excerpt != _unmodified_Excerpt)
							return true;
						if(Description != _unmodified_Description)
							return true;
						if(Product != _unmodified_Product)
							return true;
						if(Who != _unmodified_Who)
							return true;
						{
							IInformationObject item = (IInformationObject) Product;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) Who;
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
					if(Product != null) {
						if(Product.ID == replacingObject.ID)
							Product = (Product) replacingObject;
						else {
							IInformationObject iObject = Product;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(Who != null) {
						if(Who.ID == replacingObject.ID)
							Who = (Who) replacingObject;
						else {
							IInformationObject iObject = Who;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(ProductForWhom sourceObject)
				{
					ImageBaseUrl = sourceObject.ImageBaseUrl;
					Title = sourceObject.Title;
					Excerpt = sourceObject.Excerpt;
					Description = sourceObject.Description;
					Product = sourceObject.Product;
					Who = sourceObject.Who;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_ImageBaseUrl = ImageBaseUrl;
					_unmodified_Title = Title;
					_unmodified_Excerpt = Excerpt;
					_unmodified_Description = Description;
				
					_unmodified_Product = Product;
					if(Product != null)
						((IInformationObject) Product).SetInstanceTreeValuesAsUnmodified();

					_unmodified_Who = Who;
					if(Who != null)
						((IInformationObject) Who).SetInstanceTreeValuesAsUnmodified();

				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "ImageBaseUrl":
							ImageBaseUrl = value;
							break;
						case "Title":
							Title = value;
							break;
						case "Excerpt":
							Excerpt = value;
							break;
						case "Description":
							Description = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string ImageBaseUrl { get; set; }
			private string _unmodified_ImageBaseUrl;
			[DataMember]
			public string Title { get; set; }
			private string _unmodified_Title;
			[DataMember]
			public string Excerpt { get; set; }
			private string _unmodified_Excerpt;
			[DataMember]
			public string Description { get; set; }
			private string _unmodified_Description;
			[DataMember]
			public Product Product { get; set; }
			private Product _unmodified_Product;
			[DataMember]
			public Who Who { get; set; }
			private Who _unmodified_Who;
			
			}
			[DataContract]
			[Serializable]
			public partial class Product : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public Product()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "Caloom.CORE";
				    this.Name = "Product";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "Caloom.CORE/Product/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(Product), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("Caloom.CORE", "Product", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Product RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveProduct(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: Product");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(Product), null, owner);
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


                public static Product RetrieveProduct(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Product) StorageSupport.RetrieveInformation(relativeLocation, typeof(Product), null, owner);
                    return result;
                }

				public static Product RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = Product.RetrieveProduct("Content/Caloom.CORE/Product/" + contentName, containerOwner);
					var result = Product.RetrieveProduct("Caloom.CORE/Product/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/Caloom.CORE/Product/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Caloom.CORE/Product/" + contentName);
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
					CopyContentFrom((Product) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(Product));
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

				public static Product DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Product));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Product) serializer.ReadObject(xmlReader);
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
					return Path.Combine("Caloom.CORE", "Product", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "Caloom.CORE", "Product", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref Product customDemoObject);



				public static Product CreateDefault()
				{
					var result = new Product();
					result.SubProducts = ProductUsageCollection.CreateDefault();
					return result;
				}
				/*
				public static Product CreateDemoDefault()
				{
					Product customDemo = null;
					Product.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new Product();
					result.ImageBaseUrl = @"Product.ImageBaseUrl";

					result.Title = @"Product.Title";

					result.Excerpt = @"Product.Excerpt
Product.Excerpt
Product.Excerpt
Product.Excerpt
Product.Excerpt
";

					result.Description = @"Product.Description
Product.Description
Product.Description
Product.Description
Product.Description
";

					result.SubProducts = ProductUsageCollection.CreateDemoDefault();
				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(masterInstance is ProductUsageCollection) {
						Caloom.CORE.CollectionUpdateImplementation.Update_Product_SubProducts(this, localCollection:SubProducts, masterCollection:(ProductUsageCollection) masterInstance);
					} else if(SubProducts != null) {
						((IInformationObject) SubProducts).UpdateCollections(masterInstance);
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
						IInformationObject item = SubProducts;
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
						var item = SubProducts;
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
						var item = (IInformationObject) SubProducts;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(ImageBaseUrl != _unmodified_ImageBaseUrl)
							return true;
						if(Title != _unmodified_Title)
							return true;
						if(Excerpt != _unmodified_Excerpt)
							return true;
						if(Description != _unmodified_Description)
							return true;
						if(SubProducts != _unmodified_SubProducts)
							return true;
						{
							IInformationObject item = (IInformationObject) SubProducts;
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
					if(SubProducts != null) {
						if(SubProducts.ID == replacingObject.ID)
							SubProducts = (ProductUsageCollection) replacingObject;
						else {
							IInformationObject iObject = SubProducts;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(Product sourceObject)
				{
					ImageBaseUrl = sourceObject.ImageBaseUrl;
					Title = sourceObject.Title;
					Excerpt = sourceObject.Excerpt;
					Description = sourceObject.Description;
					SubProducts = sourceObject.SubProducts;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_ImageBaseUrl = ImageBaseUrl;
					_unmodified_Title = Title;
					_unmodified_Excerpt = Excerpt;
					_unmodified_Description = Description;
				
					_unmodified_SubProducts = SubProducts;
					if(SubProducts != null)
						((IInformationObject) SubProducts).SetInstanceTreeValuesAsUnmodified();

				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "ImageBaseUrl":
							ImageBaseUrl = value;
							break;
						case "Title":
							Title = value;
							break;
						case "Excerpt":
							Excerpt = value;
							break;
						case "Description":
							Description = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string ImageBaseUrl { get; set; }
			private string _unmodified_ImageBaseUrl;
			[DataMember]
			public string Title { get; set; }
			private string _unmodified_Title;
			[DataMember]
			public string Excerpt { get; set; }
			private string _unmodified_Excerpt;
			[DataMember]
			public string Description { get; set; }
			private string _unmodified_Description;
			[DataMember]
			public ProductUsageCollection SubProducts { get; set; }
			private ProductUsageCollection _unmodified_SubProducts;
			
			}
			[DataContract]
			[Serializable]
			public partial class ProductUsageCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public ProductUsageCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "Caloom.CORE";
				    this.Name = "ProductUsageCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "Caloom.CORE/ProductUsageCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(ProductUsageCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("Caloom.CORE", "ProductUsageCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static ProductUsageCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveProductUsageCollection(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: ProductUsageCollection");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(ProductUsageCollection), null, owner);
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


                public static ProductUsageCollection RetrieveProductUsageCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ProductUsageCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(ProductUsageCollection), null, owner);
                    return result;
                }

				public static ProductUsageCollection RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = ProductUsageCollection.RetrieveProductUsageCollection("Content/Caloom.CORE/ProductUsageCollection/" + contentName, containerOwner);
					var result = ProductUsageCollection.RetrieveProductUsageCollection("Caloom.CORE/ProductUsageCollection/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/Caloom.CORE/ProductUsageCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Caloom.CORE/ProductUsageCollection/" + contentName);
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
					CopyContentFrom((ProductUsageCollection) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(ProductUsageCollection));
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

				public static ProductUsageCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ProductUsageCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ProductUsageCollection) serializer.ReadObject(xmlReader);
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
					return Path.Combine("Caloom.CORE", "ProductUsageCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "Caloom.CORE", "ProductUsageCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref ProductUsageCollection customDemoObject);


				
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
					string dummyItemLocation = ProductUsage.GetRelativeLocationFromID("dummy");
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
																								 typeof(ProductUsage));
                    Array.ForEach(informationObjects, io => io.MasterETag = io.ETag);
					CollectionContent.Clear();
					CollectionContent.AddRange(informationObjects.Select(obj => (ProductUsage) obj));
            
				}

				public static ProductUsageCollection GetMasterCollectionInstance(IContainerOwner owner)
				{
					return ProductUsageCollection.RetrieveFromOwnerContent(owner, "MasterCollection");
				}

				public void SubscribeToContentSource()
				{
					// DirectoryToCollection
					string itemDirectory = GetItemDirectory();
					SubscribeSupport.AddSubscriptionToObject(itemDirectory, RelativeLocation,
															 SubscribeSupport.SubscribeType_DirectoryToCollection, null, typeof(ProductUsageCollection).FullName);
				}

				public static string GetMasterCollectionLocation(IContainerOwner owner)
				{
					return StorageSupport.GetOwnerContentLocation(owner, "Caloom.CORE/ProductUsageCollection/" + "MasterCollection");
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

				
		
				public static ProductUsageCollection CreateDefault()
				{
					var result = new ProductUsageCollection();
					return result;
				}

				/*
				public static ProductUsageCollection CreateDemoDefault()
				{
					ProductUsageCollection customDemo = null;
					ProductUsageCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new ProductUsageCollection();
					result.CollectionContent.Add(ProductUsage.CreateDemoDefault());
					//result.CollectionContent.Add(ProductUsage.CreateDemoDefault());
					//result.CollectionContent.Add(ProductUsage.CreateDemoDefault());
					return result;
				}
				*/

		
				[DataMember] public List<ProductUsage> CollectionContent = new List<ProductUsage>();
				private ProductUsage[] _unmodified_CollectionContent;

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

				public ProductUsage[] GetIDSelectedArray()
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
							CollectionContent[i] = (ProductUsage )replacingObject;
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

				private void CopyContentFrom(ProductUsageCollection sourceObject)
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
			public partial class ProductUsage : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public ProductUsage()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "Caloom.CORE";
				    this.Name = "ProductUsage";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "Caloom.CORE/ProductUsage/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(ProductUsage), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("Caloom.CORE", "ProductUsage", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static ProductUsage RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveProductUsage(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: ProductUsage");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(ProductUsage), null, owner);
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


                public static ProductUsage RetrieveProductUsage(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ProductUsage) StorageSupport.RetrieveInformation(relativeLocation, typeof(ProductUsage), null, owner);
                    return result;
                }

				public static ProductUsage RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = ProductUsage.RetrieveProductUsage("Content/Caloom.CORE/ProductUsage/" + contentName, containerOwner);
					var result = ProductUsage.RetrieveProductUsage("Caloom.CORE/ProductUsage/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/Caloom.CORE/ProductUsage/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Caloom.CORE/ProductUsage/" + contentName);
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
					CopyContentFrom((ProductUsage) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(ProductUsage));
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

				public static ProductUsage DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ProductUsage));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ProductUsage) serializer.ReadObject(xmlReader);
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
					return Path.Combine("Caloom.CORE", "ProductUsage", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "Caloom.CORE", "ProductUsage", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref ProductUsage customDemoObject);



				public static ProductUsage CreateDefault()
				{
					var result = new ProductUsage();
					result.Product = Product.CreateDefault();
					return result;
				}
				/*
				public static ProductUsage CreateDemoDefault()
				{
					ProductUsage customDemo = null;
					ProductUsage.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new ProductUsage();
					result.Product = Product.CreateDemoDefault();
				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(Product != null) {
						((IInformationObject) Product).UpdateCollections(masterInstance);
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
						IInformationObject item = Product;
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
						var item = Product;
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
						var item = (IInformationObject) Product;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(Product != _unmodified_Product)
							return true;
						if(UsageAmountInDecimal != _unmodified_UsageAmountInDecimal)
							return true;
						{
							IInformationObject item = (IInformationObject) Product;
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
					if(Product != null) {
						if(Product.ID == replacingObject.ID)
							Product = (Product) replacingObject;
						else {
							IInformationObject iObject = Product;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(ProductUsage sourceObject)
				{
					Product = sourceObject.Product;
					UsageAmountInDecimal = sourceObject.UsageAmountInDecimal;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_UsageAmountInDecimal = UsageAmountInDecimal;
				
					_unmodified_Product = Product;
					if(Product != null)
						((IInformationObject) Product).SetInstanceTreeValuesAsUnmodified();

				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "UsageAmountInDecimal":
							UsageAmountInDecimal = double.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public Product Product { get; set; }
			private Product _unmodified_Product;
			[DataMember]
			public double UsageAmountInDecimal { get; set; }
			private double _unmodified_UsageAmountInDecimal;
			
			}
			[DataContract]
			[Serializable]
			public partial class ProductCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public ProductCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "Caloom.CORE";
				    this.Name = "ProductCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "Caloom.CORE/ProductCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(ProductCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("Caloom.CORE", "ProductCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static ProductCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveProductCollection(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: ProductCollection");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(ProductCollection), null, owner);
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


                public static ProductCollection RetrieveProductCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ProductCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(ProductCollection), null, owner);
                    return result;
                }

				public static ProductCollection RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = ProductCollection.RetrieveProductCollection("Content/Caloom.CORE/ProductCollection/" + contentName, containerOwner);
					var result = ProductCollection.RetrieveProductCollection("Caloom.CORE/ProductCollection/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/Caloom.CORE/ProductCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Caloom.CORE/ProductCollection/" + contentName);
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
					CopyContentFrom((ProductCollection) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(ProductCollection));
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

				public static ProductCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ProductCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ProductCollection) serializer.ReadObject(xmlReader);
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
					return Path.Combine("Caloom.CORE", "ProductCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "Caloom.CORE", "ProductCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref ProductCollection customDemoObject);


				
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
					string dummyItemLocation = Product.GetRelativeLocationFromID("dummy");
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
																								 typeof(Product));
                    Array.ForEach(informationObjects, io => io.MasterETag = io.ETag);
					CollectionContent.Clear();
					CollectionContent.AddRange(informationObjects.Select(obj => (Product) obj));
            
				}

				public static ProductCollection GetMasterCollectionInstance(IContainerOwner owner)
				{
					return ProductCollection.RetrieveFromOwnerContent(owner, "MasterCollection");
				}

				public void SubscribeToContentSource()
				{
					// DirectoryToCollection
					string itemDirectory = GetItemDirectory();
					SubscribeSupport.AddSubscriptionToObject(itemDirectory, RelativeLocation,
															 SubscribeSupport.SubscribeType_DirectoryToCollection, null, typeof(ProductCollection).FullName);
				}

				public static string GetMasterCollectionLocation(IContainerOwner owner)
				{
					return StorageSupport.GetOwnerContentLocation(owner, "Caloom.CORE/ProductCollection/" + "MasterCollection");
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

				
		
				public static ProductCollection CreateDefault()
				{
					var result = new ProductCollection();
					return result;
				}

				/*
				public static ProductCollection CreateDemoDefault()
				{
					ProductCollection customDemo = null;
					ProductCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new ProductCollection();
					result.CollectionContent.Add(Product.CreateDemoDefault());
					//result.CollectionContent.Add(Product.CreateDemoDefault());
					//result.CollectionContent.Add(Product.CreateDemoDefault());
					return result;
				}
				*/

		
				[DataMember] public List<Product> CollectionContent = new List<Product>();
				private Product[] _unmodified_CollectionContent;

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

				public Product[] GetIDSelectedArray()
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
							CollectionContent[i] = (Product )replacingObject;
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

				private void CopyContentFrom(ProductCollection sourceObject)
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
			public partial class NodeSummaryContainer : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public NodeSummaryContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "Caloom.CORE";
				    this.Name = "NodeSummaryContainer";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "Caloom.CORE/NodeSummaryContainer/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(NodeSummaryContainer), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("Caloom.CORE", "NodeSummaryContainer", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static NodeSummaryContainer RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveNodeSummaryContainer(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: NodeSummaryContainer");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(NodeSummaryContainer), null, owner);
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


                public static NodeSummaryContainer RetrieveNodeSummaryContainer(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (NodeSummaryContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(NodeSummaryContainer), null, owner);
                    return result;
                }

				public static NodeSummaryContainer RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = NodeSummaryContainer.RetrieveNodeSummaryContainer("Content/Caloom.CORE/NodeSummaryContainer/" + contentName, containerOwner);
					var result = NodeSummaryContainer.RetrieveNodeSummaryContainer("Caloom.CORE/NodeSummaryContainer/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/Caloom.CORE/NodeSummaryContainer/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Caloom.CORE/NodeSummaryContainer/" + contentName);
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
					CopyContentFrom((NodeSummaryContainer) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(NodeSummaryContainer));
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

				public static NodeSummaryContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(NodeSummaryContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (NodeSummaryContainer) serializer.ReadObject(xmlReader);
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
					return Path.Combine("Caloom.CORE", "NodeSummaryContainer", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "Caloom.CORE", "NodeSummaryContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref NodeSummaryContainer customDemoObject);



				public static NodeSummaryContainer CreateDefault()
				{
					var result = new NodeSummaryContainer();
					result.Nodes = RenderedNodeCollection.CreateDefault();
					result.NodeSourceProducts = ProductCollection.CreateDefault();
					return result;
				}
				/*
				public static NodeSummaryContainer CreateDemoDefault()
				{
					NodeSummaryContainer customDemo = null;
					NodeSummaryContainer.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new NodeSummaryContainer();
					result.Nodes = RenderedNodeCollection.CreateDemoDefault();
					result.NodeSourceProducts = ProductCollection.CreateDemoDefault();
				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(Nodes != null) {
						((IInformationObject) Nodes).UpdateCollections(masterInstance);
					}

					if(masterInstance is ProductCollection) {
						Caloom.CORE.CollectionUpdateImplementation.Update_NodeSummaryContainer_NodeSourceProducts(this, localCollection:NodeSourceProducts, masterCollection:(ProductCollection) masterInstance);
					} else if(NodeSourceProducts != null) {
						((IInformationObject) NodeSourceProducts).UpdateCollections(masterInstance);
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
						IInformationObject item = Nodes;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = NodeSourceProducts;
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
						var item = Nodes;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = NodeSourceProducts;
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
						var item = (IInformationObject) Nodes;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) NodeSourceProducts;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(Nodes != _unmodified_Nodes)
							return true;
						if(NodeSourceProducts != _unmodified_NodeSourceProducts)
							return true;
						{
							IInformationObject item = (IInformationObject) Nodes;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) NodeSourceProducts;
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
					if(Nodes != null) {
						if(Nodes.ID == replacingObject.ID)
							Nodes = (RenderedNodeCollection) replacingObject;
						else {
							IInformationObject iObject = Nodes;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(NodeSourceProducts != null) {
						if(NodeSourceProducts.ID == replacingObject.ID)
							NodeSourceProducts = (ProductCollection) replacingObject;
						else {
							IInformationObject iObject = NodeSourceProducts;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(NodeSummaryContainer sourceObject)
				{
					Nodes = sourceObject.Nodes;
					NodeSourceProducts = sourceObject.NodeSourceProducts;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
				
					_unmodified_Nodes = Nodes;
					if(Nodes != null)
						((IInformationObject) Nodes).SetInstanceTreeValuesAsUnmodified();

					_unmodified_NodeSourceProducts = NodeSourceProducts;
					if(NodeSourceProducts != null)
						((IInformationObject) NodeSourceProducts).SetInstanceTreeValuesAsUnmodified();

				
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
			public RenderedNodeCollection Nodes { get; set; }
			private RenderedNodeCollection _unmodified_Nodes;
			[DataMember]
			public ProductCollection NodeSourceProducts { get; set; }
			private ProductCollection _unmodified_NodeSourceProducts;
			
			}
			[DataContract]
			[Serializable]
			public partial class RenderedNodeCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public RenderedNodeCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "Caloom.CORE";
				    this.Name = "RenderedNodeCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "Caloom.CORE/RenderedNodeCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(RenderedNodeCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("Caloom.CORE", "RenderedNodeCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static RenderedNodeCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveRenderedNodeCollection(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: RenderedNodeCollection");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(RenderedNodeCollection), null, owner);
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


                public static RenderedNodeCollection RetrieveRenderedNodeCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (RenderedNodeCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(RenderedNodeCollection), null, owner);
                    return result;
                }

				public static RenderedNodeCollection RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = RenderedNodeCollection.RetrieveRenderedNodeCollection("Content/Caloom.CORE/RenderedNodeCollection/" + contentName, containerOwner);
					var result = RenderedNodeCollection.RetrieveRenderedNodeCollection("Caloom.CORE/RenderedNodeCollection/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/Caloom.CORE/RenderedNodeCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Caloom.CORE/RenderedNodeCollection/" + contentName);
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
					CopyContentFrom((RenderedNodeCollection) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(RenderedNodeCollection));
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

				public static RenderedNodeCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(RenderedNodeCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (RenderedNodeCollection) serializer.ReadObject(xmlReader);
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
					return Path.Combine("Caloom.CORE", "RenderedNodeCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "Caloom.CORE", "RenderedNodeCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref RenderedNodeCollection customDemoObject);


				
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
					string dummyItemLocation = RenderedNode.GetRelativeLocationFromID("dummy");
					string nonOwnerDirectoryLocation = SubscribeSupport.GetParentDirectoryTarget(dummyItemLocation);
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
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

				
		
				public static RenderedNodeCollection CreateDefault()
				{
					var result = new RenderedNodeCollection();
					return result;
				}

				/*
				public static RenderedNodeCollection CreateDemoDefault()
				{
					RenderedNodeCollection customDemo = null;
					RenderedNodeCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new RenderedNodeCollection();
					result.CollectionContent.Add(RenderedNode.CreateDemoDefault());
					//result.CollectionContent.Add(RenderedNode.CreateDemoDefault());
					//result.CollectionContent.Add(RenderedNode.CreateDemoDefault());
					return result;
				}
				*/

		
				[DataMember] public List<RenderedNode> CollectionContent = new List<RenderedNode>();
				private RenderedNode[] _unmodified_CollectionContent;

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

				public RenderedNode[] GetIDSelectedArray()
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
							CollectionContent[i] = (RenderedNode )replacingObject;
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

				private void CopyContentFrom(RenderedNodeCollection sourceObject)
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
			public partial class RenderedNode : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public RenderedNode()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "Caloom.CORE";
				    this.Name = "RenderedNode";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "Caloom.CORE/RenderedNode/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(RenderedNode), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("Caloom.CORE", "RenderedNode", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static RenderedNode RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveRenderedNode(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: RenderedNode");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(RenderedNode), null, owner);
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


                public static RenderedNode RetrieveRenderedNode(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (RenderedNode) StorageSupport.RetrieveInformation(relativeLocation, typeof(RenderedNode), null, owner);
                    return result;
                }

				public static RenderedNode RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = RenderedNode.RetrieveRenderedNode("Content/Caloom.CORE/RenderedNode/" + contentName, containerOwner);
					var result = RenderedNode.RetrieveRenderedNode("Caloom.CORE/RenderedNode/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/Caloom.CORE/RenderedNode/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Caloom.CORE/RenderedNode/" + contentName);
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
					CopyContentFrom((RenderedNode) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(RenderedNode));
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

				public static RenderedNode DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(RenderedNode));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (RenderedNode) serializer.ReadObject(xmlReader);
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
					return Path.Combine("Caloom.CORE", "RenderedNode", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "Caloom.CORE", "RenderedNode", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref RenderedNode customDemoObject);



				public static RenderedNode CreateDefault()
				{
					var result = new RenderedNode();
					result.Categories = ShortTextCollection.CreateDefault();
					result.Authors = ShortTextCollection.CreateDefault();
					result.Locations = ShortTextCollection.CreateDefault();
					result.Filters = ShortTextCollection.CreateDefault();
					return result;
				}
				/*
				public static RenderedNode CreateDemoDefault()
				{
					RenderedNode customDemo = null;
					RenderedNode.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new RenderedNode();
					result.TechnicalSource = @"RenderedNode.TechnicalSource";

					result.ImageBaseUrl = @"RenderedNode.ImageBaseUrl";

					result.Title = @"RenderedNode.Title";

					result.ActualContentUrl = @"RenderedNode.ActualContentUrl";

					result.Excerpt = @"RenderedNode.Excerpt
RenderedNode.Excerpt
RenderedNode.Excerpt
RenderedNode.Excerpt
RenderedNode.Excerpt
";

					result.TimestampText = @"RenderedNode.TimestampText";

					result.MainSortableText = @"RenderedNode.MainSortableText";

					result.Categories = ShortTextCollection.CreateDemoDefault();
					result.Authors = ShortTextCollection.CreateDemoDefault();
					result.Locations = ShortTextCollection.CreateDemoDefault();
					result.Filters = ShortTextCollection.CreateDemoDefault();
				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(Categories != null) {
						((IInformationObject) Categories).UpdateCollections(masterInstance);
					}

					if(Authors != null) {
						((IInformationObject) Authors).UpdateCollections(masterInstance);
					}

					if(Locations != null) {
						((IInformationObject) Locations).UpdateCollections(masterInstance);
					}

					if(Filters != null) {
						((IInformationObject) Filters).UpdateCollections(masterInstance);
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
						IInformationObject item = Categories;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = Authors;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = Locations;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = Filters;
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
						var item = Categories;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = Authors;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = Locations;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = Filters;
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
						var item = (IInformationObject) Categories;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) Authors;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) Locations;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) Filters;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(TechnicalSource != _unmodified_TechnicalSource)
							return true;
						if(ImageBaseUrl != _unmodified_ImageBaseUrl)
							return true;
						if(Title != _unmodified_Title)
							return true;
						if(ActualContentUrl != _unmodified_ActualContentUrl)
							return true;
						if(Excerpt != _unmodified_Excerpt)
							return true;
						if(TimestampText != _unmodified_TimestampText)
							return true;
						if(MainSortableText != _unmodified_MainSortableText)
							return true;
						if(Categories != _unmodified_Categories)
							return true;
						if(Authors != _unmodified_Authors)
							return true;
						if(Locations != _unmodified_Locations)
							return true;
						if(Filters != _unmodified_Filters)
							return true;
						{
							IInformationObject item = (IInformationObject) Categories;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) Authors;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) Locations;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) Filters;
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
					if(Categories != null) {
						if(Categories.ID == replacingObject.ID)
							Categories = (ShortTextCollection) replacingObject;
						else {
							IInformationObject iObject = Categories;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(Authors != null) {
						if(Authors.ID == replacingObject.ID)
							Authors = (ShortTextCollection) replacingObject;
						else {
							IInformationObject iObject = Authors;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(Locations != null) {
						if(Locations.ID == replacingObject.ID)
							Locations = (ShortTextCollection) replacingObject;
						else {
							IInformationObject iObject = Locations;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(Filters != null) {
						if(Filters.ID == replacingObject.ID)
							Filters = (ShortTextCollection) replacingObject;
						else {
							IInformationObject iObject = Filters;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(RenderedNode sourceObject)
				{
					TechnicalSource = sourceObject.TechnicalSource;
					ImageBaseUrl = sourceObject.ImageBaseUrl;
					Title = sourceObject.Title;
					ActualContentUrl = sourceObject.ActualContentUrl;
					Excerpt = sourceObject.Excerpt;
					TimestampText = sourceObject.TimestampText;
					MainSortableText = sourceObject.MainSortableText;
					Categories = sourceObject.Categories;
					Authors = sourceObject.Authors;
					Locations = sourceObject.Locations;
					Filters = sourceObject.Filters;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_TechnicalSource = TechnicalSource;
					_unmodified_ImageBaseUrl = ImageBaseUrl;
					_unmodified_Title = Title;
					_unmodified_ActualContentUrl = ActualContentUrl;
					_unmodified_Excerpt = Excerpt;
					_unmodified_TimestampText = TimestampText;
					_unmodified_MainSortableText = MainSortableText;
				
					_unmodified_Categories = Categories;
					if(Categories != null)
						((IInformationObject) Categories).SetInstanceTreeValuesAsUnmodified();

					_unmodified_Authors = Authors;
					if(Authors != null)
						((IInformationObject) Authors).SetInstanceTreeValuesAsUnmodified();

					_unmodified_Locations = Locations;
					if(Locations != null)
						((IInformationObject) Locations).SetInstanceTreeValuesAsUnmodified();

					_unmodified_Filters = Filters;
					if(Filters != null)
						((IInformationObject) Filters).SetInstanceTreeValuesAsUnmodified();

				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "TechnicalSource":
							TechnicalSource = value;
							break;
						case "ImageBaseUrl":
							ImageBaseUrl = value;
							break;
						case "Title":
							Title = value;
							break;
						case "ActualContentUrl":
							ActualContentUrl = value;
							break;
						case "Excerpt":
							Excerpt = value;
							break;
						case "TimestampText":
							TimestampText = value;
							break;
						case "MainSortableText":
							MainSortableText = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string TechnicalSource { get; set; }
			private string _unmodified_TechnicalSource;
			[DataMember]
			public string ImageBaseUrl { get; set; }
			private string _unmodified_ImageBaseUrl;
			[DataMember]
			public string Title { get; set; }
			private string _unmodified_Title;
			[DataMember]
			public string ActualContentUrl { get; set; }
			private string _unmodified_ActualContentUrl;
			[DataMember]
			public string Excerpt { get; set; }
			private string _unmodified_Excerpt;
			[DataMember]
			public string TimestampText { get; set; }
			private string _unmodified_TimestampText;
			[DataMember]
			public string MainSortableText { get; set; }
			private string _unmodified_MainSortableText;
			[DataMember]
			public ShortTextCollection Categories { get; set; }
			private ShortTextCollection _unmodified_Categories;
			[DataMember]
			public ShortTextCollection Authors { get; set; }
			private ShortTextCollection _unmodified_Authors;
			[DataMember]
			public ShortTextCollection Locations { get; set; }
			private ShortTextCollection _unmodified_Locations;
			[DataMember]
			public ShortTextCollection Filters { get; set; }
			private ShortTextCollection _unmodified_Filters;
			
			}
			[DataContract]
			[Serializable]
			public partial class ShortTextCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public ShortTextCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "Caloom.CORE";
				    this.Name = "ShortTextCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "Caloom.CORE/ShortTextCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(ShortTextCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("Caloom.CORE", "ShortTextCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static ShortTextCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveShortTextCollection(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: ShortTextCollection");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(ShortTextCollection), null, owner);
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


                public static ShortTextCollection RetrieveShortTextCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ShortTextCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(ShortTextCollection), null, owner);
                    return result;
                }

				public static ShortTextCollection RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = ShortTextCollection.RetrieveShortTextCollection("Content/Caloom.CORE/ShortTextCollection/" + contentName, containerOwner);
					var result = ShortTextCollection.RetrieveShortTextCollection("Caloom.CORE/ShortTextCollection/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/Caloom.CORE/ShortTextCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Caloom.CORE/ShortTextCollection/" + contentName);
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
					CopyContentFrom((ShortTextCollection) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(ShortTextCollection));
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

				public static ShortTextCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ShortTextCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ShortTextCollection) serializer.ReadObject(xmlReader);
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
					return Path.Combine("Caloom.CORE", "ShortTextCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "Caloom.CORE", "ShortTextCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref ShortTextCollection customDemoObject);


				
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
					string dummyItemLocation = ShortTextObject.GetRelativeLocationFromID("dummy");
					string nonOwnerDirectoryLocation = SubscribeSupport.GetParentDirectoryTarget(dummyItemLocation);
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
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

				
		
				public static ShortTextCollection CreateDefault()
				{
					var result = new ShortTextCollection();
					return result;
				}

				/*
				public static ShortTextCollection CreateDemoDefault()
				{
					ShortTextCollection customDemo = null;
					ShortTextCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new ShortTextCollection();
					result.CollectionContent.Add(ShortTextObject.CreateDemoDefault());
					//result.CollectionContent.Add(ShortTextObject.CreateDemoDefault());
					//result.CollectionContent.Add(ShortTextObject.CreateDemoDefault());
					return result;
				}
				*/

		
				[DataMember] public List<ShortTextObject> CollectionContent = new List<ShortTextObject>();
				private ShortTextObject[] _unmodified_CollectionContent;

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

				public ShortTextObject[] GetIDSelectedArray()
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
							CollectionContent[i] = (ShortTextObject )replacingObject;
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

				private void CopyContentFrom(ShortTextCollection sourceObject)
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
			public partial class ShortTextObject : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public ShortTextObject()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "Caloom.CORE";
				    this.Name = "ShortTextObject";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "Caloom.CORE/ShortTextObject/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(ShortTextObject), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("Caloom.CORE", "ShortTextObject", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static ShortTextObject RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveShortTextObject(relativeLocation, owner);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: ShortTextObject");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(ShortTextObject), null, owner);
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


                public static ShortTextObject RetrieveShortTextObject(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ShortTextObject) StorageSupport.RetrieveInformation(relativeLocation, typeof(ShortTextObject), null, owner);
                    return result;
                }

				public static ShortTextObject RetrieveFromOwnerContent(IContainerOwner containerOwner, string contentName)
				{
					// var result = ShortTextObject.RetrieveShortTextObject("Content/Caloom.CORE/ShortTextObject/" + contentName, containerOwner);
					var result = ShortTextObject.RetrieveShortTextObject("Caloom.CORE/ShortTextObject/" + contentName, containerOwner);
					return result;
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/Caloom.CORE/ShortTextObject/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Caloom.CORE/ShortTextObject/" + contentName);
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
					CopyContentFrom((ShortTextObject) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(ShortTextObject));
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

				public static ShortTextObject DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ShortTextObject));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ShortTextObject) serializer.ReadObject(xmlReader);
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
					return Path.Combine("Caloom.CORE", "ShortTextObject", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "Caloom.CORE", "ShortTextObject", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref ShortTextObject customDemoObject);



				public static ShortTextObject CreateDefault()
				{
					var result = new ShortTextObject();
					return result;
				}
				/*
				public static ShortTextObject CreateDemoDefault()
				{
					ShortTextObject customDemo = null;
					ShortTextObject.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new ShortTextObject();
					result.Content = @"ShortTextObject.Content";

				
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

						if(Content != _unmodified_Content)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(ShortTextObject sourceObject)
				{
					Content = sourceObject.Content;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_Content = Content;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Content":
							Content = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Content { get; set; }
			private string _unmodified_Content;
			
			}
 } 