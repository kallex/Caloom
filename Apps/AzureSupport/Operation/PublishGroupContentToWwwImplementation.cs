using System;
using System.Collections.Generic;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    public static class PublishGroupContentToWwwImplementation
    {
        public static string GetTarget_CurrentContainerName(string groupID)
        {
            return StorageSupport.CurrActiveContainer.Name;
        }

        public static string GetTarget_WwwContainerName(string groupID)
        {
            return RenderWebSupport.GetCurrentWwwContainerName();
        }

        public static void ExecuteMethod_PublishGroupContentToWww(string groupID, bool useWorker, string currentContainerName, string wwwContainerName)
        {
            string groupWwwPublicSiteLocation = "grp/" + groupID + "/" + RenderWebSupport.DefaultPublicWwwSiteLocation;
            List<OperationRequest> operationRequests = new List<OperationRequest>();
            if (groupID == RenderWebSupport.DefaultGroupID) // Currently also publish www
            {
                var publishWww = RenderWebSupport.SyncTemplatesToSite(currentContainerName, groupWwwPublicSiteLocation,
                                                     wwwContainerName, "", useWorker, false);
                operationRequests.Add(publishWww);
            }
            if (useWorker)
            {
                //QueueSupport.PutToOperationQueue(localGroupTemplates, renderLocalTemplates);
                QueueSupport.PutToOperationQueue(operationRequests.ToArray());
            }
        }
    }
}