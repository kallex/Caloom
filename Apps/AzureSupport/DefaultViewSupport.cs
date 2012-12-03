using System;
using System.Reflection;
using System.Web;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    public class DefaultViewSupport
    {
        public static string GetDefaultViewURL(IInformationObject informationObject)
        {
            return GetDefaultStaticViewURL(informationObject);
        }

        public static string GetDefaultStaticViewURL(IInformationObject informationObject)
        {
            string viewUrl = GetDefaultStaticViewName(informationObject);
            return viewUrl;
        }

        public static string GetDefaultStaticViewName(IInformationObject informationObject)
        {
            string viewName = informationObject.GetType().FullName + "_" + informationObject.ID + "_DefaultView.phtml";
            return viewName;
        }

        private static string GetDefaultDynamicRenderingViewURL(IInformationObject informationObject)
        {
            string viewName = informationObject.GetType().FullName + "_DefaultView.phtml";
            string viewItemLocation = informationObject.RelativeLocation;
            string viewItemURL = RenderWebSupport.GetUrlFromRelativeLocation(viewItemLocation);
            return viewName + "?viewItem=" + HttpUtility.UrlEncode(viewItemURL);
        }

        public static void RefreshDefaultViews(string viewLocation, Type informationObjectType)
        {
            if (viewLocation.EndsWith("/") == false)
                viewLocation = viewLocation + "/";
            VirtualOwner owner = VirtualOwner.FigureOwner(viewLocation);
            //string typeDirectoryName = informationObjectType.FullName;
            IInformationObject[] informationObjects = (IInformationObject[]) 
                informationObjectType.InvokeMember("RetrieveCollectionFromOwnerContent", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, null, null, new object[] { owner });

            foreach (IInformationObject informationObject in informationObjects)
            {
                CreateDefaultViewRelativeToRequester(viewLocation, informationObject, owner);
            }
        }

        public static CloudBlob CreateDefaultViewRelativeToRequester(string requesterLocation, IInformationObject informationObject, IContainerOwner owner)
        {
            string viewItemDirectory = StorageSupport.GetLocationParentDirectory(requesterLocation);
            string viewName = GetDefaultStaticViewName(informationObject);
            string viewTemplateName = GetDefaultStaticTemplateName(informationObject);
            // TODO: Relative from xyzsite => xyztemplate; now we only have website - also acct/grp specific
            //string viewTemplateLocation = "webtemplate/oip-viewtemplate/" + viewTemplateName;
            string viewTemplateLocation = viewItemDirectory + viewTemplateName;
            CloudBlob viewTemplate = StorageSupport.CurrActiveContainer.GetBlob(viewTemplateLocation, owner);
            string renderedViewLocation = viewItemDirectory + viewName;
            CloudBlob renderTarget = StorageSupport.CurrActiveContainer.GetBlob(renderedViewLocation, owner);
            InformationSource defaultSource = InformationSource.GetAsDefaultSource(informationObject);
            RenderWebSupport.RenderTemplateWithContentToBlob(viewTemplate, renderTarget, defaultSource);
            return renderTarget;
        }

        public static string GetDefaultStaticTemplateName(IInformationObject informationObject)
        {
            string templateName = informationObject.GetType().FullName + "_DefaultView.phtml";
            return templateName;
        }
    }
}