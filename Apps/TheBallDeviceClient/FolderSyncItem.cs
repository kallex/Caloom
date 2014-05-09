using System;
using System.IO;

namespace TheBall.Support.DeviceClient
{
    [Serializable]
    public class FolderSyncItem
    {
        public static FolderSyncItem CreateFromStagingFolderDirectory(string stagingRootFolder, string stagingSubfolder)
        {
            if (stagingSubfolder.StartsWith("DEV_") == false && stagingSubfolder.StartsWith("LIVE_") == false)
                return null;
            string remoteFolder;
            string syncType = null;
            if (stagingSubfolder.StartsWith("DEV_"))
            {
                remoteFolder = stagingSubfolder.Substring(4);
                syncType = "DEV";
            }
            else
            {
                if (stagingSubfolder == "LIVE_wwwsite")
                {
                    remoteFolder = "wwwsite";
                    syncType = "wwwsite";
                }
                else
                {
                    remoteFolder = stagingSubfolder;
                    syncType = "LIVE";
                }
            }
            if(string.IsNullOrEmpty(remoteFolder))
                throw new InvalidDataException("Invalid remote staging subfolder: " + stagingSubfolder);
            var folderSyncItem = new FolderSyncItem
                {
                    LocalFullPath = Path.Combine(stagingRootFolder, stagingSubfolder),
                    RemoteFolder = remoteFolder,
                    SyncDirection = "UP",
                    SyncItemName = "DYNAMIC",
                    SyncType = syncType
                };
            return folderSyncItem;
        }

        public static FolderSyncItem CreateFromStagingRemoteDataFolder(string stagingRootFolder, string dataFolder)
        {
            if (string.IsNullOrEmpty(dataFolder))
                return null;
            if (dataFolder.EndsWith("/") == false)
                dataFolder += "/";
            var folderSyncItem = new FolderSyncItem
                {
                    LocalFullPath = Path.Combine(stagingRootFolder, dataFolder),
                    RemoteFolder = dataFolder,
                    SyncDirection = "DOWN",
                    SyncItemName = "DYNAMIC",
                    SyncType = "DEV",
                };
            return folderSyncItem;
        }

        public string LocalFullPath;
        public string RemoteFolder;
        public string SyncItemName;
        public string SyncDirection;
        public string SyncType;

        public void Validate()
        {
            if (RemoteFolder == "/")
                throw new ArgumentException("Root remote folder (/) not supported");
            if (RemoteFolder.EndsWith("/") == false)
                RemoteFolder += "/";
            if (SyncDirection != "UP" && SyncDirection != "DOWN")
                throw new ArgumentException("syncDirection must be either UP or DOWN");
            if (SyncType != "DEV" && SyncType != "wwwsite")
                throw new ArgumentException("syncType must be either DEV or wwwsite");
            if (SyncType == "wwwsite" && RemoteFolder != "wwwsite/")
                throw new ArgumentException("remoteFolder must also be wwwsite when syncType is wwwsite");
        }
    }
}