 


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



			[DataContract]
			public partial class DeviceOperationData 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string OperationRequestString;

			[DataMember]
			public List<string> OperationParameters= new List<string>();

			[DataMember]
			public List<string> OperationReturnValues= new List<string>();

			[DataMember]
			public bool OperationResult;

			[DataMember]
			public List<ContentItemLocationWithMD5> OperationSpecificContentData= new List<ContentItemLocationWithMD5>();

			
			}
			[DataContract]
			public partial class ContentItemLocationWithMD5 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string ContentLocation;

			[DataMember]
			public string ContentMD5;

			[DataMember]
			public List<ItemData> ItemDatas= new List<ItemData>();

			
			}
			[DataContract]
			public partial class ItemData 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string DataName;

			[DataMember]
			public string ItemTextData;

			
			}
 } 