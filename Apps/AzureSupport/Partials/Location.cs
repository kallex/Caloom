using System;

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

        public string GetLocationText()
        {
            string longitude = String.IsNullOrEmpty(Longitude.TextValue) ? "0" : Longitude.TextValue;
            string latitude = String.IsNullOrEmpty(Latitude.TextValue) ? "0" : Latitude.TextValue;
            return string.Format("{0},{1}", latitude, longitude);
        }
    }
}