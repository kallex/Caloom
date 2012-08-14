 

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.StorageClient;

namespace AaltoGlobalImpact.OIP { 
		    public interface IInformationObject
    {
        string ID { get; set; }
		string Name { get; }
    }

			[DataContract]
			public partial class Longitude : IInformationObject
			{
				public Longitude()
				{
					this.ID = "AaltoGlobalImpact.OIP.Longitude" + "." + Guid.NewGuid().ToString();
				}

				[DataMember]
				public string ID { get; set; }

                [IgnoreDataMember]
				public string Name
			    {
                    get { return "Longitude"; }
			    }


				public string GetBlobPath()
				{
					return GetBlobPath(this.ID);
				}

				public static string GetBlobPath(string id)
				{
					return id;
				}

			[DataMember]
			public string TextValue { get; set; }
			
			}
			[DataContract]
			public partial class Latitude : IInformationObject
			{
				public Latitude()
				{
					this.ID = "AaltoGlobalImpact.OIP.Latitude" + "." + Guid.NewGuid().ToString();
				}

				[DataMember]
				public string ID { get; set; }

                [IgnoreDataMember]
				public string Name
			    {
                    get { return "Latitude"; }
			    }


				public string GetBlobPath()
				{
					return GetBlobPath(this.ID);
				}

				public static string GetBlobPath(string id)
				{
					return id;
				}

			[DataMember]
			public string TextValue { get; set; }
			
			}
			[DataContract]
			public partial class Location : IInformationObject
			{
				public Location()
				{
					this.ID = "AaltoGlobalImpact.OIP.Location" + "." + Guid.NewGuid().ToString();
				}

				[DataMember]
				public string ID { get; set; }

                [IgnoreDataMember]
				public string Name
			    {
                    get { return "Location"; }
			    }


				public string GetBlobPath()
				{
					return GetBlobPath(this.ID);
				}

				public static string GetBlobPath(string id)
				{
					return id;
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
					this.ID = "AaltoGlobalImpact.OIP.What" + "." + Guid.NewGuid().ToString();
				}

				[DataMember]
				public string ID { get; set; }

                [IgnoreDataMember]
				public string Name
			    {
                    get { return "What"; }
			    }


				public string GetBlobPath()
				{
					return GetBlobPath(this.ID);
				}

				public static string GetBlobPath(string id)
				{
					return id;
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
					this.ID = "AaltoGlobalImpact.OIP.When" + "." + Guid.NewGuid().ToString();
				}

				[DataMember]
				public string ID { get; set; }

                [IgnoreDataMember]
				public string Name
			    {
                    get { return "When"; }
			    }


				public string GetBlobPath()
				{
					return GetBlobPath(this.ID);
				}

				public static string GetBlobPath(string id)
				{
					return id;
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
					this.ID = "AaltoGlobalImpact.OIP.Where" + "." + Guid.NewGuid().ToString();
				}

				[DataMember]
				public string ID { get; set; }

                [IgnoreDataMember]
				public string Name
			    {
                    get { return "Where"; }
			    }


				public string GetBlobPath()
				{
					return GetBlobPath(this.ID);
				}

				public static string GetBlobPath(string id)
				{
					return id;
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
					this.ID = "AaltoGlobalImpact.OIP.Event5W" + "." + Guid.NewGuid().ToString();
				}

				[DataMember]
				public string ID { get; set; }

                [IgnoreDataMember]
				public string Name
			    {
                    get { return "Event5W"; }
			    }


				public string GetBlobPath()
				{
					return GetBlobPath(this.ID);
				}

				public static string GetBlobPath(string id)
				{
					return id;
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
					this.ID = "AaltoGlobalImpact.OIP.Event5WCollection" + "." + Guid.NewGuid().ToString();
				}

				[DataMember]
				public string ID { get; set; }

                [IgnoreDataMember]
				public string Name
			    {
                    get { return "Event5WCollection"; }
			    }


				public string GetBlobPath()
				{
					return GetBlobPath(this.ID);
				}

				public static string GetBlobPath(string id)
				{
					return id;
				}


				[DataMember] public List<Event5W> CollectionContent = new List<Event5W>();

			
			}
			[DataContract]
			public partial class SubscriptionCollection : IInformationObject
			{
				public SubscriptionCollection()
				{
					this.ID = "AaltoGlobalImpact.OIP.SubscriptionCollection" + "." + Guid.NewGuid().ToString();
				}

				[DataMember]
				public string ID { get; set; }

                [IgnoreDataMember]
				public string Name
			    {
                    get { return "SubscriptionCollection"; }
			    }


				public string GetBlobPath()
				{
					return GetBlobPath(this.ID);
				}

				public static string GetBlobPath(string id)
				{
					return id;
				}


				[DataMember] public List<Subscription> CollectionContent = new List<Subscription>();

			
			}
			[DataContract]
			public partial class Subscription : IInformationObject
			{
				public Subscription()
				{
					this.ID = "AaltoGlobalImpact.OIP.Subscription" + "." + Guid.NewGuid().ToString();
				}

				[DataMember]
				public string ID { get; set; }

                [IgnoreDataMember]
				public string Name
			    {
                    get { return "Subscription"; }
			    }


				public string GetBlobPath()
				{
					return GetBlobPath(this.ID);
				}

				public static string GetBlobPath(string id)
				{
					return id;
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
			public string SubscriberID { get; set; }
			[DataMember]
			public string OperationActionName { get; set; }
			
			}
			[DataContract]
			public partial class Monitor : IInformationObject
			{
				public Monitor()
				{
					this.ID = "AaltoGlobalImpact.OIP.Monitor" + "." + Guid.NewGuid().ToString();
				}

				[DataMember]
				public string ID { get; set; }

                [IgnoreDataMember]
				public string Name
			    {
                    get { return "Monitor"; }
			    }


				public string GetBlobPath()
				{
					return GetBlobPath(this.ID);
				}

				public static string GetBlobPath(string id)
				{
					return id;
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
					this.ID = "AaltoGlobalImpact.OIP.IconTitleDescription" + "." + Guid.NewGuid().ToString();
				}

				[DataMember]
				public string ID { get; set; }

                [IgnoreDataMember]
				public string Name
			    {
                    get { return "IconTitleDescription"; }
			    }


				public string GetBlobPath()
				{
					return GetBlobPath(this.ID);
				}

				public static string GetBlobPath(string id)
				{
					return id;
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
					this.ID = "AaltoGlobalImpact.OIP.AboutAGIApplications" + "." + Guid.NewGuid().ToString();
				}

				[DataMember]
				public string ID { get; set; }

                [IgnoreDataMember]
				public string Name
			    {
                    get { return "AboutAGIApplications"; }
			    }


				public string GetBlobPath()
				{
					return GetBlobPath(this.ID);
				}

				public static string GetBlobPath(string id)
				{
					return id;
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
					this.ID = "AaltoGlobalImpact.OIP.Icon" + "." + Guid.NewGuid().ToString();
				}

				[DataMember]
				public string ID { get; set; }

                [IgnoreDataMember]
				public string Name
			    {
                    get { return "Icon"; }
			    }


				public string GetBlobPath()
				{
					return GetBlobPath(this.ID);
				}

				public static string GetBlobPath(string id)
				{
					return id;
				}

		// Properties to map to handle the file: Icon.png..png
		// TODO: Handle object collections
			
			}
 } 