using System;
using System.IO;
//using System.Net.WebSockets;
using System.Net.WebSockets;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using AaltoGlobalImpact.OIP;
using AzureSupport;
using DotNetOpenAuth.OpenId.RelyingParty;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using SecuritySupport;
using TheBall;

namespace WebInterface
{
    public class WebSocketHandler : IHttpHandler
    {
        private const string AuthEmailValidation = "/emailvalidation/";
        private int AuthEmailValidationLen;


        /// <summary>
        /// You will need to configure this handler in the web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            bool isSocket = false;
            if (context.IsWebSocketRequest)
            {
                isSocket = true;
            }
            if (context.IsWebSocketRequest)
                context.AcceptWebSocketRequest(HandleWebSocket);
            else
                context.Response.StatusCode = 400;

            /*
            WebSupport.InitializeContextStorage(context.Request);
            try
            {
                if (request.Path.StartsWith(AuthEmailValidation))
                {
                    HandleEmailValidation(context);
                }        
            } finally
            {
                InformationContext.ProcessAndClearCurrent();
            }*/
        }

        private async Task HandleWebSocket(WebSocketContext wsContext)
        {
            const int maxMessageSize = 1024 * 1024;
            byte[] receiveBuffer = new byte[maxMessageSize];
            WebSocket socket = wsContext.WebSocket;

            Func<InformationContext, WebSocketContext, WebSocket, byte[], string, Task> OnReceiveMessage = HandleReceivedMessage;
            Action<WebSocketContext, WebSocket> OnClose = HandleCloseMessage;
            InformationContext informationContext = new InformationContext();

            while (socket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult receiveResult =
                    await socket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                if (receiveResult.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    OnClose(wsContext, socket);
                }
                else if (receiveResult.MessageType == WebSocketMessageType.Binary)
                {
                    //await
                    //    socket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Cannot accept binary frame",
                    //                      CancellationToken.None);

                    int count = receiveResult.Count;

                    while (receiveResult.EndOfMessage == false)
                    {
                        if (count >= maxMessageSize)
                        {
                            string closeMessage = string.Format("Maximum message size: {0} bytes.", maxMessageSize);
                            await
                                socket.CloseAsync(WebSocketCloseStatus.MessageTooBig, closeMessage,
                                                  CancellationToken.None);
                            return;
                        }
                        receiveResult =
                            await
                            socket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer, count, maxMessageSize - count),
                                                CancellationToken.None);
                        count += receiveResult.Count;
                    }
                    //var receivedString = Encoding.UTF8.GetString(receiveBuffer, 0, count);
                    byte[] binaryMessage = new byte[count];
                    Array.Copy(receiveBuffer, binaryMessage, count);
                    await OnReceiveMessage(informationContext, wsContext, socket, binaryMessage, null);
                }
                else
                {
                    int count = receiveResult.Count;

                    while (receiveResult.EndOfMessage == false)
                    {
                        if (count >= maxMessageSize)
                        {
                            string closeMessage = string.Format("Maximum message size: {0} bytes.", maxMessageSize);
                            await
                                socket.CloseAsync(WebSocketCloseStatus.MessageTooBig, closeMessage,
                                                  CancellationToken.None);
                            return;
                        }
                        receiveResult =
                            await
                            socket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer, count, maxMessageSize - count),
                                                CancellationToken.None);
                        count += receiveResult.Count;
                    }
                    var textMessage = Encoding.UTF8.GetString(receiveBuffer, 0, count);
                    await OnReceiveMessage(informationContext, wsContext, socket, null, textMessage);
                }
            }
        }

        private static void HandleCloseMessage(WebSocketContext wsCtx, WebSocket socket)
        {
        }

        private async static Task HandleReceivedMessage(InformationContext iCtx, WebSocketContext wsCtx, WebSocket socket, byte[] binaryMessage, string textMessage)
        {
            if (binaryMessage != null)
            {
                bool playBob = false;
                if (playBob)
                {
                    TheBallEKE.EKEBob bob = null;
                    iCtx.AccessLockedItems(dict =>
                        {
                            if (dict.ContainsKey("EKEINITBOB") == false)
                            {
                                TheBallEKE bobInstance = new TheBallEKE();
                                bobInstance.InitiateCurrentSymmetricFromSecret("testsecretXYZ2");
                                bob = new TheBallEKE.EKEBob(bobInstance, true);
                                dict.Add("EKEINITBOB", bob);
                            }
                            else
                            {
                                bob = (TheBallEKE.EKEBob) dict["EKEINITBOB"];
                            }
                        });
                    if (bob.SendMessageToAliceAsync == null)
                    {
                        bob.SendMessageToAliceAsync = async bytes => { await SendBinaryMessage(socket, bytes); };
                    }
                    bob.LatestMessageFromAlice = binaryMessage;
                    while (bob.IsDoneWithEKE == false && bob.WaitForAlice == false)
                    {
                        await bob.PerformNextActionAsync();
                    }
                }
                else // play alice
                {
                    TheBallEKE.EKEAlice alice = null;
                    iCtx.AccessLockedItems(dict =>
                    {
                        if (dict.ContainsKey("EKEINITALICE") == false)
                        {
                            TheBallEKE aliceInstance = new TheBallEKE();
                            aliceInstance.InitiateCurrentSymmetricFromSecret("testsecretXYZ2");
                            alice = new TheBallEKE.EKEAlice(aliceInstance, true);
                            dict.Add("EKEINITALICE", alice);
                        }
                        else
                        {
                            alice = (TheBallEKE.EKEAlice)dict["EKEINITALICE"];
                        }
                    });
                    if (alice.SendMessageToBobAsync == null)
                    {
                        alice.SendMessageToBobAsync = async bytes => { await SendBinaryMessage(socket, bytes); };
                    }
                    else
                    {
                        // Alice plays first, so only after the second message we start putting messages from Bob
                        alice.LatestMessageFromBob = binaryMessage;
                    }
                    while (alice.IsDoneWithEKE == false && alice.WaitForBob == false)
                    {
                        await alice.PerformNextActionAsync();
                    }
                }

            }
            else
            {
                string echoString = "You said: " + textMessage;
                await SendTextMessage(socket, echoString);
            }
        }

        private async static Task SendTextMessage(WebSocket socket, string textMessage)
        {
            ArraySegment<byte> outputBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(textMessage));
            await socket.SendAsync(outputBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async static Task SendBinaryMessage(WebSocket socket, byte[] binaryMessage)
        {
            ArraySegment<byte> outputBuffer = new ArraySegment<byte>(binaryMessage);
            await socket.SendAsync(outputBuffer, WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        #endregion
    }
}
