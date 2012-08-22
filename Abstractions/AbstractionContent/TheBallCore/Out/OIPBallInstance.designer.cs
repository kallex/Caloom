 

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
			public partial class BlogContainer : IInformationObject
			{
				public BlogContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "BlogContainer";
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "BlogContainer", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "MapContainer", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "CalendarContainer", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "AboutContainer", ID).Replace("\\", "/");
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
			public partial class AccountContainer : IInformationObject
			{
				public AccountContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "AccountContainer";
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "AccountContainer", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "ProjectContainer", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "CourseContainer", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "ContainerHeader", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "IndexCollection", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "BlogCollection", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Blog", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "BlogIndexCollection", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "CalendarIndex", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Filter", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Calendar", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "CalendarCollection", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Map", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "MapCollection", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "MapIndexCollection", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "MapResult", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "MapResultCollection", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "MapResultsCollection", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Image", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "ImageGroup", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "ImagesCollection", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Tooltip", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "SocialPanelCollection", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "SocialPanel", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "EventCollection", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "MapEventCollection", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Longitude", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Latitude", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Location", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Date", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Sex", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Address", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Identity", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "ImageVideoSoundVectorRaw", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Category", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "What", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "When", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Where", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Whom", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Worth", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Event5W", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Event5WCollection", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "SubscriptionCollection", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Subscription", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "QueueEnvelope", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "SubscriberInput", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "SubscriberUpdateOperation", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Monitor", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "IconTitleDescription", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "AboutAGIApplications", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Icon", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "WebPageTemplate", ID).Replace("\\", "/");
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "WebPage", ID).Replace("\\", "/");
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