 

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
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
		void InitializeDefaultSubscribers();
		void SetValuesToObjects(NameValueCollection form);
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

				public static TBRLoginRoot RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBRLoginRoot(relativeLocation);
				}


                public static TBRLoginRoot RetrieveTBRLoginRoot(string relativeLocation)
                {
                    var result = (TBRLoginRoot) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBRLoginRoot));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBRLoginRoot", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static TBRLoginRoot CreateDefault()
				{
					var result = new TBRLoginRoot();
					result.Account = TBAccount.CreateDefault();
				
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

				public static TBRAccountRoot RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBRAccountRoot(relativeLocation);
				}


                public static TBRAccountRoot RetrieveTBRAccountRoot(string relativeLocation)
                {
                    var result = (TBRAccountRoot) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBRAccountRoot));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBRAccountRoot", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static TBRAccountRoot CreateDefault()
				{
					var result = new TBRAccountRoot();
					result.Account = TBAccount.CreateDefault();
				
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

				public static TBRGroupRoot RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBRGroupRoot(relativeLocation);
				}


                public static TBRGroupRoot RetrieveTBRGroupRoot(string relativeLocation)
                {
                    var result = (TBRGroupRoot) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBRGroupRoot));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBRGroupRoot", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static TBRGroupRoot CreateDefault()
				{
					var result = new TBRGroupRoot();
					result.Group = TBCollaboratingGroup.CreateDefault();
				
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

				public static TBRLoginGroupRoot RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBRLoginGroupRoot(relativeLocation);
				}


                public static TBRLoginGroupRoot RetrieveTBRLoginGroupRoot(string relativeLocation)
                {
                    var result = (TBRLoginGroupRoot) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBRLoginGroupRoot));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBRLoginGroupRoot", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static TBRLoginGroupRoot CreateDefault()
				{
					var result = new TBRLoginGroupRoot();
					result.Role = TBCollaboratorRole.CreateDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Role;
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
						case "GroupID":
							GroupID = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public TBCollaboratorRole Role { get; set; }
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

				public static TBREmailRoot RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBREmailRoot(relativeLocation);
				}


                public static TBREmailRoot RetrieveTBREmailRoot(string relativeLocation)
                {
                    var result = (TBREmailRoot) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBREmailRoot));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBREmailRoot", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static TBREmailRoot CreateDefault()
				{
					var result = new TBREmailRoot();
					result.Account = TBAccount.CreateDefault();
				
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

				public static TBAccount RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBAccount(relativeLocation);
				}


                public static TBAccount RetrieveTBAccount(string relativeLocation)
                {
                    var result = (TBAccount) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBAccount));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBAccount", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static TBAccount CreateDefault()
				{
					var result = new TBAccount();
					result.Emails = TBEmailCollection.CreateDefault();
					result.Logins = TBLoginInfoCollection.CreateDefault();
					result.GroupRoleCollection = TBAccountCollaborationGroupCollection.CreateDefault();
				
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

				public static TBAccountCollaborationGroup RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBAccountCollaborationGroup(relativeLocation);
				}


                public static TBAccountCollaborationGroup RetrieveTBAccountCollaborationGroup(string relativeLocation)
                {
                    var result = (TBAccountCollaborationGroup) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBAccountCollaborationGroup));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBAccountCollaborationGroup", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static TBAccountCollaborationGroup CreateDefault()
				{
					var result = new TBAccountCollaborationGroup();
				
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

				public static TBAccountCollaborationGroupCollection RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBAccountCollaborationGroupCollection(relativeLocation);
				}


                public static TBAccountCollaborationGroupCollection RetrieveTBAccountCollaborationGroupCollection(string relativeLocation)
                {
                    var result = (TBAccountCollaborationGroupCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBAccountCollaborationGroupCollection));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBAccountCollaborationGroupCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
				}


				public static TBAccountCollaborationGroupCollection CreateDefault()
				{
					return new TBAccountCollaborationGroupCollection();
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

				public static TBLoginInfo RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBLoginInfo(relativeLocation);
				}


                public static TBLoginInfo RetrieveTBLoginInfo(string relativeLocation)
                {
                    var result = (TBLoginInfo) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBLoginInfo));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBLoginInfo", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static TBLoginInfo CreateDefault()
				{
					var result = new TBLoginInfo();
				
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

				public static TBLoginInfoCollection RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBLoginInfoCollection(relativeLocation);
				}


                public static TBLoginInfoCollection RetrieveTBLoginInfoCollection(string relativeLocation)
                {
                    var result = (TBLoginInfoCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBLoginInfoCollection));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBLoginInfoCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
				}


				public static TBLoginInfoCollection CreateDefault()
				{
					return new TBLoginInfoCollection();
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

				public static TBEmail RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBEmail(relativeLocation);
				}


                public static TBEmail RetrieveTBEmail(string relativeLocation)
                {
                    var result = (TBEmail) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBEmail));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBEmail", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static TBEmail CreateDefault()
				{
					var result = new TBEmail();
				
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

				public static TBEmailCollection RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBEmailCollection(relativeLocation);
				}


                public static TBEmailCollection RetrieveTBEmailCollection(string relativeLocation)
                {
                    var result = (TBEmailCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBEmailCollection));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBEmailCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
				}


				public static TBEmailCollection CreateDefault()
				{
					return new TBEmailCollection();
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

				public static TBCollaboratorRole RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBCollaboratorRole(relativeLocation);
				}


                public static TBCollaboratorRole RetrieveTBCollaboratorRole(string relativeLocation)
                {
                    var result = (TBCollaboratorRole) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBCollaboratorRole));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBCollaboratorRole", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static TBCollaboratorRole CreateDefault()
				{
					var result = new TBCollaboratorRole();
					result.Email = TBEmail.CreateDefault();
				
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

				public static TBCollaboratorRoleCollection RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBCollaboratorRoleCollection(relativeLocation);
				}


                public static TBCollaboratorRoleCollection RetrieveTBCollaboratorRoleCollection(string relativeLocation)
                {
                    var result = (TBCollaboratorRoleCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBCollaboratorRoleCollection));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBCollaboratorRoleCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
				}


				public static TBCollaboratorRoleCollection CreateDefault()
				{
					return new TBCollaboratorRoleCollection();
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

				public static TBCollaboratingGroup RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBCollaboratingGroup(relativeLocation);
				}


                public static TBCollaboratingGroup RetrieveTBCollaboratingGroup(string relativeLocation)
                {
                    var result = (TBCollaboratingGroup) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBCollaboratingGroup));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBCollaboratingGroup", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static TBCollaboratingGroup CreateDefault()
				{
					var result = new TBCollaboratingGroup();
					result.Roles = TBCollaboratorRoleCollection.CreateDefault();
				
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
			public partial class TBEmailValidationContainer : IInformationObject
			{
				public TBEmailValidationContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBEmailValidationContainer";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "TBEmailValidationContainer", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static TBEmailValidationContainer RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBEmailValidationContainer(relativeLocation);
				}


                public static TBEmailValidationContainer RetrieveTBEmailValidationContainer(string relativeLocation)
                {
                    var result = (TBEmailValidationContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBEmailValidationContainer));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBEmailValidationContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static TBEmailValidationContainer CreateDefault()
				{
					var result = new TBEmailValidationContainer();
					result.Email = TBEmail.CreateDefault();
					result.LoginInfo = TBLoginInfo.CreateDefault();
				
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
					{
						var item = LoginInfo;
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
						case "ValidUntil":
							ValidUntil = DateTime.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public TBEmail Email { get; set; }
			[DataMember]
			public TBLoginInfo LoginInfo { get; set; }
			[DataMember]
			public DateTime ValidUntil { get; set; }
			
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

				public static TBPRegisterEmail RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTBPRegisterEmail(relativeLocation);
				}


                public static TBPRegisterEmail RetrieveTBPRegisterEmail(string relativeLocation)
                {
                    var result = (TBPRegisterEmail) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBPRegisterEmail));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "TBPRegisterEmail", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static TBPRegisterEmail CreateDefault()
				{
					var result = new TBPRegisterEmail();
				
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

				public static AccountContainer RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAccountContainer(relativeLocation);
				}


                public static AccountContainer RetrieveAccountContainer(string relativeLocation)
                {
                    var result = (AccountContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(AccountContainer));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AccountContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static AccountContainer CreateDefault()
				{
					var result = new AccountContainer();
					result.Header = ContainerHeader.CreateDefault();
					result.AccountIndex = AccountIndex.CreateDefault();
					result.AccountModule = AccountModule.CreateDefault();
				
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

				public static AccountIndex RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAccountIndex(relativeLocation);
				}


                public static AccountIndex RetrieveAccountIndex(string relativeLocation)
                {
                    var result = (AccountIndex) StorageSupport.RetrieveInformation(relativeLocation, typeof(AccountIndex));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AccountIndex", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static AccountIndex CreateDefault()
				{
					var result = new AccountIndex();
				
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

				public static AccountModule RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAccountModule(relativeLocation);
				}


                public static AccountModule RetrieveAccountModule(string relativeLocation)
                {
                    var result = (AccountModule) StorageSupport.RetrieveInformation(relativeLocation, typeof(AccountModule));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AccountModule", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static AccountModule CreateDefault()
				{
					var result = new AccountModule();
					result.AccountIdentity = AccountProfile.CreateDefault();
					result.AccountRoles = AccountRoles.CreateDefault();
					result.AccountSkills = AccountSkills.CreateDefault();
					result.AccountLocations = AccountLocations.CreateDefault();
					result.AccountStatistics = AccountStatistics.CreateDefault();
					result.AccountProjects = AccountProjects.CreateDefault();
					result.AccountContent = AccountContent.CreateDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = AccountIdentity;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = AccountRoles;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = AccountSkills;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = AccountLocations;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = AccountStatistics;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = AccountProjects;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = AccountContent;
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
			public AccountProfile AccountIdentity { get; set; }
			[DataMember]
			public AccountRoles AccountRoles { get; set; }
			[DataMember]
			public AccountSkills AccountSkills { get; set; }
			[DataMember]
			public AccountLocations AccountLocations { get; set; }
			[DataMember]
			public AccountStatistics AccountStatistics { get; set; }
			[DataMember]
			public AccountProjects AccountProjects { get; set; }
			[DataMember]
			public AccountContent AccountContent { get; set; }
			
			}
			[DataContract]
			public partial class AccountStatistics : IInformationObject
			{
				public AccountStatistics()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "AccountStatistics";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "AccountStatistics", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static AccountStatistics RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAccountStatistics(relativeLocation);
				}


                public static AccountStatistics RetrieveAccountStatistics(string relativeLocation)
                {
                    var result = (AccountStatistics) StorageSupport.RetrieveInformation(relativeLocation, typeof(AccountStatistics));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AccountStatistics", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static AccountStatistics CreateDefault()
				{
					var result = new AccountStatistics();
				
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
			public partial class AccountSkills : IInformationObject
			{
				public AccountSkills()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "AccountSkills";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "AccountSkills", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static AccountSkills RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAccountSkills(relativeLocation);
				}


                public static AccountSkills RetrieveAccountSkills(string relativeLocation)
                {
                    var result = (AccountSkills) StorageSupport.RetrieveInformation(relativeLocation, typeof(AccountSkills));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AccountSkills", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static AccountSkills CreateDefault()
				{
					var result = new AccountSkills();
				
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
			public partial class AccountProjects : IInformationObject
			{
				public AccountProjects()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "AccountProjects";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "AccountProjects", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static AccountProjects RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAccountProjects(relativeLocation);
				}


                public static AccountProjects RetrieveAccountProjects(string relativeLocation)
                {
                    var result = (AccountProjects) StorageSupport.RetrieveInformation(relativeLocation, typeof(AccountProjects));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AccountProjects", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static AccountProjects CreateDefault()
				{
					var result = new AccountProjects();
				
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
			public partial class AccountLocations : IInformationObject
			{
				public AccountLocations()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "AccountLocations";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "AccountLocations", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static AccountLocations RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAccountLocations(relativeLocation);
				}


                public static AccountLocations RetrieveAccountLocations(string relativeLocation)
                {
                    var result = (AccountLocations) StorageSupport.RetrieveInformation(relativeLocation, typeof(AccountLocations));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AccountLocations", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static AccountLocations CreateDefault()
				{
					var result = new AccountLocations();
				
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

				public static AccountContent RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAccountContent(relativeLocation);
				}


                public static AccountContent RetrieveAccountContent(string relativeLocation)
                {
                    var result = (AccountContent) StorageSupport.RetrieveInformation(relativeLocation, typeof(AccountContent));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AccountContent", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static AccountContent CreateDefault()
				{
					var result = new AccountContent();
				
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

				public static AccountProfile RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAccountProfile(relativeLocation);
				}


                public static AccountProfile RetrieveAccountProfile(string relativeLocation)
                {
                    var result = (AccountProfile) StorageSupport.RetrieveInformation(relativeLocation, typeof(AccountProfile));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AccountProfile", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static AccountProfile CreateDefault()
				{
					var result = new AccountProfile();
					result.PersonalInfoVisibility = PersonalInfoVisibility.CreateDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = PersonalInfoVisibility;
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
						case "EmailAddress":
							EmailAddress = value;
							break;
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
			public string FirstName { get; set; }
			[DataMember]
			public string LastName { get; set; }
			[DataMember]
			public string EmailAddress { get; set; }
			[DataMember]
			public string Street { get; set; }
			[DataMember]
			public string ZipCode { get; set; }
			[DataMember]
			public string Town { get; set; }
			[DataMember]
			public string Country { get; set; }
			[DataMember]
			public PersonalInfoVisibility PersonalInfoVisibility { get; set; }
			
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

				public static AccountRoles RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAccountRoles(relativeLocation);
				}


                public static AccountRoles RetrieveAccountRoles(string relativeLocation)
                {
                    var result = (AccountRoles) StorageSupport.RetrieveInformation(relativeLocation, typeof(AccountRoles));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AccountRoles", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static AccountRoles CreateDefault()
				{
					var result = new AccountRoles();
					result.ModeratorInGroups = ReferenceCollection.CreateDefault();
					result.MemberInGroups = ReferenceCollection.CreateDefault();
					result.MemberInOrganizations = ReferenceCollection.CreateDefault();
					result.FollowingGroups = ReferenceCollection.CreateDefault();
				
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
					{
						var item = MemberInOrganizations;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = FollowingGroups;
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
			public ReferenceCollection ModeratorInGroups { get; set; }
			[DataMember]
			public ReferenceCollection MemberInGroups { get; set; }
			[DataMember]
			public ReferenceCollection MemberInOrganizations { get; set; }
			[DataMember]
			public ReferenceCollection FollowingGroups { get; set; }
			
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

				public static PersonalInfoVisibility RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrievePersonalInfoVisibility(relativeLocation);
				}


                public static PersonalInfoVisibility RetrievePersonalInfoVisibility(string relativeLocation)
                {
                    var result = (PersonalInfoVisibility) StorageSupport.RetrieveInformation(relativeLocation, typeof(PersonalInfoVisibility));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "PersonalInfoVisibility", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static PersonalInfoVisibility CreateDefault()
				{
					var result = new PersonalInfoVisibility();
				
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

				public static ReferenceToInformation RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveReferenceToInformation(relativeLocation);
				}


                public static ReferenceToInformation RetrieveReferenceToInformation(string relativeLocation)
                {
                    var result = (ReferenceToInformation) StorageSupport.RetrieveInformation(relativeLocation, typeof(ReferenceToInformation));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "ReferenceToInformation", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static ReferenceToInformation CreateDefault()
				{
					var result = new ReferenceToInformation();
				
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

				public static ReferenceCollection RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveReferenceCollection(relativeLocation);
				}


                public static ReferenceCollection RetrieveReferenceCollection(string relativeLocation)
                {
                    var result = (ReferenceCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(ReferenceCollection));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "ReferenceCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
				}


				public static ReferenceCollection CreateDefault()
				{
					return new ReferenceCollection();
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

				public static BlogContainer RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveBlogContainer(relativeLocation);
				}


                public static BlogContainer RetrieveBlogContainer(string relativeLocation)
                {
                    var result = (BlogContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(BlogContainer));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "BlogContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static BlogContainer CreateDefault()
				{
					var result = new BlogContainer();
					result.BlogContainerHeader = ContainerHeader.CreateDefault();
					result.FeaturedBlog = Blog.CreateDefault();
					result.BlogCollection = BlogCollection.CreateDefault();
					result.BlogIndexCollection = IndexCollection.CreateDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = BlogContainerHeader;
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
						var item = BlogCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = BlogIndexCollection;
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
			public ContainerHeader BlogContainerHeader { get; set; }
			[DataMember]
			public Blog FeaturedBlog { get; set; }
			[DataMember]
			public BlogCollection BlogCollection { get; set; }
			[DataMember]
			public IndexCollection BlogIndexCollection { get; set; }
			
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

				public static MapContainer RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveMapContainer(relativeLocation);
				}


                public static MapContainer RetrieveMapContainer(string relativeLocation)
                {
                    var result = (MapContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(MapContainer));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "MapContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static MapContainer CreateDefault()
				{
					var result = new MapContainer();
					result.MapContainerHeader = ContainerHeader.CreateDefault();
					result.MapFeatured = Map.CreateDefault();
					result.MapCollection = MapCollection.CreateDefault();
					result.MapResultCollection = MapResultCollection.CreateDefault();
					result.MapIndexCollection = MapIndexCollection.CreateDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = MapContainerHeader;
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
			public ContainerHeader MapContainerHeader { get; set; }
			[DataMember]
			public Map MapFeatured { get; set; }
			[DataMember]
			public MapCollection MapCollection { get; set; }
			[DataMember]
			public MapResultCollection MapResultCollection { get; set; }
			[DataMember]
			public MapIndexCollection MapIndexCollection { get; set; }
			
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

				public static CalendarContainer RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveCalendarContainer(relativeLocation);
				}


                public static CalendarContainer RetrieveCalendarContainer(string relativeLocation)
                {
                    var result = (CalendarContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(CalendarContainer));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "CalendarContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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

				public static AboutContainer RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAboutContainer(relativeLocation);
				}


                public static AboutContainer RetrieveAboutContainer(string relativeLocation)
                {
                    var result = (AboutContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(AboutContainer));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AboutContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static AboutContainer CreateDefault()
				{
					var result = new AboutContainer();
					result.AboutContainerHeader = ContainerHeader.CreateDefault();
					result.AboutFeatured = Calendar.CreateDefault();
					result.AboutCollection = CalendarCollection.CreateDefault();
					result.AboutIndexCollection = CalendarIndex.CreateDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = AboutContainerHeader;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = AboutFeatured;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = AboutCollection;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = AboutIndexCollection;
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
			public ContainerHeader AboutContainerHeader { get; set; }
			[DataMember]
			public Calendar AboutFeatured { get; set; }
			[DataMember]
			public CalendarCollection AboutCollection { get; set; }
			[DataMember]
			public CalendarIndex AboutIndexCollection { get; set; }
			
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

				public static OBSAccountContainer RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveOBSAccountContainer(relativeLocation);
				}


                public static OBSAccountContainer RetrieveOBSAccountContainer(string relativeLocation)
                {
                    var result = (OBSAccountContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(OBSAccountContainer));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "OBSAccountContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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

				public static ProjectContainer RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveProjectContainer(relativeLocation);
				}


                public static ProjectContainer RetrieveProjectContainer(string relativeLocation)
                {
                    var result = (ProjectContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(ProjectContainer));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "ProjectContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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

				public static CourseContainer RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveCourseContainer(relativeLocation);
				}


                public static CourseContainer RetrieveCourseContainer(string relativeLocation)
                {
                    var result = (CourseContainer) StorageSupport.RetrieveInformation(relativeLocation, typeof(CourseContainer));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "CourseContainer", masterObject.RelativeLocation).Replace("\\", "/"); 
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

				public static ContainerHeader RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveContainerHeader(relativeLocation);
				}


                public static ContainerHeader RetrieveContainerHeader(string relativeLocation)
                {
                    var result = (ContainerHeader) StorageSupport.RetrieveInformation(relativeLocation, typeof(ContainerHeader));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "ContainerHeader", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static ContainerHeader CreateDefault()
				{
					var result = new ContainerHeader();
				
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
			public partial class IndexCollection : IInformationObject
			{
				public IndexCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "IndexCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "IndexCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static IndexCollection RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveIndexCollection(relativeLocation);
				}


                public static IndexCollection RetrieveIndexCollection(string relativeLocation)
                {
                    var result = (IndexCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(IndexCollection));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "IndexCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static IndexCollection CreateDefault()
				{
					var result = new IndexCollection();
				
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
			public string Introduction { get; set; }
			[DataMember]
			public string Summary { get; set; }
			
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

				public static BlogCollection RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveBlogCollection(relativeLocation);
				}


                public static BlogCollection RetrieveBlogCollection(string relativeLocation)
                {
                    var result = (BlogCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(BlogCollection));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "BlogCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
				}


				public static BlogCollection CreateDefault()
				{
					return new BlogCollection();
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

				public static Blog RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveBlog(relativeLocation);
				}


                public static Blog RetrieveBlog(string relativeLocation)
                {
                    var result = (Blog) StorageSupport.RetrieveInformation(relativeLocation, typeof(Blog));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Blog", masterObject.RelativeLocation).Replace("\\", "/"); 
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
			public partial class BlogIndexCollection : IInformationObject
			{
				public BlogIndexCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "BlogIndexCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "BlogIndexCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static BlogIndexCollection RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveBlogIndexCollection(relativeLocation);
				}


                public static BlogIndexCollection RetrieveBlogIndexCollection(string relativeLocation)
                {
                    var result = (BlogIndexCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(BlogIndexCollection));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "BlogIndexCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static BlogIndexCollection CreateDefault()
				{
					var result = new BlogIndexCollection();
					result.BlogByDate = BlogCollection.CreateDefault();
					result.BlogByLocation = BlogCollection.CreateDefault();
					result.BlogByAuthor = BlogCollection.CreateDefault();
					result.BlogByCategory = BlogCollection.CreateDefault();
				
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
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public BlogCollection BlogByDate { get; set; }
			[DataMember]
			public BlogCollection BlogByLocation { get; set; }
			[DataMember]
			public BlogCollection BlogByAuthor { get; set; }
			[DataMember]
			public BlogCollection BlogByCategory { get; set; }
			
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

				public static CalendarIndex RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveCalendarIndex(relativeLocation);
				}


                public static CalendarIndex RetrieveCalendarIndex(string relativeLocation)
                {
                    var result = (CalendarIndex) StorageSupport.RetrieveInformation(relativeLocation, typeof(CalendarIndex));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "CalendarIndex", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static CalendarIndex CreateDefault()
				{
					var result = new CalendarIndex();
				
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

				public static Filter RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveFilter(relativeLocation);
				}


                public static Filter RetrieveFilter(string relativeLocation)
                {
                    var result = (Filter) StorageSupport.RetrieveInformation(relativeLocation, typeof(Filter));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Filter", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static Filter CreateDefault()
				{
					var result = new Filter();
				
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

				public static Calendar RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveCalendar(relativeLocation);
				}


                public static Calendar RetrieveCalendar(string relativeLocation)
                {
                    var result = (Calendar) StorageSupport.RetrieveInformation(relativeLocation, typeof(Calendar));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Calendar", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static Calendar CreateDefault()
				{
					var result = new Calendar();
				
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

				public static CalendarCollection RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveCalendarCollection(relativeLocation);
				}


                public static CalendarCollection RetrieveCalendarCollection(string relativeLocation)
                {
                    var result = (CalendarCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(CalendarCollection));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "CalendarCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
				}


				public static CalendarCollection CreateDefault()
				{
					return new CalendarCollection();
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

				public static Map RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveMap(relativeLocation);
				}


                public static Map RetrieveMap(string relativeLocation)
                {
                    var result = (Map) StorageSupport.RetrieveInformation(relativeLocation, typeof(Map));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Map", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static Map CreateDefault()
				{
					var result = new Map();
				
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

				public static MapCollection RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveMapCollection(relativeLocation);
				}


                public static MapCollection RetrieveMapCollection(string relativeLocation)
                {
                    var result = (MapCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(MapCollection));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "MapCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
				}


				public static MapCollection CreateDefault()
				{
					return new MapCollection();
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

				public static MapIndexCollection RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveMapIndexCollection(relativeLocation);
				}


                public static MapIndexCollection RetrieveMapIndexCollection(string relativeLocation)
                {
                    var result = (MapIndexCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(MapIndexCollection));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "MapIndexCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
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

				public static MapResult RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveMapResult(relativeLocation);
				}


                public static MapResult RetrieveMapResult(string relativeLocation)
                {
                    var result = (MapResult) StorageSupport.RetrieveInformation(relativeLocation, typeof(MapResult));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "MapResult", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static MapResult CreateDefault()
				{
					var result = new MapResult();
					result.Location = Location.CreateDefault();
				
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

				public static MapResultCollection RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveMapResultCollection(relativeLocation);
				}


                public static MapResultCollection RetrieveMapResultCollection(string relativeLocation)
                {
                    var result = (MapResultCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(MapResultCollection));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "MapResultCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
				}


				public static MapResultCollection CreateDefault()
				{
					return new MapResultCollection();
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

				public static MapResultsCollection RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveMapResultsCollection(relativeLocation);
				}


                public static MapResultsCollection RetrieveMapResultsCollection(string relativeLocation)
                {
                    var result = (MapResultsCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(MapResultsCollection));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "MapResultsCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static MapResultsCollection CreateDefault()
				{
					var result = new MapResultsCollection();
					result.ResultByDate = MapResultCollection.CreateDefault();
					result.ResultByAuthor = MapResultCollection.CreateDefault();
					result.ResultByProximity = MapResultCollection.CreateDefault();
				
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

				public static Image RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveImage(relativeLocation);
				}


                public static Image RetrieveImage(string relativeLocation)
                {
                    var result = (Image) StorageSupport.RetrieveInformation(relativeLocation, typeof(Image));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Image", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static Image CreateDefault()
				{
					var result = new Image();
					result.Tooltip = Tooltip.CreateDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Tooltip;
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
						case "Caption":
							Caption = value;
							break;
						case "ImageAlt":
							ImageAlt = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Caption { get; set; }
			[DataMember]
			public string ImageAlt { get; set; }
			[DataMember]
			public Tooltip Tooltip { get; set; }
			
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

				public static ImageGroup RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveImageGroup(relativeLocation);
				}


                public static ImageGroup RetrieveImageGroup(string relativeLocation)
                {
                    var result = (ImageGroup) StorageSupport.RetrieveInformation(relativeLocation, typeof(ImageGroup));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "ImageGroup", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static ImageGroup CreateDefault()
				{
					var result = new ImageGroup();
					result.ImagesCollection = ImagesCollection.CreateDefault();
				
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

				public static ImagesCollection RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveImagesCollection(relativeLocation);
				}


                public static ImagesCollection RetrieveImagesCollection(string relativeLocation)
                {
                    var result = (ImagesCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(ImagesCollection));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "ImagesCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
				}


				public static ImagesCollection CreateDefault()
				{
					return new ImagesCollection();
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

				public static Tooltip RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveTooltip(relativeLocation);
				}


                public static Tooltip RetrieveTooltip(string relativeLocation)
                {
                    var result = (Tooltip) StorageSupport.RetrieveInformation(relativeLocation, typeof(Tooltip));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Tooltip", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static Tooltip CreateDefault()
				{
					var result = new Tooltip();
				
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

				public static SocialPanelCollection RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveSocialPanelCollection(relativeLocation);
				}


                public static SocialPanelCollection RetrieveSocialPanelCollection(string relativeLocation)
                {
                    var result = (SocialPanelCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(SocialPanelCollection));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "SocialPanelCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
				}


				public static SocialPanelCollection CreateDefault()
				{
					return new SocialPanelCollection();
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

				public static SocialPanel RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveSocialPanel(relativeLocation);
				}


                public static SocialPanel RetrieveSocialPanel(string relativeLocation)
                {
                    var result = (SocialPanel) StorageSupport.RetrieveInformation(relativeLocation, typeof(SocialPanel));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "SocialPanel", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static SocialPanel CreateDefault()
				{
					var result = new SocialPanel();
					result.SocialFilter = Filter.CreateDefault();
				
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
			public partial class EventCollection : IInformationObject
			{
				public EventCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "EventCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "EventCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static EventCollection RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveEventCollection(relativeLocation);
				}


                public static EventCollection RetrieveEventCollection(string relativeLocation)
                {
                    var result = (EventCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(EventCollection));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "EventCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
				}


				public static EventCollection CreateDefault()
				{
					return new EventCollection();
				}

				[DataMember] public List<Event5W> CollectionContent = new List<Event5W>();

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
			public partial class MapEventCollection : IInformationObject
			{
				public MapEventCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "MapEventCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "MapEventCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static MapEventCollection RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveMapEventCollection(relativeLocation);
				}


                public static MapEventCollection RetrieveMapEventCollection(string relativeLocation)
                {
                    var result = (MapEventCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(MapEventCollection));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "MapEventCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static MapEventCollection CreateDefault()
				{
					var result = new MapEventCollection();
					result.Events = EventCollection.CreateDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Events;
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
			public EventCollection Events { get; set; }
			
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

				public static Longitude RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveLongitude(relativeLocation);
				}


                public static Longitude RetrieveLongitude(string relativeLocation)
                {
                    var result = (Longitude) StorageSupport.RetrieveInformation(relativeLocation, typeof(Longitude));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Longitude", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static Longitude CreateDefault()
				{
					var result = new Longitude();
				
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

				public static Latitude RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveLatitude(relativeLocation);
				}


                public static Latitude RetrieveLatitude(string relativeLocation)
                {
                    var result = (Latitude) StorageSupport.RetrieveInformation(relativeLocation, typeof(Latitude));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Latitude", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static Latitude CreateDefault()
				{
					var result = new Latitude();
				
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

				public static Location RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveLocation(relativeLocation);
				}


                public static Location RetrieveLocation(string relativeLocation)
                {
                    var result = (Location) StorageSupport.RetrieveInformation(relativeLocation, typeof(Location));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Location", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static Location CreateDefault()
				{
					var result = new Location();
					result.Longitude = Longitude.CreateDefault();
					result.Latitude = Latitude.CreateDefault();
				
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
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public Longitude Longitude { get; set; }
			[DataMember]
			public Latitude Latitude { get; set; }
			
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

				public static Date RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveDate(relativeLocation);
				}


                public static Date RetrieveDate(string relativeLocation)
                {
                    var result = (Date) StorageSupport.RetrieveInformation(relativeLocation, typeof(Date));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Date", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static Date CreateDefault()
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

				public static Sex RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveSex(relativeLocation);
				}


                public static Sex RetrieveSex(string relativeLocation)
                {
                    var result = (Sex) StorageSupport.RetrieveInformation(relativeLocation, typeof(Sex));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Sex", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static Sex CreateDefault()
				{
					var result = new Sex();
				
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
			public partial class Address : IInformationObject
			{
				public Address()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Address";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Address", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Address RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAddress(relativeLocation);
				}


                public static Address RetrieveAddress(string relativeLocation)
                {
                    var result = (Address) StorageSupport.RetrieveInformation(relativeLocation, typeof(Address));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Address", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static Address CreateDefault()
				{
					var result = new Address();
				
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

				public static Identity RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveIdentity(relativeLocation);
				}


                public static Identity RetrieveIdentity(string relativeLocation)
                {
                    var result = (Identity) StorageSupport.RetrieveInformation(relativeLocation, typeof(Identity));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Identity", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static Identity CreateDefault()
				{
					var result = new Identity();
					result.Sex = Sex.CreateDefault();
					result.Birthday = Date.CreateDefault();
				
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

				public static ImageVideoSoundVectorRaw RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveImageVideoSoundVectorRaw(relativeLocation);
				}


                public static ImageVideoSoundVectorRaw RetrieveImageVideoSoundVectorRaw(string relativeLocation)
                {
                    var result = (ImageVideoSoundVectorRaw) StorageSupport.RetrieveInformation(relativeLocation, typeof(ImageVideoSoundVectorRaw));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "ImageVideoSoundVectorRaw", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static ImageVideoSoundVectorRaw CreateDefault()
				{
					var result = new ImageVideoSoundVectorRaw();
				
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

				public static Category RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveCategory(relativeLocation);
				}


                public static Category RetrieveCategory(string relativeLocation)
                {
                    var result = (Category) StorageSupport.RetrieveInformation(relativeLocation, typeof(Category));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Category", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static Category CreateDefault()
				{
					var result = new Category();
				
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
			public partial class What : IInformationObject
			{
				public What()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "What";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "What", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static What RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveWhat(relativeLocation);
				}


                public static What RetrieveWhat(string relativeLocation)
                {
                    var result = (What) StorageSupport.RetrieveInformation(relativeLocation, typeof(What));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "What", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static What CreateDefault()
				{
					var result = new What();
					result.Illustration = ImageVideoSoundVectorRaw.CreateDefault();
					result.Category1 = Category.CreateDefault();
					result.Category2 = Category.CreateDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Illustration;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = Category1;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = Category2;
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
						case "ServiceName":
							ServiceName = value;
							break;
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
			public string ServiceName { get; set; }
			[DataMember]
			public string Title { get; set; }
			[DataMember]
			public string Description { get; set; }
			[DataMember]
			public ImageVideoSoundVectorRaw Illustration { get; set; }
			[DataMember]
			public Category Category1 { get; set; }
			[DataMember]
			public Category Category2 { get; set; }
			
			}
			[DataContract]
			public partial class When : IInformationObject
			{
				public When()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "When";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "When", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static When RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveWhen(relativeLocation);
				}


                public static When RetrieveWhen(string relativeLocation)
                {
                    var result = (When) StorageSupport.RetrieveInformation(relativeLocation, typeof(When));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "When", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static When CreateDefault()
				{
					var result = new When();
				
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
						case "Time":
							Time = DateTime.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Title { get; set; }
			[DataMember]
			public DateTime Time { get; set; }
			
			}
			[DataContract]
			public partial class Where : IInformationObject
			{
				public Where()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Where";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Where", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Where RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveWhere(relativeLocation);
				}


                public static Where RetrieveWhere(string relativeLocation)
                {
                    var result = (Where) StorageSupport.RetrieveInformation(relativeLocation, typeof(Where));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Where", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static Where CreateDefault()
				{
					var result = new Where();
					result.Location = Location.CreateDefault();
					result.Address = Address.CreateDefault();
				
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
			public Location Location { get; set; }
			[DataMember]
			public Address Address { get; set; }
			
			}
			[DataContract]
			public partial class Whom : IInformationObject
			{
				public Whom()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Whom";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Whom", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Whom RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveWhom(relativeLocation);
				}


                public static Whom RetrieveWhom(string relativeLocation)
                {
                    var result = (Whom) StorageSupport.RetrieveInformation(relativeLocation, typeof(Whom));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Whom", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static Whom CreateDefault()
				{
					var result = new Whom();
					result.Identity = Identity.CreateDefault();
					result.ProfileImage = ImageVideoSoundVectorRaw.CreateDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Identity;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = ProfileImage;
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
						case "BirthDate":
							BirthDate = DateTime.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public Identity Identity { get; set; }
			[DataMember]
			public ImageVideoSoundVectorRaw ProfileImage { get; set; }
			[DataMember]
			public DateTime BirthDate { get; set; }
			
			}
			[DataContract]
			public partial class Worth : IInformationObject
			{
				public Worth()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Worth";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Worth", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Worth RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveWorth(relativeLocation);
				}


                public static Worth RetrieveWorth(string relativeLocation)
                {
                    var result = (Worth) StorageSupport.RetrieveInformation(relativeLocation, typeof(Worth));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Worth", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static Worth CreateDefault()
				{
					var result = new Worth();
					result.Location = Location.CreateDefault();
					result.Address = Address.CreateDefault();
				
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
			public Location Location { get; set; }
			[DataMember]
			public Address Address { get; set; }
			
			}
			[DataContract]
			public partial class Event5W : IInformationObject
			{
				public Event5W()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Event5W";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Event5W", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Event5W RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveEvent5W(relativeLocation);
				}


                public static Event5W RetrieveEvent5W(string relativeLocation)
                {
                    var result = (Event5W) StorageSupport.RetrieveInformation(relativeLocation, typeof(Event5W));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Event5W", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static Event5W CreateDefault()
				{
					var result = new Event5W();
					result.What = What.CreateDefault();
					result.When = When.CreateDefault();
					result.Where = Where.CreateDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = What;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = When;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = Where;
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
			public What What { get; set; }
			[DataMember]
			public When When { get; set; }
			[DataMember]
			public Where Where { get; set; }
			
			}
			[DataContract]
			public partial class Event5WCollection : IInformationObject
			{
				public Event5WCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "Event5WCollection";
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

                public static string GetRelativeLocationFromID(string id)
                {
                    return Path.Combine("AaltoGlobalImpact.OIP", "Event5WCollection", id).Replace("\\", "/");
                }

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = GetRelativeLocationFromID(ID);
				}

				public static Event5WCollection RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveEvent5WCollection(relativeLocation);
				}


                public static Event5WCollection RetrieveEvent5WCollection(string relativeLocation)
                {
                    var result = (Event5WCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(Event5WCollection));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Event5WCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
				}


				public static Event5WCollection CreateDefault()
				{
					return new Event5WCollection();
				}

				[DataMember] public List<Event5W> CollectionContent = new List<Event5W>();

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

				public static SubscriptionCollection RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveSubscriptionCollection(relativeLocation);
				}


                public static SubscriptionCollection RetrieveSubscriptionCollection(string relativeLocation)
                {
                    var result = (SubscriptionCollection) StorageSupport.RetrieveInformation(relativeLocation, typeof(SubscriptionCollection));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "SubscriptionCollection", masterObject.RelativeLocation).Replace("\\", "/"); 
				}


				public static SubscriptionCollection CreateDefault()
				{
					return new SubscriptionCollection();
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

				public static Subscription RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveSubscription(relativeLocation);
				}


                public static Subscription RetrieveSubscription(string relativeLocation)
                {
                    var result = (Subscription) StorageSupport.RetrieveInformation(relativeLocation, typeof(Subscription));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Subscription", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static Subscription CreateDefault()
				{
					var result = new Subscription();
				
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

				public static QueueEnvelope RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveQueueEnvelope(relativeLocation);
				}


                public static QueueEnvelope RetrieveQueueEnvelope(string relativeLocation)
                {
                    var result = (QueueEnvelope) StorageSupport.RetrieveInformation(relativeLocation, typeof(QueueEnvelope));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "QueueEnvelope", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static QueueEnvelope CreateDefault()
				{
					var result = new QueueEnvelope();
					result.SubscriberUpdateOperation = SubscriberUpdateOperation.CreateDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = SubscriberUpdateOperation;
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

				public static SubscriberInput RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveSubscriberInput(relativeLocation);
				}


                public static SubscriberInput RetrieveSubscriberInput(string relativeLocation)
                {
                    var result = (SubscriberInput) StorageSupport.RetrieveInformation(relativeLocation, typeof(SubscriberInput));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "SubscriberInput", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static SubscriberInput CreateDefault()
				{
					var result = new SubscriberInput();
				
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

				public static SubscriberUpdateOperation RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveSubscriberUpdateOperation(relativeLocation);
				}


                public static SubscriberUpdateOperation RetrieveSubscriberUpdateOperation(string relativeLocation)
                {
                    var result = (SubscriberUpdateOperation) StorageSupport.RetrieveInformation(relativeLocation, typeof(SubscriberUpdateOperation));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "SubscriberUpdateOperation", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static SubscriberUpdateOperation CreateDefault()
				{
					var result = new SubscriberUpdateOperation();
					result.OperationParameters = SubscriberInput.CreateDefault();
				
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

				public static Monitor RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveMonitor(relativeLocation);
				}


                public static Monitor RetrieveMonitor(string relativeLocation)
                {
                    var result = (Monitor) StorageSupport.RetrieveInformation(relativeLocation, typeof(Monitor));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Monitor", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static Monitor CreateDefault()
				{
					var result = new Monitor();
				
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

				public static IconTitleDescription RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveIconTitleDescription(relativeLocation);
				}


                public static IconTitleDescription RetrieveIconTitleDescription(string relativeLocation)
                {
                    var result = (IconTitleDescription) StorageSupport.RetrieveInformation(relativeLocation, typeof(IconTitleDescription));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "IconTitleDescription", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static IconTitleDescription CreateDefault()
				{
					var result = new IconTitleDescription();
				
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

				public static AboutAGIApplications RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveAboutAGIApplications(relativeLocation);
				}


                public static AboutAGIApplications RetrieveAboutAGIApplications(string relativeLocation)
                {
                    var result = (AboutAGIApplications) StorageSupport.RetrieveInformation(relativeLocation, typeof(AboutAGIApplications));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "AboutAGIApplications", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static AboutAGIApplications CreateDefault()
				{
					var result = new AboutAGIApplications();
					result.BuiltForAnybody = IconTitleDescription.CreateDefault();
					result.ForAllPeople = IconTitleDescription.CreateDefault();
				
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

				public static Icon RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveIcon(relativeLocation);
				}


                public static Icon RetrieveIcon(string relativeLocation)
                {
                    var result = (Icon) StorageSupport.RetrieveInformation(relativeLocation, typeof(Icon));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "Icon", masterObject.RelativeLocation).Replace("\\", "/"); 
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

				public static WebPageTemplate RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveWebPageTemplate(relativeLocation);
				}


                public static WebPageTemplate RetrieveWebPageTemplate(string relativeLocation)
                {
                    var result = (WebPageTemplate) StorageSupport.RetrieveInformation(relativeLocation, typeof(WebPageTemplate));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "WebPageTemplate", masterObject.RelativeLocation).Replace("\\", "/"); 
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

				public static WebPage RetrieveFromDefaultLocation(string id)
				{
					string relativeLocation = GetRelativeLocationFromID(id);
					return RetrieveWebPage(relativeLocation);
				}


                public static WebPage RetrieveWebPage(string relativeLocation)
                {
                    var result = (WebPage) StorageSupport.RetrieveInformation(relativeLocation, typeof(WebPage));
                    return result;
                }

			    public void InitializeDefaultSubscribers()
			    {
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

				public void SetRelativeLocationTo(IInformationObject masterObject)
				{
					RelativeLocation = GetRelativeLocationTo(masterObject);
				}

				public static string GetRelativeLocationTo(IInformationObject masterObject)
				{
					return Path.Combine("AaltoGlobalImpact.OIP", "WebPage", masterObject.RelativeLocation).Replace("\\", "/"); 
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

			
			}
 } 