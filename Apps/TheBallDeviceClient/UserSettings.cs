using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace TheBall.Support.DeviceClient
{
    public class UserSettings
    {
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