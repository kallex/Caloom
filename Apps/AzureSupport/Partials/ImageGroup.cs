using System.Linq;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class ImageGroup : IBeforeStoreHandler
    {
        public void PerformBeforeStoreUpdate()
        {
            if (ReferenceToInformation == null)
                ReferenceToInformation = OIP.ReferenceToInformation.CreateDefault();
            this.ReferenceToInformation.Title = this.Title;
            ReferenceToInformation.URL = DefaultViewSupport.GetDefaultViewURL(this);
            FeaturedImage = ImagesCollection.CollectionContent.FirstOrDefault();
        }

        partial void DoPostDeleteExecute(TheBall.CORE.IContainerOwner owner)
        {
            foreach (var image in ImagesCollection.CollectionContent)
                image.DeleteInformationObject();
        }
    }
}