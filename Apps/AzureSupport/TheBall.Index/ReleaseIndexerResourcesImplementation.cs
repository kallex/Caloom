using TheBall.Infrastructure;

namespace TheBall.Index
{
    public class ReleaseIndexerResourcesImplementation
    {
        public static void ExecuteMethod_ReleaseResources(AttemptToBecomeInfrastructureIndexerReturnValue resourceInfo)
        {
            if (resourceInfo.Success == false)
                return;
            CloudDriveSupport.UnmountDrive(resourceInfo.CloudDrive);
        }
    }
}