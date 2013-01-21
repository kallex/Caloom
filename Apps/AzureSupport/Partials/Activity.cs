using System;

namespace AaltoGlobalImpact.OIP
{
    partial class Activity : IBeforeStoreHandler
    {
        public void PerformBeforeStoreUpdate()
        {
            if (ReferenceToInformation == null)
                ReferenceToInformation = OIP.ReferenceToInformation.CreateDefault();
            ReferenceToInformation.Title = this.ActivityName;
            ReferenceToInformation.URL = DefaultViewSupport.GetDefaultViewURL(this);
            this.LocationCollection.IsCollectionFiltered = true;
            this.CategoryCollection.IsCollectionFiltered = true;
            this.ImageGroupCollection.IsCollectionFiltered = true;
            if (Excerpt == null)
                Excerpt = "";
            if (Excerpt.Length > 200)
                Excerpt = Excerpt.Substring(0, 200);
        }
    }
}