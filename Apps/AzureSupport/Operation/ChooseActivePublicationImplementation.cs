using TheBall;
using TheBall.CORE;

namespace AaltoGlobalImpact.OIP
{
    public class ChooseActivePublicationImplementation
    {
        public static WebPublishInfo GetTarget_PublishInfo(IContainerOwner owner)
        {
            WebPublishInfo publishInfo = WebPublishInfo.RetrieveFromOwnerContent(owner, "default");
            if (publishInfo == null)
            {
                publishInfo = WebPublishInfo.CreateDefault();
                publishInfo.SetLocationAsOwnerContent(owner, "default");
                publishInfo.StoreInformation();
            }
            return publishInfo;
        }

        public static void ExecuteMethod_SetActivePublicationFromName(string publicationName, WebPublishInfo publishInfo)
        {
            throw new System.NotImplementedException();
        }
    }
}