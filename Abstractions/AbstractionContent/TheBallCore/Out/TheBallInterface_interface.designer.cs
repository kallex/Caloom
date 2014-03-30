 


namespace TheBall.Interface { 
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



			[DataContract]
			public partial class ConnectionCommunicationData 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string ActiveSideConnectionID;

			[DataMember]
			public string ReceivingSideConnectionID;

			[DataMember]
			public string ProcessRequest;

			[DataMember]
			public string ProcessParametersString;

			[DataMember]
			public string ProcessResultString;

			[DataMember]
			public List<string> ProcessResultArray= new List<string>();

			[DataMember]
			public List<CategoryInfo> CategoryCollectionData= new List<CategoryInfo>();

			[DataMember]
			public List<CategoryLinkItem> LinkItems= new List<CategoryLinkItem>();

			
			}
			[DataContract]
			public partial class CategoryInfo 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string CategoryID;

			[DataMember]
			public string NativeCategoryID;

			[DataMember]
			public string NativeCategoryDomainName;

			[DataMember]
			public string NativeCategoryObjectName;

			[DataMember]
			public string NativeCategoryTitle;

			[DataMember]
			public string IdentifyingCategoryName;

			[DataMember]
			public string ParentCategoryID;

			
			}
			[DataContract]
			public partial class CategoryLinkParameters 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string ConnectionID;

			[DataMember]
			public List<CategoryLinkItem> LinkItems= new List<CategoryLinkItem>();

			
			}
			[DataContract]
			public partial class CategoryLinkItem 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string SourceCategoryID;

			[DataMember]
			public string TargetCategoryID;

			[DataMember]
			public string LinkingType;

			
			}
 } 