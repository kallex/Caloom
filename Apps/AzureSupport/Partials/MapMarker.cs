namespace AaltoGlobalImpact.OIP
{
    partial class MapMarker
    {
        static partial void CreateCustomDemo(ref MapMarker customDemoObject)
        {
            customDemoObject = CreateDefault();
            customDemoObject.Location.Longitude.TextValue = "0";
            customDemoObject.Location.Latitude.TextValue = "0";
            customDemoObject.Location.LocationName = "Demo location";
            customDemoObject.LocationText = "0,0";
        }
    }
}