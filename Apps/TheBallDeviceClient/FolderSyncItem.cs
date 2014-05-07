using System;

namespace TheBall.Support.DeviceClient
{
    [Serializable]
    public class FolderSyncItem
    {
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
            if (SyncType == "wwwsite" && RemoteFolder != "wwwsite")
                throw new ArgumentException("remoteFolder must also be wwwsite when syncType is wwwsite");
        }
    }
}