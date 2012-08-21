 

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
			public partial class TBReferenceEvent : IInformationObject
			{
				public TBReferenceEvent()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBReferenceEvent";
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "TBReferenceEvent", ID).Replace("\\", "/");
				}

                public static TBReferenceEvent RetrieveTBReferenceEvent(string relativeLocation)
                {
                    var result = (TBReferenceEvent) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBReferenceEvent));
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
					return Path.Combine("AaltoGlobalImpact.OIP", "TBReferenceEvent", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static TBReferenceEvent CreateDefault()
				{
					var result = new TBReferenceEvent();
				
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
						case "AttendeeCount":
							AttendeeCount = long.Parse(value);
							break;
						case "EnoughToAttend":
							EnoughToAttend = bool.Parse(value);
							break;
						case "DueTime":
							DueTime = DateTime.Parse(value);
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
			public long AttendeeCount { get; set; }
			[DataMember]
			public bool EnoughToAttend { get; set; }
			[DataMember]
			public DateTime DueTime { get; set; }
			
			}
			[DataContract]
			public partial class TBEventReminder : IInformationObject
			{
				public TBEventReminder()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "TBEventReminder";
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "TBEventReminder", ID).Replace("\\", "/");
				}

                public static TBEventReminder RetrieveTBEventReminder(string relativeLocation)
                {
                    var result = (TBEventReminder) StorageSupport.RetrieveInformation(relativeLocation, typeof(TBEventReminder));
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
					return Path.Combine("AaltoGlobalImpact.OIP", "TBEventReminder", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static TBEventReminder CreateDefault()
				{
					var result = new TBEventReminder();
				
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
						case "EventTitle":
							EventTitle = value;
							break;
						case "TitleColor":
							TitleColor = value;
							break;
						case "DueInfo":
							DueInfo = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string EventTitle { get; set; }
			[DataMember]
			public string TitleColor { get; set; }
			[DataMember]
			public string DueInfo { get; set; }
			
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
					result.BlogContainerHeader = BlogContainerHeader.CreateDefault();
					result.FeaturedBlog = Blog.CreateDefault();
					result.BlogCollection = BlogCollection.CreateDefault();
					result.BlogIndexCollection = BlogIndexCollections.CreateDefault();
				
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
			public BlogContainerHeader BlogContainerHeader { get; set; }
			[DataMember]
			public Blog FeaturedBlog { get; set; }
			[DataMember]
			public BlogCollection BlogCollection { get; set; }
			[DataMember]
			public BlogIndexCollections BlogIndexCollection { get; set; }
			
			}
			[DataContract]
			public partial class BlogContainerHeader : IInformationObject
			{
				public BlogContainerHeader()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "BlogContainerHeader";
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "BlogContainerHeader", ID).Replace("\\", "/");
				}

                public static BlogContainerHeader RetrieveBlogContainerHeader(string relativeLocation)
                {
                    var result = (BlogContainerHeader) StorageSupport.RetrieveInformation(relativeLocation, typeof(BlogContainerHeader));
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
					return Path.Combine("AaltoGlobalImpact.OIP", "BlogContainerHeader", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static BlogContainerHeader CreateDefault()
				{
					var result = new BlogContainerHeader();
				
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
			
			}
			[DataContract]
			public partial class BlogIndexCollections : IInformationObject
			{
				public BlogIndexCollections()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "AaltoGlobalImpact.OIP";
				    this.Name = "BlogIndexCollections";
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "BlogIndexCollections", ID).Replace("\\", "/");
				}

                public static BlogIndexCollections RetrieveBlogIndexCollections(string relativeLocation)
                {
                    var result = (BlogIndexCollections) StorageSupport.RetrieveInformation(relativeLocation, typeof(BlogIndexCollections));
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
					return Path.Combine("AaltoGlobalImpact.OIP", "BlogIndexCollections", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static BlogIndexCollections CreateDefault()
				{
					var result = new BlogIndexCollections();
					result.BlogsByDate = BlogCollection.CreateDefault();
					result.BlogsByLocation = BlogCollection.CreateDefault();
					result.BlogsByAuthor = BlogCollection.CreateDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = BlogsByDate;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = BlogsByLocation;
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					{
						var item = BlogsByAuthor;
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
			public BlogCollection BlogsByDate { get; set; }
			[DataMember]
			public BlogCollection BlogsByLocation { get; set; }
			[DataMember]
			public BlogCollection BlogsByAuthor { get; set; }
			
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
			public byte[] ImageData { get; set; }
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
						case "Text":
							Text = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string Text { get; set; }
			
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
						case "SocialFilter":
							SocialFilter = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember]
			public string SocialFilter { get; set; }
			
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
                    RelativeLocation = Path.Combine("AaltoGlobalImpact.OIP", "Collaborator", ID).Replace("\\", "/");
				}

                public static Collaborator RetrieveCollaborator(string relativeLocation)
                {
                    var result = (Collaborator) StorageSupport.RetrieveInformation(relativeLocation, typeof(Collaborator));
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
					return Path.Combine("AaltoGlobalImpact.OIP", "Collaborator", masterObject.RelativeLocation).Replace("\\", "/"); 
				}

				public static Collaborator CreateDefault()
				{
					var result = new Collaborator();
				
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
			public string Title { get; set; }
			[DataMember]
			public string Description { get; set; }
			
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
			public Location Location { get; set; }
			
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
					result.Icon = Icon.CreateDefault();
				
					return result;
				}
				private object FindFromObjectTree(string objectId)
				{
					{
						var item = Icon;
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
			public Icon Icon { get; set; }
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