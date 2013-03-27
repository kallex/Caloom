using System;
using System.IO;
using System.Reflection;
using System.Web;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;
using TheBall.CORE;

namespace AaltoGlobalImpact.OIP
{
    public class DefaultViewSupport
    {
        public static string[] FixedAccountSiteLocations = new string[]
                                                                {
                                                                    RenderWebSupport.DefaultAccountViewLocation,
                                                                };

        public static string[] FixedGroupSiteLocations = new string[]
                                                              {
                                                                  RenderWebSupport.DefaultGroupViewLocation,
                                                                  RenderWebSupport.DefaultPublicGroupViewLocation,
                                                                  RenderWebSupport.DefaultPublicWwwViewLocation
                                                              };

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

        /// <summary>
        /// Creates default views and returns the one relative to the requester
        /// </summary>
        /// <param name="requesterLocation">Requester relative location</param>
        /// <param name="informationObject">Information object to create the view for</param>
        /// <param name="owner">Container owner</param>
        /// <returns></returns>
        public static CloudBlob CreateDefaultViewRelativeToRequester(string requesterLocation, IInformationObject informationObject, IContainerOwner owner)
        {
            bool isAccountOwner = owner.IsAccountContainer();
            bool isGroupOwner = owner.IsGroupContainer();
            bool isDeveloperView = owner.ContainerName == "dev";
            string[] viewLocations;
            if (isAccountOwner)
                viewLocations = FixedAccountSiteLocations;
            else if (isGroupOwner)
                viewLocations = FixedGroupSiteLocations;
            else throw new NotSupportedException("Invalid owner container type for default view (non acct, non group): " + owner.ContainerName);

            string requesterDirectory = StorageSupport.GetLocationParentDirectory(requesterLocation);
            FileInfo fileInfo = new FileInfo(requesterLocation);
            //string viewRoot = fileInfo.Directory.Parent != null
            //                      ? fileInfo.Directory.Parent.Name
            //                      : fileInfo.Directory.Name;
            CloudBlob relativeViewBlob = null;
            bool hasException = false;
            bool allException = true;
            foreach (string viewLocation in viewLocations)
            {
                try
                {
                    string viewRoot = isDeveloperView ? "developer-00000000000000000000000000" : GetViewRoot(viewLocation);
                    string viewItemDirectory = Path.Combine(viewRoot, viewLocation).Replace("\\", "/") + "/";
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
                    if (viewItemDirectory == requesterDirectory)
                        relativeViewBlob = renderTarget;
                    allException = false;
                }
                catch (Exception ex)
                {
                    hasException = true;
                }

            }
            if (relativeViewBlob == null && hasException == false && false)
                throw new InvalidDataException(
                    String.Format("Default view with relative location {0} not found for owner type {1}",
                                  requesterLocation, owner.ContainerName));
            return relativeViewBlob;
        }

        private static string GetViewRoot(string viewLocation)
        {
            switch(viewLocation)
            {
                case RenderWebSupport.DefaultAccountViewLocation:
                    return "website";
                case RenderWebSupport.DefaultGroupViewLocation:
                    return "website";
                case RenderWebSupport.DefaultPublicGroupViewLocation:
                    return "publicsite";
                case RenderWebSupport.DefaultPublicWwwViewLocation:
                    return "wwwsite";
                default:
                    throw new NotSupportedException("Not supported root location for view location: " + viewLocation);
            }
        }

        public static string GetDefaultStaticTemplateName(IInformationObject informationObject)
        {
            string templateName = informationObject.GetType().FullName + "_DefaultView.phtml";
            return templateName;
        }
    }
}