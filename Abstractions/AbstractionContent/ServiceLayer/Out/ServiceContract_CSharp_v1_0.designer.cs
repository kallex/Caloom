 

using System.Runtime.Serialization;
using System.ServiceModel;
		// Services
namespace Caloom.Service {
		    [ServiceContract]
    public interface ICaloomMainService
    {

		        [OperationContract]
        GetEventsResult GetEventsAroundPOI(LocationPoint PointOfInterest);
		
		        [OperationContract]
        GetEventsResult GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When);
		
		        [OperationContract]
        Caloom5WStructure Get5W(Guid EventID);
		
		        [OperationContract]
        GetEventsResult GetEventsAroundPOI(LocationPoint PointOfInterest);
		
		        [OperationContract]
        GetEventsResult GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When);
		
		        [OperationContract]
        Caloom5WStructure Get5W(Guid EventID);
		
		        [OperationContract]
        GetEventsResult GetEventsAroundPOI(LocationPoint PointOfInterest);
		
		        [OperationContract]
        GetEventsResult GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When);
		
		        [OperationContract]
        Caloom5WStructure Get5W(Guid EventID);
		
		    }
		
		    [ServiceContract]
    public interface ICaloomMainService
    {

		        [OperationContract]
        GetEventsResult GetEventsAroundPOI(LocationPoint PointOfInterest);
		
		        [OperationContract]
        GetEventsResult GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When);
		
		        [OperationContract]
        Caloom5WStructure Get5W(Guid EventID);
		
		        [OperationContract]
        GetEventsResult GetEventsAroundPOI(LocationPoint PointOfInterest);
		
		        [OperationContract]
        GetEventsResult GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When);
		
		        [OperationContract]
        Caloom5WStructure Get5W(Guid EventID);
		
		        [OperationContract]
        GetEventsResult GetEventsAroundPOI(LocationPoint PointOfInterest);
		
		        [OperationContract]
        GetEventsResult GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When);
		
		        [OperationContract]
        Caloom5WStructure Get5W(Guid EventID);
		
		    }
		
		    [ServiceContract]
    public interface ICaloomMainService
    {

		        [OperationContract]
        GetEventsResult GetEventsAroundPOI(LocationPoint PointOfInterest);
		
		        [OperationContract]
        GetEventsResult GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When);
		
		        [OperationContract]
        Caloom5WStructure Get5W(Guid EventID);
		
		        [OperationContract]
        GetEventsResult GetEventsAroundPOI(LocationPoint PointOfInterest);
		
		        [OperationContract]
        GetEventsResult GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When);
		
		        [OperationContract]
        Caloom5WStructure Get5W(Guid EventID);
		
		        [OperationContract]
        GetEventsResult GetEventsAroundPOI(LocationPoint PointOfInterest);
		
		        [OperationContract]
        GetEventsResult GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When);
		
		        [OperationContract]
        Caloom5WStructure Get5W(Guid EventID);
		
		    }
		
		    [DataContract]
    public class Caloom5WStructure
    {
		        WhatData _What;

        [DataMember]
        public WhatData What
        {
            get { return _What; }
            set { _What = value; }
        }
		        WhereData _Where;

        [DataMember]
        public WhereData Where
        {
            get { return _Where; }
            set { _Where = value; }
        }
		        WhenData _When;

        [DataMember]
        public WhenData When
        {
            get { return _When; }
            set { _When = value; }
        }
		        WhomData _Whom;

        [DataMember]
        public WhomData Whom
        {
            get { return _Whom; }
            set { _Whom = value; }
        }
		        WorthData _Worth;

        [DataMember]
        public WorthData Worth
        {
            get { return _Worth; }
            set { _Worth = value; }
        }
			}
		    [DataContract]
    public class WhenData
    {
		        DateTime _StartTime;

        [DataMember]
        public DateTime StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }
		        DateTime _EndTime;

        [DataMember]
        public DateTime EndTime
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }
			}
		    [DataContract]
    public class WhereData
    {
		        decimal _Latitude;

        [DataMember]
        public decimal Latitude
        {
            get { return _Latitude; }
            set { _Latitude = value; }
        }
		        decimal _Longitude;

        [DataMember]
        public decimal Longitude
        {
            get { return _Longitude; }
            set { _Longitude = value; }
        }
		        decimal _RangeInMeters;

        [DataMember]
        public decimal RangeInMeters
        {
            get { return _RangeInMeters; }
            set { _RangeInMeters = value; }
        }
			}
		    [DataContract]
    public class CaloomEvent
    {
		        Guid _ID;

        [DataMember]
        public Guid ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
		        Caloom5WData _CaloomCoreData;

        [DataMember]
        public Caloom5WData CaloomCoreData
        {
            get { return _CaloomCoreData; }
            set { _CaloomCoreData = value; }
        }
			}
		    [DataContract]
    public class GetEventsResult
    {
		        CaloomEvent[] _EventArray;

        [DataMember]
        public CaloomEvent[] EventArray
        {
            get { return _EventArray; }
            set { _EventArray = value; }
        }
			}
		    [DataContract]
    public class Caloom5WStructure
    {
		        WhatData _What;

        [DataMember]
        public WhatData What
        {
            get { return _What; }
            set { _What = value; }
        }
		        WhereData _Where;

        [DataMember]
        public WhereData Where
        {
            get { return _Where; }
            set { _Where = value; }
        }
		        WhenData _When;

        [DataMember]
        public WhenData When
        {
            get { return _When; }
            set { _When = value; }
        }
		        WhomData _Whom;

        [DataMember]
        public WhomData Whom
        {
            get { return _Whom; }
            set { _Whom = value; }
        }
		        WorthData _Worth;

        [DataMember]
        public WorthData Worth
        {
            get { return _Worth; }
            set { _Worth = value; }
        }
			}
		    [DataContract]
    public class WhenData
    {
		        DateTime _StartTime;

        [DataMember]
        public DateTime StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }
		        DateTime _EndTime;

        [DataMember]
        public DateTime EndTime
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }
			}
		    [DataContract]
    public class WhereData
    {
		        decimal _Latitude;

        [DataMember]
        public decimal Latitude
        {
            get { return _Latitude; }
            set { _Latitude = value; }
        }
		        decimal _Longitude;

        [DataMember]
        public decimal Longitude
        {
            get { return _Longitude; }
            set { _Longitude = value; }
        }
		        decimal _RangeInMeters;

        [DataMember]
        public decimal RangeInMeters
        {
            get { return _RangeInMeters; }
            set { _RangeInMeters = value; }
        }
			}
		    [DataContract]
    public class CaloomEvent
    {
		        Guid _ID;

        [DataMember]
        public Guid ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
		        Caloom5WData _CaloomCoreData;

        [DataMember]
        public Caloom5WData CaloomCoreData
        {
            get { return _CaloomCoreData; }
            set { _CaloomCoreData = value; }
        }
			}
		    [DataContract]
    public class GetEventsResult
    {
		        CaloomEvent[] _EventArray;

        [DataMember]
        public CaloomEvent[] EventArray
        {
            get { return _EventArray; }
            set { _EventArray = value; }
        }
			}
		    [DataContract]
    public class Caloom5WStructure
    {
		        WhatData _What;

        [DataMember]
        public WhatData What
        {
            get { return _What; }
            set { _What = value; }
        }
		        WhereData _Where;

        [DataMember]
        public WhereData Where
        {
            get { return _Where; }
            set { _Where = value; }
        }
		        WhenData _When;

        [DataMember]
        public WhenData When
        {
            get { return _When; }
            set { _When = value; }
        }
		        WhomData _Whom;

        [DataMember]
        public WhomData Whom
        {
            get { return _Whom; }
            set { _Whom = value; }
        }
		        WorthData _Worth;

        [DataMember]
        public WorthData Worth
        {
            get { return _Worth; }
            set { _Worth = value; }
        }
			}
		    [DataContract]
    public class WhenData
    {
		        DateTime _StartTime;

        [DataMember]
        public DateTime StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }
		        DateTime _EndTime;

        [DataMember]
        public DateTime EndTime
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }
			}
		    [DataContract]
    public class WhereData
    {
		        decimal _Latitude;

        [DataMember]
        public decimal Latitude
        {
            get { return _Latitude; }
            set { _Latitude = value; }
        }
		        decimal _Longitude;

        [DataMember]
        public decimal Longitude
        {
            get { return _Longitude; }
            set { _Longitude = value; }
        }
		        decimal _RangeInMeters;

        [DataMember]
        public decimal RangeInMeters
        {
            get { return _RangeInMeters; }
            set { _RangeInMeters = value; }
        }
			}
		    [DataContract]
    public class CaloomEvent
    {
		        Guid _ID;

        [DataMember]
        public Guid ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
		        Caloom5WData _CaloomCoreData;

        [DataMember]
        public Caloom5WData CaloomCoreData
        {
            get { return _CaloomCoreData; }
            set { _CaloomCoreData = value; }
        }
			}
		    [DataContract]
    public class GetEventsResult
    {
		        CaloomEvent[] _EventArray;

        [DataMember]
        public CaloomEvent[] EventArray
        {
            get { return _EventArray; }
            set { _EventArray = value; }
        }
			}
		}
		// Services
namespace Caloom.Service {
		    [ServiceContract]
    public interface ICaloomMainService
    {

		        [OperationContract]
        GetEventsResult GetEventsAroundPOI(LocationPoint PointOfInterest);
		
		        [OperationContract]
        GetEventsResult GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When);
		
		        [OperationContract]
        Caloom5WStructure Get5W(Guid EventID);
		
		        [OperationContract]
        GetEventsResult GetEventsAroundPOI(LocationPoint PointOfInterest);
		
		        [OperationContract]
        GetEventsResult GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When);
		
		        [OperationContract]
        Caloom5WStructure Get5W(Guid EventID);
		
		        [OperationContract]
        GetEventsResult GetEventsAroundPOI(LocationPoint PointOfInterest);
		
		        [OperationContract]
        GetEventsResult GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When);
		
		        [OperationContract]
        Caloom5WStructure Get5W(Guid EventID);
		
		    }
		
		    [ServiceContract]
    public interface ICaloomMainService
    {

		        [OperationContract]
        GetEventsResult GetEventsAroundPOI(LocationPoint PointOfInterest);
		
		        [OperationContract]
        GetEventsResult GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When);
		
		        [OperationContract]
        Caloom5WStructure Get5W(Guid EventID);
		
		        [OperationContract]
        GetEventsResult GetEventsAroundPOI(LocationPoint PointOfInterest);
		
		        [OperationContract]
        GetEventsResult GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When);
		
		        [OperationContract]
        Caloom5WStructure Get5W(Guid EventID);
		
		        [OperationContract]
        GetEventsResult GetEventsAroundPOI(LocationPoint PointOfInterest);
		
		        [OperationContract]
        GetEventsResult GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When);
		
		        [OperationContract]
        Caloom5WStructure Get5W(Guid EventID);
		
		    }
		
		    [ServiceContract]
    public interface ICaloomMainService
    {

		        [OperationContract]
        GetEventsResult GetEventsAroundPOI(LocationPoint PointOfInterest);
		
		        [OperationContract]
        GetEventsResult GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When);
		
		        [OperationContract]
        Caloom5WStructure Get5W(Guid EventID);
		
		        [OperationContract]
        GetEventsResult GetEventsAroundPOI(LocationPoint PointOfInterest);
		
		        [OperationContract]
        GetEventsResult GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When);
		
		        [OperationContract]
        Caloom5WStructure Get5W(Guid EventID);
		
		        [OperationContract]
        GetEventsResult GetEventsAroundPOI(LocationPoint PointOfInterest);
		
		        [OperationContract]
        GetEventsResult GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When);
		
		        [OperationContract]
        Caloom5WStructure Get5W(Guid EventID);
		
		    }
		
		    [DataContract]
    public class Caloom5WStructure
    {
		        WhatData _What;

        [DataMember]
        public WhatData What
        {
            get { return _What; }
            set { _What = value; }
        }
		        WhereData _Where;

        [DataMember]
        public WhereData Where
        {
            get { return _Where; }
            set { _Where = value; }
        }
		        WhenData _When;

        [DataMember]
        public WhenData When
        {
            get { return _When; }
            set { _When = value; }
        }
		        WhomData _Whom;

        [DataMember]
        public WhomData Whom
        {
            get { return _Whom; }
            set { _Whom = value; }
        }
		        WorthData _Worth;

        [DataMember]
        public WorthData Worth
        {
            get { return _Worth; }
            set { _Worth = value; }
        }
			}
		    [DataContract]
    public class WhenData
    {
		        DateTime _StartTime;

        [DataMember]
        public DateTime StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }
		        DateTime _EndTime;

        [DataMember]
        public DateTime EndTime
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }
			}
		    [DataContract]
    public class WhereData
    {
		        decimal _Latitude;

        [DataMember]
        public decimal Latitude
        {
            get { return _Latitude; }
            set { _Latitude = value; }
        }
		        decimal _Longitude;

        [DataMember]
        public decimal Longitude
        {
            get { return _Longitude; }
            set { _Longitude = value; }
        }
		        decimal _RangeInMeters;

        [DataMember]
        public decimal RangeInMeters
        {
            get { return _RangeInMeters; }
            set { _RangeInMeters = value; }
        }
			}
		    [DataContract]
    public class CaloomEvent
    {
		        Guid _ID;

        [DataMember]
        public Guid ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
		        Caloom5WData _CaloomCoreData;

        [DataMember]
        public Caloom5WData CaloomCoreData
        {
            get { return _CaloomCoreData; }
            set { _CaloomCoreData = value; }
        }
			}
		    [DataContract]
    public class GetEventsResult
    {
		        CaloomEvent[] _EventArray;

        [DataMember]
        public CaloomEvent[] EventArray
        {
            get { return _EventArray; }
            set { _EventArray = value; }
        }
			}
		    [DataContract]
    public class Caloom5WStructure
    {
		        WhatData _What;

        [DataMember]
        public WhatData What
        {
            get { return _What; }
            set { _What = value; }
        }
		        WhereData _Where;

        [DataMember]
        public WhereData Where
        {
            get { return _Where; }
            set { _Where = value; }
        }
		        WhenData _When;

        [DataMember]
        public WhenData When
        {
            get { return _When; }
            set { _When = value; }
        }
		        WhomData _Whom;

        [DataMember]
        public WhomData Whom
        {
            get { return _Whom; }
            set { _Whom = value; }
        }
		        WorthData _Worth;

        [DataMember]
        public WorthData Worth
        {
            get { return _Worth; }
            set { _Worth = value; }
        }
			}
		    [DataContract]
    public class WhenData
    {
		        DateTime _StartTime;

        [DataMember]
        public DateTime StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }
		        DateTime _EndTime;

        [DataMember]
        public DateTime EndTime
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }
			}
		    [DataContract]
    public class WhereData
    {
		        decimal _Latitude;

        [DataMember]
        public decimal Latitude
        {
            get { return _Latitude; }
            set { _Latitude = value; }
        }
		        decimal _Longitude;

        [DataMember]
        public decimal Longitude
        {
            get { return _Longitude; }
            set { _Longitude = value; }
        }
		        decimal _RangeInMeters;

        [DataMember]
        public decimal RangeInMeters
        {
            get { return _RangeInMeters; }
            set { _RangeInMeters = value; }
        }
			}
		    [DataContract]
    public class CaloomEvent
    {
		        Guid _ID;

        [DataMember]
        public Guid ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
		        Caloom5WData _CaloomCoreData;

        [DataMember]
        public Caloom5WData CaloomCoreData
        {
            get { return _CaloomCoreData; }
            set { _CaloomCoreData = value; }
        }
			}
		    [DataContract]
    public class GetEventsResult
    {
		        CaloomEvent[] _EventArray;

        [DataMember]
        public CaloomEvent[] EventArray
        {
            get { return _EventArray; }
            set { _EventArray = value; }
        }
			}
		    [DataContract]
    public class Caloom5WStructure
    {
		        WhatData _What;

        [DataMember]
        public WhatData What
        {
            get { return _What; }
            set { _What = value; }
        }
		        WhereData _Where;

        [DataMember]
        public WhereData Where
        {
            get { return _Where; }
            set { _Where = value; }
        }
		        WhenData _When;

        [DataMember]
        public WhenData When
        {
            get { return _When; }
            set { _When = value; }
        }
		        WhomData _Whom;

        [DataMember]
        public WhomData Whom
        {
            get { return _Whom; }
            set { _Whom = value; }
        }
		        WorthData _Worth;

        [DataMember]
        public WorthData Worth
        {
            get { return _Worth; }
            set { _Worth = value; }
        }
			}
		    [DataContract]
    public class WhenData
    {
		        DateTime _StartTime;

        [DataMember]
        public DateTime StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }
		        DateTime _EndTime;

        [DataMember]
        public DateTime EndTime
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }
			}
		    [DataContract]
    public class WhereData
    {
		        decimal _Latitude;

        [DataMember]
        public decimal Latitude
        {
            get { return _Latitude; }
            set { _Latitude = value; }
        }
		        decimal _Longitude;

        [DataMember]
        public decimal Longitude
        {
            get { return _Longitude; }
            set { _Longitude = value; }
        }
		        decimal _RangeInMeters;

        [DataMember]
        public decimal RangeInMeters
        {
            get { return _RangeInMeters; }
            set { _RangeInMeters = value; }
        }
			}
		    [DataContract]
    public class CaloomEvent
    {
		        Guid _ID;

        [DataMember]
        public Guid ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
		        Caloom5WData _CaloomCoreData;

        [DataMember]
        public Caloom5WData CaloomCoreData
        {
            get { return _CaloomCoreData; }
            set { _CaloomCoreData = value; }
        }
			}
		    [DataContract]
    public class GetEventsResult
    {
		        CaloomEvent[] _EventArray;

        [DataMember]
        public CaloomEvent[] EventArray
        {
            get { return _EventArray; }
            set { _EventArray = value; }
        }
			}
		}
		