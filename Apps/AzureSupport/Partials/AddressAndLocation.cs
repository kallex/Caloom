using System;

namespace AaltoGlobalImpact.OIP
{
    partial class AddressAndLocation : IBeforeStoreHandler
    {
        public void PerformBeforeStoreUpdate()
        {
            if (ReferenceToInformation == null)
                ReferenceToInformation = ReferenceToInformation.CreateDefault();
            ReferenceToInformation.Title = this.Location.LocationName;
            ReferenceToInformation.URL = RelativeLocation; // DefaultViewSupport.GetDefaultViewURL(this);
            if (String.IsNullOrEmpty(Location.Latitude.TextValue))
                Location.Latitude.TextValue = "0";
            if (String.IsNullOrEmpty(Location.Longitude.TextValue))
                Location.Longitude.TextValue = "0";
        }
    }
}