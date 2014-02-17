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
            QueueSupport.RegisterQueue(queryQueueName);
            QueueSupport.RegisterQueue(indexRequestQueueName);
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
            try
            {
                var createdDriveResult = CreateCloudDrive.Execute(new CreateCloudDriveParameters
                    {
                        DriveName = indexDriveName,
                        SizeInMegabytes = IndexSupport.IndexDriveStorageSizeInMB
                    });
                var drive = createdDriveResult.CloudDrive;
                MountCloudDrive.Execute(new MountCloudDriveParameters
                    {
                        DriveReference = drive
                    });
                return drive;
            }
            catch
            {
                return null;
            }
        }
    }
}