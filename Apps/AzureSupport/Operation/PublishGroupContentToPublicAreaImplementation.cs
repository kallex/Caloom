using System;
using System.Collections.Generic;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    public static class PublishGroupContentToPublicAreaImplementation
    {
        public static string GetTarget_CurrentContainerName(string groupID)
        {
            return StorageSupport.CurrActiveContainer.Name;
        }

        public static string GetTarget_PublicContainerName(string groupID)
        {
            return RenderWebSupport.GetCurrentAnonContainerName();
        }

        public static void ExecuteMethod_PublishGroupContent(string groupID, bool useWorker, string currentContainerName, string publicContainerName)
        {
            string groupPublicSiteLocation = "grp/" + groupID + "/" + RenderWebSupport.DefaultPublicGroupSiteLocation;
            string defaultPublicSiteLocation = "grp/default/" + RenderWebSupport.DefaultPublicGroupSiteLocation;
            string aboutAuthTargetLocation = RenderWebSupport.DefaultAboutTargetLocation;
            List<OperationRequest> operationRequests = new List<OperationRequest>();
            var publishPublicContent = RenderWebSupport.SyncTemplatesToSite(currentContainerName, groupPublicSiteLocation, publicContainerName, groupPublicSiteLocation, useWorker, false);
            operationRequests.Add(publishPublicContent);
            string defaultGroupID = "undefinedForNow";
            if (groupID == defaultGroupID) 
            {
                OperationRequest publishDefault = RenderWebSupport.SyncTemplatesToSite(currentContainerName, groupPublicSiteLocation, publicContainerName, defaultPublicSiteLocation, useWorker, false);
                operationRequests.Add(publishDefault);
                publishDefault = RenderWebSupport.SyncTemplatesToSite(currentContainerName, groupPublicSiteLocation, currentContainerName,
                    aboutAuthTargetLocation, useWorker, false);
                operationRequests.Add(publishDefault);
            }
            if (useWorker)
            {
                //QueueSupport.PutToOperationQueue(localGroupTemplates, renderLocalTemplates);
                QueueSupport.PutToOperationQueue(operationRequests.ToArray());
            }
        }
    }
}