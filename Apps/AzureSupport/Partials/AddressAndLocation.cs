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
            ReferenceToInformation.URL = DefaultViewSupport.GetDefaultViewURL(this);
        }
    }
}