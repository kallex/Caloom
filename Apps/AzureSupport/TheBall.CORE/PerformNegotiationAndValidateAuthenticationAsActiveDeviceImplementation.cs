using System.IO;
using System.Linq;
using System.Net;
using SecuritySupport;

namespace TheBall.CORE
{
    public class PerformNegotiationAndValidateAuthenticationAsActiveDeviceImplementation
    {
        public static AuthenticatedAsActiveDevice GetTarget_AuthenticatedAsActiveDevice(IContainerOwner owner, string authenticatedAsActiveDeviceId)
        {
            return AuthenticatedAsActiveDevice.RetrieveFromOwnerContent(owner, authenticatedAsActiveDeviceId);
        }

        public static string GetTarget_RemoteBallSecretRequestUrl(AuthenticatedAsActiveDevice authenticatedAsActiveDevice)
        {
            string baseWebsocketUrl = authenticatedAsActiveDevice.NegotiationURL.Replace("wss://", "https://").Replace("ws://", "http://");
            int lastIndexOfSlash = baseWebsocketUrl.LastIndexOf('/');
            return baseWebsocketUrl.Substring(0, lastIndexOfSlash) + "/RequestSharedSecret";
        }

        public static byte[] GetTarget_SharedSecretFullPayload(string remoteBallSecretRequestUrl)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(remoteBallSecretRequestUrl);
            request.Method = "POST";
            request.ContentLength = 0;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
                throw new WebException("Invalid response from remote secret request");
            using (MemoryStream memStream = new MemoryStream())
            {
                var responseStream = response.GetResponseStream();
                responseStream.CopyTo(memStream);
                return memStream.ToArray();
            }
        }

        public static byte[] GetTarget_SharedSecretData(byte[] sharedSecretFullPayload)
        {
            return sharedSecretFullPayload.Take(32).ToArray();
        }

        public static byte[] GetTarget_SharedSecretPayload(byte[] sharedSecretFullPayload)
        {
            return sharedSecretFullPayload.Skip(32).ToArray();
        }

        public static void ExecuteMethod_NegotiateWithTarget(AuthenticatedAsActiveDevice authenticatedAsActiveDevice, byte[] sharedSecretData, byte[] sharedSecretPayload)
        {
            var negotiationResult =
                SecurityNegotiationManager.PerformEKEInitiatorAsBob(authenticatedAsActiveDevice.NegotiationURL,
                                                                    sharedSecretData, authenticatedAsActiveDevice.AuthenticationDescription,
                                                                    sharedSecretPayload);
            authenticatedAsActiveDevice.ActiveSymmetricAESKey = negotiationResult.AESKey;
            authenticatedAsActiveDevice.EstablishedTrustID = negotiationResult.EstablishedTrustID;
            authenticatedAsActiveDevice.IsValidatedAndActive = true;
        }

        public static void ExecuteMethod_StoreObject(AuthenticatedAsActiveDevice authenticatedAsActiveDevice)
        {
            authenticatedAsActiveDevice.StoreInformation();
        }
    }
}