using System;
using System.IO;
using System.Linq;
using System.Net;
using TheBall.Support.DeviceClient;

namespace ContentSyncTool
{
    internal class CommandImplementation
    {
        internal static void selfTest(SelfTestSubOptions verbSubOptions)
        {
            Console.WriteLine("Performing HTTPS connection test...");
            var request = WebRequest.Create("https://test.theball.me");
            var resp = request.GetResponse();
            Console.WriteLine("HTTPS connection test OK");
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

        public static void removeSyncFolder(RemoveSyncFolderSubOptions removeSyncFolderSubOptions)
        {
            ClientExecute.RemoveSyncFolder(removeSyncFolderSubOptions.ConnectionName, removeSyncFolderSubOptions.SyncName);
        }

        public static void addSyncFolder(AddSyncFolderSubOptions options)
        {
            ClientExecute.AddSyncFolder(options.ConnectionName, options.SyncName, options.SyncType, options.SyncDirection,
                          options.LocalFullPath, options.RemoteFolder);
        }

        public static void syncFolder(SyncFolderSubOptions syncFolderSubOptions)
        {
            ClientExecute.SyncFolder(syncFolderSubOptions.ConnectionName, syncFolderSubOptions.SyncName);
        }

        public static void deleteConnection(DeleteConnectionSubOptions deleteConnectionSubOptions)
        {
            ClientExecute.DeleteConnection(deleteConnectionSubOptions.ConnectionName, deleteConnectionSubOptions.Force);
        }

        public static void createConnection(CreateConnectionSubOptions createConnectionSubOptions)
        {
            ClientExecute.CreateConnection(createConnectionSubOptions.HostName, createConnectionSubOptions.GroupID, createConnectionSubOptions.ConnectionName);
        }
    }

}