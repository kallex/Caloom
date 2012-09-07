using System;
using System.Linq;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class GroupContainer 
    {
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