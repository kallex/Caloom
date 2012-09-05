 

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;

namespace AaltoGlobalImpact.OIP { 
		    public interface IInformationObject
    {
        Guid OwnerID { get; set; }
        string ID { get; set; }
        string ETag { get; set;  }
        string RelativeLocation { get; set; }
        string SemanticDomainName { get; set; }
        string Name { get; set; }
		void InitializeDefaultSubscribers(IContainerOwner owner);
		void SetValuesToObjects(NameValueCollection form);
		void PostStoringExecute(IContainerOwner owner);
		void PostDeleteExecute(IContainerOwner owner);
		void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName);
		string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName);
    }

			[DataContract]
			public partial class TBSystem : IInformationObject
			{
				public TBSystem()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBSystem";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "TBSystem", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static TBSystem RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBSystem(relativeLocation, owner);
				}


                public static TBSystem RetrieveTBSystem(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (TBSystem) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBSystem), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBSystem));
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

				public static TBSystem DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBSystem));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TBSystem) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBSystem", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "TBSystem", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static TBSystem CreateDefault()
				{
					var result = new TBSystem();
					return result;
				}

				public static TBSystem CreateDemoDefault()
				{
					var result = new TBSystem();
					result.InstanceName = @"TBSystem.InstanceName";

					result.AdminGroupID = @"TBSystem.AdminGroupID";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "InstanceName":
							InstanceName = value;
							break;
						case "AdminGroupID":
							AdminGroupID = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string InstanceName { get; set; }
			[DataMember]
			public string AdminGroupID { get; set; }
			
			}
			[DataContract]
			public partial class TBRLoginRoot : IInformationObject
			{
				public TBRLoginRoot()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBRLoginRoot";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "TBRLoginRoot", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static TBRLoginRoot RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBRLoginRoot(relativeLocation, owner);
				}


                public static TBRLoginRoot RetrieveTBRLoginRoot(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (TBRLoginRoot) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBRLoginRoot), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBRLoginRoot));
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

				public static TBRLoginRoot DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBRLoginRoot));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TBRLoginRoot) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBRLoginRoot", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "TBRLoginRoot", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static TBRLoginRoot CreateDefault()
				{
					var result = new TBRLoginRoot();
					result.Account = TBAccount.CreateDefault();
					return result;
				}

				public static TBRLoginRoot CreateDemoDefault()
				{
					var result = new TBRLoginRoot();
					result.Account = TBAccount.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Account;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public TBAccount Account { get; set; }
			
			}
			[DataContract]
			public partial class TBRAccountRoot : IInformationObject
			{
				public TBRAccountRoot()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBRAccountRoot";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "TBRAccountRoot", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static TBRAccountRoot RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBRAccountRoot(relativeLocation, owner);
				}


                public static TBRAccountRoot RetrieveTBRAccountRoot(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (TBRAccountRoot) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBRAccountRoot), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBRAccountRoot));
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

				public static TBRAccountRoot DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBRAccountRoot));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TBRAccountRoot) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBRAccountRoot", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "TBRAccountRoot", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static TBRAccountRoot CreateDefault()
				{
					var result = new TBRAccountRoot();
					result.Account = TBAccount.CreateDefault();
					return result;
				}

				public static TBRAccountRoot CreateDemoDefault()
				{
					var result = new TBRAccountRoot();
					result.Account = TBAccount.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Account;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public TBAccount Account { get; set; }
			
			}
			[DataContract]
			public partial class TBRGroupRoot : IInformationObject
			{
				public TBRGroupRoot()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBRGroupRoot";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "TBRGroupRoot", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static TBRGroupRoot RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBRGroupRoot(relativeLocation, owner);
				}


                public static TBRGroupRoot RetrieveTBRGroupRoot(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (TBRGroupRoot) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBRGroupRoot), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBRGroupRoot));
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

				public static TBRGroupRoot DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBRGroupRoot));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TBRGroupRoot) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBRGroupRoot", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "TBRGroupRoot", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static TBRGroupRoot CreateDefault()
				{
					var result = new TBRGroupRoot();
					result.Group = TBCollaboratingGroup.CreateDefault();
					return result;
				}

				public static TBRGroupRoot CreateDemoDefault()
				{
					var result = new TBRGroupRoot();
					result.Group = TBCollaboratingGroup.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Group;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public TBCollaboratingGroup Group { get; set; }
			
			}
			[DataContract]
			public partial class TBRLoginGroupRoot : IInformationObject
			{
				public TBRLoginGroupRoot()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBRLoginGroupRoot";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "TBRLoginGroupRoot", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static TBRLoginGroupRoot RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBRLoginGroupRoot(relativeLocation, owner);
				}


                public static TBRLoginGroupRoot RetrieveTBRLoginGroupRoot(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (TBRLoginGroupRoot) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBRLoginGroupRoot), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBRLoginGroupRoot));
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

				public static TBRLoginGroupRoot DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBRLoginGroupRoot));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TBRLoginGroupRoot) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBRLoginGroupRoot", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "TBRLoginGroupRoot", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static TBRLoginGroupRoot CreateDefault()
				{
					var result = new TBRLoginGroupRoot();
					return result;
				}

				public static TBRLoginGroupRoot CreateDemoDefault()
				{
					var result = new TBRLoginGroupRoot();
					result.Role = @"TBRLoginGroupRoot.Role";

					result.GroupID = @"TBRLoginGroupRoot.GroupID";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Role":
							Role = value;
							break;
						case "GroupID":
							GroupID = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Role { get; set; }
			[DataMember]
			public string GroupID { get; set; }
			
			}
			[DataContract]
			public partial class TBREmailRoot : IInformationObject
			{
				public TBREmailRoot()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBREmailRoot";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "TBREmailRoot", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static TBREmailRoot RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBREmailRoot(relativeLocation, owner);
				}


                public static TBREmailRoot RetrieveTBREmailRoot(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (TBREmailRoot) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBREmailRoot), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBREmailRoot));
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

				public static TBREmailRoot DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBREmailRoot));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TBREmailRoot) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBREmailRoot", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "TBREmailRoot", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static TBREmailRoot CreateDefault()
				{
					var result = new TBREmailRoot();
					result.Account = TBAccount.CreateDefault();
					return result;
				}

				public static TBREmailRoot CreateDemoDefault()
				{
					var result = new TBREmailRoot();
					result.Account = TBAccount.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Account;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public TBAccount Account { get; set; }
			
			}
			[DataContract]
			public partial class TBAccount : IInformationObject
			{
				public TBAccount()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBAccount";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "TBAccount", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static TBAccount RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBAccount(relativeLocation, owner);
				}


                public static TBAccount RetrieveTBAccount(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (TBAccount) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBAccount), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBAccount));
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

				public static TBAccount DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBAccount));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TBAccount) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBAccount", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "TBAccount", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static TBAccount CreateDefault()
				{
					var result = new TBAccount();
					result.Emails = TBEmailCollection.CreateDefault();
					result.Logins = TBLoginInfoCollection.CreateDefault();
					result.GroupRoleCollection = TBAccountCollaborationGroupCollection.CreateDefault();
					return result;
				}

				public static TBAccount CreateDemoDefault()
				{
					var result = new TBAccount();
					result.Emails = TBEmailCollection.CreateDemoDefault();
					result.Logins = TBLoginInfoCollection.CreateDemoDefault();
					result.GroupRoleCollection = TBAccountCollaborationGroupCollection.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Emails;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = Logins;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = GroupRoleCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public TBEmailCollection Emails { get; set; }
			[DataMember]
			public TBLoginInfoCollection Logins { get; set; }
			[DataMember]
			public TBAccountCollaborationGroupCollection GroupRoleCollection { get; set; }
			
			}
			[DataContract]
			public partial class TBAccountCollaborationGroup : IInformationObject
			{
				public TBAccountCollaborationGroup()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBAccountCollaborationGroup";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "TBAccountCollaborationGroup", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static TBAccountCollaborationGroup RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBAccountCollaborationGroup(relativeLocation, owner);
				}


                public static TBAccountCollaborationGroup RetrieveTBAccountCollaborationGroup(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (TBAccountCollaborationGroup) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBAccountCollaborationGroup), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBAccountCollaborationGroup));
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

				public static TBAccountCollaborationGroup DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBAccountCollaborationGroup));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TBAccountCollaborationGroup) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBAccountCollaborationGroup", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "TBAccountCollaborationGroup", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static TBAccountCollaborationGroup CreateDefault()
				{
					var result = new TBAccountCollaborationGroup();
					return result;
				}

				public static TBAccountCollaborationGroup CreateDemoDefault()
				{
					var result = new TBAccountCollaborationGroup();
					result.GroupID = @"TBAccountCollaborationGroup.GroupID";

					result.GroupRole = @"TBAccountCollaborationGroup.GroupRole";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "GroupID":
							GroupID = value;
							break;
						case "GroupRole":
							GroupRole = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string GroupID { get; set; }
			[DataMember]
			public string GroupRole { get; set; }
			
			}
			[DataContract]
			public partial class TBAccountCollaborationGroupCollection : IInformationObject
			{
				public TBAccountCollaborationGroupCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBAccountCollaborationGroupCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "TBAccountCollaborationGroupCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static TBAccountCollaborationGroupCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBAccountCollaborationGroupCollection(relativeLocation, owner);
				}


                public static TBAccountCollaborationGroupCollection RetrieveTBAccountCollaborationGroupCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (TBAccountCollaborationGroupCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBAccountCollaborationGroupCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBAccountCollaborationGroupCollection));
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

				public static TBAccountCollaborationGroupCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBAccountCollaborationGroupCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TBAccountCollaborationGroupCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBAccountCollaborationGroupCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "TBAccountCollaborationGroupCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static TBAccountCollaborationGroupCollection CreateDefault()
				{
					var result = new TBAccountCollaborationGroupCollection();
					return result;
				}

				public static TBAccountCollaborationGroupCollection CreateDemoDefault()
				{
					var result = new TBAccountCollaborationGroupCollection();
					result.CollectionContent.Add(TBAccountCollaborationGroup.CreateDemoDefault());
					result.CollectionContent.Add(TBAccountCollaborationGroup.CreateDemoDefault());
					result.CollectionContent.Add(TBAccountCollaborationGroup.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<TBAccountCollaborationGroup> CollectionContent = new List<TBAccountCollaborationGroup>();

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


			
			}
			[DataContract]
			public partial class TBLoginInfo : IInformationObject
			{
				public TBLoginInfo()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBLoginInfo";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "TBLoginInfo", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static TBLoginInfo RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBLoginInfo(relativeLocation, owner);
				}


                public static TBLoginInfo RetrieveTBLoginInfo(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (TBLoginInfo) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBLoginInfo), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBLoginInfo));
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

				public static TBLoginInfo DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBLoginInfo));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TBLoginInfo) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBLoginInfo", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "TBLoginInfo", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static TBLoginInfo CreateDefault()
				{
					var result = new TBLoginInfo();
					return result;
				}

				public static TBLoginInfo CreateDemoDefault()
				{
					var result = new TBLoginInfo();
					result.OpenIDUrl = @"TBLoginInfo.OpenIDUrl";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "OpenIDUrl":
							OpenIDUrl = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string OpenIDUrl { get; set; }
			
			}
			[DataContract]
			public partial class TBLoginInfoCollection : IInformationObject
			{
				public TBLoginInfoCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBLoginInfoCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "TBLoginInfoCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static TBLoginInfoCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBLoginInfoCollection(relativeLocation, owner);
				}


                public static TBLoginInfoCollection RetrieveTBLoginInfoCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (TBLoginInfoCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBLoginInfoCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBLoginInfoCollection));
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

				public static TBLoginInfoCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBLoginInfoCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TBLoginInfoCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBLoginInfoCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "TBLoginInfoCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static TBLoginInfoCollection CreateDefault()
				{
					var result = new TBLoginInfoCollection();
					return result;
				}

				public static TBLoginInfoCollection CreateDemoDefault()
				{
					var result = new TBLoginInfoCollection();
					result.CollectionContent.Add(TBLoginInfo.CreateDemoDefault());
					result.CollectionContent.Add(TBLoginInfo.CreateDemoDefault());
					result.CollectionContent.Add(TBLoginInfo.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<TBLoginInfo> CollectionContent = new List<TBLoginInfo>();

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


			
			}
			[DataContract]
			public partial class TBEmail : IInformationObject
			{
				public TBEmail()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBEmail";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "TBEmail", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static TBEmail RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBEmail(relativeLocation, owner);
				}


                public static TBEmail RetrieveTBEmail(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (TBEmail) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBEmail), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBEmail));
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

				public static TBEmail DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBEmail));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TBEmail) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBEmail", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "TBEmail", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static TBEmail CreateDefault()
				{
					var result = new TBEmail();
					return result;
				}

				public static TBEmail CreateDemoDefault()
				{
					var result = new TBEmail();
					result.EmailAddress = @"TBEmail.EmailAddress";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "EmailAddress":
							EmailAddress = value;
							break;
						case "ValidatedAt":
							ValidatedAt = DateTime.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string EmailAddress { get; set; }
			[DataMember]
			public DateTime ValidatedAt { get; set; }
			
			}
			[DataContract]
			public partial class TBEmailCollection : IInformationObject
			{
				public TBEmailCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBEmailCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "TBEmailCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static TBEmailCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBEmailCollection(relativeLocation, owner);
				}


                public static TBEmailCollection RetrieveTBEmailCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (TBEmailCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBEmailCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBEmailCollection));
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

				public static TBEmailCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBEmailCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TBEmailCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBEmailCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "TBEmailCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static TBEmailCollection CreateDefault()
				{
					var result = new TBEmailCollection();
					return result;
				}

				public static TBEmailCollection CreateDemoDefault()
				{
					var result = new TBEmailCollection();
					result.CollectionContent.Add(TBEmail.CreateDemoDefault());
					result.CollectionContent.Add(TBEmail.CreateDemoDefault());
					result.CollectionContent.Add(TBEmail.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<TBEmail> CollectionContent = new List<TBEmail>();

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


			
			}
			[DataContract]
			public partial class TBCollaboratorRole : IInformationObject
			{
				public TBCollaboratorRole()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBCollaboratorRole";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "TBCollaboratorRole", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static TBCollaboratorRole RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBCollaboratorRole(relativeLocation, owner);
				}


                public static TBCollaboratorRole RetrieveTBCollaboratorRole(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (TBCollaboratorRole) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBCollaboratorRole), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBCollaboratorRole));
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

				public static TBCollaboratorRole DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBCollaboratorRole));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TBCollaboratorRole) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBCollaboratorRole", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "TBCollaboratorRole", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static TBCollaboratorRole CreateDefault()
				{
					var result = new TBCollaboratorRole();
					result.Email = TBEmail.CreateDefault();
					return result;
				}

				public static TBCollaboratorRole CreateDemoDefault()
				{
					var result = new TBCollaboratorRole();
					result.Email = TBEmail.CreateDemoDefault();
					result.Role = @"TBCollaboratorRole.Role";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Email;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Role":
							Role = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public TBEmail Email { get; set; }
			[DataMember]
			public string Role { get; set; }
			
			}
			[DataContract]
			public partial class TBCollaboratorRoleCollection : IInformationObject
			{
				public TBCollaboratorRoleCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBCollaboratorRoleCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "TBCollaboratorRoleCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static TBCollaboratorRoleCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBCollaboratorRoleCollection(relativeLocation, owner);
				}


                public static TBCollaboratorRoleCollection RetrieveTBCollaboratorRoleCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (TBCollaboratorRoleCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBCollaboratorRoleCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBCollaboratorRoleCollection));
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

				public static TBCollaboratorRoleCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBCollaboratorRoleCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TBCollaboratorRoleCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBCollaboratorRoleCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "TBCollaboratorRoleCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static TBCollaboratorRoleCollection CreateDefault()
				{
					var result = new TBCollaboratorRoleCollection();
					return result;
				}

				public static TBCollaboratorRoleCollection CreateDemoDefault()
				{
					var result = new TBCollaboratorRoleCollection();
					result.CollectionContent.Add(TBCollaboratorRole.CreateDemoDefault());
					result.CollectionContent.Add(TBCollaboratorRole.CreateDemoDefault());
					result.CollectionContent.Add(TBCollaboratorRole.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<TBCollaboratorRole> CollectionContent = new List<TBCollaboratorRole>();

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


			
			}
			[DataContract]
			public partial class TBCollaboratingGroup : IInformationObject
			{
				public TBCollaboratingGroup()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBCollaboratingGroup";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "TBCollaboratingGroup", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static TBCollaboratingGroup RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBCollaboratingGroup(relativeLocation, owner);
				}


                public static TBCollaboratingGroup RetrieveTBCollaboratingGroup(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (TBCollaboratingGroup) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBCollaboratingGroup), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBCollaboratingGroup));
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

				public static TBCollaboratingGroup DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBCollaboratingGroup));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TBCollaboratingGroup) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBCollaboratingGroup", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "TBCollaboratingGroup", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static TBCollaboratingGroup CreateDefault()
				{
					var result = new TBCollaboratingGroup();
					result.Roles = TBCollaboratorRoleCollection.CreateDefault();
					return result;
				}

				public static TBCollaboratingGroup CreateDemoDefault()
				{
					var result = new TBCollaboratingGroup();
					result.Title = @"TBCollaboratingGroup.Title";

					result.Roles = TBCollaboratorRoleCollection.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Roles;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Title":
							Title = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Title { get; set; }
			[DataMember]
			public TBCollaboratorRoleCollection Roles { get; set; }
			
			}
			[DataContract]
			public partial class TBEmailValidation : IInformationObject
			{
				public TBEmailValidation()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBEmailValidation";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "TBEmailValidation", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static TBEmailValidation RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBEmailValidation(relativeLocation, owner);
				}


                public static TBEmailValidation RetrieveTBEmailValidation(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (TBEmailValidation) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBEmailValidation), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBEmailValidation));
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

				public static TBEmailValidation DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBEmailValidation));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TBEmailValidation) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBEmailValidation", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "TBEmailValidation", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static TBEmailValidation CreateDefault()
				{
					var result = new TBEmailValidation();
					return result;
				}

				public static TBEmailValidation CreateDemoDefault()
				{
					var result = new TBEmailValidation();
					result.Email = @"TBEmailValidation.Email";

					result.AccountID = @"TBEmailValidation.AccountID";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Email":
							Email = value;
							break;
						case "AccountID":
							AccountID = value;
							break;
						case "ValidUntil":
							ValidUntil = DateTime.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Email { get; set; }
			[DataMember]
			public string AccountID { get; set; }
			[DataMember]
			public DateTime ValidUntil { get; set; }
			
			}
			[DataContract]
			public partial class TBRegisterContainer : IInformationObject
			{
				public TBRegisterContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBRegisterContainer";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "TBRegisterContainer", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static TBRegisterContainer RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBRegisterContainer(relativeLocation, owner);
				}


                public static TBRegisterContainer RetrieveTBRegisterContainer(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (TBRegisterContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBRegisterContainer), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBRegisterContainer));
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

				public static TBRegisterContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBRegisterContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TBRegisterContainer) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBRegisterContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "TBRegisterContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static TBRegisterContainer CreateDefault()
				{
					var result = new TBRegisterContainer();
					result.Header = ContainerHeader.CreateDefault();
					result.LoginProviderCollection = LoginProviderCollection.CreateDefault();
					return result;
				}

				public static TBRegisterContainer CreateDemoDefault()
				{
					var result = new TBRegisterContainer();
					result.Header = ContainerHeader.CreateDemoDefault();
					result.ReturnUrl = @"TBRegisterContainer.ReturnUrl";

					result.LoginProviderCollection = LoginProviderCollection.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Header;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = LoginProviderCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "ReturnUrl":
							ReturnUrl = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public ContainerHeader Header { get; set; }
			[DataMember]
			public string ReturnUrl { get; set; }
			[DataMember]
			public LoginProviderCollection LoginProviderCollection { get; set; }
			
			}
			[DataContract]
			public partial class LoginProvider : IInformationObject
			{
				public LoginProvider()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "LoginProvider";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "LoginProvider", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static LoginProvider RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveLoginProvider(relativeLocation, owner);
				}


                public static LoginProvider RetrieveLoginProvider(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (LoginProvider) StorageSupport.RetrieveInformation(relativeLocation, typeof(LoginProvider), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(LoginProvider));
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

				public static LoginProvider DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(LoginProvider));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (LoginProvider) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "LoginProvider", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "LoginProvider", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static LoginProvider CreateDefault()
				{
					var result = new LoginProvider();
					return result;
				}

				public static LoginProvider CreateDemoDefault()
				{
					var result = new LoginProvider();
					result.ProviderName = @"LoginProvider.ProviderName";

					result.ProviderIconClass = @"LoginProvider.ProviderIconClass";

					result.ProviderType = @"LoginProvider.ProviderType";

					result.ProviderUrl = @"LoginProvider.ProviderUrl";

					result.ReturnUrl = @"LoginProvider.ReturnUrl";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "ProviderName":
							ProviderName = value;
							break;
						case "ProviderIconClass":
							ProviderIconClass = value;
							break;
						case "ProviderType":
							ProviderType = value;
							break;
						case "ProviderUrl":
							ProviderUrl = value;
							break;
						case "ReturnUrl":
							ReturnUrl = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string ProviderName { get; set; }
			[DataMember]
			public string ProviderIconClass { get; set; }
			[DataMember]
			public string ProviderType { get; set; }
			[DataMember]
			public string ProviderUrl { get; set; }
			[DataMember]
			public string ReturnUrl { get; set; }
			
			}
			[DataContract]
			public partial class LoginProviderCollection : IInformationObject
			{
				public LoginProviderCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "LoginProviderCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "LoginProviderCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static LoginProviderCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveLoginProviderCollection(relativeLocation, owner);
				}


                public static LoginProviderCollection RetrieveLoginProviderCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (LoginProviderCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(LoginProviderCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(LoginProviderCollection));
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

				public static LoginProviderCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(LoginProviderCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (LoginProviderCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "LoginProviderCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "LoginProviderCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static LoginProviderCollection CreateDefault()
				{
					var result = new LoginProviderCollection();
					return result;
				}

				public static LoginProviderCollection CreateDemoDefault()
				{
					var result = new LoginProviderCollection();
					result.CollectionContent.Add(LoginProvider.CreateDemoDefault());
					result.CollectionContent.Add(LoginProvider.CreateDemoDefault());
					result.CollectionContent.Add(LoginProvider.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<LoginProvider> CollectionContent = new List<LoginProvider>();

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


			
			}
			[DataContract]
			public partial class ContactOipContainer : IInformationObject
			{
				public ContactOipContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "ContactOipContainer";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "ContactOipContainer", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static ContactOipContainer RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveContactOipContainer(relativeLocation, owner);
				}


                public static ContactOipContainer RetrieveContactOipContainer(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ContactOipContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(ContactOipContainer), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ContactOipContainer));
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

				public static ContactOipContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ContactOipContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ContactOipContainer) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "ContactOipContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "ContactOipContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static ContactOipContainer CreateDefault()
				{
					var result = new ContactOipContainer();
					return result;
				}

				public static ContactOipContainer CreateDemoDefault()
				{
					var result = new ContactOipContainer();
					result.OIPModeratorGroupID = @"ContactOipContainer.OIPModeratorGroupID";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "OIPModeratorGroupID":
							OIPModeratorGroupID = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string OIPModeratorGroupID { get; set; }
			
			}
			[DataContract]
			public partial class TBPRegisterEmail : IInformationObject
			{
				public TBPRegisterEmail()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBPRegisterEmail";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "TBPRegisterEmail", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static TBPRegisterEmail RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBPRegisterEmail(relativeLocation, owner);
				}


                public static TBPRegisterEmail RetrieveTBPRegisterEmail(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (TBPRegisterEmail) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBPRegisterEmail), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBPRegisterEmail));
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

				public static TBPRegisterEmail DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TBPRegisterEmail));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TBPRegisterEmail) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBPRegisterEmail", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "TBPRegisterEmail", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static TBPRegisterEmail CreateDefault()
				{
					var result = new TBPRegisterEmail();
					return result;
				}

				public static TBPRegisterEmail CreateDemoDefault()
				{
					var result = new TBPRegisterEmail();
					result.EmailAddress = @"TBPRegisterEmail.EmailAddress";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "EmailAddress":
							EmailAddress = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string EmailAddress { get; set; }
			
			}
			[DataContract]
			public partial class JavaScriptContainer : IInformationObject
			{
				public JavaScriptContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "JavaScriptContainer";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "JavaScriptContainer", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static JavaScriptContainer RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveJavaScriptContainer(relativeLocation, owner);
				}


                public static JavaScriptContainer RetrieveJavaScriptContainer(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (JavaScriptContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(JavaScriptContainer), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(JavaScriptContainer));
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

				public static JavaScriptContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(JavaScriptContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (JavaScriptContainer) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "JavaScriptContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "JavaScriptContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static JavaScriptContainer CreateDefault()
				{
					var result = new JavaScriptContainer();
					return result;
				}

				public static JavaScriptContainer CreateDemoDefault()
				{
					var result = new JavaScriptContainer();
					result.HtmlContent = @"JavaScriptContainer.HtmlContent
JavaScriptContainer.HtmlContent
JavaScriptContainer.HtmlContent
JavaScriptContainer.HtmlContent
JavaScriptContainer.HtmlContent
";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "HtmlContent":
							HtmlContent = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string HtmlContent { get; set; }
			
			}
			[DataContract]
			public partial class JavascriptContainer : IInformationObject
			{
				public JavascriptContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "JavascriptContainer";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "JavascriptContainer", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static JavascriptContainer RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveJavascriptContainer(relativeLocation, owner);
				}


                public static JavascriptContainer RetrieveJavascriptContainer(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (JavascriptContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(JavascriptContainer), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(JavascriptContainer));
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

				public static JavascriptContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(JavascriptContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (JavascriptContainer) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "JavascriptContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "JavascriptContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static JavascriptContainer CreateDefault()
				{
					var result = new JavascriptContainer();
					return result;
				}

				public static JavascriptContainer CreateDemoDefault()
				{
					var result = new JavascriptContainer();
					result.HtmlContent = @"JavascriptContainer.HtmlContent
JavascriptContainer.HtmlContent
JavascriptContainer.HtmlContent
JavascriptContainer.HtmlContent
JavascriptContainer.HtmlContent
";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "HtmlContent":
							HtmlContent = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string HtmlContent { get; set; }
			
			}
			[DataContract]
			public partial class FooterContainer : IInformationObject
			{
				public FooterContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "FooterContainer";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "FooterContainer", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static FooterContainer RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveFooterContainer(relativeLocation, owner);
				}


                public static FooterContainer RetrieveFooterContainer(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (FooterContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(FooterContainer), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(FooterContainer));
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

				public static FooterContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(FooterContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (FooterContainer) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "FooterContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "FooterContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static FooterContainer CreateDefault()
				{
					var result = new FooterContainer();
					return result;
				}

				public static FooterContainer CreateDemoDefault()
				{
					var result = new FooterContainer();
					result.HtmlContent = @"FooterContainer.HtmlContent
FooterContainer.HtmlContent
FooterContainer.HtmlContent
FooterContainer.HtmlContent
FooterContainer.HtmlContent
";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "HtmlContent":
							HtmlContent = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string HtmlContent { get; set; }
			
			}
			[DataContract]
			public partial class NavigationContainer : IInformationObject
			{
				public NavigationContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "NavigationContainer";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "NavigationContainer", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static NavigationContainer RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveNavigationContainer(relativeLocation, owner);
				}


                public static NavigationContainer RetrieveNavigationContainer(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (NavigationContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(NavigationContainer), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(NavigationContainer));
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

				public static NavigationContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(NavigationContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (NavigationContainer) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "NavigationContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "NavigationContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static NavigationContainer CreateDefault()
				{
					var result = new NavigationContainer();
					return result;
				}

				public static NavigationContainer CreateDemoDefault()
				{
					var result = new NavigationContainer();
					result.Dummy = @"NavigationContainer.Dummy";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Dummy":
							Dummy = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Dummy { get; set; }
			
			}
			[DataContract]
			public partial class AccountContainer : IInformationObject
			{
				public AccountContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "AccountContainer";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "AccountContainer", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static AccountContainer RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAccountContainer(relativeLocation, owner);
				}


                public static AccountContainer RetrieveAccountContainer(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (AccountContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(AccountContainer), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AccountContainer));
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

				public static AccountContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AccountContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (AccountContainer) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AccountContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "AccountContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static AccountContainer CreateDefault()
				{
					var result = new AccountContainer();
					result.Header = ContainerHeader.CreateDefault();
					result.AccountIndex = AccountIndex.CreateDefault();
					result.AccountModule = AccountModule.CreateDefault();
					return result;
				}

				public static AccountContainer CreateDemoDefault()
				{
					var result = new AccountContainer();
					result.Header = ContainerHeader.CreateDemoDefault();
					result.AccountIndex = AccountIndex.CreateDemoDefault();
					result.AccountModule = AccountModule.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Header;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = AccountIndex;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = AccountModule;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public ContainerHeader Header { get; set; }
			[DataMember]
			public AccountIndex AccountIndex { get; set; }
			[DataMember]
			public AccountModule AccountModule { get; set; }
			
			}
			[DataContract]
			public partial class AccountIndex : IInformationObject
			{
				public AccountIndex()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "AccountIndex";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "AccountIndex", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static AccountIndex RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAccountIndex(relativeLocation, owner);
				}


                public static AccountIndex RetrieveAccountIndex(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (AccountIndex) StorageSupport.RetrieveInformation(relativeLocation, typeof(AccountIndex), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AccountIndex));
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

				public static AccountIndex DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AccountIndex));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (AccountIndex) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AccountIndex", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "AccountIndex", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static AccountIndex CreateDefault()
				{
					var result = new AccountIndex();
					return result;
				}

				public static AccountIndex CreateDemoDefault()
				{
					var result = new AccountIndex();
					result.Title = @"AccountIndex.Title";

					result.Introduction = @"AccountIndex.Introduction
AccountIndex.Introduction
AccountIndex.Introduction
AccountIndex.Introduction
AccountIndex.Introduction
";

					result.Summary = @"AccountIndex.Summary
AccountIndex.Summary
AccountIndex.Summary
AccountIndex.Summary
AccountIndex.Summary
";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Title":
							Title = value;
							break;
						case "Introduction":
							Introduction = value;
							break;
						case "Summary":
							Summary = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Title { get; set; }
			[DataMember]
			public string Introduction { get; set; }
			[DataMember]
			public string Summary { get; set; }
			
			}
			[DataContract]
			public partial class AccountModule : IInformationObject
			{
				public AccountModule()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "AccountModule";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "AccountModule", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static AccountModule RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAccountModule(relativeLocation, owner);
				}


                public static AccountModule RetrieveAccountModule(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (AccountModule) StorageSupport.RetrieveInformation(relativeLocation, typeof(AccountModule), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AccountModule));
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

				public static AccountModule DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AccountModule));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (AccountModule) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AccountModule", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "AccountModule", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static AccountModule CreateDefault()
				{
					var result = new AccountModule();
					result.Profile = AccountProfile.CreateDefault();
					result.Security = AccountSecurity.CreateDefault();
					result.Roles = AccountRoles.CreateDefault();
					result.LocationCollection = AddressAndLocationCollection.CreateDefault();
					result.AddLocationInfo = AddLocationInfo.CreateDefault();
					return result;
				}

				public static AccountModule CreateDemoDefault()
				{
					var result = new AccountModule();
					result.Profile = AccountProfile.CreateDemoDefault();
					result.Security = AccountSecurity.CreateDemoDefault();
					result.Roles = AccountRoles.CreateDemoDefault();
					result.LocationCollection = AddressAndLocationCollection.CreateDemoDefault();
					result.AddLocationInfo = AddLocationInfo.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Profile;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = Security;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = Roles;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = LocationCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = AddLocationInfo;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public AccountProfile Profile { get; set; }
			[DataMember]
			public AccountSecurity Security { get; set; }
			[DataMember]
			public AccountRoles Roles { get; set; }
			[DataMember]
			public AddressAndLocationCollection LocationCollection { get; set; }
			[DataMember]
			public AddLocationInfo AddLocationInfo { get; set; }
			
			}
			[DataContract]
			public partial class AddressAndLocationCollection : IInformationObject
			{
				public AddressAndLocationCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "AddressAndLocationCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "AddressAndLocationCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static AddressAndLocationCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAddressAndLocationCollection(relativeLocation, owner);
				}


                public static AddressAndLocationCollection RetrieveAddressAndLocationCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (AddressAndLocationCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(AddressAndLocationCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AddressAndLocationCollection));
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

				public static AddressAndLocationCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AddressAndLocationCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (AddressAndLocationCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AddressAndLocationCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "AddressAndLocationCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static AddressAndLocationCollection CreateDefault()
				{
					var result = new AddressAndLocationCollection();
					return result;
				}

				public static AddressAndLocationCollection CreateDemoDefault()
				{
					var result = new AddressAndLocationCollection();
					result.CollectionContent.Add(AddressAndLocation.CreateDemoDefault());
					result.CollectionContent.Add(AddressAndLocation.CreateDemoDefault());
					result.CollectionContent.Add(AddressAndLocation.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<AddressAndLocation> CollectionContent = new List<AddressAndLocation>();

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


			
			}
			[DataContract]
			public partial class AddressAndLocation : IInformationObject
			{
				public AddressAndLocation()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "AddressAndLocation";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "AddressAndLocation", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static AddressAndLocation RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAddressAndLocation(relativeLocation, owner);
				}


                public static AddressAndLocation RetrieveAddressAndLocation(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (AddressAndLocation) StorageSupport.RetrieveInformation(relativeLocation, typeof(AddressAndLocation), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AddressAndLocation));
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

				public static AddressAndLocation DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AddressAndLocation));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (AddressAndLocation) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AddressAndLocation", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "AddressAndLocation", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static AddressAndLocation CreateDefault()
				{
					var result = new AddressAndLocation();
					result.Address = StreetAddress.CreateDefault();
					result.Location = Location.CreateDefault();
					return result;
				}

				public static AddressAndLocation CreateDemoDefault()
				{
					var result = new AddressAndLocation();
					result.Address = StreetAddress.CreateDemoDefault();
					result.Location = Location.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Address;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = Location;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public StreetAddress Address { get; set; }
			[DataMember]
			public Location Location { get; set; }
			
			}
			[DataContract]
			public partial class StreetAddress : IInformationObject
			{
				public StreetAddress()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "StreetAddress";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "StreetAddress", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static StreetAddress RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveStreetAddress(relativeLocation, owner);
				}


                public static StreetAddress RetrieveStreetAddress(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (StreetAddress) StorageSupport.RetrieveInformation(relativeLocation, typeof(StreetAddress), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(StreetAddress));
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

				public static StreetAddress DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(StreetAddress));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (StreetAddress) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "StreetAddress", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "StreetAddress", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static StreetAddress CreateDefault()
				{
					var result = new StreetAddress();
					return result;
				}

				public static StreetAddress CreateDemoDefault()
				{
					var result = new StreetAddress();
					result.Street = @"StreetAddress.Street";

					result.ZipCode = @"StreetAddress.ZipCode";

					result.Town = @"StreetAddress.Town";

					result.Country = @"StreetAddress.Country";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Street":
							Street = value;
							break;
						case "ZipCode":
							ZipCode = value;
							break;
						case "Town":
							Town = value;
							break;
						case "Country":
							Country = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Street { get; set; }
			[DataMember]
			public string ZipCode { get; set; }
			[DataMember]
			public string Town { get; set; }
			[DataMember]
			public string Country { get; set; }
			
			}
			[DataContract]
			public partial class AccountContent : IInformationObject
			{
				public AccountContent()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "AccountContent";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "AccountContent", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static AccountContent RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAccountContent(relativeLocation, owner);
				}


                public static AccountContent RetrieveAccountContent(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (AccountContent) StorageSupport.RetrieveInformation(relativeLocation, typeof(AccountContent), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AccountContent));
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

				public static AccountContent DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AccountContent));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (AccountContent) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AccountContent", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "AccountContent", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static AccountContent CreateDefault()
				{
					var result = new AccountContent();
					return result;
				}

				public static AccountContent CreateDemoDefault()
				{
					var result = new AccountContent();
					result.Dummy = @"AccountContent.Dummy";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Dummy":
							Dummy = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Dummy { get; set; }
			
			}
			[DataContract]
			public partial class AccountProfile : IInformationObject
			{
				public AccountProfile()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "AccountProfile";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "AccountProfile", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static AccountProfile RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAccountProfile(relativeLocation, owner);
				}


                public static AccountProfile RetrieveAccountProfile(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (AccountProfile) StorageSupport.RetrieveInformation(relativeLocation, typeof(AccountProfile), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AccountProfile));
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

				public static AccountProfile DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AccountProfile));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (AccountProfile) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AccountProfile", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "AccountProfile", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static AccountProfile CreateDefault()
				{
					var result = new AccountProfile();
					result.ProfileImage = Image.CreateDefault();
					result.Address = StreetAddress.CreateDefault();
					return result;
				}

				public static AccountProfile CreateDemoDefault()
				{
					var result = new AccountProfile();
					result.ProfileImage = Image.CreateDemoDefault();
					result.FirstName = @"AccountProfile.FirstName";

					result.LastName = @"AccountProfile.LastName";

					result.Address = StreetAddress.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = ProfileImage;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = Address;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "FirstName":
							FirstName = value;
							break;
						case "LastName":
							LastName = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public Image ProfileImage { get; set; }
			[DataMember]
			public string FirstName { get; set; }
			[DataMember]
			public string LastName { get; set; }
			[DataMember]
			public StreetAddress Address { get; set; }
			
			}
			[DataContract]
			public partial class AccountSecurity : IInformationObject
			{
				public AccountSecurity()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "AccountSecurity";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "AccountSecurity", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static AccountSecurity RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAccountSecurity(relativeLocation, owner);
				}


                public static AccountSecurity RetrieveAccountSecurity(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (AccountSecurity) StorageSupport.RetrieveInformation(relativeLocation, typeof(AccountSecurity), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AccountSecurity));
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

				public static AccountSecurity DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AccountSecurity));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (AccountSecurity) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AccountSecurity", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "AccountSecurity", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static AccountSecurity CreateDefault()
				{
					var result = new AccountSecurity();
					result.LoginInfoCollection = TBLoginInfoCollection.CreateDefault();
					result.EmailCollection = TBEmailCollection.CreateDefault();
					return result;
				}

				public static AccountSecurity CreateDemoDefault()
				{
					var result = new AccountSecurity();
					result.LoginInfoCollection = TBLoginInfoCollection.CreateDemoDefault();
					result.EmailCollection = TBEmailCollection.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = LoginInfoCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = EmailCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public TBLoginInfoCollection LoginInfoCollection { get; set; }
			[DataMember]
			public TBEmailCollection EmailCollection { get; set; }
			
			}
			[DataContract]
			public partial class AccountRoles : IInformationObject
			{
				public AccountRoles()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "AccountRoles";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "AccountRoles", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static AccountRoles RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAccountRoles(relativeLocation, owner);
				}


                public static AccountRoles RetrieveAccountRoles(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (AccountRoles) StorageSupport.RetrieveInformation(relativeLocation, typeof(AccountRoles), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AccountRoles));
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

				public static AccountRoles DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AccountRoles));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (AccountRoles) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AccountRoles", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "AccountRoles", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static AccountRoles CreateDefault()
				{
					var result = new AccountRoles();
					result.ModeratorInGroups = ReferenceCollection.CreateDefault();
					result.MemberInGroups = ReferenceCollection.CreateDefault();
					return result;
				}

				public static AccountRoles CreateDemoDefault()
				{
					var result = new AccountRoles();
					result.ModeratorInGroups = ReferenceCollection.CreateDemoDefault();
					result.MemberInGroups = ReferenceCollection.CreateDemoDefault();
					result.OrganizationsImPartOf = @"AccountRoles.OrganizationsImPartOf
AccountRoles.OrganizationsImPartOf
AccountRoles.OrganizationsImPartOf
AccountRoles.OrganizationsImPartOf
AccountRoles.OrganizationsImPartOf
";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = ModeratorInGroups;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = MemberInGroups;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "OrganizationsImPartOf":
							OrganizationsImPartOf = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public ReferenceCollection ModeratorInGroups { get; set; }
			[DataMember]
			public ReferenceCollection MemberInGroups { get; set; }
			[DataMember]
			public string OrganizationsImPartOf { get; set; }
			
			}
			[DataContract]
			public partial class PersonalInfoVisibility : IInformationObject
			{
				public PersonalInfoVisibility()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "PersonalInfoVisibility";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "PersonalInfoVisibility", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static PersonalInfoVisibility RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrievePersonalInfoVisibility(relativeLocation, owner);
				}


                public static PersonalInfoVisibility RetrievePersonalInfoVisibility(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (PersonalInfoVisibility) StorageSupport.RetrieveInformation(relativeLocation, typeof(PersonalInfoVisibility), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(PersonalInfoVisibility));
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

				public static PersonalInfoVisibility DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(PersonalInfoVisibility));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (PersonalInfoVisibility) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "PersonalInfoVisibility", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "PersonalInfoVisibility", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static PersonalInfoVisibility CreateDefault()
				{
					var result = new PersonalInfoVisibility();
					return result;
				}

				public static PersonalInfoVisibility CreateDemoDefault()
				{
					var result = new PersonalInfoVisibility();
					result.NoOne_Network_All = @"PersonalInfoVisibility.NoOne_Network_All";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "NoOne_Network_All":
							NoOne_Network_All = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string NoOne_Network_All { get; set; }
			
			}
			[DataContract]
			public partial class ReferenceToInformation : IInformationObject
			{
				public ReferenceToInformation()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "ReferenceToInformation";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "ReferenceToInformation", id).Replace("\\", "/");
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


                public static ReferenceToInformation RetrieveReferenceToInformation(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ReferenceToInformation) StorageSupport.RetrieveInformation(relativeLocation, typeof(ReferenceToInformation), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "ReferenceToInformation", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "ReferenceToInformation", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static ReferenceToInformation CreateDefault()
				{
					var result = new ReferenceToInformation();
					return result;
				}

				public static ReferenceToInformation CreateDemoDefault()
				{
					var result = new ReferenceToInformation();
					result.Title = @"ReferenceToInformation.Title";

					result.URL = @"ReferenceToInformation.URL";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
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
			[DataMember]
			public string URL { get; set; }
			
			}
			[DataContract]
			public partial class ReferenceCollection : IInformationObject
			{
				public ReferenceCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "ReferenceCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "ReferenceCollection", id).Replace("\\", "/");
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


                public static ReferenceCollection RetrieveReferenceCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ReferenceCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(ReferenceCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "ReferenceCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "ReferenceCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static ReferenceCollection CreateDefault()
				{
					var result = new ReferenceCollection();
					return result;
				}

				public static ReferenceCollection CreateDemoDefault()
				{
					var result = new ReferenceCollection();
					result.CollectionContent.Add(ReferenceToInformation.CreateDemoDefault());
					result.CollectionContent.Add(ReferenceToInformation.CreateDemoDefault());
					result.CollectionContent.Add(ReferenceToInformation.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<ReferenceToInformation> CollectionContent = new List<ReferenceToInformation>();

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


			
			}
			[DataContract]
			public partial class BlogContainer : IInformationObject
			{
				public BlogContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "BlogContainer";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "BlogContainer", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static BlogContainer RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveBlogContainer(relativeLocation, owner);
				}


                public static BlogContainer RetrieveBlogContainer(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (BlogContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(BlogContainer), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(BlogContainer));
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

				public static BlogContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(BlogContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (BlogContainer) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "BlogContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "BlogContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static BlogContainer CreateDefault()
				{
					var result = new BlogContainer();
					result.Header = ContainerHeader.CreateDefault();
					result.FeaturedBlog = Blog.CreateDefault();
					result.RecentBlogCollection = BlogCollection.CreateDefault();
					result.BlogIndexGroup = BlogIndexGroup.CreateDefault();
					return result;
				}

				public static BlogContainer CreateDemoDefault()
				{
					var result = new BlogContainer();
					result.Header = ContainerHeader.CreateDemoDefault();
					result.FeaturedBlog = Blog.CreateDemoDefault();
					result.RecentBlogCollection = BlogCollection.CreateDemoDefault();
					result.BlogIndexGroup = BlogIndexGroup.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Header;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = FeaturedBlog;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = RecentBlogCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = BlogIndexGroup;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public ContainerHeader Header { get; set; }
			[DataMember]
			public Blog FeaturedBlog { get; set; }
			[DataMember]
			public BlogCollection RecentBlogCollection { get; set; }
			[DataMember]
			public BlogIndexGroup BlogIndexGroup { get; set; }
			
			}
			[DataContract]
			public partial class MapContainer : IInformationObject
			{
				public MapContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "MapContainer";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "MapContainer", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static MapContainer RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveMapContainer(relativeLocation, owner);
				}


                public static MapContainer RetrieveMapContainer(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (MapContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(MapContainer), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MapContainer));
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

				public static MapContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MapContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (MapContainer) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "MapContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "MapContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static MapContainer CreateDefault()
				{
					var result = new MapContainer();
					result.Header = ContainerHeader.CreateDefault();
					result.MapFeatured = Map.CreateDefault();
					result.MapCollection = MapCollection.CreateDefault();
					result.MapResultCollection = MapResultCollection.CreateDefault();
					result.MapIndexCollection = MapIndexCollection.CreateDefault();
					result.MapMarkers = MapMarkerCollection.CreateDefault();
					return result;
				}

				public static MapContainer CreateDemoDefault()
				{
					var result = new MapContainer();
					result.Header = ContainerHeader.CreateDemoDefault();
					result.MapFeatured = Map.CreateDemoDefault();
					result.MapCollection = MapCollection.CreateDemoDefault();
					result.MapResultCollection = MapResultCollection.CreateDemoDefault();
					result.MapIndexCollection = MapIndexCollection.CreateDemoDefault();
					result.MapMarkers = MapMarkerCollection.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Header;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = MapFeatured;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = MapCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = MapResultCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = MapIndexCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = MapMarkers;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public ContainerHeader Header { get; set; }
			[DataMember]
			public Map MapFeatured { get; set; }
			[DataMember]
			public MapCollection MapCollection { get; set; }
			[DataMember]
			public MapResultCollection MapResultCollection { get; set; }
			[DataMember]
			public MapIndexCollection MapIndexCollection { get; set; }
			[DataMember]
			public MapMarkerCollection MapMarkers { get; set; }
			
			}
			[DataContract]
			public partial class MapMarker : IInformationObject
			{
				public MapMarker()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "MapMarker";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "MapMarker", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static MapMarker RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveMapMarker(relativeLocation, owner);
				}


                public static MapMarker RetrieveMapMarker(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (MapMarker) StorageSupport.RetrieveInformation(relativeLocation, typeof(MapMarker), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MapMarker));
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

				public static MapMarker DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MapMarker));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (MapMarker) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "MapMarker", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "MapMarker", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static MapMarker CreateDefault()
				{
					var result = new MapMarker();
					result.Location = Location.CreateDefault();
					return result;
				}

				public static MapMarker CreateDemoDefault()
				{
					var result = new MapMarker();
					result.LocationText = @"MapMarker.LocationText";

					result.Location = Location.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Location;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "LocationText":
							LocationText = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string LocationText { get; set; }
			[DataMember]
			public Location Location { get; set; }
			
			}
			[DataContract]
			public partial class MapMarkerCollection : IInformationObject
			{
				public MapMarkerCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "MapMarkerCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "MapMarkerCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static MapMarkerCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveMapMarkerCollection(relativeLocation, owner);
				}


                public static MapMarkerCollection RetrieveMapMarkerCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (MapMarkerCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(MapMarkerCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MapMarkerCollection));
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

				public static MapMarkerCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MapMarkerCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (MapMarkerCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "MapMarkerCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "MapMarkerCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static MapMarkerCollection CreateDefault()
				{
					var result = new MapMarkerCollection();
					return result;
				}

				public static MapMarkerCollection CreateDemoDefault()
				{
					var result = new MapMarkerCollection();
					result.CollectionContent.Add(MapMarker.CreateDemoDefault());
					result.CollectionContent.Add(MapMarker.CreateDemoDefault());
					result.CollectionContent.Add(MapMarker.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<MapMarker> CollectionContent = new List<MapMarker>();

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


			
			}
			[DataContract]
			public partial class CalendarContainer : IInformationObject
			{
				public CalendarContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "CalendarContainer";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "CalendarContainer", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static CalendarContainer RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveCalendarContainer(relativeLocation, owner);
				}


                public static CalendarContainer RetrieveCalendarContainer(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (CalendarContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(CalendarContainer), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CalendarContainer));
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

				public static CalendarContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CalendarContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (CalendarContainer) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "CalendarContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "CalendarContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static CalendarContainer CreateDefault()
				{
					var result = new CalendarContainer();
					result.CalendarContainerHeader = ContainerHeader.CreateDefault();
					result.CalendarFeatured = Calendar.CreateDefault();
					result.CalendarCollection = CalendarCollection.CreateDefault();
					result.CalendarIndexCollection = CalendarIndex.CreateDefault();
					return result;
				}

				public static CalendarContainer CreateDemoDefault()
				{
					var result = new CalendarContainer();
					result.CalendarContainerHeader = ContainerHeader.CreateDemoDefault();
					result.CalendarFeatured = Calendar.CreateDemoDefault();
					result.CalendarCollection = CalendarCollection.CreateDemoDefault();
					result.CalendarIndexCollection = CalendarIndex.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = CalendarContainerHeader;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = CalendarFeatured;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = CalendarCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = CalendarIndexCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public ContainerHeader CalendarContainerHeader { get; set; }
			[DataMember]
			public Calendar CalendarFeatured { get; set; }
			[DataMember]
			public CalendarCollection CalendarCollection { get; set; }
			[DataMember]
			public CalendarIndex CalendarIndexCollection { get; set; }
			
			}
			[DataContract]
			public partial class AboutContainer : IInformationObject
			{
				public AboutContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "AboutContainer";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "AboutContainer", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static AboutContainer RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAboutContainer(relativeLocation, owner);
				}


                public static AboutContainer RetrieveAboutContainer(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (AboutContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(AboutContainer), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AboutContainer));
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

				public static AboutContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AboutContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (AboutContainer) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AboutContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "AboutContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static AboutContainer CreateDefault()
				{
					var result = new AboutContainer();
					result.MainImage = Image.CreateDefault();
					result.Header = ContainerHeader.CreateDefault();
					result.ImageGroup = ImageGroup.CreateDefault();
					return result;
				}

				public static AboutContainer CreateDemoDefault()
				{
					var result = new AboutContainer();
					result.MainImage = Image.CreateDemoDefault();
					result.Header = ContainerHeader.CreateDemoDefault();
					result.Excerpt = @"AboutContainer.Excerpt
AboutContainer.Excerpt
AboutContainer.Excerpt
AboutContainer.Excerpt
AboutContainer.Excerpt
";

					result.Body = @"AboutContainer.Body
AboutContainer.Body
AboutContainer.Body
AboutContainer.Body
AboutContainer.Body
";

					result.Author = @"AboutContainer.Author";

					result.ImageGroup = ImageGroup.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = MainImage;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = Header;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = ImageGroup;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Excerpt":
							Excerpt = value;
							break;
						case "Body":
							Body = value;
							break;
						case "Published":
							Published = DateTime.Parse(value);
							break;
						case "Author":
							Author = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public Image MainImage { get; set; }
			[DataMember]
			public ContainerHeader Header { get; set; }
			[DataMember]
			public string Excerpt { get; set; }
			[DataMember]
			public string Body { get; set; }
			[DataMember]
			public DateTime Published { get; set; }
			[DataMember]
			public string Author { get; set; }
			[DataMember]
			public ImageGroup ImageGroup { get; set; }
			
			}
			[DataContract]
			public partial class OBSAccountContainer : IInformationObject
			{
				public OBSAccountContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "OBSAccountContainer";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "OBSAccountContainer", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static OBSAccountContainer RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveOBSAccountContainer(relativeLocation, owner);
				}


                public static OBSAccountContainer RetrieveOBSAccountContainer(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (OBSAccountContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(OBSAccountContainer), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(OBSAccountContainer));
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

				public static OBSAccountContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(OBSAccountContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (OBSAccountContainer) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "OBSAccountContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "OBSAccountContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static OBSAccountContainer CreateDefault()
				{
					var result = new OBSAccountContainer();
					result.AccountContainerHeader = ContainerHeader.CreateDefault();
					result.AccountFeatured = Calendar.CreateDefault();
					result.AccountCollection = CalendarCollection.CreateDefault();
					result.AccountIndexCollection = CalendarIndex.CreateDefault();
					return result;
				}

				public static OBSAccountContainer CreateDemoDefault()
				{
					var result = new OBSAccountContainer();
					result.AccountContainerHeader = ContainerHeader.CreateDemoDefault();
					result.AccountFeatured = Calendar.CreateDemoDefault();
					result.AccountCollection = CalendarCollection.CreateDemoDefault();
					result.AccountIndexCollection = CalendarIndex.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = AccountContainerHeader;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = AccountFeatured;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = AccountCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = AccountIndexCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public ContainerHeader AccountContainerHeader { get; set; }
			[DataMember]
			public Calendar AccountFeatured { get; set; }
			[DataMember]
			public CalendarCollection AccountCollection { get; set; }
			[DataMember]
			public CalendarIndex AccountIndexCollection { get; set; }
			
			}
			[DataContract]
			public partial class ProjectContainer : IInformationObject
			{
				public ProjectContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "ProjectContainer";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "ProjectContainer", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static ProjectContainer RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveProjectContainer(relativeLocation, owner);
				}


                public static ProjectContainer RetrieveProjectContainer(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ProjectContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(ProjectContainer), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ProjectContainer));
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

				public static ProjectContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ProjectContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ProjectContainer) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "ProjectContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "ProjectContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static ProjectContainer CreateDefault()
				{
					var result = new ProjectContainer();
					result.ProjectContainerHeader = ContainerHeader.CreateDefault();
					result.ProjectFeatured = Calendar.CreateDefault();
					result.ProjectCollection = CalendarCollection.CreateDefault();
					result.ProjectIndexCollection = CalendarIndex.CreateDefault();
					return result;
				}

				public static ProjectContainer CreateDemoDefault()
				{
					var result = new ProjectContainer();
					result.ProjectContainerHeader = ContainerHeader.CreateDemoDefault();
					result.ProjectFeatured = Calendar.CreateDemoDefault();
					result.ProjectCollection = CalendarCollection.CreateDemoDefault();
					result.ProjectIndexCollection = CalendarIndex.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = ProjectContainerHeader;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = ProjectFeatured;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = ProjectCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = ProjectIndexCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public ContainerHeader ProjectContainerHeader { get; set; }
			[DataMember]
			public Calendar ProjectFeatured { get; set; }
			[DataMember]
			public CalendarCollection ProjectCollection { get; set; }
			[DataMember]
			public CalendarIndex ProjectIndexCollection { get; set; }
			
			}
			[DataContract]
			public partial class CourseContainer : IInformationObject
			{
				public CourseContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "CourseContainer";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "CourseContainer", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static CourseContainer RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveCourseContainer(relativeLocation, owner);
				}


                public static CourseContainer RetrieveCourseContainer(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (CourseContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(CourseContainer), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CourseContainer));
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

				public static CourseContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CourseContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (CourseContainer) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "CourseContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "CourseContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static CourseContainer CreateDefault()
				{
					var result = new CourseContainer();
					result.CourseContainerHeader = ContainerHeader.CreateDefault();
					result.CourseFeatured = Calendar.CreateDefault();
					result.CourseCollection = CalendarCollection.CreateDefault();
					result.CourseIndexCollection = CalendarIndex.CreateDefault();
					return result;
				}

				public static CourseContainer CreateDemoDefault()
				{
					var result = new CourseContainer();
					result.CourseContainerHeader = ContainerHeader.CreateDemoDefault();
					result.CourseFeatured = Calendar.CreateDemoDefault();
					result.CourseCollection = CalendarCollection.CreateDemoDefault();
					result.CourseIndexCollection = CalendarIndex.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = CourseContainerHeader;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = CourseFeatured;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = CourseCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = CourseIndexCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public ContainerHeader CourseContainerHeader { get; set; }
			[DataMember]
			public Calendar CourseFeatured { get; set; }
			[DataMember]
			public CalendarCollection CourseCollection { get; set; }
			[DataMember]
			public CalendarIndex CourseIndexCollection { get; set; }
			
			}
			[DataContract]
			public partial class ContainerHeader : IInformationObject
			{
				public ContainerHeader()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "ContainerHeader";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "ContainerHeader", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static ContainerHeader RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveContainerHeader(relativeLocation, owner);
				}


                public static ContainerHeader RetrieveContainerHeader(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ContainerHeader) StorageSupport.RetrieveInformation(relativeLocation, typeof(ContainerHeader), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ContainerHeader));
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

				public static ContainerHeader DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ContainerHeader));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ContainerHeader) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "ContainerHeader", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "ContainerHeader", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static ContainerHeader CreateDefault()
				{
					var result = new ContainerHeader();
					return result;
				}

				public static ContainerHeader CreateDemoDefault()
				{
					var result = new ContainerHeader();
					result.Title = @"ContainerHeader.Title";

					result.SubTitle = @"ContainerHeader.SubTitle";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Title":
							Title = value;
							break;
						case "SubTitle":
							SubTitle = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Title { get; set; }
			[DataMember]
			public string SubTitle { get; set; }
			
			}
			[DataContract]
			public partial class ActivitySummaryContainer : IInformationObject
			{
				public ActivitySummaryContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "ActivitySummaryContainer";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "ActivitySummaryContainer", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static ActivitySummaryContainer RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveActivitySummaryContainer(relativeLocation, owner);
				}


                public static ActivitySummaryContainer RetrieveActivitySummaryContainer(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ActivitySummaryContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(ActivitySummaryContainer), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ActivitySummaryContainer));
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

				public static ActivitySummaryContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ActivitySummaryContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ActivitySummaryContainer) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "ActivitySummaryContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "ActivitySummaryContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static ActivitySummaryContainer CreateDefault()
				{
					var result = new ActivitySummaryContainer();
					result.Header = ContainerHeader.CreateDefault();
					result.Introduction = Introduction.CreateDefault();
					result.ActivityCollection = ActivityCollection.CreateDefault();
					return result;
				}

				public static ActivitySummaryContainer CreateDemoDefault()
				{
					var result = new ActivitySummaryContainer();
					result.Header = ContainerHeader.CreateDemoDefault();
					result.SummaryBody = @"ActivitySummaryContainer.SummaryBody
ActivitySummaryContainer.SummaryBody
ActivitySummaryContainer.SummaryBody
ActivitySummaryContainer.SummaryBody
ActivitySummaryContainer.SummaryBody
";

					result.Introduction = Introduction.CreateDemoDefault();
					result.ActivityCollection = ActivityCollection.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Header;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = Introduction;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = ActivityCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "SummaryBody":
							SummaryBody = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public ContainerHeader Header { get; set; }
			[DataMember]
			public string SummaryBody { get; set; }
			[DataMember]
			public Introduction Introduction { get; set; }
			[DataMember]
			public ActivityCollection ActivityCollection { get; set; }
			
			}
			[DataContract]
			public partial class ActivityContainer : IInformationObject
			{
				public ActivityContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "ActivityContainer";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "ActivityContainer", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static ActivityContainer RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveActivityContainer(relativeLocation, owner);
				}


                public static ActivityContainer RetrieveActivityContainer(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ActivityContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(ActivityContainer), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ActivityContainer));
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

				public static ActivityContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ActivityContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ActivityContainer) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "ActivityContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "ActivityContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static ActivityContainer CreateDefault()
				{
					var result = new ActivityContainer();
					result.Activities = ActivityCollection.CreateDefault();
					return result;
				}

				public static ActivityContainer CreateDemoDefault()
				{
					var result = new ActivityContainer();
					result.Activities = ActivityCollection.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Activities;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public ActivityCollection Activities { get; set; }
			
			}
			[DataContract]
			public partial class ActivityCollection : IInformationObject
			{
				public ActivityCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "ActivityCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "ActivityCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static ActivityCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveActivityCollection(relativeLocation, owner);
				}


                public static ActivityCollection RetrieveActivityCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ActivityCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(ActivityCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ActivityCollection));
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

				public static ActivityCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ActivityCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ActivityCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "ActivityCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "ActivityCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static ActivityCollection CreateDefault()
				{
					var result = new ActivityCollection();
					return result;
				}

				public static ActivityCollection CreateDemoDefault()
				{
					var result = new ActivityCollection();
					result.CollectionContent.Add(Activity.CreateDemoDefault());
					result.CollectionContent.Add(Activity.CreateDemoDefault());
					result.CollectionContent.Add(Activity.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<Activity> CollectionContent = new List<Activity>();

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


			
			}
			[DataContract]
			public partial class Activity : IInformationObject
			{
				public Activity()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Activity";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Activity", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Activity RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveActivity(relativeLocation, owner);
				}


                public static Activity RetrieveActivity(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Activity) StorageSupport.RetrieveInformation(relativeLocation, typeof(Activity), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Activity));
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

				public static Activity DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Activity));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Activity) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Activity", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "Activity", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static Activity CreateDefault()
				{
					var result = new Activity();
					result.MainImage = Image.CreateDefault();
					result.Moderators = ModeratorCollection.CreateDefault();
					result.ImageSets = ImageGroupCollection.CreateDefault();
					return result;
				}

				public static Activity CreateDemoDefault()
				{
					var result = new Activity();
					result.MainImage = Image.CreateDemoDefault();
					result.ActivityName = @"Activity.ActivityName";

					result.Description = @"Activity.Description
Activity.Description
Activity.Description
Activity.Description
Activity.Description
";

					result.Moderators = ModeratorCollection.CreateDemoDefault();
					result.ImageSets = ImageGroupCollection.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = MainImage;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = Moderators;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = ImageSets;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "ActivityName":
							ActivityName = value;
							break;
						case "Description":
							Description = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public Image MainImage { get; set; }
			[DataMember]
			public string ActivityName { get; set; }
			[DataMember]
			public string Description { get; set; }
			[DataMember]
			public ModeratorCollection Moderators { get; set; }
			[DataMember]
			public ImageGroupCollection ImageSets { get; set; }
			
			}
			[DataContract]
			public partial class ModeratorCollection : IInformationObject
			{
				public ModeratorCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "ModeratorCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "ModeratorCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static ModeratorCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveModeratorCollection(relativeLocation, owner);
				}


                public static ModeratorCollection RetrieveModeratorCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ModeratorCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(ModeratorCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ModeratorCollection));
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

				public static ModeratorCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ModeratorCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ModeratorCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "ModeratorCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "ModeratorCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static ModeratorCollection CreateDefault()
				{
					var result = new ModeratorCollection();
					return result;
				}

				public static ModeratorCollection CreateDemoDefault()
				{
					var result = new ModeratorCollection();
					result.CollectionContent.Add(Moderator.CreateDemoDefault());
					result.CollectionContent.Add(Moderator.CreateDemoDefault());
					result.CollectionContent.Add(Moderator.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<Moderator> CollectionContent = new List<Moderator>();

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


			
			}
			[DataContract]
			public partial class Moderator : IInformationObject
			{
				public Moderator()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Moderator";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Moderator", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Moderator RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveModerator(relativeLocation, owner);
				}


                public static Moderator RetrieveModerator(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Moderator) StorageSupport.RetrieveInformation(relativeLocation, typeof(Moderator), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Moderator));
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

				public static Moderator DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Moderator));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Moderator) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Moderator", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "Moderator", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static Moderator CreateDefault()
				{
					var result = new Moderator();
					return result;
				}

				public static Moderator CreateDemoDefault()
				{
					var result = new Moderator();
					result.ModeratorName = @"Moderator.ModeratorName";

					result.ProfileUrl = @"Moderator.ProfileUrl";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "ModeratorName":
							ModeratorName = value;
							break;
						case "ProfileUrl":
							ProfileUrl = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string ModeratorName { get; set; }
			[DataMember]
			public string ProfileUrl { get; set; }
			
			}
			[DataContract]
			public partial class CollaboratorCollection : IInformationObject
			{
				public CollaboratorCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "CollaboratorCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "CollaboratorCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static CollaboratorCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveCollaboratorCollection(relativeLocation, owner);
				}


                public static CollaboratorCollection RetrieveCollaboratorCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (CollaboratorCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(CollaboratorCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CollaboratorCollection));
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

				public static CollaboratorCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CollaboratorCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (CollaboratorCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "CollaboratorCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "CollaboratorCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static CollaboratorCollection CreateDefault()
				{
					var result = new CollaboratorCollection();
					return result;
				}

				public static CollaboratorCollection CreateDemoDefault()
				{
					var result = new CollaboratorCollection();
					result.CollectionContent.Add(Collaborator.CreateDemoDefault());
					result.CollectionContent.Add(Collaborator.CreateDemoDefault());
					result.CollectionContent.Add(Collaborator.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<Collaborator> CollectionContent = new List<Collaborator>();

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


			
			}
			[DataContract]
			public partial class Collaborator : IInformationObject
			{
				public Collaborator()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Collaborator";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Collaborator", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Collaborator RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveCollaborator(relativeLocation, owner);
				}


                public static Collaborator RetrieveCollaborator(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Collaborator) StorageSupport.RetrieveInformation(relativeLocation, typeof(Collaborator), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Collaborator));
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

				public static Collaborator DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Collaborator));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Collaborator) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Collaborator", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "Collaborator", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static Collaborator CreateDefault()
				{
					var result = new Collaborator();
					return result;
				}

				public static Collaborator CreateDemoDefault()
				{
					var result = new Collaborator();
					result.CollaboratorName = @"Collaborator.CollaboratorName";

					result.Role = @"Collaborator.Role";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "CollaboratorName":
							CollaboratorName = value;
							break;
						case "Role":
							Role = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string CollaboratorName { get; set; }
			[DataMember]
			public string Role { get; set; }
			
			}
			[DataContract]
			public partial class CollaboratingGroup : IInformationObject
			{
				public CollaboratingGroup()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "CollaboratingGroup";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "CollaboratingGroup", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static CollaboratingGroup RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveCollaboratingGroup(relativeLocation, owner);
				}


                public static CollaboratingGroup RetrieveCollaboratingGroup(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (CollaboratingGroup) StorageSupport.RetrieveInformation(relativeLocation, typeof(CollaboratingGroup), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CollaboratingGroup));
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

				public static CollaboratingGroup DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CollaboratingGroup));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (CollaboratingGroup) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "CollaboratingGroup", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "CollaboratingGroup", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static CollaboratingGroup CreateDefault()
				{
					var result = new CollaboratingGroup();
					return result;
				}

				public static CollaboratingGroup CreateDemoDefault()
				{
					var result = new CollaboratingGroup();
					result.CollaboratingGroupName = @"CollaboratingGroup.CollaboratingGroupName";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "CollaboratingGroupName":
							CollaboratingGroupName = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string CollaboratingGroupName { get; set; }
			
			}
			[DataContract]
			public partial class CollaboratingGroupCollection : IInformationObject
			{
				public CollaboratingGroupCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "CollaboratingGroupCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "CollaboratingGroupCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static CollaboratingGroupCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveCollaboratingGroupCollection(relativeLocation, owner);
				}


                public static CollaboratingGroupCollection RetrieveCollaboratingGroupCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (CollaboratingGroupCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(CollaboratingGroupCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CollaboratingGroupCollection));
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

				public static CollaboratingGroupCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CollaboratingGroupCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (CollaboratingGroupCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "CollaboratingGroupCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "CollaboratingGroupCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static CollaboratingGroupCollection CreateDefault()
				{
					var result = new CollaboratingGroupCollection();
					return result;
				}

				public static CollaboratingGroupCollection CreateDemoDefault()
				{
					var result = new CollaboratingGroupCollection();
					result.CollectionContent.Add(CollaboratingGroup.CreateDemoDefault());
					result.CollectionContent.Add(CollaboratingGroup.CreateDemoDefault());
					result.CollectionContent.Add(CollaboratingGroup.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<CollaboratingGroup> CollectionContent = new List<CollaboratingGroup>();

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


			
			}
			[DataContract]
			public partial class CollaboratingOrganization : IInformationObject
			{
				public CollaboratingOrganization()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "CollaboratingOrganization";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "CollaboratingOrganization", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static CollaboratingOrganization RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveCollaboratingOrganization(relativeLocation, owner);
				}


                public static CollaboratingOrganization RetrieveCollaboratingOrganization(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (CollaboratingOrganization) StorageSupport.RetrieveInformation(relativeLocation, typeof(CollaboratingOrganization), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CollaboratingOrganization));
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

				public static CollaboratingOrganization DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CollaboratingOrganization));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (CollaboratingOrganization) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "CollaboratingOrganization", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "CollaboratingOrganization", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static CollaboratingOrganization CreateDefault()
				{
					var result = new CollaboratingOrganization();
					return result;
				}

				public static CollaboratingOrganization CreateDemoDefault()
				{
					var result = new CollaboratingOrganization();
					result.CollaboratingOrganizationName = @"CollaboratingOrganization.CollaboratingOrganizationName";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "CollaboratingOrganizationName":
							CollaboratingOrganizationName = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string CollaboratingOrganizationName { get; set; }
			
			}
			[DataContract]
			public partial class CollaboratingOrganizationCollection : IInformationObject
			{
				public CollaboratingOrganizationCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "CollaboratingOrganizationCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "CollaboratingOrganizationCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static CollaboratingOrganizationCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveCollaboratingOrganizationCollection(relativeLocation, owner);
				}


                public static CollaboratingOrganizationCollection RetrieveCollaboratingOrganizationCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (CollaboratingOrganizationCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(CollaboratingOrganizationCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CollaboratingOrganizationCollection));
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

				public static CollaboratingOrganizationCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CollaboratingOrganizationCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (CollaboratingOrganizationCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "CollaboratingOrganizationCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "CollaboratingOrganizationCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static CollaboratingOrganizationCollection CreateDefault()
				{
					var result = new CollaboratingOrganizationCollection();
					return result;
				}

				public static CollaboratingOrganizationCollection CreateDemoDefault()
				{
					var result = new CollaboratingOrganizationCollection();
					result.CollectionContent.Add(CollaboratingOrganization.CreateDemoDefault());
					result.CollectionContent.Add(CollaboratingOrganization.CreateDemoDefault());
					result.CollectionContent.Add(CollaboratingOrganization.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<CollaboratingOrganization> CollectionContent = new List<CollaboratingOrganization>();

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


			
			}
			[DataContract]
			public partial class GroupSummaryContainer : IInformationObject
			{
				public GroupSummaryContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "GroupSummaryContainer";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "GroupSummaryContainer", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static GroupSummaryContainer RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveGroupSummaryContainer(relativeLocation, owner);
				}


                public static GroupSummaryContainer RetrieveGroupSummaryContainer(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (GroupSummaryContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(GroupSummaryContainer), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GroupSummaryContainer));
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

				public static GroupSummaryContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GroupSummaryContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (GroupSummaryContainer) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "GroupSummaryContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "GroupSummaryContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static GroupSummaryContainer CreateDefault()
				{
					var result = new GroupSummaryContainer();
					result.Header = ContainerHeader.CreateDefault();
					result.Introduction = Introduction.CreateDefault();
					result.GroupCollection = GroupCollection.CreateDefault();
					return result;
				}

				public static GroupSummaryContainer CreateDemoDefault()
				{
					var result = new GroupSummaryContainer();
					result.Header = ContainerHeader.CreateDemoDefault();
					result.SummaryBody = @"GroupSummaryContainer.SummaryBody
GroupSummaryContainer.SummaryBody
GroupSummaryContainer.SummaryBody
GroupSummaryContainer.SummaryBody
GroupSummaryContainer.SummaryBody
";

					result.Introduction = Introduction.CreateDemoDefault();
					result.GroupCollection = GroupCollection.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Header;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = Introduction;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = GroupCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "SummaryBody":
							SummaryBody = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public ContainerHeader Header { get; set; }
			[DataMember]
			public string SummaryBody { get; set; }
			[DataMember]
			public Introduction Introduction { get; set; }
			[DataMember]
			public GroupCollection GroupCollection { get; set; }
			
			}
			[DataContract]
			public partial class GroupContainer : IInformationObject
			{
				public GroupContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "GroupContainer";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "GroupContainer", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static GroupContainer RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveGroupContainer(relativeLocation, owner);
				}


                public static GroupContainer RetrieveGroupContainer(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (GroupContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(GroupContainer), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GroupContainer));
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

				public static GroupContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GroupContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (GroupContainer) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "GroupContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "GroupContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static GroupContainer CreateDefault()
				{
					var result = new GroupContainer();
					result.Header = ContainerHeader.CreateDefault();
					result.GroupProfile = Group.CreateDefault();
					result.Collaborators = CollaboratorCollection.CreateDefault();
					result.ActivityCollection = ActivityCollection.CreateDefault();
					result.Locations = LocationCollection.CreateDefault();
					result.AddLocationInfo = AddLocationInfo.CreateDefault();
					return result;
				}

				public static GroupContainer CreateDemoDefault()
				{
					var result = new GroupContainer();
					result.Header = ContainerHeader.CreateDemoDefault();
					result.GroupProfile = Group.CreateDemoDefault();
					result.Collaborators = CollaboratorCollection.CreateDemoDefault();
					result.ActivityCollection = ActivityCollection.CreateDemoDefault();
					result.Locations = LocationCollection.CreateDemoDefault();
					result.AddLocationInfo = AddLocationInfo.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Header;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = GroupProfile;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = Collaborators;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = ActivityCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = Locations;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = AddLocationInfo;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public ContainerHeader Header { get; set; }
			[DataMember]
			public Group GroupProfile { get; set; }
			[DataMember]
			public CollaboratorCollection Collaborators { get; set; }
			[DataMember]
			public ActivityCollection ActivityCollection { get; set; }
			[DataMember]
			public LocationCollection Locations { get; set; }
			[DataMember]
			public AddLocationInfo AddLocationInfo { get; set; }
			
			}
			[DataContract]
			public partial class AddLocationInfo : IInformationObject
			{
				public AddLocationInfo()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "AddLocationInfo";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "AddLocationInfo", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static AddLocationInfo RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAddLocationInfo(relativeLocation, owner);
				}


                public static AddLocationInfo RetrieveAddLocationInfo(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (AddLocationInfo) StorageSupport.RetrieveInformation(relativeLocation, typeof(AddLocationInfo), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AddLocationInfo));
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

				public static AddLocationInfo DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AddLocationInfo));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (AddLocationInfo) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AddLocationInfo", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "AddLocationInfo", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static AddLocationInfo CreateDefault()
				{
					var result = new AddLocationInfo();
					result.Address = StreetAddress.CreateDefault();
					return result;
				}

				public static AddLocationInfo CreateDemoDefault()
				{
					var result = new AddLocationInfo();
					result.LocationName = @"AddLocationInfo.LocationName";

					result.Longitude = @"AddLocationInfo.Longitude";

					result.Latitude = @"AddLocationInfo.Latitude";

					result.Address = StreetAddress.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Address;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "LocationName":
							LocationName = value;
							break;
						case "Longitude":
							Longitude = value;
							break;
						case "Latitude":
							Latitude = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string LocationName { get; set; }
			[DataMember]
			public string Longitude { get; set; }
			[DataMember]
			public string Latitude { get; set; }
			[DataMember]
			public StreetAddress Address { get; set; }
			
			}
			[DataContract]
			public partial class GroupCollection : IInformationObject
			{
				public GroupCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "GroupCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "GroupCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static GroupCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveGroupCollection(relativeLocation, owner);
				}


                public static GroupCollection RetrieveGroupCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (GroupCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(GroupCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GroupCollection));
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

				public static GroupCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GroupCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (GroupCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "GroupCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "GroupCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static GroupCollection CreateDefault()
				{
					var result = new GroupCollection();
					return result;
				}

				public static GroupCollection CreateDemoDefault()
				{
					var result = new GroupCollection();
					result.CollectionContent.Add(Group.CreateDemoDefault());
					result.CollectionContent.Add(Group.CreateDemoDefault());
					result.CollectionContent.Add(Group.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<Group> CollectionContent = new List<Group>();

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


			
			}
			[DataContract]
			public partial class Group : IInformationObject
			{
				public Group()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Group";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Group", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Group RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveGroup(relativeLocation, owner);
				}


                public static Group RetrieveGroup(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Group) StorageSupport.RetrieveInformation(relativeLocation, typeof(Group), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Group));
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

				public static Group DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Group));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Group) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Group", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "Group", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static Group CreateDefault()
				{
					var result = new Group();
					result.ProfileImage = Image.CreateDefault();
					result.IconImage = Image.CreateDefault();
					result.Moderators = ModeratorCollection.CreateDefault();
					result.ImageSets = ImageGroupCollection.CreateDefault();
					return result;
				}

				public static Group CreateDemoDefault()
				{
					var result = new Group();
					result.ProfileImage = Image.CreateDemoDefault();
					result.IconImage = Image.CreateDemoDefault();
					result.GroupName = @"Group.GroupName";

					result.Description = @"Group.Description
Group.Description
Group.Description
Group.Description
Group.Description
";

					result.OrganizationsAndGroupsLinkedToUs = @"Group.OrganizationsAndGroupsLinkedToUs
Group.OrganizationsAndGroupsLinkedToUs
Group.OrganizationsAndGroupsLinkedToUs
Group.OrganizationsAndGroupsLinkedToUs
Group.OrganizationsAndGroupsLinkedToUs
";

					result.Moderators = ModeratorCollection.CreateDemoDefault();
					result.ImageSets = ImageGroupCollection.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = ProfileImage;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = IconImage;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = Moderators;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = ImageSets;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "GroupName":
							GroupName = value;
							break;
						case "Description":
							Description = value;
							break;
						case "OrganizationsAndGroupsLinkedToUs":
							OrganizationsAndGroupsLinkedToUs = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public Image ProfileImage { get; set; }
			[DataMember]
			public Image IconImage { get; set; }
			[DataMember]
			public string GroupName { get; set; }
			[DataMember]
			public string Description { get; set; }
			[DataMember]
			public string OrganizationsAndGroupsLinkedToUs { get; set; }
			[DataMember]
			public ModeratorCollection Moderators { get; set; }
			[DataMember]
			public ImageGroupCollection ImageSets { get; set; }
			
			}
			[DataContract]
			public partial class Introduction : IInformationObject
			{
				public Introduction()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Introduction";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Introduction", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Introduction RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveIntroduction(relativeLocation, owner);
				}


                public static Introduction RetrieveIntroduction(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Introduction) StorageSupport.RetrieveInformation(relativeLocation, typeof(Introduction), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Introduction));
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

				public static Introduction DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Introduction));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Introduction) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Introduction", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "Introduction", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static Introduction CreateDefault()
				{
					var result = new Introduction();
					return result;
				}

				public static Introduction CreateDemoDefault()
				{
					var result = new Introduction();
					result.Title = @"Introduction.Title";

					result.Body = @"Introduction.Body
Introduction.Body
Introduction.Body
Introduction.Body
Introduction.Body
";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Title":
							Title = value;
							break;
						case "Body":
							Body = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Title { get; set; }
			[DataMember]
			public string Body { get; set; }
			
			}
			[DataContract]
			public partial class BlogCollection : IInformationObject
			{
				public BlogCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "BlogCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "BlogCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static BlogCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveBlogCollection(relativeLocation, owner);
				}


                public static BlogCollection RetrieveBlogCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (BlogCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(BlogCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(BlogCollection));
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

				public static BlogCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(BlogCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (BlogCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "BlogCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "BlogCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static BlogCollection CreateDefault()
				{
					var result = new BlogCollection();
					return result;
				}

				public static BlogCollection CreateDemoDefault()
				{
					var result = new BlogCollection();
					result.CollectionContent.Add(Blog.CreateDemoDefault());
					result.CollectionContent.Add(Blog.CreateDemoDefault());
					result.CollectionContent.Add(Blog.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<Blog> CollectionContent = new List<Blog>();

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


			
			}
			[DataContract]
			public partial class Blog : IInformationObject
			{
				public Blog()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Blog";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Blog", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Blog RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveBlog(relativeLocation, owner);
				}


                public static Blog RetrieveBlog(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Blog) StorageSupport.RetrieveInformation(relativeLocation, typeof(Blog), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Blog));
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

				public static Blog DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Blog));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Blog) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Blog", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "Blog", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static Blog CreateDefault()
				{
					var result = new Blog();
					result.ImageGroup = ImageGroup.CreateDefault();
					result.Location = Location.CreateDefault();
					result.Category = Category.CreateDefault();
					result.SocialPanel = SocialPanelCollection.CreateDefault();
					return result;
				}

				public static Blog CreateDemoDefault()
				{
					var result = new Blog();
					result.Title = @"Blog.Title";

					result.SubTitle = @"Blog.SubTitle";

					result.Author = @"Blog.Author";

					result.ImageGroup = ImageGroup.CreateDemoDefault();
					result.Body = @"Blog.Body
Blog.Body
Blog.Body
Blog.Body
Blog.Body
";

					result.Excerpt = @"Blog.Excerpt
Blog.Excerpt
Blog.Excerpt
Blog.Excerpt
Blog.Excerpt
";

					result.Location = Location.CreateDemoDefault();
					result.Category = Category.CreateDemoDefault();
					result.SocialPanel = SocialPanelCollection.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = ImageGroup;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = Location;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = Category;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = SocialPanel;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Title":
							Title = value;
							break;
						case "SubTitle":
							SubTitle = value;
							break;
						case "Published":
							Published = DateTime.Parse(value);
							break;
						case "Author":
							Author = value;
							break;
						case "Body":
							Body = value;
							break;
						case "Excerpt":
							Excerpt = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Title { get; set; }
			[DataMember]
			public string SubTitle { get; set; }
			[DataMember]
			public DateTime Published { get; set; }
			[DataMember]
			public string Author { get; set; }
			[DataMember]
			public ImageGroup ImageGroup { get; set; }
			[DataMember]
			public string Body { get; set; }
			[DataMember]
			public string Excerpt { get; set; }
			[DataMember]
			public Location Location { get; set; }
			[DataMember]
			public Category Category { get; set; }
			[DataMember]
			public SocialPanelCollection SocialPanel { get; set; }
			
			}
			[DataContract]
			public partial class BlogIndexGroup : IInformationObject
			{
				public BlogIndexGroup()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "BlogIndexGroup";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "BlogIndexGroup", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static BlogIndexGroup RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveBlogIndexGroup(relativeLocation, owner);
				}


                public static BlogIndexGroup RetrieveBlogIndexGroup(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (BlogIndexGroup) StorageSupport.RetrieveInformation(relativeLocation, typeof(BlogIndexGroup), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(BlogIndexGroup));
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

				public static BlogIndexGroup DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(BlogIndexGroup));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (BlogIndexGroup) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "BlogIndexGroup", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "BlogIndexGroup", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static BlogIndexGroup CreateDefault()
				{
					var result = new BlogIndexGroup();
					result.BlogByDate = BlogCollection.CreateDefault();
					result.BlogByLocation = BlogCollection.CreateDefault();
					result.BlogByAuthor = BlogCollection.CreateDefault();
					result.BlogByCategory = BlogCollection.CreateDefault();
					return result;
				}

				public static BlogIndexGroup CreateDemoDefault()
				{
					var result = new BlogIndexGroup();
					result.Title = @"BlogIndexGroup.Title";

					result.SubTitle = @"BlogIndexGroup.SubTitle";

					result.Introduction = @"BlogIndexGroup.Introduction
BlogIndexGroup.Introduction
BlogIndexGroup.Introduction
BlogIndexGroup.Introduction
BlogIndexGroup.Introduction
";

					result.BlogByDate = BlogCollection.CreateDemoDefault();
					result.BlogByLocation = BlogCollection.CreateDemoDefault();
					result.BlogByAuthor = BlogCollection.CreateDemoDefault();
					result.BlogByCategory = BlogCollection.CreateDemoDefault();
					result.Summary = @"BlogIndexGroup.Summary
BlogIndexGroup.Summary
BlogIndexGroup.Summary
BlogIndexGroup.Summary
BlogIndexGroup.Summary
";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = BlogByDate;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = BlogByLocation;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = BlogByAuthor;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = BlogByCategory;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Title":
							Title = value;
							break;
						case "SubTitle":
							SubTitle = value;
							break;
						case "Introduction":
							Introduction = value;
							break;
						case "Summary":
							Summary = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Title { get; set; }
			[DataMember]
			public string SubTitle { get; set; }
			[DataMember]
			public string Introduction { get; set; }
			[DataMember]
			public BlogCollection BlogByDate { get; set; }
			[DataMember]
			public BlogCollection BlogByLocation { get; set; }
			[DataMember]
			public BlogCollection BlogByAuthor { get; set; }
			[DataMember]
			public BlogCollection BlogByCategory { get; set; }
			[DataMember]
			public string Summary { get; set; }
			
			}
			[DataContract]
			public partial class CalendarIndex : IInformationObject
			{
				public CalendarIndex()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "CalendarIndex";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "CalendarIndex", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static CalendarIndex RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveCalendarIndex(relativeLocation, owner);
				}


                public static CalendarIndex RetrieveCalendarIndex(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (CalendarIndex) StorageSupport.RetrieveInformation(relativeLocation, typeof(CalendarIndex), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CalendarIndex));
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

				public static CalendarIndex DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CalendarIndex));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (CalendarIndex) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "CalendarIndex", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "CalendarIndex", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static CalendarIndex CreateDefault()
				{
					var result = new CalendarIndex();
					return result;
				}

				public static CalendarIndex CreateDemoDefault()
				{
					var result = new CalendarIndex();
					result.Title = @"CalendarIndex.Title";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Title":
							Title = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Title { get; set; }
			
			}
			[DataContract]
			public partial class Filter : IInformationObject
			{
				public Filter()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Filter";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Filter", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Filter RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveFilter(relativeLocation, owner);
				}


                public static Filter RetrieveFilter(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Filter) StorageSupport.RetrieveInformation(relativeLocation, typeof(Filter), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Filter));
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

				public static Filter DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Filter));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Filter) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Filter", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "Filter", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static Filter CreateDefault()
				{
					var result = new Filter();
					return result;
				}

				public static Filter CreateDemoDefault()
				{
					var result = new Filter();
					result.Title = @"Filter.Title";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Title":
							Title = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Title { get; set; }
			
			}
			[DataContract]
			public partial class Calendar : IInformationObject
			{
				public Calendar()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Calendar";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Calendar", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Calendar RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveCalendar(relativeLocation, owner);
				}


                public static Calendar RetrieveCalendar(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Calendar) StorageSupport.RetrieveInformation(relativeLocation, typeof(Calendar), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Calendar));
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

				public static Calendar DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Calendar));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Calendar) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Calendar", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "Calendar", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static Calendar CreateDefault()
				{
					var result = new Calendar();
					return result;
				}

				public static Calendar CreateDemoDefault()
				{
					var result = new Calendar();
					result.Title = @"Calendar.Title";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Title":
							Title = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Title { get; set; }
			
			}
			[DataContract]
			public partial class CalendarCollection : IInformationObject
			{
				public CalendarCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "CalendarCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "CalendarCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static CalendarCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveCalendarCollection(relativeLocation, owner);
				}


                public static CalendarCollection RetrieveCalendarCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (CalendarCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(CalendarCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CalendarCollection));
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

				public static CalendarCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CalendarCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (CalendarCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "CalendarCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "CalendarCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static CalendarCollection CreateDefault()
				{
					var result = new CalendarCollection();
					return result;
				}

				public static CalendarCollection CreateDemoDefault()
				{
					var result = new CalendarCollection();
					result.CollectionContent.Add(Calendar.CreateDemoDefault());
					result.CollectionContent.Add(Calendar.CreateDemoDefault());
					result.CollectionContent.Add(Calendar.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<Calendar> CollectionContent = new List<Calendar>();

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


			
			}
			[DataContract]
			public partial class Map : IInformationObject
			{
				public Map()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Map";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Map", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Map RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveMap(relativeLocation, owner);
				}


                public static Map RetrieveMap(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Map) StorageSupport.RetrieveInformation(relativeLocation, typeof(Map), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Map));
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

				public static Map DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Map));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Map) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Map", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "Map", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static Map CreateDefault()
				{
					var result = new Map();
					return result;
				}

				public static Map CreateDemoDefault()
				{
					var result = new Map();
					result.Title = @"Map.Title";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Title":
							Title = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Title { get; set; }
			
			}
			[DataContract]
			public partial class MapCollection : IInformationObject
			{
				public MapCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "MapCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "MapCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static MapCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveMapCollection(relativeLocation, owner);
				}


                public static MapCollection RetrieveMapCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (MapCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(MapCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MapCollection));
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

				public static MapCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MapCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (MapCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "MapCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "MapCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static MapCollection CreateDefault()
				{
					var result = new MapCollection();
					return result;
				}

				public static MapCollection CreateDemoDefault()
				{
					var result = new MapCollection();
					result.CollectionContent.Add(Map.CreateDemoDefault());
					result.CollectionContent.Add(Map.CreateDemoDefault());
					result.CollectionContent.Add(Map.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<Map> CollectionContent = new List<Map>();

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


			
			}
			[DataContract]
			public partial class MapIndexCollection : IInformationObject
			{
				public MapIndexCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "MapIndexCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "MapIndexCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static MapIndexCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveMapIndexCollection(relativeLocation, owner);
				}


                public static MapIndexCollection RetrieveMapIndexCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (MapIndexCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(MapIndexCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MapIndexCollection));
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

				public static MapIndexCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MapIndexCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (MapIndexCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "MapIndexCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "MapIndexCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static MapIndexCollection CreateDefault()
				{
					var result = new MapIndexCollection();
					result.MapByDate = MapCollection.CreateDefault();
					result.MapByLocation = MapCollection.CreateDefault();
					result.MapByAuthor = MapCollection.CreateDefault();
					result.MapByCategory = MapCollection.CreateDefault();
					return result;
				}

				public static MapIndexCollection CreateDemoDefault()
				{
					var result = new MapIndexCollection();
					result.MapByDate = MapCollection.CreateDemoDefault();
					result.MapByLocation = MapCollection.CreateDemoDefault();
					result.MapByAuthor = MapCollection.CreateDemoDefault();
					result.MapByCategory = MapCollection.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = MapByDate;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = MapByLocation;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = MapByAuthor;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = MapByCategory;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public MapCollection MapByDate { get; set; }
			[DataMember]
			public MapCollection MapByLocation { get; set; }
			[DataMember]
			public MapCollection MapByAuthor { get; set; }
			[DataMember]
			public MapCollection MapByCategory { get; set; }
			
			}
			[DataContract]
			public partial class MapResult : IInformationObject
			{
				public MapResult()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "MapResult";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "MapResult", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static MapResult RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveMapResult(relativeLocation, owner);
				}


                public static MapResult RetrieveMapResult(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (MapResult) StorageSupport.RetrieveInformation(relativeLocation, typeof(MapResult), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MapResult));
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

				public static MapResult DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MapResult));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (MapResult) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "MapResult", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "MapResult", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static MapResult CreateDefault()
				{
					var result = new MapResult();
					result.Location = Location.CreateDefault();
					return result;
				}

				public static MapResult CreateDemoDefault()
				{
					var result = new MapResult();
					result.Location = Location.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Location;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public Location Location { get; set; }
			
			}
			[DataContract]
			public partial class MapResultCollection : IInformationObject
			{
				public MapResultCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "MapResultCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "MapResultCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static MapResultCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveMapResultCollection(relativeLocation, owner);
				}


                public static MapResultCollection RetrieveMapResultCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (MapResultCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(MapResultCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MapResultCollection));
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

				public static MapResultCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MapResultCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (MapResultCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "MapResultCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "MapResultCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static MapResultCollection CreateDefault()
				{
					var result = new MapResultCollection();
					return result;
				}

				public static MapResultCollection CreateDemoDefault()
				{
					var result = new MapResultCollection();
					result.CollectionContent.Add(MapResult.CreateDemoDefault());
					result.CollectionContent.Add(MapResult.CreateDemoDefault());
					result.CollectionContent.Add(MapResult.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<MapResult> CollectionContent = new List<MapResult>();

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


			
			}
			[DataContract]
			public partial class MapResultsCollection : IInformationObject
			{
				public MapResultsCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "MapResultsCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "MapResultsCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static MapResultsCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveMapResultsCollection(relativeLocation, owner);
				}


                public static MapResultsCollection RetrieveMapResultsCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (MapResultsCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(MapResultsCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MapResultsCollection));
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

				public static MapResultsCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MapResultsCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (MapResultsCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "MapResultsCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "MapResultsCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static MapResultsCollection CreateDefault()
				{
					var result = new MapResultsCollection();
					result.ResultByDate = MapResultCollection.CreateDefault();
					result.ResultByAuthor = MapResultCollection.CreateDefault();
					result.ResultByProximity = MapResultCollection.CreateDefault();
					return result;
				}

				public static MapResultsCollection CreateDemoDefault()
				{
					var result = new MapResultsCollection();
					result.ResultByDate = MapResultCollection.CreateDemoDefault();
					result.ResultByAuthor = MapResultCollection.CreateDemoDefault();
					result.ResultByProximity = MapResultCollection.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = ResultByDate;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = ResultByAuthor;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = ResultByProximity;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public MapResultCollection ResultByDate { get; set; }
			[DataMember]
			public MapResultCollection ResultByAuthor { get; set; }
			[DataMember]
			public MapResultCollection ResultByProximity { get; set; }
			
			}
			[DataContract]
			public partial class Image : IInformationObject
			{
				public Image()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Image";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Image", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Image RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveImage(relativeLocation, owner);
				}


                public static Image RetrieveImage(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Image) StorageSupport.RetrieveInformation(relativeLocation, typeof(Image), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Image));
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

				public static Image DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Image));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Image) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Image", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "Image", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static Image CreateDefault()
				{
					var result = new Image();
					return result;
				}

				public static Image CreateDemoDefault()
				{
					var result = new Image();
					result.Title = @"Image.Title";

					result.Caption = @"Image.Caption";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Title":
							Title = value;
							break;
						case "Caption":
							Caption = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Title { get; set; }
			[DataMember]
			public string Caption { get; set; }
			
			}
			[DataContract]
			public partial class ImageGroupCollection : IInformationObject
			{
				public ImageGroupCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "ImageGroupCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "ImageGroupCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static ImageGroupCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveImageGroupCollection(relativeLocation, owner);
				}


                public static ImageGroupCollection RetrieveImageGroupCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ImageGroupCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(ImageGroupCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ImageGroupCollection));
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

				public static ImageGroupCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ImageGroupCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ImageGroupCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "ImageGroupCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "ImageGroupCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static ImageGroupCollection CreateDefault()
				{
					var result = new ImageGroupCollection();
					return result;
				}

				public static ImageGroupCollection CreateDemoDefault()
				{
					var result = new ImageGroupCollection();
					result.CollectionContent.Add(ImageGroup.CreateDemoDefault());
					result.CollectionContent.Add(ImageGroup.CreateDemoDefault());
					result.CollectionContent.Add(ImageGroup.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<ImageGroup> CollectionContent = new List<ImageGroup>();

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


			
			}
			[DataContract]
			public partial class ImageGroup : IInformationObject
			{
				public ImageGroup()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "ImageGroup";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "ImageGroup", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static ImageGroup RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveImageGroup(relativeLocation, owner);
				}


                public static ImageGroup RetrieveImageGroup(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ImageGroup) StorageSupport.RetrieveInformation(relativeLocation, typeof(ImageGroup), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ImageGroup));
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

				public static ImageGroup DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ImageGroup));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ImageGroup) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "ImageGroup", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "ImageGroup", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static ImageGroup CreateDefault()
				{
					var result = new ImageGroup();
					result.ImagesCollection = ImagesCollection.CreateDefault();
					return result;
				}

				public static ImageGroup CreateDemoDefault()
				{
					var result = new ImageGroup();
					result.Title = @"ImageGroup.Title";

					result.Description = @"ImageGroup.Description";

					result.ImagesCollection = ImagesCollection.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = ImagesCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Title":
							Title = value;
							break;
						case "Description":
							Description = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Title { get; set; }
			[DataMember]
			public string Description { get; set; }
			[DataMember]
			public ImagesCollection ImagesCollection { get; set; }
			
			}
			[DataContract]
			public partial class ImagesCollection : IInformationObject
			{
				public ImagesCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "ImagesCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "ImagesCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static ImagesCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveImagesCollection(relativeLocation, owner);
				}


                public static ImagesCollection RetrieveImagesCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ImagesCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(ImagesCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ImagesCollection));
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

				public static ImagesCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ImagesCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ImagesCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "ImagesCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "ImagesCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static ImagesCollection CreateDefault()
				{
					var result = new ImagesCollection();
					return result;
				}

				public static ImagesCollection CreateDemoDefault()
				{
					var result = new ImagesCollection();
					result.CollectionContent.Add(Image.CreateDemoDefault());
					result.CollectionContent.Add(Image.CreateDemoDefault());
					result.CollectionContent.Add(Image.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<Image> CollectionContent = new List<Image>();

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


			
			}
			[DataContract]
			public partial class Tooltip : IInformationObject
			{
				public Tooltip()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Tooltip";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Tooltip", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Tooltip RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTooltip(relativeLocation, owner);
				}


                public static Tooltip RetrieveTooltip(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Tooltip) StorageSupport.RetrieveInformation(relativeLocation, typeof(Tooltip), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Tooltip));
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

				public static Tooltip DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Tooltip));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Tooltip) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Tooltip", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "Tooltip", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static Tooltip CreateDefault()
				{
					var result = new Tooltip();
					return result;
				}

				public static Tooltip CreateDemoDefault()
				{
					var result = new Tooltip();
					result.TooltipText = @"Tooltip.TooltipText";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "TooltipText":
							TooltipText = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string TooltipText { get; set; }
			
			}
			[DataContract]
			public partial class SocialPanelCollection : IInformationObject
			{
				public SocialPanelCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "SocialPanelCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "SocialPanelCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static SocialPanelCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveSocialPanelCollection(relativeLocation, owner);
				}


                public static SocialPanelCollection RetrieveSocialPanelCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (SocialPanelCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(SocialPanelCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SocialPanelCollection));
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

				public static SocialPanelCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SocialPanelCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (SocialPanelCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "SocialPanelCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "SocialPanelCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static SocialPanelCollection CreateDefault()
				{
					var result = new SocialPanelCollection();
					return result;
				}

				public static SocialPanelCollection CreateDemoDefault()
				{
					var result = new SocialPanelCollection();
					result.CollectionContent.Add(SocialPanel.CreateDemoDefault());
					result.CollectionContent.Add(SocialPanel.CreateDemoDefault());
					result.CollectionContent.Add(SocialPanel.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<SocialPanel> CollectionContent = new List<SocialPanel>();

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


			
			}
			[DataContract]
			public partial class SocialPanel : IInformationObject
			{
				public SocialPanel()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "SocialPanel";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "SocialPanel", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static SocialPanel RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveSocialPanel(relativeLocation, owner);
				}


                public static SocialPanel RetrieveSocialPanel(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (SocialPanel) StorageSupport.RetrieveInformation(relativeLocation, typeof(SocialPanel), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SocialPanel));
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

				public static SocialPanel DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SocialPanel));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (SocialPanel) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "SocialPanel", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "SocialPanel", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static SocialPanel CreateDefault()
				{
					var result = new SocialPanel();
					result.SocialFilter = Filter.CreateDefault();
					return result;
				}

				public static SocialPanel CreateDemoDefault()
				{
					var result = new SocialPanel();
					result.SocialFilter = Filter.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = SocialFilter;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public Filter SocialFilter { get; set; }
			
			}
			[DataContract]
			public partial class Longitude : IInformationObject
			{
				public Longitude()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Longitude";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Longitude", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Longitude RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveLongitude(relativeLocation, owner);
				}


                public static Longitude RetrieveLongitude(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Longitude) StorageSupport.RetrieveInformation(relativeLocation, typeof(Longitude), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Longitude));
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

				public static Longitude DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Longitude));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Longitude) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Longitude", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "Longitude", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static Longitude CreateDefault()
				{
					var result = new Longitude();
					return result;
				}

				public static Longitude CreateDemoDefault()
				{
					var result = new Longitude();
					result.TextValue = @"Longitude.TextValue";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "TextValue":
							TextValue = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string TextValue { get; set; }
			
			}
			[DataContract]
			public partial class Latitude : IInformationObject
			{
				public Latitude()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Latitude";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Latitude", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Latitude RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveLatitude(relativeLocation, owner);
				}


                public static Latitude RetrieveLatitude(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Latitude) StorageSupport.RetrieveInformation(relativeLocation, typeof(Latitude), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Latitude));
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

				public static Latitude DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Latitude));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Latitude) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Latitude", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "Latitude", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static Latitude CreateDefault()
				{
					var result = new Latitude();
					return result;
				}

				public static Latitude CreateDemoDefault()
				{
					var result = new Latitude();
					result.TextValue = @"Latitude.TextValue";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "TextValue":
							TextValue = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string TextValue { get; set; }
			
			}
			[DataContract]
			public partial class Location : IInformationObject
			{
				public Location()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Location";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Location", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Location RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveLocation(relativeLocation, owner);
				}


                public static Location RetrieveLocation(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Location) StorageSupport.RetrieveInformation(relativeLocation, typeof(Location), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Location));
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

				public static Location DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Location));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Location) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Location", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "Location", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static Location CreateDefault()
				{
					var result = new Location();
					result.Longitude = Longitude.CreateDefault();
					result.Latitude = Latitude.CreateDefault();
					return result;
				}

				public static Location CreateDemoDefault()
				{
					var result = new Location();
					result.LocationName = @"Location.LocationName";

					result.Longitude = Longitude.CreateDemoDefault();
					result.Latitude = Latitude.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Longitude;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = Latitude;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "LocationName":
							LocationName = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string LocationName { get; set; }
			[DataMember]
			public Longitude Longitude { get; set; }
			[DataMember]
			public Latitude Latitude { get; set; }
			
			}
			[DataContract]
			public partial class LocationCollection : IInformationObject
			{
				public LocationCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "LocationCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "LocationCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static LocationCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveLocationCollection(relativeLocation, owner);
				}


                public static LocationCollection RetrieveLocationCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (LocationCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(LocationCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(LocationCollection));
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

				public static LocationCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(LocationCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (LocationCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "LocationCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "LocationCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static LocationCollection CreateDefault()
				{
					var result = new LocationCollection();
					return result;
				}

				public static LocationCollection CreateDemoDefault()
				{
					var result = new LocationCollection();
					result.CollectionContent.Add(Location.CreateDemoDefault());
					result.CollectionContent.Add(Location.CreateDemoDefault());
					result.CollectionContent.Add(Location.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<Location> CollectionContent = new List<Location>();

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


			
			}
			[DataContract]
			public partial class Date : IInformationObject
			{
				public Date()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Date";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Date", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Date RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveDate(relativeLocation, owner);
				}


                public static Date RetrieveDate(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Date) StorageSupport.RetrieveInformation(relativeLocation, typeof(Date), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Date));
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

				public static Date DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Date));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Date) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Date", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "Date", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static Date CreateDefault()
				{
					var result = new Date();
					return result;
				}

				public static Date CreateDemoDefault()
				{
					var result = new Date();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Day":
							Day = DateTime.Parse(value);
							break;
						case "Week":
							Week = DateTime.Parse(value);
							break;
						case "Month":
							Month = DateTime.Parse(value);
							break;
						case "Year":
							Year = DateTime.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public DateTime Day { get; set; }
			[DataMember]
			public DateTime Week { get; set; }
			[DataMember]
			public DateTime Month { get; set; }
			[DataMember]
			public DateTime Year { get; set; }
			
			}
			[DataContract]
			public partial class Sex : IInformationObject
			{
				public Sex()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Sex";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Sex", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Sex RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveSex(relativeLocation, owner);
				}


                public static Sex RetrieveSex(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Sex) StorageSupport.RetrieveInformation(relativeLocation, typeof(Sex), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Sex));
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

				public static Sex DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Sex));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Sex) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Sex", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "Sex", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static Sex CreateDefault()
				{
					var result = new Sex();
					return result;
				}

				public static Sex CreateDemoDefault()
				{
					var result = new Sex();
					result.SexText = @"Sex.SexText";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "SexText":
							SexText = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string SexText { get; set; }
			
			}
			[DataContract]
			public partial class OBSAddress : IInformationObject
			{
				public OBSAddress()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "OBSAddress";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "OBSAddress", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static OBSAddress RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveOBSAddress(relativeLocation, owner);
				}


                public static OBSAddress RetrieveOBSAddress(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (OBSAddress) StorageSupport.RetrieveInformation(relativeLocation, typeof(OBSAddress), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(OBSAddress));
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

				public static OBSAddress DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(OBSAddress));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (OBSAddress) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "OBSAddress", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "OBSAddress", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static OBSAddress CreateDefault()
				{
					var result = new OBSAddress();
					return result;
				}

				public static OBSAddress CreateDemoDefault()
				{
					var result = new OBSAddress();
					result.StreetName = @"OBSAddress.StreetName";

					result.BuildingNumber = @"OBSAddress.BuildingNumber";

					result.PostOfficeBox = @"OBSAddress.PostOfficeBox";

					result.PostalCode = @"OBSAddress.PostalCode";

					result.Municipality = @"OBSAddress.Municipality";

					result.Region = @"OBSAddress.Region";

					result.Province = @"OBSAddress.Province";

					result.state = @"OBSAddress.state";

					result.Country = @"OBSAddress.Country";

					result.Continent = @"OBSAddress.Continent";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "StreetName":
							StreetName = value;
							break;
						case "BuildingNumber":
							BuildingNumber = value;
							break;
						case "PostOfficeBox":
							PostOfficeBox = value;
							break;
						case "PostalCode":
							PostalCode = value;
							break;
						case "Municipality":
							Municipality = value;
							break;
						case "Region":
							Region = value;
							break;
						case "Province":
							Province = value;
							break;
						case "state":
							state = value;
							break;
						case "Country":
							Country = value;
							break;
						case "Continent":
							Continent = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string StreetName { get; set; }
			[DataMember]
			public string BuildingNumber { get; set; }
			[DataMember]
			public string PostOfficeBox { get; set; }
			[DataMember]
			public string PostalCode { get; set; }
			[DataMember]
			public string Municipality { get; set; }
			[DataMember]
			public string Region { get; set; }
			[DataMember]
			public string Province { get; set; }
			[DataMember]
			public string state { get; set; }
			[DataMember]
			public string Country { get; set; }
			[DataMember]
			public string Continent { get; set; }
			
			}
			[DataContract]
			public partial class Identity : IInformationObject
			{
				public Identity()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Identity";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Identity", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Identity RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveIdentity(relativeLocation, owner);
				}


                public static Identity RetrieveIdentity(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Identity) StorageSupport.RetrieveInformation(relativeLocation, typeof(Identity), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Identity));
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

				public static Identity DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Identity));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Identity) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Identity", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "Identity", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static Identity CreateDefault()
				{
					var result = new Identity();
					result.Sex = Sex.CreateDefault();
					result.Birthday = Date.CreateDefault();
					return result;
				}

				public static Identity CreateDemoDefault()
				{
					var result = new Identity();
					result.FirstName = @"Identity.FirstName";

					result.LastName = @"Identity.LastName";

					result.Initials = @"Identity.Initials";

					result.Sex = Sex.CreateDemoDefault();
					result.Birthday = Date.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Sex;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = Birthday;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "FirstName":
							FirstName = value;
							break;
						case "LastName":
							LastName = value;
							break;
						case "Initials":
							Initials = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string FirstName { get; set; }
			[DataMember]
			public string LastName { get; set; }
			[DataMember]
			public string Initials { get; set; }
			[DataMember]
			public Sex Sex { get; set; }
			[DataMember]
			public Date Birthday { get; set; }
			
			}
			[DataContract]
			public partial class ImageVideoSoundVectorRaw : IInformationObject
			{
				public ImageVideoSoundVectorRaw()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "ImageVideoSoundVectorRaw";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "ImageVideoSoundVectorRaw", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static ImageVideoSoundVectorRaw RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveImageVideoSoundVectorRaw(relativeLocation, owner);
				}


                public static ImageVideoSoundVectorRaw RetrieveImageVideoSoundVectorRaw(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (ImageVideoSoundVectorRaw) StorageSupport.RetrieveInformation(relativeLocation, typeof(ImageVideoSoundVectorRaw), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ImageVideoSoundVectorRaw));
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

				public static ImageVideoSoundVectorRaw DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ImageVideoSoundVectorRaw));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ImageVideoSoundVectorRaw) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "ImageVideoSoundVectorRaw", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "ImageVideoSoundVectorRaw", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static ImageVideoSoundVectorRaw CreateDefault()
				{
					var result = new ImageVideoSoundVectorRaw();
					return result;
				}

				public static ImageVideoSoundVectorRaw CreateDemoDefault()
				{
					var result = new ImageVideoSoundVectorRaw();
					result.Vector = @"ImageVideoSoundVectorRaw.Vector
ImageVideoSoundVectorRaw.Vector
ImageVideoSoundVectorRaw.Vector
ImageVideoSoundVectorRaw.Vector
ImageVideoSoundVectorRaw.Vector
";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Vector":
							Vector = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public byte[] Image { get; set; }
			[DataMember]
			public byte[] Video { get; set; }
			[DataMember]
			public byte[] Sound { get; set; }
			[DataMember]
			public string Vector { get; set; }
			[DataMember]
			public byte[] Raw { get; set; }
			
			}
			[DataContract]
			public partial class Category : IInformationObject
			{
				public Category()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Category";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Category", id).Replace("\\", "/");
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


                public static Category RetrieveCategory(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Category) StorageSupport.RetrieveInformation(relativeLocation, typeof(Category), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Category", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "Category", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static Category CreateDefault()
				{
					var result = new Category();
					return result;
				}

				public static Category CreateDemoDefault()
				{
					var result = new Category();
					result.TextValue = @"Category.TextValue";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "TextValue":
							TextValue = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string TextValue { get; set; }
			
			}
			[DataContract]
			public partial class SubscriptionCollection : IInformationObject
			{
				public SubscriptionCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "SubscriptionCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "SubscriptionCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static SubscriptionCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveSubscriptionCollection(relativeLocation, owner);
				}


                public static SubscriptionCollection RetrieveSubscriptionCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (SubscriptionCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(SubscriptionCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SubscriptionCollection));
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

				public static SubscriptionCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SubscriptionCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (SubscriptionCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "SubscriptionCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "SubscriptionCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static SubscriptionCollection CreateDefault()
				{
					var result = new SubscriptionCollection();
					return result;
				}

				public static SubscriptionCollection CreateDemoDefault()
				{
					var result = new SubscriptionCollection();
					result.CollectionContent.Add(Subscription.CreateDemoDefault());
					result.CollectionContent.Add(Subscription.CreateDemoDefault());
					result.CollectionContent.Add(Subscription.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<Subscription> CollectionContent = new List<Subscription>();

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


			
			}
			[DataContract]
			public partial class Subscription : IInformationObject
			{
				public Subscription()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Subscription";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Subscription", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Subscription RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveSubscription(relativeLocation, owner);
				}


                public static Subscription RetrieveSubscription(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Subscription) StorageSupport.RetrieveInformation(relativeLocation, typeof(Subscription), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Subscription));
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

				public static Subscription DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Subscription));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Subscription) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Subscription", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "Subscription", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static Subscription CreateDefault()
				{
					var result = new Subscription();
					return result;
				}

				public static Subscription CreateDemoDefault()
				{
					var result = new Subscription();
					result.TargetItemID = @"Subscription.TargetItemID";

					result.TargetObjectName = @"Subscription.TargetObjectName";

					result.TargetItemName = @"Subscription.TargetItemName";

					result.TargetRelativeLocation = @"Subscription.TargetRelativeLocation";

					result.SubscriberID = @"Subscription.SubscriberID";

					result.SubscriberRelativeLocation = @"Subscription.SubscriberRelativeLocation";

					result.OperationActionName = @"Subscription.OperationActionName";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Priority":
							Priority = long.Parse(value);
							break;
						case "TargetItemID":
							TargetItemID = value;
							break;
						case "TargetObjectName":
							TargetObjectName = value;
							break;
						case "TargetItemName":
							TargetItemName = value;
							break;
						case "TargetRelativeLocation":
							TargetRelativeLocation = value;
							break;
						case "SubscriberID":
							SubscriberID = value;
							break;
						case "SubscriberRelativeLocation":
							SubscriberRelativeLocation = value;
							break;
						case "OperationActionName":
							OperationActionName = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public long Priority { get; set; }
			[DataMember]
			public string TargetItemID { get; set; }
			[DataMember]
			public string TargetObjectName { get; set; }
			[DataMember]
			public string TargetItemName { get; set; }
			[DataMember]
			public string TargetRelativeLocation { get; set; }
			[DataMember]
			public string SubscriberID { get; set; }
			[DataMember]
			public string SubscriberRelativeLocation { get; set; }
			[DataMember]
			public string OperationActionName { get; set; }
			
			}
			[DataContract]
			public partial class QueueEnvelope : IInformationObject
			{
				public QueueEnvelope()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "QueueEnvelope";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "QueueEnvelope", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static QueueEnvelope RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveQueueEnvelope(relativeLocation, owner);
				}


                public static QueueEnvelope RetrieveQueueEnvelope(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (QueueEnvelope) StorageSupport.RetrieveInformation(relativeLocation, typeof(QueueEnvelope), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(QueueEnvelope));
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

				public static QueueEnvelope DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(QueueEnvelope));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (QueueEnvelope) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "QueueEnvelope", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "QueueEnvelope", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }


				private object FindFromObjectTree(string objectId)
				{
					{
						var item = SubscriberUpdateOperation;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = UpdateWebContentOperation;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = ErrorContent;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public SubscriberUpdateOperation SubscriberUpdateOperation { get; set; }
			[DataMember]
			public UpdateWebContentOperation UpdateWebContentOperation { get; set; }
			[DataMember]
			public SystemError ErrorContent { get; set; }
			
			}
			[DataContract]
			public partial class SystemError : IInformationObject
			{
				public SystemError()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "SystemError";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "SystemError", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static SystemError RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveSystemError(relativeLocation, owner);
				}


                public static SystemError RetrieveSystemError(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (SystemError) StorageSupport.RetrieveInformation(relativeLocation, typeof(SystemError), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SystemError));
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

				public static SystemError DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SystemError));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (SystemError) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "SystemError", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "SystemError", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static SystemError CreateDefault()
				{
					var result = new SystemError();
					result.SystemErrorItems = SystemErrorItemCollection.CreateDefault();
					return result;
				}

				public static SystemError CreateDemoDefault()
				{
					var result = new SystemError();
					result.ErrorTitle = @"SystemError.ErrorTitle";

					result.SystemErrorItems = SystemErrorItemCollection.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = SystemErrorItems;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "ErrorTitle":
							ErrorTitle = value;
							break;
						case "OccurredAt":
							OccurredAt = DateTime.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string ErrorTitle { get; set; }
			[DataMember]
			public DateTime OccurredAt { get; set; }
			[DataMember]
			public SystemErrorItemCollection SystemErrorItems { get; set; }
			
			}
			[DataContract]
			public partial class SystemErrorItem : IInformationObject
			{
				public SystemErrorItem()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "SystemErrorItem";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "SystemErrorItem", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static SystemErrorItem RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveSystemErrorItem(relativeLocation, owner);
				}


                public static SystemErrorItem RetrieveSystemErrorItem(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (SystemErrorItem) StorageSupport.RetrieveInformation(relativeLocation, typeof(SystemErrorItem), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SystemErrorItem));
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

				public static SystemErrorItem DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SystemErrorItem));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (SystemErrorItem) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "SystemErrorItem", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "SystemErrorItem", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static SystemErrorItem CreateDefault()
				{
					var result = new SystemErrorItem();
					return result;
				}

				public static SystemErrorItem CreateDemoDefault()
				{
					var result = new SystemErrorItem();
					result.ShortDescription = @"SystemErrorItem.ShortDescription";

					result.LongDescription = @"SystemErrorItem.LongDescription
SystemErrorItem.LongDescription
SystemErrorItem.LongDescription
SystemErrorItem.LongDescription
SystemErrorItem.LongDescription
";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "ShortDescription":
							ShortDescription = value;
							break;
						case "LongDescription":
							LongDescription = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string ShortDescription { get; set; }
			[DataMember]
			public string LongDescription { get; set; }
			
			}
			[DataContract]
			public partial class SystemErrorItemCollection : IInformationObject
			{
				public SystemErrorItemCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "SystemErrorItemCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "SystemErrorItemCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static SystemErrorItemCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveSystemErrorItemCollection(relativeLocation, owner);
				}


                public static SystemErrorItemCollection RetrieveSystemErrorItemCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (SystemErrorItemCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(SystemErrorItemCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SystemErrorItemCollection));
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

				public static SystemErrorItemCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SystemErrorItemCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (SystemErrorItemCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "SystemErrorItemCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "SystemErrorItemCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static SystemErrorItemCollection CreateDefault()
				{
					var result = new SystemErrorItemCollection();
					return result;
				}

				public static SystemErrorItemCollection CreateDemoDefault()
				{
					var result = new SystemErrorItemCollection();
					result.CollectionContent.Add(SystemErrorItem.CreateDemoDefault());
					result.CollectionContent.Add(SystemErrorItem.CreateDemoDefault());
					result.CollectionContent.Add(SystemErrorItem.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<SystemErrorItem> CollectionContent = new List<SystemErrorItem>();

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


			
			}
			[DataContract]
			public partial class InformationSource : IInformationObject
			{
				public InformationSource()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "InformationSource";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "InformationSource", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static InformationSource RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveInformationSource(relativeLocation, owner);
				}


                public static InformationSource RetrieveInformationSource(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (InformationSource) StorageSupport.RetrieveInformation(relativeLocation, typeof(InformationSource), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InformationSource));
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

				public static InformationSource DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InformationSource));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InformationSource) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "InformationSource", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "InformationSource", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static InformationSource CreateDefault()
				{
					var result = new InformationSource();
					return result;
				}

				public static InformationSource CreateDemoDefault()
				{
					var result = new InformationSource();
					result.SourceName = @"InformationSource.SourceName";

					result.SourceLocation = @"InformationSource.SourceLocation";

					result.SourceType = @"InformationSource.SourceType";

					result.SourceInformationObjectType = @"InformationSource.SourceInformationObjectType";

					result.SourceETag = @"InformationSource.SourceETag";

					result.SourceMD5 = @"InformationSource.SourceMD5";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "SourceName":
							SourceName = value;
							break;
						case "SourceLocation":
							SourceLocation = value;
							break;
						case "SourceType":
							SourceType = value;
							break;
						case "SourceInformationObjectType":
							SourceInformationObjectType = value;
							break;
						case "SourceETag":
							SourceETag = value;
							break;
						case "SourceMD5":
							SourceMD5 = value;
							break;
						case "SourceLastModified":
							SourceLastModified = DateTime.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string SourceName { get; set; }
			[DataMember]
			public string SourceLocation { get; set; }
			[DataMember]
			public string SourceType { get; set; }
			[DataMember]
			public string SourceInformationObjectType { get; set; }
			[DataMember]
			public string SourceETag { get; set; }
			[DataMember]
			public string SourceMD5 { get; set; }
			[DataMember]
			public DateTime SourceLastModified { get; set; }
			
			}
			[DataContract]
			public partial class InformationSourceCollection : IInformationObject
			{
				public InformationSourceCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "InformationSourceCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "InformationSourceCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static InformationSourceCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveInformationSourceCollection(relativeLocation, owner);
				}


                public static InformationSourceCollection RetrieveInformationSourceCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (InformationSourceCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(InformationSourceCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InformationSourceCollection));
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

				public static InformationSourceCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InformationSourceCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InformationSourceCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "InformationSourceCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "InformationSourceCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static InformationSourceCollection CreateDefault()
				{
					var result = new InformationSourceCollection();
					return result;
				}

				public static InformationSourceCollection CreateDemoDefault()
				{
					var result = new InformationSourceCollection();
					result.CollectionContent.Add(InformationSource.CreateDemoDefault());
					result.CollectionContent.Add(InformationSource.CreateDemoDefault());
					result.CollectionContent.Add(InformationSource.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<InformationSource> CollectionContent = new List<InformationSource>();

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


			
			}
			[DataContract]
			public partial class UpdateWebContentOperation : IInformationObject
			{
				public UpdateWebContentOperation()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "UpdateWebContentOperation";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "UpdateWebContentOperation", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static UpdateWebContentOperation RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveUpdateWebContentOperation(relativeLocation, owner);
				}


                public static UpdateWebContentOperation RetrieveUpdateWebContentOperation(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (UpdateWebContentOperation) StorageSupport.RetrieveInformation(relativeLocation, typeof(UpdateWebContentOperation), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(UpdateWebContentOperation));
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

				public static UpdateWebContentOperation DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(UpdateWebContentOperation));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (UpdateWebContentOperation) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "UpdateWebContentOperation", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "UpdateWebContentOperation", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static UpdateWebContentOperation CreateDefault()
				{
					var result = new UpdateWebContentOperation();
					result.Handlers = UpdateWebContentHandlerCollection.CreateDefault();
					return result;
				}

				public static UpdateWebContentOperation CreateDemoDefault()
				{
					var result = new UpdateWebContentOperation();
					result.SourceContainerName = @"UpdateWebContentOperation.SourceContainerName";

					result.SourcePathRoot = @"UpdateWebContentOperation.SourcePathRoot";

					result.TargetContainerName = @"UpdateWebContentOperation.TargetContainerName";

					result.TargetPathRoot = @"UpdateWebContentOperation.TargetPathRoot";

					result.Handlers = UpdateWebContentHandlerCollection.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Handlers;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "SourceContainerName":
							SourceContainerName = value;
							break;
						case "SourcePathRoot":
							SourcePathRoot = value;
							break;
						case "TargetContainerName":
							TargetContainerName = value;
							break;
						case "TargetPathRoot":
							TargetPathRoot = value;
							break;
						case "RenderWhileSync":
							RenderWhileSync = bool.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string SourceContainerName { get; set; }
			[DataMember]
			public string SourcePathRoot { get; set; }
			[DataMember]
			public string TargetContainerName { get; set; }
			[DataMember]
			public string TargetPathRoot { get; set; }
			[DataMember]
			public bool RenderWhileSync { get; set; }
			[DataMember]
			public UpdateWebContentHandlerCollection Handlers { get; set; }
			
			}
			[DataContract]
			public partial class UpdateWebContentHandlerItem : IInformationObject
			{
				public UpdateWebContentHandlerItem()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "UpdateWebContentHandlerItem";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "UpdateWebContentHandlerItem", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static UpdateWebContentHandlerItem RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveUpdateWebContentHandlerItem(relativeLocation, owner);
				}


                public static UpdateWebContentHandlerItem RetrieveUpdateWebContentHandlerItem(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (UpdateWebContentHandlerItem) StorageSupport.RetrieveInformation(relativeLocation, typeof(UpdateWebContentHandlerItem), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(UpdateWebContentHandlerItem));
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

				public static UpdateWebContentHandlerItem DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(UpdateWebContentHandlerItem));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (UpdateWebContentHandlerItem) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "UpdateWebContentHandlerItem", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "UpdateWebContentHandlerItem", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static UpdateWebContentHandlerItem CreateDefault()
				{
					var result = new UpdateWebContentHandlerItem();
					return result;
				}

				public static UpdateWebContentHandlerItem CreateDemoDefault()
				{
					var result = new UpdateWebContentHandlerItem();
					result.InformationTypeName = @"UpdateWebContentHandlerItem.InformationTypeName";

					result.OptionName = @"UpdateWebContentHandlerItem.OptionName";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "InformationTypeName":
							InformationTypeName = value;
							break;
						case "OptionName":
							OptionName = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string InformationTypeName { get; set; }
			[DataMember]
			public string OptionName { get; set; }
			
			}
			[DataContract]
			public partial class UpdateWebContentHandlerCollection : IInformationObject
			{
				public UpdateWebContentHandlerCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "UpdateWebContentHandlerCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "UpdateWebContentHandlerCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static UpdateWebContentHandlerCollection RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveUpdateWebContentHandlerCollection(relativeLocation, owner);
				}


                public static UpdateWebContentHandlerCollection RetrieveUpdateWebContentHandlerCollection(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (UpdateWebContentHandlerCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(UpdateWebContentHandlerCollection), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(UpdateWebContentHandlerCollection));
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

				public static UpdateWebContentHandlerCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(UpdateWebContentHandlerCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (UpdateWebContentHandlerCollection) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "UpdateWebContentHandlerCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "UpdateWebContentHandlerCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				
		
				public static UpdateWebContentHandlerCollection CreateDefault()
				{
					var result = new UpdateWebContentHandlerCollection();
					return result;
				}

				public static UpdateWebContentHandlerCollection CreateDemoDefault()
				{
					var result = new UpdateWebContentHandlerCollection();
					result.CollectionContent.Add(UpdateWebContentHandlerItem.CreateDemoDefault());
					result.CollectionContent.Add(UpdateWebContentHandlerItem.CreateDemoDefault());
					result.CollectionContent.Add(UpdateWebContentHandlerItem.CreateDemoDefault());
					return result;
				}

		
				[DataMember] public List<UpdateWebContentHandlerItem> CollectionContent = new List<UpdateWebContentHandlerItem>();

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


			
			}
			[DataContract]
			public partial class SubscriberInput : IInformationObject
			{
				public SubscriberInput()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "SubscriberInput";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "SubscriberInput", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static SubscriberInput RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveSubscriberInput(relativeLocation, owner);
				}


                public static SubscriberInput RetrieveSubscriberInput(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (SubscriberInput) StorageSupport.RetrieveInformation(relativeLocation, typeof(SubscriberInput), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SubscriberInput));
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

				public static SubscriberInput DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SubscriberInput));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (SubscriberInput) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "SubscriberInput", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "SubscriberInput", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static SubscriberInput CreateDefault()
				{
					var result = new SubscriberInput();
					return result;
				}

				public static SubscriberInput CreateDemoDefault()
				{
					var result = new SubscriberInput();
					result.InputRelativeLocation = @"SubscriberInput.InputRelativeLocation";

					result.InformationObjectName = @"SubscriberInput.InformationObjectName";

					result.InformationItemName = @"SubscriberInput.InformationItemName";

					result.SubscriberRelativeLocation = @"SubscriberInput.SubscriberRelativeLocation";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "InputRelativeLocation":
							InputRelativeLocation = value;
							break;
						case "InformationObjectName":
							InformationObjectName = value;
							break;
						case "InformationItemName":
							InformationItemName = value;
							break;
						case "SubscriberRelativeLocation":
							SubscriberRelativeLocation = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string InputRelativeLocation { get; set; }
			[DataMember]
			public string InformationObjectName { get; set; }
			[DataMember]
			public string InformationItemName { get; set; }
			[DataMember]
			public string SubscriberRelativeLocation { get; set; }
			
			}
			[DataContract]
			public partial class SubscriberUpdateOperation : IInformationObject
			{
				public SubscriberUpdateOperation()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "SubscriberUpdateOperation";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "SubscriberUpdateOperation", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static SubscriberUpdateOperation RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveSubscriberUpdateOperation(relativeLocation, owner);
				}


                public static SubscriberUpdateOperation RetrieveSubscriberUpdateOperation(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (SubscriberUpdateOperation) StorageSupport.RetrieveInformation(relativeLocation, typeof(SubscriberUpdateOperation), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SubscriberUpdateOperation));
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

				public static SubscriberUpdateOperation DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SubscriberUpdateOperation));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (SubscriberUpdateOperation) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "SubscriberUpdateOperation", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "SubscriberUpdateOperation", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static SubscriberUpdateOperation CreateDefault()
				{
					var result = new SubscriberUpdateOperation();
					result.OperationParameters = SubscriberInput.CreateDefault();
					return result;
				}

				public static SubscriberUpdateOperation CreateDemoDefault()
				{
					var result = new SubscriberUpdateOperation();
					result.TargetOwnerID = @"SubscriberUpdateOperation.TargetOwnerID";

					result.SubscriberOwnerID = @"SubscriberUpdateOperation.SubscriberOwnerID";

					result.OperationParameters = SubscriberInput.CreateDemoDefault();
					result.OperationName = @"SubscriberUpdateOperation.OperationName";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = OperationParameters;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "TargetOwnerID":
							TargetOwnerID = value;
							break;
						case "SubscriberOwnerID":
							SubscriberOwnerID = value;
							break;
						case "OperationName":
							OperationName = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string TargetOwnerID { get; set; }
			[DataMember]
			public string SubscriberOwnerID { get; set; }
			[DataMember]
			public SubscriberInput OperationParameters { get; set; }
			[DataMember]
			public string OperationName { get; set; }
			
			}
			[DataContract]
			public partial class Monitor : IInformationObject
			{
				public Monitor()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Monitor";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Monitor", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Monitor RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveMonitor(relativeLocation, owner);
				}


                public static Monitor RetrieveMonitor(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Monitor) StorageSupport.RetrieveInformation(relativeLocation, typeof(Monitor), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Monitor));
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

				public static Monitor DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Monitor));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Monitor) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Monitor", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "Monitor", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static Monitor CreateDefault()
				{
					var result = new Monitor();
					return result;
				}

				public static Monitor CreateDemoDefault()
				{
					var result = new Monitor();
					result.TargetObjectName = @"Monitor.TargetObjectName";

					result.TargetItemName = @"Monitor.TargetItemName";

					result.MonitoringCycleFrequencyUnit = @"Monitor.MonitoringCycleFrequencyUnit";

					result.CustomMonitoringCycleOperationName = @"Monitor.CustomMonitoringCycleOperationName";

					result.OperationActionName = @"Monitor.OperationActionName";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "TargetObjectName":
							TargetObjectName = value;
							break;
						case "TargetItemName":
							TargetItemName = value;
							break;
						case "MonitoringUtcTimeStampToStart":
							MonitoringUtcTimeStampToStart = DateTime.Parse(value);
							break;
						case "MonitoringCycleFrequencyUnit":
							MonitoringCycleFrequencyUnit = value;
							break;
						case "MonitoringCycleEveryXthOfUnit":
							MonitoringCycleEveryXthOfUnit = long.Parse(value);
							break;
						case "CustomMonitoringCycleOperationName":
							CustomMonitoringCycleOperationName = value;
							break;
						case "OperationActionName":
							OperationActionName = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string TargetObjectName { get; set; }
			[DataMember]
			public string TargetItemName { get; set; }
			[DataMember]
			public DateTime MonitoringUtcTimeStampToStart { get; set; }
			[DataMember]
			public string MonitoringCycleFrequencyUnit { get; set; }
			[DataMember]
			public long MonitoringCycleEveryXthOfUnit { get; set; }
			[DataMember]
			public string CustomMonitoringCycleOperationName { get; set; }
			[DataMember]
			public string OperationActionName { get; set; }
			
			}
			[DataContract]
			public partial class IconTitleDescription : IInformationObject
			{
				public IconTitleDescription()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "IconTitleDescription";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "IconTitleDescription", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IconTitleDescription RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveIconTitleDescription(relativeLocation, owner);
				}


                public static IconTitleDescription RetrieveIconTitleDescription(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (IconTitleDescription) StorageSupport.RetrieveInformation(relativeLocation, typeof(IconTitleDescription), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(IconTitleDescription));
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

				public static IconTitleDescription DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(IconTitleDescription));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (IconTitleDescription) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "IconTitleDescription", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "IconTitleDescription", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static IconTitleDescription CreateDefault()
				{
					var result = new IconTitleDescription();
					return result;
				}

				public static IconTitleDescription CreateDemoDefault()
				{
					var result = new IconTitleDescription();
					result.Title = @"IconTitleDescription.Title";

					result.Description = @"IconTitleDescription.Description
IconTitleDescription.Description
IconTitleDescription.Description
IconTitleDescription.Description
IconTitleDescription.Description
";

				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}

				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Title":
							Title = value;
							break;
						case "Description":
							Description = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public byte[] Icon { get; set; }
			[DataMember]
			public string Title { get; set; }
			[DataMember]
			public string Description { get; set; }
			
			}
			[DataContract]
			public partial class AboutAGIApplications : IInformationObject
			{
				public AboutAGIApplications()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "AboutAGIApplications";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "AboutAGIApplications", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static AboutAGIApplications RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAboutAGIApplications(relativeLocation, owner);
				}


                public static AboutAGIApplications RetrieveAboutAGIApplications(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (AboutAGIApplications) StorageSupport.RetrieveInformation(relativeLocation, typeof(AboutAGIApplications), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AboutAGIApplications));
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

				public static AboutAGIApplications DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AboutAGIApplications));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (AboutAGIApplications) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AboutAGIApplications", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "AboutAGIApplications", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }



				public static AboutAGIApplications CreateDefault()
				{
					var result = new AboutAGIApplications();
					result.BuiltForAnybody = IconTitleDescription.CreateDefault();
					result.ForAllPeople = IconTitleDescription.CreateDefault();
					return result;
				}

				public static AboutAGIApplications CreateDemoDefault()
				{
					var result = new AboutAGIApplications();
					result.BuiltForAnybody = IconTitleDescription.CreateDemoDefault();
					result.ForAllPeople = IconTitleDescription.CreateDemoDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = BuiltForAnybody;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = ForAllPeople;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
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
			public IconTitleDescription BuiltForAnybody { get; set; }
			[DataMember]
			public IconTitleDescription ForAllPeople { get; set; }
			
			}
			[DataContract]
			public partial class Icon : IInformationObject
			{
				public Icon()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Icon";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Icon", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Icon RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveIcon(relativeLocation, owner);
				}


                public static Icon RetrieveIcon(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (Icon) StorageSupport.RetrieveInformation(relativeLocation, typeof(Icon), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Icon));
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

				public static Icon DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Icon));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Icon) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Icon", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "Icon", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }


				// Properties to map to handle the file: Icon.png..png
				// TODO: Handle object collections
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				public static Icon CreateDefault()
				{
					var result = new Icon();
					return result;
				}

				public static Icon CreateDemoDefault()
				{
					var result = new Icon();
					return result;
				}


			
			}
			[DataContract]
			public partial class WebPageTemplate : IInformationObject
			{
				public WebPageTemplate()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "WebPageTemplate";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "WebPageTemplate", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static WebPageTemplate RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveWebPageTemplate(relativeLocation, owner);
				}


                public static WebPageTemplate RetrieveWebPageTemplate(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (WebPageTemplate) StorageSupport.RetrieveInformation(relativeLocation, typeof(WebPageTemplate), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(WebPageTemplate));
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

				public static WebPageTemplate DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(WebPageTemplate));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (WebPageTemplate) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "WebPageTemplate", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "WebPageTemplate", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }


				// Properties to map to handle the file: WebPageTemplate.html..html
				// TODO: Handle object collections
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				public static WebPageTemplate CreateDefault()
				{
					var result = new WebPageTemplate();
					return result;
				}

				public static WebPageTemplate CreateDemoDefault()
				{
					var result = new WebPageTemplate();
					return result;
				}


			
			}
			[DataContract]
			public partial class WebPage : IInformationObject
			{
				public WebPage()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "WebPage";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "WebPage", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static WebPage RetrieveFromDefaultLocation(string id, IContainerOwner owner = null)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveWebPage(relativeLocation, owner);
				}


                public static WebPage RetrieveWebPage(string relativeLocation, IContainerOwner owner = null)
                {
                    var result = (WebPage) StorageSupport.RetrieveInformation(relativeLocation, typeof(WebPage), null, owner);
                    return result;
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
                        if (key.StartsWith("RootObject"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
                        string objectID = key.Substring(0, indexOfUnderscore);
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
                        object targetObject = FindObjectByID(objectID);
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

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(WebPage));
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

				public static WebPage DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(WebPage));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (WebPage) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterObject);
				}

				public static string GetRelativeLocationAsMetadataTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "WebPage", masterObject.RelativeLocation).Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "AaltoGlobalImpact.OIP", "WebPage", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }


				// Properties to map to handle the file: WebPage.html..html
				// TODO: Handle object collections
				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				public static WebPage CreateDefault()
				{
					var result = new WebPage();
					return result;
				}

				public static WebPage CreateDemoDefault()
				{
					var result = new WebPage();
					return result;
				}


			
			}
 } 