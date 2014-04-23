 


namespace TheBall.Index { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;



			[DataContract]
			public partial class UserQuery 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string QueryString;

			[DataMember]
			public string DefaultFieldName;

			
			}
			[DataContract]
			public partial class QueryToken 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string QueryRequestObjectDomainName;

			[DataMember]
			public string QueryRequestObjectName;

			[DataMember]
			public string QueryRequestObjectID;

			
			}
 } 