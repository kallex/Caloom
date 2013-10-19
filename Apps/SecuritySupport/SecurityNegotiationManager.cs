using System;
using System.Diagnostics;
using System.Linq;
//using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace SecuritySupport
{
    public class SecurityNegotiationResult
    {
        
    }

    public class SecurityNegotiationManager
    {
        //public static async Task EchoClient()
        private WebSocket socket;
        private INegotiationProtocolMember protocolMember;
        Stopwatch watch = new Stopwatch();
        private bool playAlice = false;

        public static SecurityNegotiationResult PerformEKEInitiatorAsAlice(string connectionUrl, string sharedSecret)
        {
            return performEkeInitiator(connectionUrl, sharedSecret, true);
        }

        public static SecurityNegotiationResult PerformEKEInitiatorAsBob(string connectionUrl, string sharedSecret)
        {
            return performEkeInitiator(connectionUrl, sharedSecret, false);
        }

        private static SecurityNegotiationResult performEkeInitiator(string connectionUrl, string sharedSecret,
                                                                     bool playAsAlice)
        {
            return null;
        }

        public static void EchoClient()
        {
            Console.WriteLine("Starting EKE WSS connection");
            //string hostWithProtocolAndPort = "ws://localhost:50430";
            string hostWithProtocolAndPort = "wss://theball.protonit.net";
            //string idParam = "accountemail=kalle.launiala@gmail.com";
            string idParam = "groupID=4ddf4bef-0f60-41b6-925d-02721e89d637";
            string deviceConnectionUrl = hostWithProtocolAndPort + "/websocket/NegotiateDeviceConnection?" + idParam;
            //socket = new WebSocket("wss://theball.protonit.net/websocket/mytest.k");
            SecurityNegotiationManager securityNegotiationManager = new SecurityNegotiationManager();
            securityNegotiationManager.socket = new WebSocket(deviceConnectionUrl);
            securityNegotiationManager.socket.OnOpen += securityNegotiationManager.socket_OnOpen;
            securityNegotiationManager.socket.OnClose += securityNegotiationManager.socket_OnClose;
            securityNegotiationManager.socket.OnError += securityNegotiationManager.socket_OnError;
            securityNegotiationManager.socket.OnMessage += securityNegotiationManager.socket_OnMessage;
            TheBallEKE instance = new TheBallEKE();
            instance.InitiateCurrentSymmetricFromSecret("testsecretXYZ33");
            securityNegotiationManager.playAlice = false;
            if (securityNegotiationManager.playAlice)
            {
                securityNegotiationManager.protocolMember = new TheBallEKE.EKEAlice(instance);
            }
            else
            {
                securityNegotiationManager.protocolMember = new TheBallEKE.EKEBob(instance);
            }
            securityNegotiationManager.protocolMember.SendMessageToOtherParty = bytes => { securityNegotiationManager.socket.Send(bytes); };
            securityNegotiationManager.watch.Start();
            securityNegotiationManager.socket.Connect();
#if native45

    //WebSocket socket = new ClientWebSocket();
    //WebSocket.CreateClientWebSocket()
            ClientWebSocket socket = new ClientWebSocket();
            Uri uri = new Uri("ws://localhost:50430/websocket/mytest.k");
            var cts = new CancellationTokenSource();
            await socket.ConnectAsync(uri, cts.Token);

            Console.WriteLine(socket.State);

            Task.Factory.StartNew(
                async () =>
                {
                    var rcvBytes = new byte[128];
                    var rcvBuffer = new ArraySegment<byte>(rcvBytes);
                    while (true)
                    {
                        WebSocketReceiveResult rcvResult = await socket.ReceiveAsync(rcvBuffer, cts.Token);
                        byte[] msgBytes = rcvBuffer.Skip(rcvBuffer.Offset).Take(rcvResult.Count).ToArray();
                        string rcvMsg = Encoding.UTF8.GetString(msgBytes);
                        Console.WriteLine("Received: {0}", rcvMsg);
                    }
                }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            while (true)
            {
                var message = Console.ReadLine();
                if (message == "Bye")
                {
                    cts.Cancel();
                    return;
                }
                byte[] sendBytes = Encoding.UTF8.GetBytes(message);
                var sendBuffer = new ArraySegment<byte>(sendBytes);
                await
                    socket.SendAsync(sendBuffer, WebSocketMessageType.Text, endOfMessage: true,
                                     cancellationToken: cts.Token);
            }

#endif
        }

        void socket_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine("Received message: " + (e.RawData != null? e.RawData.Length.ToString() : e.Data));
            protocolMember.LatestMessageFromOtherParty = e.RawData;
            ProceedProtocol();
        }

        void socket_OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("ERROR: " + e.Message);
        }

        void socket_OnClose(object sender, CloseEventArgs e)
        {
            Console.WriteLine("Closed");
        }

        void socket_OnOpen(object sender, EventArgs e)
        {
            Console.WriteLine("Opened");
            if (playAlice)
                ProceedProtocol();
            else
                PingAlice();
            /*
            Console.WriteLine("Opened");
            socket.Send("Pöö");
            byte[] bigChunk = new byte[1024*1024];
            bigChunk[0] = (byte) 12;
            bigChunk[1024*1024 - 1] = (byte) 23;
            socket.Send(bigChunk);
            byte[] smallerChunk = new byte[123];
            socket.Send(smallerChunk);
             * */

        }

        private void PingAlice()
        {
            socket.Send(new byte[0]);
        }

        void ProceedProtocol()
        {
            while(protocolMember.IsDoneWithProtocol == false && protocolMember.WaitForOtherParty == false)
            {
                protocolMember.PerformNextAction();
            } 
            if (protocolMember.IsDoneWithProtocol)
            {
                watch.Stop();
                Console.WriteLine((playAlice ? "Alice" : "Bob") + " done with EKE in " + watch.ElapsedMilliseconds.ToString() + " ms!");
                socket.Send("Demo device details, rly!");
            }
        }

    }
}