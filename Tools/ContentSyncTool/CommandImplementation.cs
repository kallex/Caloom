using System;
using System.IO;
using System.Linq;
using System.Net;
using SecuritySupport;

namespace ContentSyncTool
{
    internal class CommandImplementation
    {
        internal static UserSettings.Connection GetConnection(NamedConnectionSubOptions verbSubOptions)
        {
            var connection = UserSettings.CurrentSettings.Connections.Single(conn => conn.Name == verbSubOptions.ConnectionName);
            if(connection == null)
                throw new ArgumentException("Connection not found: " + verbSubOptions.ConnectionName);
            return connection;
        }

        internal static FolderSyncItem GetFolderSyncItem(ConnectionSyncFolderSubOptions verbSubOptions)
        {
            var connection = GetConnection(verbSubOptions);
            var syncItem = connection.FolderSyncItems.SingleOrDefault(item => item.SyncItemName == verbSubOptions.SyncName);
            if(syncItem == null)
                throw new ArgumentException("Sync item not found: " + verbSubOptions.SyncName);
            return syncItem;
        }

        internal static void selfTest(SelfTestSubOptions verbSubOptions)
        {
            Console.WriteLine("Performing HTTPS connection test...");
            var request = WebRequest.Create("https://test.theball.me");
            var resp = request.GetResponse();
            Console.WriteLine("HTTPS connection test OK");
        }

        internal static void deleteConnection(DeleteConnectionSubOptions verbSubOptions)
        {
            var connection = UserSettings.CurrentSettings.Connections.FirstOrDefault(conn => conn.Name == verbSubOptions.ConnectionName);
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
                if (verbSubOptions.Force == false)
                    throw;
            }
            UserSettings.CurrentSettings.Connections.Remove(connection);
        }

        internal static void listConnections(ListConnectionsSubOptions verbSubOptions)
        {
            Console.WriteLine("Connections:" + Environment.NewLine);
            UserSettings.CurrentSettings.Connections.ForEach(connection =>
                {
                    Console.WriteLine("Name: {1}{0}Host:{2}{0}GroupID:{3}",
                                      Environment.NewLine,
                                      connection.Name,
                                      connection.HostName,
                                      connection.GroupID);
                    connection.FolderSyncItems.ForEach(syncItem =>
                        {
                            string directionArrow = syncItem.SyncDirection == "UP" ? "=>" : "<=";
                            Console.WriteLine("{1} - {2}: {3}{0}{4} {5} {6}",
                                Environment.NewLine,
                                syncItem.SyncDirection,
                                syncItem.SyncType,
                                syncItem.SyncItemName,
                                syncItem.LocalFullPath, 
                                directionArrow,
                                syncItem.RemoteFolder
                                );
                        });
                    Console.WriteLine("- -- -- -- -- -");
                });
        }

        internal static void createConnection(CreateConnectionSubOptions verbSubOptions)
        {
            string sharedSecret = "testsecretXYZ33";
            string host = verbSubOptions.HostName;
            string targetGroupID = verbSubOptions.GroupID;
            string protocol = host.StartsWith("localdev") ? "ws" : "wss";
            string connectionProtocol = host.StartsWith("localdev") ? "http" : "https";
            var result = SecurityNegotiationManager.PerformEKEInitiatorAsBob(protocol + "://" + verbSubOptions.HostName + "/websocket/NegotiateDeviceConnection?groupID=" + targetGroupID,
                                                                                             sharedSecret, "Connection from Tool with name: " + verbSubOptions.ConnectionName);
            string connectionUrl = String.Format("{2}://{0}/auth/grp/{1}/DEV", host, targetGroupID, connectionProtocol);
            var connection = new UserSettings.Connection
                {
                    Name = verbSubOptions.ConnectionName,
                    HostName = verbSubOptions.HostName,
                    GroupID = verbSubOptions.GroupID,
                    Device = new Device
                        {
                            AESKey = result.AESKey,
                            ConnectionURL = connectionUrl,
                            EstablishedTrustID = result.EstablishedTrustID
                        }
                };
            UserSettings.CurrentSettings.Connections.Add(connection);
        }

        public static void upsync(UserSettings.Connection connection, FolderSyncItem upSyncItem)
        {
            var rootFolder = upSyncItem.LocalFullPath;
            var sourceList = FileSystemSupport.GetContentRelativeFromRoot(rootFolder);
            string destinationPrefix = upSyncItem.SyncType == "DEV" ? "DEV_" : "";
            string destinationCopyRoot = destinationPrefix + upSyncItem.RemoteFolder;
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

        public static void downsync(UserSettings.Connection connection, FolderSyncItem downSyncItem)
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

        private static ContentItemLocationWithMD5[] getConnectionContentMD5s(UserSettings.Connection connection, string[] downSyncFolders)
        {
            DeviceOperationData dod = new DeviceOperationData
                {
                    OperationRequestString = "GETCONTENTMD5LIST",
                    OperationParameters = downSyncFolders
                };
            dod = connection.Device.ExecuteDeviceOperation(dod);
            return dod.OperationSpecificContentData;
        }

        private static ContentItemLocationWithMD5[] getConnectionToCopyMD5s(UserSettings.Connection connection, ContentItemLocationWithMD5[] localContentToSyncTargetFrom, string destinationCopyRoot)
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

        public static void syncFolder(SyncFolderSubOptions syncFolderSubOptions)
        {
            var connection = GetConnection(syncFolderSubOptions);
            var syncItem = GetFolderSyncItem(syncFolderSubOptions);
            if(syncItem.SyncDirection == "UP")
                upsync(connection, syncItem);
            else if(syncItem.SyncDirection == "DOWN")
                downsync(connection, syncItem);
            else 
                throw new NotSupportedException("Sync direction not supported: " + syncItem.SyncDirection);
        }

        public static void addSyncFolder(AddSyncFolderSubOptions addSyncFolderSubOptions)
        {
            var connection = GetConnection(addSyncFolderSubOptions);
            if(connection.FolderSyncItems.Any(item => item.SyncItemName == addSyncFolderSubOptions.SyncName))
                throw new ArgumentException("Sync folder already exists: " + addSyncFolderSubOptions.SyncName);
            var syncFolderItem = new FolderSyncItem
                {
                    SyncItemName = addSyncFolderSubOptions.SyncName,
                    SyncDirection = addSyncFolderSubOptions.SyncDirection,
                    SyncType = addSyncFolderSubOptions.SyncType,
                    LocalFullPath = addSyncFolderSubOptions.LocalFullPath,
                    RemoteFolder = addSyncFolderSubOptions.RemoteFolder
                };
            connection.FolderSyncItems.Add(syncFolderItem);
        }

        public static void removeSyncFolder(RemoveSyncFolderSubOptions removeSyncFolderSubOptions)
        {
            var connection = GetConnection(removeSyncFolderSubOptions);
            int removed = connection.FolderSyncItems.RemoveAll(item => item.SyncItemName == removeSyncFolderSubOptions.SyncName);
            if(removed == 0)
                throw new ArgumentException("Sync item to remove not found: " + removeSyncFolderSubOptions.SyncName);
        }
    }

}