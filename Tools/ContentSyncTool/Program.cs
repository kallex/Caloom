using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ContentSyncTool
{
    class Program
    {
        static void Main(string[] args)
        {
            string testStr = "Kikkarainen";
            var prots =
            ProtectedData.Protect(Encoding.UTF8.GetBytes(testStr), null, DataProtectionScope.CurrentUser);
            var unprotected = ProtectedData.Unprotect(prots, null, DataProtectionScope.CurrentUser);
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
