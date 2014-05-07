using System;
using System.Collections.Generic;

namespace TheBall.Support.DeviceClient
{
    [Serializable]
    public class Connection
    {
        public string Name;
        public string HostName;
        public string GroupID;
        public string EstablishedTrustID;
        public Device Device = new Device();
        public List<FolderSyncItem> FolderSyncItems = new List<FolderSyncItem>();
    }
}