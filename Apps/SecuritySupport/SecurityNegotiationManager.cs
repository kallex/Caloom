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
    public class SecurityNegotiationManager
    {
        //public static async Task EchoClient()
        private static WebSocket socket;
        private static INegotiationProtocolMember protocolMember;
        private static int aliceActionIX;
        static Stopwatch watch = new Stopwatch();
        private static bool playAlice = false;
        public static void EchoClient()
        {
            Console.WriteLine("Starting EKE WSS connection");
            //socket = new WebSocket("wss://theball.protonit.net/websocket/mytest.k");
            socket = new WebSocket("ws://localhost:50430/websocket/mytest.k");
            socket.OnOpen += socket_OnOpen;
            socket.OnClose += socket_OnClose;
            socket.OnError += socket_OnError;
            socket.OnMessage += socket_OnMessage;
            TheBallEKE instance = new TheBallEKE();
            instance.InitiateCurrentSymmetricFromSecret("testsecretXYZ33");
            playAlice = false;
            if (playAlice)
            {
                protocolMember = new TheBallEKE.EKEAlice(instance);
            }
            else
            {
                protocolMember = new TheBallEKE.EKEBob(instance);
            }
            protocolMember.SendMessageToOtherParty = bytes => { socket.Send(bytes); };
            watch.Start();
            socket.Connect();
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

        static void socket_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine("Received message: " + (e.RawData != null? e.RawData.Length.ToString() : e.Data));
            protocolMember.LatestMessageFromOtherParty = e.RawData;
            ProceedProtocol();
        }

        static void socket_OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("ERROR: " + e.Message);
        }

        static void socket_OnClose(object sender, CloseEventArgs e)
        {
            Console.WriteLine("Closed");
        }

        static void socket_OnOpen(object sender, EventArgs e)
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

        private static void PingAlice()
        {
            socket.Send(new byte[0]);
        }

        static void ProceedProtocol()
        {
            while(protocolMember.IsDoneWithProtocol == false && protocolMember.WaitForOtherParty == false)
            {
                protocolMember.PerformNextAction();
            } 
            if (protocolMember.IsDoneWithProtocol)
            {
                watch.Stop();
                Console.WriteLine((playAlice ? "Alice" : "Bob") + " done with EKE in " + watch.ElapsedMilliseconds.ToString() + " ms!");
            }
        }

    }
}