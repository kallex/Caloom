using System;
using System.IO;
using Microsoft.WindowsAzure.StorageClient;

namespace TheBall.Infrastructure
{
    public class MountCloudDriveImplementation
    {
        public static MountCloudDriveReturnValue ExecuteMethod_MountDrive(CloudDrive driveReference)
        {
            Exception exception = null;
            string driveLetter = null;
            try
            {
                const int CacheSizeMB = 1024;
                driveLetter = driveReference.Mount(CacheSizeMB, DriveMountOptions.None);
                DateTime nowUtc = DateTime.UtcNow;
                string refText = nowUtc.ToString();
                string newFileName = Path.Combine(driveLetter, "Testfile.txt");
                File.WriteAllText(newFileName, refText);
                string testContent = File.ReadAllText(newFileName);
                if (testContent != refText)
                    throw new InvalidDataException("CloudDrive write/read is not matching original data");
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            return new MountCloudDriveReturnValue
                {
                    MountedDriveLetter = driveLetter,
                    Exception = exception
                };
        }

        public static MountCloudDriveReturnValue Get_ReturnValue(MountCloudDriveReturnValue executionResult)
        {
            return executionResult;
        }
    }
}