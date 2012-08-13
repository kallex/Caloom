 

using System;
using Microsoft.WindowsAzure.StorageClient;

namespace AaltoGlobalImpact.OIP { 
					public partial class Subscription : TableServiceEntity
			{
				public Subscription() 
				{
					this.PartitionKey = "AaltoGlobalImpact.OIP" + Guid.NewGuid().ToString();
					this.RowKey = "Subscription" + Guid.NewGuid().ToString();
				}
			public long Priority { get; set; }
					public string TargetItemPartitionKey { get; set; }
					public string TargetItemRowKey { get; set; }
					public string TargetItemObjectName { get; set; }
					public string TargetItemFieldName { get; set; }
					public string SubscriberPartitionKey { get; set; }
					public string SubscriberRowKey { get; set; }
					public string OperationActionName { get; set; }
					
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
			
			}
			public partial class AboutAGIApplicationsForAllPeopleIcon : TableServiceEntity
			{
				public AboutAGIApplicationsForAllPeopleIcon() 
				{
					this.PartitionKey = "AaltoGlobalImpact.OIP" + Guid.NewGuid().ToString();
					this.RowKey = "AboutAGIApplicationsForAllPeopleIcon" + Guid.NewGuid().ToString();
				}
		// Properties to map to handle the file: AboutAGIApplicationsBuiltForAnybodyIcon.png..png
			
			}
 } 