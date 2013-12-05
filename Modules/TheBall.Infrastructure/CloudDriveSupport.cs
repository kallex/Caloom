using System;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.StorageClient.Protocol;

namespace TheBall.Infrastructure
{
    public static class CloudDriveSupport
    {
        private static CloudStorageAccount Account;
        private static string DriveContainerName;
        static CloudDriveSupport()
        {
            Account =
                CloudStorageAccount.Parse(InstanceConfiguration.AzureStorageConnectionString);
            DriveContainerName = InstanceConfiguration.CloudDriveContainerName;
        }

        public static string MountLatestReadOnly()
        {
            string mountName = null;



            return mountName;
        }

        public static CloudDrive CreatePageBlobDrive(string driveBlobName, int capacityInMegabytes)
        {
            var account = Account;
            var blobClient = account.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(InstanceConfiguration.CloudDriveContainerName);
            container.CreateIfNotExist();
            CloudPageBlob pageBlob = container.GetPageBlobReference(driveBlobName);
            string driveUriValue;
            driveUriValue = pageBlob.Uri.AbsoluteUri;
            CloudDrive myDrive = account.CreateCloudDrive(driveUriValue);
            if (capacityInMegabytes < 16)
                throw new ArgumentException("Cloud drive capacity needs to be at least 16MB");
            myDrive.Create(capacityInMegabytes);
            return myDrive;
        }

        private static void unmountDriveBySystemName(string systemPath)
        {
            var account = Account;
            foreach (var drive in CloudDrive.GetMountedDrives())
            {
                if (drive.Key == systemPath)
                {
                    var mountedDrive = account.CreateCloudDrive(drive.Value.PathAndQuery);
                    mountedDrive.Unmount();
                    break;
                }
            }
        }

        private static void unmountDrive(CloudDrive drive)
        {
            drive.Unmount();
        }

        private static CloudDrive mountDrive(string driveBlobName, bool readOnlyLatestSnapshot)
        {
            var account = Account;

            ////Blob client to give us access to various blob services.
            var blobClient = account.CreateCloudBlobClient();

            ////Container named drives where our VHD is going to stay.
            CloudBlobContainer container = blobClient.GetContainerReference(DriveContainerName);
            container.CreateIfNotExist();

            ////We need a page blob since VHDs are page blobs.
            CloudPageBlob pageBlob = container.GetPageBlobReference(driveBlobName);
            string driveUriValue;
            if (readOnlyLatestSnapshot)
                driveUriValue = getDriveUriValue(pageBlob);
            else 
                driveUriValue = pageBlob.Uri.AbsoluteUri;

            ////Un-mount any previously mounted drive.
            
            foreach (var drive in CloudDrive.GetMountedDrives())
            {
                var mountedDrive = account.CreateCloudDrive(drive.Value.PathAndQuery);
                mountedDrive.Unmount();
            }

            ////Create the Windows Azure drive and its associated page blob
            CloudDrive myDrive = account.CreateCloudDrive(driveUriValue);

            //var snapShot = myDrive.Snapshot();
            //myDrive = account.CreateCloudDrive(snapShot.AbsoluteUri);
            //Console.WriteLine("Snapshotted uri: " + snapShot.AbsoluteUri);

            ////Create the CloudDrive if not already present.
            //myDrive.CreateIfNotExist(driveSizeInMB);
            //CloudDrive myDrive = new CloudDrive(pageBlob.Uri, credentials);
            //myDrive.CreateIfNotExist(64);

            ////Mount the drive and initialize the application with the path to the date store on the Azure drive
            myDrive.Mount(0, DriveMountOptions.None);
            return myDrive;

            /*
            ////Do some I\O operations with File APIs. Let's create a folder and add some files.
            Directory.CreateDirectory(Path.Combine(drivePath, "Data").ToString());
            var fStream = System.IO.File.Create(Path.Combine(drivePath, "Data", "First.txt").ToString());
            fStream.Close();
            fStream.Dispose();
            System.IO.File.WriteAllText(Path.Combine(drivePath, "Data", "First.txt").ToString(), "First File Data");
             * */
            Console.WriteLine("Press any key to unmount...");
            Console.ReadKey();
            //myDrive.Unmount();
            Console.WriteLine("Unmounted");
        }

        private static string getDriveUriValue(CloudPageBlob pageBlob)
        {
            var blobList =
                pageBlob.Container.ListBlobs(new BlobRequestOptions
                {
                    UseFlatBlobListing = true,
                    BlobListingDetails = BlobListingDetails.Snapshots
                }).Cast<CloudPageBlob>().ToArray();
            var snapshotsOfThis = blobList.Where(blob => blob.Name == pageBlob.Name && blob.SnapshotTime.HasValue).OrderByDescending(blob => blob.SnapshotTime).ToArray();
            if (snapshotsOfThis.Length == 0)
                return pageBlob.Uri.AbsoluteUri;
            var snapshot = snapshotsOfThis[0];
            string snapshotUri = BlobRequest.Get(snapshot.Uri, 0, snapshot.SnapshotTime.Value.ToUniversalTime(), null).Address.AbsoluteUri;
            return snapshotUri;
        }

    }
}
