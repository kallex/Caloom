using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using SecuritySupport;

namespace ContentSyncTool
{
    public class UserSettings
    {
        [Serializable]
        public class Connection
        {
            public string Name;
            public string HostName;
            public string GroupID;
            public string EstablishedTrustID;
            public string LocalTemplateRootLocation;
            public string LocalDataRootLocation;
            public Device Device = new Device();
            public string[] DownSyncFolders;
            public string[] UpSyncFolders;
        }

        [Serializable]
        public class Device
        {
            public string ConnectionURL;
            public byte[] AESKey;
            public string EstablishedTrustID;
        }

        public List<Connection> Connections = new List<Connection>();

        public static UserSettings CurrentSettings { get; private set; }

        public static string CurrSettingsFilePath
        {
            get
            {
                return
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                 "TheBallSyncTool/SecureData.enc");
            }
        }

        public static UserSettings GetCurrentSettings()
        {
            if (CurrentSettings == null)
            {
                if (File.Exists(CurrSettingsFilePath))
                {
                    byte[] currentData = File.ReadAllBytes(CurrSettingsFilePath);
                    currentData = ProtectionSupport.Unprotect(currentData);
                    XmlSerializer serializer = new XmlSerializer(typeof(UserSettings));
                    using (MemoryStream memoryStream = new MemoryStream(currentData))
                    {
                        CurrentSettings = (UserSettings)serializer.Deserialize(memoryStream);
                    }
                }
                else
                {
                    string dirName = Path.GetDirectoryName(CurrSettingsFilePath);
                    if (Directory.Exists(dirName) == false)
                        Directory.CreateDirectory(dirName);
                    CurrentSettings = new UserSettings();
                }
                
            }
            return CurrentSettings;
        }

        public static void SaveCurrentSettings()
        {
            byte[] currentData = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(UserSettings));
                serializer.Serialize(memoryStream, CurrentSettings);
                currentData = memoryStream.ToArray();
            }
            currentData = ProtectionSupport.Protect(currentData);
            File.WriteAllBytes(CurrSettingsFilePath, currentData);
        }

    }
}