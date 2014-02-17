using System;
using System.IO;
using Microsoft.WindowsAzure.StorageClient;

namespace TheBall.Infrastructure
{
    public class MountCloudDriveImplementation
    {
        public static string ExecuteMethod_MountDrive(CloudDrive driveReference)
        {
            const int CacheSizeMB = 1024;
            string driveLetter = driveReference.Mount(CacheSizeMB, DriveMountOptions.None);
            DateTime nowUtc = DateTime.UtcNow;
            string refText = nowUtc.ToString();
            string newFileName = Path.Combine(driveLetter, "Testfile.txt");
            File.WriteAllText(newFileName, refText);
            string testContent = File.ReadAllText(newFileName);
            if(testContent != refText)
                throw new InvalidDataException("CloudDrive write/read is not matching original data");
            return driveLetter;
        }

        public static MountCloudDriveReturnValue Get_ReturnValue(string mountDriveOutput)
        {
            return new MountCloudDriveReturnValue {MountedDriveLetter = mountDriveOutput};
        }
    }
}