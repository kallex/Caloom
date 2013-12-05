using System;
using Microsoft.WindowsAzure.StorageClient;

namespace TheBall.Infrastructure
{
    public class CreateCloudDriveImplementation
    {
        public static string GetTarget_DriveBlobName(string driveName)
        {
            if(driveName.Contains(".vhd"))
                throw new ArgumentException("Drive name must not contain .vhd extension");
            return driveName + ".vhd";
        }

        public static CloudDrive ExecuteMethod_CreateDrive(int sizeInMegabytes, string driveBlobName)
        {
            return CloudDriveSupport.CreatePageBlobDrive(driveBlobName, sizeInMegabytes);
        }

        public static CreateCloudDriveReturnValue Get_ReturnValue(CloudDrive createDriveOutput)
        {
            return new CreateCloudDriveReturnValue {CloudDrive = createDriveOutput};
        }
    }
}