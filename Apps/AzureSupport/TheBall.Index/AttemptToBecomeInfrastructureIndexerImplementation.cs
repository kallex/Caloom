using System;
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

        public static void ExecuteMethod_EnsureQueuesIfMountSucceeded(bool success, string queryQueueName, string indexRequestQueueName)
        {
            if (success == false)
                return;
            QueueSupport.RegisterQueue(queryQueueName);
            QueueSupport.RegisterQueue(indexRequestQueueName);
        }

        public static AttemptToBecomeInfrastructureIndexerReturnValue Get_ReturnValue(AttemptToBecomeInfrastructureIndexerReturnValue executionResult)
        {
            return executionResult;
        }

        public static string GetTarget_IndexDriveName(string indexName)
        {
            return indexName + "Storage";
        }

        public static AttemptToBecomeInfrastructureIndexerReturnValue ExecuteMethod_MountIndexDrive(string indexDriveName)
        {
            CloudDrive drive = null;
            Exception exception = null;
            try
            {
                var createdDriveResult = CreateCloudDrive.Execute(new CreateCloudDriveParameters
                    {
                        DriveName = indexDriveName,
                        SizeInMegabytes = IndexSupport.IndexDriveStorageSizeInMB
                    });
                drive = createdDriveResult.CloudDrive;
                exception = createdDriveResult.Exception;
                if (exception == null)
                {
                    var mountDriveResult = MountCloudDrive.Execute(new MountCloudDriveParameters
                    {
                        DriveReference = drive
                    });
                    exception = mountDriveResult.Exception;
                }
            }
            catch(Exception ex)
            {
                exception = ex;
            }
            return new AttemptToBecomeInfrastructureIndexerReturnValue
                {
                    Exception = exception,
                    Success = exception == null,
                    CloudDrive = drive
                };
        }
    }
}