using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using WebSocketSharp;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;

//using System.Net.WebSockets;
#if ASYNC
using System.Threading.Tasks;
#endif

namespace TheBall.Support.DeviceClient
{
    public class SecurityNegotiationManager
    {
        //public static async Task EchoClient()
        private WebSocket Socket;
        private INegotiationProtocolMember ProtocolMember;
        private string DeviceDescription;
        Stopwatch watch = new Stopwatch();
        private bool PlayAsAlice = false;
#if not4
        private SemaphoreSlim WaitingSemaphore = new SemaphoreSlim(0);
#else
        private Semaphore WaitingSemaphore = new Semaphore(0, 1);
#endif
        private TimeSpan MAX_NEGOTIATION_TIME = new TimeSpan(0, 0, 1, 0);
        private string EstablishedTrustID;

        public static SecurityNegotiationResult PerformEKEInitiatorAsAlice(string connectionUrl, byte[] sharedSecret, string deviceDescription)
        {
            return performEkeInitiator(connectionUrl, sharedSecret, deviceDescription, true, null);
        }

        public static SecurityNegotiationResult PerformEKEInitiatorAsBob(string connectionUrl, byte[] sharedSecret, string deviceDescription,
            byte[] sharedSecretPayload)
        {
            return performEkeInitiator(connectionUrl, sharedSecret, deviceDescription, false, sharedSecretPayload);
        }

        private static SecurityNegotiationResult performEkeInitiator(string connectionUrl, byte[] sharedSecret, string deviceDescription, bool playAsAlice, byte[] sharedSecretPayload)
        {
            if(sharedSecretPayload != null && playAsAlice)
                throw new NotSupportedException("Shared secret payload is supported only on Bob - delivered at pinging remote Alice");
            var securityNegotiationManager = InitSecurityNegotiationManager(connectionUrl, sharedSecret, deviceDescription, playAsAlice);
            securityNegotiationManager.SharedSecretPayload = sharedSecretPayload;
            // Perform negotiation
            securityNegotiationManager.PerformNegotiation();
            var aesKey = securityNegotiationManager.ProtocolMember.NegotiationResults[0];
            string deviceID = securityNegotiationManager.EstablishedTrustID;
            return new SecurityNegotiationResult {AESKey = aesKey, EstablishedTrustID = deviceID};
        }


        private void PerformNegotiation()
        {
            watch.Start();
            Socket.Connect();
#if not4
            bool negotiationSuccess = WaitingSemaphore.Wait(MAX_NEGOTIATION_TIME);
#else
            bool negotiationSuccess = WaitingSemaphore.WaitOne(MAX_NEGOTIATION_TIME);
#endif
            Socket.Close();
            if(!negotiationSuccess)
                throw new TimeoutException("Trust negotiation timed out");
        }

        private static SecurityNegotiationManager InitSecurityNegotiationManager(string deviceConnectionUrl, byte[] sharedSecret, string deviceDescription, bool playAsAlice)
        {
            SecurityNegotiationManager securityNegotiationManager = new SecurityNegotiationManager();
            securityNegotiationManager.Socket = new WebSocket(deviceConnectionUrl);
            //securityNegotiationManager.Socket.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
            securityNegotiationManager.Socket.OnOpen += securityNegotiationManager.socket_OnOpen;
            securityNegotiationManager.Socket.OnClose += securityNegotiationManager.socket_OnClose;
            securityNegotiationManager.Socket.OnError += securityNegotiationManager.socket_OnError;
            securityNegotiationManager.Socket.OnMessage += securityNegotiationManager.socket_OnMessage;
            TheBallEKE instance = new TheBallEKE();
            instance.InitiateCurrentSymmetricFromSecret(sharedSecret);
            securityNegotiationManager.PlayAsAlice = playAsAlice;
            securityNegotiationManager.DeviceDescription = deviceDescription;
            if (securityNegotiationManager.PlayAsAlice)
            {
                securityNegotiationManager.ProtocolMember = new TheBallEKE.EKEAlice(instance);
            }
            else
            {
                securityNegotiationManager.ProtocolMember = new TheBallEKE.EKEBob(instance);
            }
            securityNegotiationManager.ProtocolMember.SendMessageToOtherParty =
                bytes => { securityNegotiationManager.Socket.Send(bytes); };
            return securityNegotiationManager;
        }

        private void socket_OnError(object sender, ErrorEventArgs e)
        {
        }

        void socket_OnMessage(object sender, MessageEventArgs e)
        {
            Debug.WriteLine("Received message: " + (e.RawData != null? e.RawData.Length.ToString() : e.Data));
            if (!ProtocolMember.IsDoneWithProtocol)
            {
                ProtocolMember.LatestMessageFromOtherParty = e.RawData;
                ProceedProtocol();
            }
            else // Last message after the protocol and then close up
            {
                if(String.IsNullOrEmpty(e.Data))
                    throw new InvalidDataException("Negotiation protocol end requires EstablishedTrustID as text");
                EstablishedTrustID = e.Data;
                watch.Stop();
                WaitingSemaphore.Release();
            }
        }

        void socket_OnClose(object sender, CloseEventArgs e)
        {
            Debug.WriteLine("Closed");
        }

        void socket_OnOpen(object sender, EventArgs e)
        {
            Debug.WriteLine("Opened");
            if (PlayAsAlice)
                ProceedProtocol();
            else
                PingAlice();
        }

        private void PingAlice()
        {
            Socket.Send(SharedSecretPayload);
        }

        protected byte[] SharedSecretPayload { get; set; }

        void ProceedProtocol()
        {
            while(ProtocolMember.IsDoneWithProtocol == false && ProtocolMember.WaitForOtherParty == false)
            {
                ProtocolMember.PerformNextAction();
            } 
            if (ProtocolMember.IsDoneWithProtocol)
            {
                Socket.Send(DeviceDescription); 
                Console.WriteLine((PlayAsAlice ? "Alice" : "Bob") + " done with EKE in " + watch.ElapsedMilliseconds.ToString() + " ms!");
            }
        }

    }
}