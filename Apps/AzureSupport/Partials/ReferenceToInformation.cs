using System.Web;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class ReferenceToInformation
    {
        public static string GetDefaultViewURL(IInformationObject informationObject)
        {
            string viewName = informationObject.GetType().FullName + "_DefaultView.phtml";
            string viewItemLocation = informationObject.RelativeLocation;
            string viewItemURL = RenderWebSupport.GetUrlFromRelativeLocation(viewItemLocation);
            return viewName + "?viewItem=" + HttpUtility.UrlEncode(viewItemURL);
        }
    }
}