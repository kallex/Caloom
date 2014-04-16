using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ContentSyncTool
{
    class Program
    {
        private static Options options = new Options();
        private static void Main(string[] args)
        {
            //Debugger.Launch();
            bool success = CommandLine.Parser.Default.ParseArguments(args, options, OnVerbCommand);
            if (!success)
            {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

        }

        private static void OnVerbCommand(string verb, object verbSubOptions)
        {
            if (verbSubOptions == null)
                return;
            try
            {
                //Debugger.Launch();
                UserSettings.GetCurrentSettings();
                switch (verb)
                {
                    case "createConnection":
                        createConnection((CreateConnectionSubOptions) verbSubOptions);
                        break;
                    case "listConnections":
                        listConnections((EmptySubOptions) verbSubOptions);
                        break;
                    case "deleteConnection":
                        deleteConnection((DeleteConnectionSubOptions) verbSubOptions);
                        break;
                    case "selfTest":
                        selfTest((EmptySubOptions) verbSubOptions);
                        break;
                    default:
                        throw new ArgumentException("Not implemented verb: " + verb);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
            finally
            {
                UserSettings.SaveCurrentSettings();
            }
        }

        private static void selfTest(EmptySubOptions verbSubOptions)
        {
            Console.WriteLine("Performing HTTPS connection test...");
            var request = WebRequest.Create("https://test.caloom.com");
            var resp = request.GetResponse();
            Console.WriteLine("HTTPS connection test OK");
        }

        private static void deleteConnection(DeleteConnectionSubOptions verbSubOptions)
        {
            var connection = UserSettings.CurrentSettings.Connections.FirstOrDefault(conn => conn.Name == verbSubOptions.ConnectionName);
            if(connection == null)
                throw new ArgumentException("ConnectionName is invalid");
            DeviceSupport.ExecuteRemoteOperationVoid(connection.Device, "TheBall.CORE.RemoteDeviceCoreOperation", new DeviceOperationData
                {
                    OperationRequestString = "DELETEREMOTEDEVICE"
                });
            UserSettings.CurrentSettings.Connections.Remove(connection);
        }

        private static void listConnections(EmptySubOptions verbSubOptions)
        {
            Console.WriteLine("Connections:");
            UserSettings.CurrentSettings.Connections.ForEach(connection => Console.WriteLine("{0} {1} {2}",
                connection.Name, connection.HostName, connection.GroupID));
        }

        private static void createConnection(CreateConnectionSubOptions verbSubOptions)
        {
            string sharedSecret = "testsecretXYZ33";
            string host = verbSubOptions.HostName;
            string targetGroupID = verbSubOptions.GroupID;
            string protocol = host.StartsWith("localdev") ? "ws" : "wss";
            string connectionProtocol = host.StartsWith("localdev") ? "http" : "https";
            var result = SecuritySupport.SecurityNegotiationManager.PerformEKEInitiatorAsBob(protocol + "://" + verbSubOptions.HostName + "/websocket/NegotiateDeviceConnection?groupID=" + targetGroupID,
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

        void testFunc()
            {
            string testStr = "Kikkarainen";
            var prots =
            ProtectedData.Protect(Encoding.UTF8.GetBytes(testStr), null, DataProtectionScope.CurrentUser);
            var unprotected = ProtectedData.Unprotect(prots, null, DataProtectionScope.CurrentUser);
            /*
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) =>
                {
                    Console.WriteLine("Validating server cert...");
                    return true;
                };
             * */
            var request = WebRequest.Create("https://test.caloom.com");
            var resp = request.GetResponse();
            Console.WriteLine(resp.ContentLength);
            Console.WriteLine(Encoding.UTF8.GetString(unprotected));
            Console.ReadLine();
            string targetGroupID = "16061fc9-86c6-4549-a241-dda8b980c1b2";
            string sharedSecret = "testsecretXYZ33";
            //SecuritySupport.SecurityNegotiationManager.EchoClient();
            var result = SecuritySupport.SecurityNegotiationManager.PerformEKEInitiatorAsBob("wss://test.caloom.com/websocket/NegotiateDeviceConnection?groupID=" + targetGroupID,
                                                                                  sharedSecret, "Mono Device Negotation");
            Console.WriteLine(result.EstablishedTrustID);
            Console.ReadLine();
        }
    }
}
