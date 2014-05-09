using System;
using System.Collections.Generic;

namespace TheBall.Support.DeviceClient
{
    [Serializable]
    public class StageDefinition
    {
        public List<string> DataFolders = new List<string>();
        public string LocalStagingRootFolder;
    }
}