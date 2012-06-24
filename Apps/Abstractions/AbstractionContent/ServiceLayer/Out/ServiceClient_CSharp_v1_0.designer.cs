 



using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Caloom.Service;
using Caloom.Service;

		
namespace Caloom.MVC.UI
{
		    public class CaloomMainServiceClient
    {
		        

        public static GetEventsResult  GetEventsAroundPOI(LocationPoint PointOfInterest)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetEventsAroundPOI(PointOfInterest);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetActiveEventsAroundPOI(PointOfInterest, When);
            return serviceResponse;
        }
		
		
		        

        public static Caloom5WStructure  Get5W(Guid EventID)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.Get5W(EventID);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetEventsAroundPOI(LocationPoint PointOfInterest)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetEventsAroundPOI(PointOfInterest);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetActiveEventsAroundPOI(PointOfInterest, When);
            return serviceResponse;
        }
		
		
		        

        public static Caloom5WStructure  Get5W(Guid EventID)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.Get5W(EventID);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetEventsAroundPOI(LocationPoint PointOfInterest)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetEventsAroundPOI(PointOfInterest);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetActiveEventsAroundPOI(PointOfInterest, When);
            return serviceResponse;
        }
		
		
		        

        public static Caloom5WStructure  Get5W(Guid EventID)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.Get5W(EventID);
            return serviceResponse;
        }
		
		
		    }
		    public class CaloomMainServiceClient
    {
		        

        public static GetEventsResult  GetEventsAroundPOI(LocationPoint PointOfInterest)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetEventsAroundPOI(PointOfInterest);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetActiveEventsAroundPOI(PointOfInterest, When);
            return serviceResponse;
        }
		
		
		        

        public static Caloom5WStructure  Get5W(Guid EventID)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.Get5W(EventID);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetEventsAroundPOI(LocationPoint PointOfInterest)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetEventsAroundPOI(PointOfInterest);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetActiveEventsAroundPOI(PointOfInterest, When);
            return serviceResponse;
        }
		
		
		        

        public static Caloom5WStructure  Get5W(Guid EventID)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.Get5W(EventID);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetEventsAroundPOI(LocationPoint PointOfInterest)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetEventsAroundPOI(PointOfInterest);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetActiveEventsAroundPOI(PointOfInterest, When);
            return serviceResponse;
        }
		
		
		        

        public static Caloom5WStructure  Get5W(Guid EventID)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.Get5W(EventID);
            return serviceResponse;
        }
		
		
		    }
		    public class CaloomMainServiceClient
    {
		        

        public static GetEventsResult  GetEventsAroundPOI(LocationPoint PointOfInterest)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetEventsAroundPOI(PointOfInterest);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetActiveEventsAroundPOI(PointOfInterest, When);
            return serviceResponse;
        }
		
		
		        

        public static Caloom5WStructure  Get5W(Guid EventID)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.Get5W(EventID);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetEventsAroundPOI(LocationPoint PointOfInterest)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetEventsAroundPOI(PointOfInterest);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetActiveEventsAroundPOI(PointOfInterest, When);
            return serviceResponse;
        }
		
		
		        

        public static Caloom5WStructure  Get5W(Guid EventID)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.Get5W(EventID);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetEventsAroundPOI(LocationPoint PointOfInterest)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetEventsAroundPOI(PointOfInterest);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetActiveEventsAroundPOI(PointOfInterest, When);
            return serviceResponse;
        }
		
		
		        

        public static Caloom5WStructure  Get5W(Guid EventID)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.Get5W(EventID);
            return serviceResponse;
        }
		
		
		    }
		}
		
namespace Caloom.MVC.UI
{
		    public class CaloomMainServiceClient
    {
		        

        public static GetEventsResult  GetEventsAroundPOI(LocationPoint PointOfInterest)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetEventsAroundPOI(PointOfInterest);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetActiveEventsAroundPOI(PointOfInterest, When);
            return serviceResponse;
        }
		
		
		        

        public static Caloom5WStructure  Get5W(Guid EventID)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.Get5W(EventID);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetEventsAroundPOI(LocationPoint PointOfInterest)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetEventsAroundPOI(PointOfInterest);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetActiveEventsAroundPOI(PointOfInterest, When);
            return serviceResponse;
        }
		
		
		        

        public static Caloom5WStructure  Get5W(Guid EventID)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.Get5W(EventID);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetEventsAroundPOI(LocationPoint PointOfInterest)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetEventsAroundPOI(PointOfInterest);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetActiveEventsAroundPOI(PointOfInterest, When);
            return serviceResponse;
        }
		
		
		        

        public static Caloom5WStructure  Get5W(Guid EventID)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.Get5W(EventID);
            return serviceResponse;
        }
		
		
		    }
		    public class CaloomMainServiceClient
    {
		        

        public static GetEventsResult  GetEventsAroundPOI(LocationPoint PointOfInterest)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetEventsAroundPOI(PointOfInterest);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetActiveEventsAroundPOI(PointOfInterest, When);
            return serviceResponse;
        }
		
		
		        

        public static Caloom5WStructure  Get5W(Guid EventID)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.Get5W(EventID);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetEventsAroundPOI(LocationPoint PointOfInterest)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetEventsAroundPOI(PointOfInterest);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetActiveEventsAroundPOI(PointOfInterest, When);
            return serviceResponse;
        }
		
		
		        

        public static Caloom5WStructure  Get5W(Guid EventID)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.Get5W(EventID);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetEventsAroundPOI(LocationPoint PointOfInterest)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetEventsAroundPOI(PointOfInterest);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetActiveEventsAroundPOI(PointOfInterest, When);
            return serviceResponse;
        }
		
		
		        

        public static Caloom5WStructure  Get5W(Guid EventID)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.Get5W(EventID);
            return serviceResponse;
        }
		
		
		    }
		    public class CaloomMainServiceClient
    {
		        

        public static GetEventsResult  GetEventsAroundPOI(LocationPoint PointOfInterest)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetEventsAroundPOI(PointOfInterest);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetActiveEventsAroundPOI(PointOfInterest, When);
            return serviceResponse;
        }
		
		
		        

        public static Caloom5WStructure  Get5W(Guid EventID)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.Get5W(EventID);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetEventsAroundPOI(LocationPoint PointOfInterest)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetEventsAroundPOI(PointOfInterest);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetActiveEventsAroundPOI(PointOfInterest, When);
            return serviceResponse;
        }
		
		
		        

        public static Caloom5WStructure  Get5W(Guid EventID)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.Get5W(EventID);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetEventsAroundPOI(LocationPoint PointOfInterest)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetEventsAroundPOI(PointOfInterest);
            return serviceResponse;
        }
		
		
		        

        public static GetEventsResult  GetActiveEventsAroundPOI(LocationPoint PointOfInterest, TimeRange When)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.GetActiveEventsAroundPOI(PointOfInterest, When);
            return serviceResponse;
        }
		
		
		        

        public static Caloom5WStructure  Get5W(Guid EventID)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:56071/CaloomMainService.svc");
            ICaloomMainService service =
                new ChannelFactory< ICaloomMainService >(basicHttpBinding, endpointAddress).CreateChannel();
            var serviceResponse = service.Get5W(EventID);
            return serviceResponse;
        }
		
		
		    }
		}
		