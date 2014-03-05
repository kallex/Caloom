using System.IO;
using System.Web;
using TheBall;
using TheBall.CORE;

namespace AaltoGlobalImpact.OIP
{
    partial class AddImageInfo : IAddOperationProvider
    {
        public bool PerformAddOperation(string commandName, InformationSourceCollection sources, string requesterLocation, HttpFileCollection files)
        {
            if(ImageTitle == "")
                throw new InvalidDataException("Image title is mandatory");
            //IInformationObject container = sources.GetDefaultSource().RetrieveInformationObject();
            Image image = Image.CreateDefault();
            VirtualOwner owner = VirtualOwner.FigureOwner(this);
            image.SetLocationAsOwnerContent(owner, image.ID);
            image.Title = ImageTitle;
            StorageSupport.StoreInformationMasterFirst(image, owner, true);
            //DefaultViewSupport.CreateDefaultViewRelativeToRequester(requesterLocation, image, owner);
            return true;
        }
    }
}