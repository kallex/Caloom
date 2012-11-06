using System.IO;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class AddActivityInfo : IAddOperationProvider
    {
        public bool PerformAddOperation(InformationSourceCollection sources, string requesterLocation)
        {
            if (ActivityName == "")
                throw new InvalidDataException("Activity name is mandatory");
            Activity activity = Activity.CreateDefault();
            VirtualOwner owner = VirtualOwner.FigureOwner(this);
            activity.SetLocationAsOwnerContent(owner, activity.ID);
            activity.ActivityName = ActivityName;
            activity.ReferenceToInformation.Title = activity.ActivityName;
            activity.ReferenceToInformation.URL = DefaultViewSupport.GetDefaultViewURL(activity);
            StorageSupport.StoreInformationMasterFirst(activity, owner);
            DefaultViewSupport.CreateDefaultViewRelativeToRequester(requesterLocation, activity, owner);
            //ActivitySummaryContainer summaryContainer = ActivitySummaryContainer.RetrieveFromOwnerContent(owner, "default");
            //summaryContainer.AddNewActivity(activity);
            //StorageSupport.StoreInformation(summaryContainer);
            return false;
        }
    }
}