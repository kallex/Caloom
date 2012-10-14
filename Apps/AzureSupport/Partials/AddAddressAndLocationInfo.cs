using System;
using System.IO;
using System.Linq;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class AddAddressAndLocationInfo : IAddOperationProvider
    {
        public bool PerformAddOperation(InformationSourceCollection sources, string requesterLocation)
        {
            if (LocationName == "")
                throw new InvalidDataException("Location name is mandatory");
            IInformationObject container = sources.GetDefaultSource().RetrieveInformationObject();
            AddressAndLocation location = AddressAndLocation.CreateDefault();

            VirtualOwner owner = VirtualOwner.FigureOwner(this);
            location.SetLocationAsOwnerContent(owner, location.ID);
            location.Location.LocationName = LocationName;
            location.ReferenceToInformation.Title = location.Location.LocationName;
            location.ReferenceToInformation.URL = DefaultViewSupport.GetDefaultViewURL(location);
            StorageSupport.StoreInformation(location);
            DefaultViewSupport.CreateDefaultViewRelativeToRequester(requesterLocation, location, owner);

            LocationContainer locationContainer = LocationContainer.RetrieveFromOwnerContent(owner, "Locations");
            locationContainer.Locations.CollectionContent.Add(location);
            StorageSupport.StoreInformation(locationContainer);

            AccountContainer accountContainer = container as AccountContainer;
            GroupContainer groupContainer = container as GroupContainer;
            if (accountContainer != null)
            {
                accountContainer.AccountModule.LocationCollection.CollectionContent.Add(location);
            } else if(groupContainer != null)
            {
                //groupContainer.Locations.CollectionContent.Add(location);
            }
            StorageSupport.StoreInformation(container);
            return false;
            
            /*
            location.Address = Address;
            location.Location.Longitude.TextValue = this.Longitude;
            location.Location.Latitude.TextValue = this.Latitude;
            location.Location.LocationName = this.LocationName;
            container.AccountModule.LocationCollection.CollectionContent.Add(location);
            StorageSupport.StoreInformation(container);
            VirtualOwner owner = VirtualOwner.FigureOwner(this);
            MapContainer mapContainer = MapContainer.RetrieveFromOwnerContent(owner, "default");
            mapContainer.MapMarkers.CollectionContent.Clear();
            mapContainer.MapMarkers.CollectionContent.AddRange(
                container.AccountModule.LocationCollection.CollectionContent.
                    Select(loc =>
                               {
                                   MapMarker marker = MapMarker.CreateDefault();
                                   marker.Location = loc.Location;
                                   marker.LocationText = string.Format("{0},{1}",
                                                                       loc.Location.Latitude.TextValue,
                                                                       loc.Location.Longitude.TextValue);
                                   return marker;
                               }));
            StorageSupport.StoreInformation(mapContainer);
            LocationName = "";
            Address = StreetAddress.CreateDefault();
            Longitude = "";
            Latitude = "";
             */
            return true;
        }
    }
}