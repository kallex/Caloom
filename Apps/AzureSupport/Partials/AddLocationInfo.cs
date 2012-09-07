using System.IO;
using System.Linq;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class AddLocationInfo : IAddOperationProvider
    {
        public bool PerformAddOperation(InformationSourceCollection sources)
        {
            if (LocationName == "")
                throw new InvalidDataException("Location name is mandatory");
            AccountContainer container = (AccountContainer)sources.GetDefaultSource().RetrieveInformationObject();
            AddressAndLocation location = AddressAndLocation.CreateDefault();
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
            return true;
        }
    }
}