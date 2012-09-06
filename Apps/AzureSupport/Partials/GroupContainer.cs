using System;
using System.Linq;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class GroupContainer : IAddOperationProvider
    {
        public bool PerformAddOperation()
        {
            return false;
            AddLocationInfo addLocationInfo = AddLocationInfo;
            if (String.IsNullOrEmpty(addLocationInfo.Latitude) ||
                String.IsNullOrEmpty(addLocationInfo.Longitude))
                return false;
            AddLocationInfo = OIP.AddLocationInfo.CreateDefault();
            AddLocationInfo.LocationName = "";
            AddLocationInfo.Latitude = "";
            AddLocationInfo.Longitude = "";
            //AddLocationInfo.Town = "";
            //AddLocationInfo.Country = "";
            Locations.CollectionContent.RemoveAll(loc => loc.LocationName == "Location.LocationName");
            Location location = Location.CreateDefault();
            location.LocationName = addLocationInfo.LocationName;
            location.Latitude.TextValue = addLocationInfo.Latitude;
            location.Longitude.TextValue = addLocationInfo.Longitude;
            Locations.CollectionContent.Add(location);
            return false;
        }

        partial void DoPostStoringExecute(IContainerOwner owner)
        {
            return;
            CloudBlob mainPage =
                StorageSupport.CurrActiveContainer.GetBlob("livesite/oip-layouts/oip-layout-default-view.phtml", owner);
            InformationSourceCollection sources = mainPage.GetBlobInformationSources();
            var source =
                sources.CollectionContent.First(
                    src => src.IsInformationObjectSource && src.SourceInformationObjectType.EndsWith(".MapContainer"));
            MapContainer mapContainer = (MapContainer) source.RetrieveInformationObject();
            mapContainer.MapMarkers.CollectionContent.Clear();
            foreach(var location in Locations.CollectionContent)
            {
                MapMarker marker = MapMarker.CreateDefault();
                marker.Location = location;
                marker.LocationText = location.Latitude.TextValue + "," + location.Longitude.TextValue;
                mapContainer.MapMarkers.CollectionContent.Add(marker);
            }
            StorageSupport.StoreInformation(mapContainer, owner);
            RenderWebSupport.RefreshContent(mainPage);
        }
    }
}