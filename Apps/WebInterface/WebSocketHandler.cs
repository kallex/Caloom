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

            Action<WebSocketContext, WebSocket, byte[], string> OnReceiveMessage = HandleReceivedMessage;
            Action<WebSocketContext, WebSocket> OnClose = HandleCloseMessage;

            while (socket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult receiveResult =
                    await socket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                if (receiveResult.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
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
                    var responseString = "First byte: " + receiveBuffer[0].ToString() + " last byte: " +
                                         receiveBuffer[receiveBuffer.Length - 1];
                    //var receivedString = Encoding.UTF8.GetString(receiveBuffer, 0, count);
                    var echoString = "You sent binary: " + responseString;
                    ArraySegment<byte> outputBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(echoString));
                    await socket.SendAsync(outputBuffer, WebSocketMessageType.Text, true, CancellationToken.None);

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
                    var receivedString = Encoding.UTF8.GetString(receiveBuffer, 0, count);
                    var echoString = "You said something like: " + receivedString;
                    ArraySegment<byte> outputBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(echoString));
                    await socket.SendAsync(outputBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        private void HandleCloseMessage(WebSocketContext wsCtx, WebSocket socket)
        {
        }

        private static void HandleReceivedMessage(WebSocketContext wsCtx, WebSocket socket, byte[] binaryMessage, string textMessage)
        {

        }

        #endregion
    }
}
