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
                ICommandExecution executionSupport = verbSubOptions as ICommandExecution;
                if (executionSupport != null)
                {
                    executionSupport.ExecuteCommand(verb);
                }
                else // Custom verb implementation
                {
                    switch (verb)
                    {
                        default:
                            throw new ArgumentException("Not implemented verb: " + verb);
                    }
                    
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
