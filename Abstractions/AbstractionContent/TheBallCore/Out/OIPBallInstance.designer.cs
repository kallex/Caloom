 

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.StorageClient;

namespace AaltoGlobalImpact.OIP { 
					public partial class SubscriptionCollection
			{
				public SubscriptionCollection()
				{
					this.PartitionKey = "AaltoGlobalImpact.OIP" + Guid.NewGuid().ToString();
					this.RowKey = "Subscription" + Guid.NewGuid().ToString();
				}
				[IgnoreDataMember]
				public string PartitionKey { get; set; }
				[IgnoreDataMember]
				public string RowKey { get; set; }

				public string GetBlobPath()
                {
                    return GetBlobPath(this.PartitionKey, this.RowKey);
                }

                public static string GetBlobPath(string partitionKey, string rowKey)
                {
                    return partitionKey + "_" + rowKey;
                }


				[DataMember] public List<Subscription> CollectionContent = new List<Subscription>();

			}

			[DataContract]
			public partial class Subscription
			{
				public Subscription()
				{
				}

			[DataMember] public long Priority { get; set; }
			[DataMember] public string TargetItemPartitionKey { get; set; }
			[DataMember] public string TargetItemRowKey { get; set; }
			[DataMember] public string TargetItemObjectName { get; set; }
			[DataMember] public string TargetItemFieldName { get; set; }
			[DataMember] public string SubscriberPartitionKey { get; set; }
			[DataMember] public string SubscriberRowKey { get; set; }
			[DataMember] public string OperationActionName { get; set; }
			
			}
			public partial class Monitor : TableServiceEntity
			{
				public Monitor() 
				{
					this.PartitionKey = "AaltoGlobalImpact.OIP" + Guid.NewGuid().ToString();
					this.RowKey = "Monitor" + Guid.NewGuid().ToString();
				}
			 public string TargetItemObjectName { get; set; }
			 public string TargetItemFieldName { get; set; }
			 public DateTime MonitoringUtcTimeStampToStart { get; set; }
			 public string MonitoringCycleFrequencyUnit { get; set; }
			 public long MonitoringCycleEveryXthOfUnit { get; set; }
			 public string CustomMonitoringCycleOperationName { get; set; }
			 public string OperationActionName { get; set; }
			
			}
			public partial class AboutAGIApplications : TableServiceEntity
			{
				public AboutAGIApplications() 
				{
					this.PartitionKey = "AaltoGlobalImpact.OIP" + Guid.NewGuid().ToString();
					this.RowKey = "AboutAGIApplications" + Guid.NewGuid().ToString();
				}
			 public string BuiltForAnybodyTitle { get; set; }
			 public string BuiltForAnybodyTextarea { get; set; }
			 public string ForAllPeopleTitle { get; set; }
			 public string ForAllPeopleTextarea { get; set; }
			
			}
			public partial class AboutAGIApplicationsBuiltForAnybodyIcon : TableServiceEntity
			{
				public AboutAGIApplicationsBuiltForAnybodyIcon() 
				{
					this.PartitionKey = "AaltoGlobalImpact.OIP" + Guid.NewGuid().ToString();
					this.RowKey = "AboutAGIApplicationsBuiltForAnybodyIcon" + Guid.NewGuid().ToString();
				}
		// Properties to map to handle the file: AboutAGIApplicationsBuiltForAnybodyIcon.png..png
		// TODO: Handle object collections
			
			}
			public partial class AboutAGIApplicationsForAllPeopleIcon : TableServiceEntity
			{
				public AboutAGIApplicationsForAllPeopleIcon() 
				{
					this.PartitionKey = "AaltoGlobalImpact.OIP" + Guid.NewGuid().ToString();
					this.RowKey = "AboutAGIApplicationsForAllPeopleIcon" + Guid.NewGuid().ToString();
				}
		// Properties to map to handle the file: AboutAGIApplicationsBuiltForAnybodyIcon.png..png
		// TODO: Handle object collections
			
			}
 } 