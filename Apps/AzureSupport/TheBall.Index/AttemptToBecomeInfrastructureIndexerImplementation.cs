using Microsoft.WindowsAzure.StorageClient;
using TheBall.Infrastructure;

namespace TheBall.Index
{
    public class AttemptToBecomeInfrastructureIndexerImplementation
    {
        public static string GetTarget_QueryQueueName(string indexName)
        {
            return IndexSupport.GetQueryRequestQueueName(indexName);
        }

        public static string GetTarget_IndexRequestQueueName(string indexName)
        {
            return IndexSupport.GetIndexRequestQueueName(indexName);
        }

        public static void ExecuteMethod_EnsureQueuesIfMountSucceeded(CloudDrive mountIndexDriveOutput, string queryQueueName, string indexRequestQueueName)
        {
            if (mountIndexDriveOutput == null)
                return;
        }

        public static AttemptToBecomeInfrastructureIndexerReturnValue Get_ReturnValue(CloudDrive mountIndexDriveOutput)
        {
            return new AttemptToBecomeInfrastructureIndexerReturnValue {Success = mountIndexDriveOutput != null};
        }

        public static string GetTarget_IndexDriveName(string indexName)
        {
            return indexName + "Storage";
        }

        public static CloudDrive ExecuteMethod_MountIndexDrive(string indexDriveName)
        {
            var createdDrive = CreateCloudDrive.Execute(new CreateCloudDriveParameters
                {
                    DriveName = indexDriveName,
                    SizeInMegabytes = IndexSupport.IndexDriveStorageSizeInMB
                });
            return createdDrive.CloudDrive;
        }
    }
}