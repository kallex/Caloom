using System;
using System.Linq;
using System.Net;
using SecuritySupport;

namespace ContentSyncTool
{
    internal class CommandImplementation
    {
        internal static void setConnectionRootLocations(ConnectionRootLocationSubOptions verbSubOptions)
        {
            var connection = GetConnection(verbSubOptions);
            if (String.IsNullOrEmpty(verbSubOptions.DataRoot) == false)
                connection.LocalDataRootLocation = verbSubOptions.DataRoot;
            if (String.IsNullOrEmpty(verbSubOptions.TemplateRoot) == false)
                connection.LocalTemplateRootLocation = verbSubOptions.TemplateRoot;
        }

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
    }
}