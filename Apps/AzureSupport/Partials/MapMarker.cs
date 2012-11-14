using System;

namespace AaltoGlobalImpact.OIP
{
    partial class MapMarker
    {
        public const string MarkerSourceActivityValue = "Activity";
        public const string MarkerSourceBlogValue = "Blog";
        public const string MarkerSourceLocationValue = "Location";

        static partial void CreateCustomDemo(ref MapMarker customDemoObject)
        {
            customDemoObject = CreateDefault();
            customDemoObject.Location.Longitude.TextValue = "0";
            customDemoObject.Location.Latitude.TextValue = "0";
            customDemoObject.Location.LocationName = "Demo location";
            customDemoObject.LocationText = "0,0";
        }

        public void SetLocationTextFromLocation(Location location)
        {
            string longitude = String.IsNullOrEmpty(location.Longitude.TextValue) ? "0" : location.Longitude.TextValue;
            string latitude = String.IsNullOrEmpty(location.Latitude.TextValue) ? "0" : location.Latitude.TextValue;
            LocationText = string.Format("{0},{1}", longitude, latitude);
        }
    }
}