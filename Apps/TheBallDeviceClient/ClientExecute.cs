using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace TheBall.Support.DeviceClient
{
    public static class ClientExecute
    {
        public static Connection GetConnection(string connectionName)
        {
            var connection = UserSettings.CurrentSettings.Connections.Single(conn => conn.Name == connectionName);
            if(connection == null)
                throw new ArgumentException("Connection not found: " + connectionName);
            return connection;
        }

        public static FolderSyncItem GetFolderSyncItem(string connectionName, string syncName)
        {
            var connection = GetConnection(connectionName);
            var syncItem = connection.FolderSyncItems.SingleOrDefault(item => item.SyncItemName == syncName);
            if(syncItem == null)
                throw new ArgumentException("Sync item not found: " + syncName);
            return syncItem;
        }

        public static void UpSync(string connectionName, string syncItemName)
        {
            var connection = GetConnection(connectionName);
            var syncItem = GetFolderSyncItem(connectionName, syncItemName);
            UpSync(connection, syncItem);
        }

        public static void UpSync(Connection connection, FolderSyncItem upSyncItem)
        {
            var rootFolder = upSyncItem.LocalFullPath;
            var sourceList = FileSystemSupport.GetContentRelativeFromRoot(rootFolder);
            string destinationPrefix = upSyncItem.SyncType == "DEV" ? "DEV_" : "";
            string destinationCopyRoot = destinationPrefix + upSyncItem.RemoteFolder;
            if (destinationCopyRoot.EndsWith("/") == false)
                destinationCopyRoot += "/";
            ContentItemLocationWithMD5[] remoteContentBasedActionList = getConnectionToCopyMD5s(connection, sourceList, destinationCopyRoot);

            var itemsToCopy = remoteContentBasedActionList.Where(item => item.ItemDatas.Any(iData => iData.DataName == "OPTODO" && iData.ItemTextData == "COPY")).ToArray();
            var itemsDeleted = remoteContentBasedActionList.Where(item => item.ItemDatas.Any(iData => iData.DataName == "OPDONE" && iData.ItemTextData == "DELETED")).ToArray();
            var device = connection.Device;
            SyncSupport.SynchronizeSourceListToTargetFolder(
                itemsToCopy, new ContentItemLocationWithMD5[0],
                delegate(ContentItemLocationWithMD5 source, ContentItemLocationWithMD5 target)
                    {
                        string fullLocalName = Path.Combine(rootFolder, source.ContentLocation);
                        string destinationContentName = destinationCopyRoot + source.ContentLocation;
                        DeviceSupport.PushContentToDevice(device, fullLocalName, destinationContentName);
                        Console.WriteLine("Uploaded: " + source.ContentLocation);
                    },
                target =>
                    {

                    }, 10);
            var dod = device.ExecuteDeviceOperation(new DeviceOperationData
                {
                    OperationParameters = new[] {destinationCopyRoot},
                    OperationRequestString = "COPYSYNCEDCONTENTTOOWNER"
                });
            if(dod.OperationResult == false)
                throw new OperationCanceledException("Error on remote call operation");
            Console.WriteLine("Finished copying data to owner location: " + destinationCopyRoot);
        }

        public static void DownSync(string connectionName, string syncItemName)
        {
            var connection = GetConnection(connectionName);
            var syncItem = GetFolderSyncItem(connectionName, syncItemName);
            DownSync(connection, syncItem);
        }

        public static void DownSync(Connection connection, FolderSyncItem downSyncItem)
        {
            var rootFolder = downSyncItem.LocalFullPath;
            var myDataContents = FileSystemSupport.GetContentRelativeFromRoot(rootFolder);
            foreach (var myDataItem in myDataContents)
            {
                myDataItem.ContentLocation = downSyncItem.RemoteFolder + myDataItem.ContentLocation;
            }
            ContentItemLocationWithMD5[] remoteContentSourceList = getConnectionContentMD5s(connection, new string[] { downSyncItem.RemoteFolder });
            var device = connection.Device;
            int stripRemoteFolderIndex = downSyncItem.RemoteFolder.Length;
            SyncSupport.SynchronizeSourceListToTargetFolder(
                remoteContentSourceList, myDataContents,
                delegate(ContentItemLocationWithMD5 source, ContentItemLocationWithMD5 target)
                    {
                        string targetFullName = Path.Combine(rootFolder, target.ContentLocation.Substring(stripRemoteFolderIndex));
                        string targetDirectoryName = Path.GetDirectoryName(targetFullName);
                        try
                        {
                            if (Directory.Exists(targetDirectoryName) == false)
                                Directory.CreateDirectory(targetDirectoryName);
                        }
                        catch
                        {

                        }
                        //Console.Write("Copying: " + source.ContentLocation);
                        DeviceSupport.FetchContentFromDevice(device, source.ContentLocation,
                                                             targetFullName);
                        //Console.WriteLine(" ... done");
                        Console.WriteLine("Copied: " + source.ContentLocation);
                    }, delegate(ContentItemLocationWithMD5 target)
                        {
                            string targetContentLocation = target.ContentLocation.Substring(stripRemoteFolderIndex);
                            string targetFullName = Path.Combine(rootFolder, targetContentLocation);
                            File.Delete(targetFullName);
                            Console.WriteLine("Deleted: " + targetContentLocation);
                        }, 10);
        }

        public static ContentItemLocationWithMD5[] getConnectionContentMD5s(Connection connection, string[] downSyncFolders)
        {
            DeviceOperationData dod = new DeviceOperationData
                {
                    OperationRequestString = "GETCONTENTMD5LIST",
                    OperationParameters = downSyncFolders
                };
            dod = connection.Device.ExecuteDeviceOperation(dod);
            return dod.OperationSpecificContentData;
        }

        public static ContentItemLocationWithMD5[] getConnectionToCopyMD5s(Connection connection, ContentItemLocationWithMD5[] localContentToSyncTargetFrom, string destinationCopyRoot)
        {
            DeviceOperationData dod = new DeviceOperationData
                {
                    OperationRequestString = "SYNCCOPYCONTENT",
                    OperationSpecificContentData = localContentToSyncTargetFrom,
                    OperationParameters = new string[] { destinationCopyRoot}
                };
            dod = connection.Device.ExecuteDeviceOperation(dod);
            return dod.OperationSpecificContentData;
        }

        public static FolderSyncItem AddSyncFolder(string connectionName, string syncName, string syncType, string syncDirection, string localFullPath, string remoteFolder)
        {
            localFullPath = Path.GetFullPath(localFullPath);
            var connection = GetConnection(connectionName);
            if(connection.FolderSyncItems.Any(item => item.SyncItemName == syncName))
                throw new ArgumentException("Sync folder already exists: " + syncName);
            var syncFolderItem = new FolderSyncItem
                {
                    SyncItemName = syncName,
                    SyncDirection = syncDirection,
                    SyncType = syncType,
                    LocalFullPath = localFullPath,
                    RemoteFolder = remoteFolder
                };
            syncFolderItem.Validate();
            connection.FolderSyncItems.Add(syncFolderItem);
            return syncFolderItem;
        }

        public static void RemoveSyncFolder(string connectionName, string syncItemName)
        {
            var connection = GetConnection(connectionName);
            int removed = connection.FolderSyncItems.RemoveAll(item => item.SyncItemName == syncItemName);
            if(removed == 0)
                throw new ArgumentException("Sync item to remove not found: " + syncItemName);
        }

        public static void DeleteConnection(string connectionName, bool forceDeleteWhenRemoteDeleteFails)
        {
            var connection = UserSettings.CurrentSettings.Connections.FirstOrDefault(conn => conn.Name == connectionName);
            if(connection == null)
                throw new ArgumentException("ConnectionName is invalid");
            try
            {
                DeviceSupport.ExecuteRemoteOperationVoid(connection.Device, "TheBall.CORE.RemoteDeviceCoreOperation", new DeviceOperationData
                    {
                        OperationRequestString = "DELETEREMOTEDEVICE"
                    });
            }
            catch(Exception)
            {
                if (forceDeleteWhenRemoteDeleteFails == false)
                    throw;
            }
            UserSettings.CurrentSettings.Connections.Remove(connection);
        }

        public static void CreateConnection(string hostName, string groupID, string connectionName)
        {
            byte[] sharedSecretFullPayload = GetSharedSecretPayload(hostName);
            var sharedSecret = sharedSecretFullPayload.Take(32).ToArray();
            var sharedSecretPayload = sharedSecretFullPayload.Skip(32).ToArray();
            string protocol = hostName.StartsWith("localdev") ? "ws" : "wss";
            string connectionProtocol = hostName.StartsWith("localdev") ? "http" : "https";
            var result = SecurityNegotiationManager.PerformEKEInitiatorAsBob(protocol + "://" + hostName + "/websocket/NegotiateDeviceConnection?groupID=" + groupID,
                                                                             sharedSecret, "Connection from Tool with name: " + connectionName, sharedSecretPayload);
            string connectionUrl = String.Format("{2}://{0}/auth/grp/{1}/DEV", hostName, groupID, connectionProtocol);
            var connection = new Connection
                {
                    Name = connectionName,
                    HostName = hostName,
                    GroupID = groupID,
                    Device = new Device
                        {
                            AESKey = result.AESKey,
                            ConnectionURL = connectionUrl,
                            EstablishedTrustID = result.EstablishedTrustID
                        }
                };
            UserSettings.CurrentSettings.Connections.Add(connection);
        }

        public static byte[] GetSharedSecretPayload(string hostName)
        {
            string connectionProtocol = hostName.StartsWith("localdev") ? "http" : "https";
            string sharedSecretRequestUrl = string.Format("{0}://{1}/websocket/RequestSharedSecret", connectionProtocol, hostName);
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(sharedSecretRequestUrl);
            request.Method = "POST";
            // Hack on writing 0 length below, because Mono on Mac didn't send server proper "Content-Length=0"
            // when the content length was only set
            using(BinaryWriter writer = new BinaryWriter(request.GetRequestStream()))
                writer.Write(new byte[0]);
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            if(response.StatusCode != HttpStatusCode.OK)
                throw new WebException("Invalid response from remote secret request");
            using (MemoryStream memStream = new MemoryStream())
            {
                var responseStream = response.GetResponseStream();
                responseStream.CopyTo(memStream);
                return memStream.ToArray();
            }
        }

        public static void SyncFolder(string connectionName, string syncItemName)
        {
            var connection = GetConnection(connectionName);
            var syncItem = GetFolderSyncItem(connectionName, syncItemName);
            if(syncItem.SyncDirection == "UP")
                UpSync(connection, syncItem);
            else if(syncItem.SyncDirection == "DOWN")
                DownSync(connection, syncItem);
            else 
                throw new NotSupportedException("Sync direction not supported: " + syncItem.SyncDirection);
        }

        public static void ExecuteWithSettings(Action executionAction, Action<Exception> exceptionHandling)
        {
            UserSettings.GetCurrentSettings();
            try
            {
                executionAction();
            }
            catch (Exception ex)
            {
                if (exceptionHandling != null)
                    exceptionHandling(ex);
                else
                    throw;
            }
            finally
            {
                UserSettings.SaveCurrentSettings();
            }
        }

        public static void StageOperation(string connectionName, bool getData, bool putDev, bool putLive)
        {
            var connection = GetConnection(connectionName);
            var stageDef = connection.StageDefinition;
            if(stageDef == null)
                throw new InvalidDataException("Staging definition not found for connection: " + connectionName);
            string stagingRootFolder = stageDef.LocalStagingRootFolder;
            if(String.IsNullOrEmpty(stagingRootFolder))
                throw new InvalidDataException("Staging root folder definition is missing");
            if (getData)
            {
                if(stageDef.DataFolders.Count == 0)
                    throw new InvalidDataException("Staging data folders are not defined (getdata is not possible)");
                var folderSyncItems = stageDef.DataFolders.Select(dataFolder =>
                    {
                        string fullName = Path.Combine(stagingRootFolder, dataFolder);
                        if (!Directory.Exists(fullName))
                        {
                            Directory.CreateDirectory(fullName);
                        }
                        return FolderSyncItem.CreateFromStagingRemoteDataFolder(stagingRootFolder, dataFolder);
                    }).Where(fsi => fsi != null).ToArray();
                foreach (var folderSyncItem in folderSyncItems)
                {
                    DownSync(connection, folderSyncItem);
                }
            }
            List<FolderSyncItem> upsyncItems = new List<FolderSyncItem>();
            if (putDev)
            {
                addUpsyncItems(stagingRootFolder, upsyncItems, "DEV_*");
            }
            if (putLive)
            {
                addUpsyncItems(stagingRootFolder, upsyncItems, "LIVE_*");   
            }
            foreach (var upSyncItem in upsyncItems)
            {
                UpSync(connection, upSyncItem);
            }
        }

        private static void addUpsyncItems(string stagingRootFolder, List<FolderSyncItem> upsyncItems, string folderSearchPattern)
        {
            DirectoryInfo rootDirectory = new DirectoryInfo(stagingRootFolder);
            var subDirectories = rootDirectory.GetDirectories(folderSearchPattern);
            foreach (var subDir in subDirectories)
            {
                var fsi = FolderSyncItem.CreateFromStagingFolderDirectory(stagingRootFolder, subDir.Name);
                if (fsi != null)
                    upsyncItems.Add(fsi);
            }
        }

        public static StageDefinition SetStaging(string connectionName, string stagingFolderFullPath, string dataFolders)
        {

            var connection = GetConnection(connectionName);
            stagingFolderFullPath = Path.GetFullPath(stagingFolderFullPath);
            var stageDef = connection.StageDefinition;
            if (stageDef == null)
            {
                stageDef = new StageDefinition();
                connection.StageDefinition = stageDef;
            }
            if(stagingFolderFullPath != null)
                stageDef.LocalStagingRootFolder = stagingFolderFullPath;
            if (dataFolders != null)
                stageDef.DataFolders = dataFolders.Split(',').ToList();
            return stageDef;
        }

        public static void DetachStaging(string connectionName)
        {
            var connection = GetConnection(connectionName);
            connection.StageDefinition = null;
        }
    }
}
