namespace AaltoGlobalImpact.OIP
{
    partial class Location
    {
        static partial void CreateCustomDemo(ref Location customDemoObject)
        {
            customDemoObject = Location.CreateDefault();
            customDemoObject.LocationName = "Demo location";
            customDemoObject.Latitude.TextValue = "0";
            customDemoObject.Longitude.TextValue = "0";
        }
    }
}