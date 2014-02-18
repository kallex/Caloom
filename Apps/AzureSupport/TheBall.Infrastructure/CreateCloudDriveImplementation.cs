using System;
using System.IO;
using Microsoft.WindowsAzure.StorageClient;

namespace TheBall.Infrastructure
{
    public class CreateCloudDriveImplementation
    {
        public static string GetTarget_DriveBlobName(string driveName)
        {
            if(driveName.Contains(".vhd"))
                throw new ArgumentException("Drive name must not contain .vhd extension");
            driveName = driveName + ".vhd";
            return driveName;
        }

        public static CreateCloudDriveReturnValue ExecuteMethod_CreateDrive(int sizeInMegabytes, string driveBlobName)
        {
            CloudDrive drive = null;
            Exception exception = null;
            try
            {
                drive = CloudDriveSupport.CreatePageBlobDrive(driveBlobName, sizeInMegabytes);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            return new CreateCloudDriveReturnValue
                {
                    CloudDrive = drive,
                    Exception = exception
                };
        }

        public static CreateCloudDriveReturnValue Get_ReturnValue(CreateCloudDriveReturnValue executionResult)
        {
            return executionResult;
        }
    }
}