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
            return UserSettings.CurrentSettings.Connections.Single(conn => conn.Name == verbSubOptions.ConnectionName);
        }

        internal static void selfTest(EmptySubOptions verbSubOptions)
        {
            Console.WriteLine("Performing HTTPS connection test...");
            var request = WebRequest.Create("https://test.caloom.com");
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

        internal static void listConnections(EmptySubOptions verbSubOptions)
        {
            Console.WriteLine("Connections:");
            UserSettings.CurrentSettings.Connections.ForEach(connection => Console.WriteLine("{0} {1} {2} {3} {4}",
                                                                                             connection.Name, connection.HostName, connection.GroupID,
                                                                                             connection.LocalDataRootLocation, connection.LocalTemplateRootLocation));
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
                    Device = new UserSettings.Device
                        {
                            AESKey = result.AESKey,
                            ConnectionURL = connectionUrl,
                            EstablishedTrustID = result.EstablishedTrustID
                        }
                };
            UserSettings.CurrentSettings.Connections.Add(connection);
        }

        internal static void setConnectionRootLocations(ConnectionRootLocationSubOptions verbSubOptions)
        {
            var connection = GetConnection(verbSubOptions);
            if (String.IsNullOrEmpty(verbSubOptions.DataRoot) == false)
                connection.LocalDataRootLocation = verbSubOptions.DataRoot;
            if (String.IsNullOrEmpty(verbSubOptions.TemplateRoot) == false)
                connection.LocalTemplateRootLocation = verbSubOptions.TemplateRoot;
        }

        public static void setConnectionSyncFolders(ConnectionSyncFoldersSubOptions verbSubOptions)
        {
            var connection = GetConnection(verbSubOptions);
            connection.DownSyncFolders = (verbSubOptions.DownSyncFolders ?? "").Split(',').OrderBy(name => name).ToArray();
            connection.UpSyncFolders = (verbSubOptions.UpSyncFolders ?? "").Split(',').OrderBy(name => name).ToArray();
        }

        public static void upsync(ConnectionUpSyncSubOptions verbSubOptions)
        {
            var connection = GetConnection(verbSubOptions);
            if (String.IsNullOrEmpty(connection.LocalTemplateRootLocation))
                throw new InvalidDataException("Connection LocalTemplateRootLocation must be set before upsync command");
            if (connection.UpSyncFolders == null || connection.UpSyncFolders.Length == 0)
                throw new InvalidDataException("Connection upsync folders must be set before upsync command");
            var rootFolder = connection.LocalTemplateRootLocation;
            var customSyncFolders = connection.UpSyncFolders
                                              .Select(folder =>
                                                      new
                                                          {
                                                              Folder = folder,
                                                              ContentMD5s = FileSystemSupport.GetContentRelativeFromRoot(Path.Combine(rootFolder, folder))
                                                          }
                ).ToArray();
            Console.WriteLine(customSyncFolders.Length);
        }

        public static void downsync(ConnectionDownSyncSubOptions verbSubOptions)
        {
            var connection = GetConnection(verbSubOptions);
            if(String.IsNullOrEmpty(connection.LocalDataRootLocation))
                throw new InvalidDataException("Connection LocalDataRootLocation must be set before downsync command");
            if (connection.DownSyncFolders == null || connection.DownSyncFolders.Length == 0)
                throw new InvalidDataException("Connection downsync folders must be set before downsync command");
            var rootFolder = connection.LocalDataRootLocation;
            var myDataSyncFolders = connection.DownSyncFolders
                                              .Select(folder => new
                                                  {
                                                      Folder = folder,
                                                      ContentMD5s = FileSystemSupport.GetContentRelativeFromRoot(Path.Combine(rootFolder, folder))
                                                  }
                ).ToArray();
            ContentItemLocationWithMD5[] remoteContentSourceList = getConnectionContentMD5s(connection, connection.DownSyncFolders);
            var combinedTargetList = myDataSyncFolders.SelectMany(sFolder =>
                {
                    foreach (var contentMD5 in sFolder.ContentMD5s)
                    {
                        contentMD5.ContentLocation = sFolder.Folder + "/" + contentMD5.ContentLocation;
                    }
                    return sFolder.ContentMD5s;
                }).ToArray();
            var device = connection.Device;
            SyncSupport.SynchronizeSourceListToTargetFolder(
                remoteContentSourceList, combinedTargetList,
                delegate(ContentItemLocationWithMD5 source, ContentItemLocationWithMD5 target)
                    {
                        string targetFullName = Path.Combine(rootFolder, target.ContentLocation);
                        string targetDirectoryName = Path.GetDirectoryName(targetFullName);
                        if (Directory.Exists(targetDirectoryName) == false)
                            Directory.CreateDirectory(targetDirectoryName);
                        Console.Write("Copying: " + source.ContentLocation);
                        DeviceSupport.FetchContentFromDevice(device, source.ContentLocation,
                                                             targetFullName);
                        Console.WriteLine(" ... done");
                    }, delegate(ContentItemLocationWithMD5 target)
                        {
                            string targetFullName = Path.Combine(rootFolder, target.ContentLocation);
                            File.Delete(targetFullName);
                            Console.WriteLine("Deleted: " + target.ContentLocation);
                        });
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
    }

}