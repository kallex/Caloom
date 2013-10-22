using System;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Web;

namespace TheBall.CORE
{
    public class PushToInformationOutputImplementation
    {
        public static InformationOutput GetTarget_InformationOutput(IContainerOwner owner, string informationOutputId)
        {
            return InformationOutput.RetrieveFromOwnerContent(owner, informationOutputId);
        }

        public static void ExecuteMethod_VerifyValidOutput(InformationOutput informationOutput)
        {
            if (informationOutput.IsValidatedAndActive == false)
                throw new SecurityException("InformationOutput is not active");
        }

        public static string GetTarget_DestinationURL(InformationOutput informationOutput)
        {
            return informationOutput.DestinationURL;
        }

        public static string GetTarget_LocalContentURL(InformationOutput informationOutput)
        {
            return informationOutput.LocalContentURL;
        }

        public static AuthenticatedAsActiveDevice GetTarget_AuthenticatedAsActiveDevice(InformationOutput informationOutput)
        {
            var authenticationID = informationOutput.AuthenticatedDeviceID;
            if (string.IsNullOrEmpty(authenticationID))
                return null;
            VirtualOwner owner = VirtualOwner.FigureOwner(informationOutput);
            return AuthenticatedAsActiveDevice.RetrieveFromOwnerContent(owner, authenticationID);
        }

        public static void ExecuteMethod_PushToInformationOutput(IContainerOwner owner, InformationOutput informationOutput, string destinationUrl, string destinationContentName, string localContentUrl, AuthenticatedAsActiveDevice authenticatedAsActiveDevice)
        {
            if(authenticatedAsActiveDevice == null)
                throw new NotSupportedException("Push not currently supported without authenticated as device connection");
            WebClient webClient = new WebClient();
            if (destinationUrl.EndsWith("/") == false)
                destinationUrl += "/";
            var blob = StorageSupport.GetOwnerBlobReference(owner, localContentUrl);

            //var blobStream = blob.DownloadToStream()
            //CryptoStream cryptoStream = new CryptoStream()
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(destinationUrl);
            request.Method = "POST";
            AesManaged aes = new AesManaged();
            aes.KeySize = 256;
            aes.GenerateIV();
            aes.Key = authenticatedAsActiveDevice.ActiveSymmetricAESKey;
            var ivBase64 = Convert.ToBase64String(aes.IV);
            request.Headers.Add("Authorization", "DeviceAES:" + ivBase64 + ":" + authenticatedAsActiveDevice.EstablishedTrustID + ":" + destinationContentName);
            var requestStream = request.GetRequestStream();
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var cryptoStream = new CryptoStream(requestStream, encryptor, CryptoStreamMode.Write);
            blob.DownloadToStream(cryptoStream);
            cryptoStream.Close();
            var response = (HttpWebResponse) request.GetResponse();
            if(response.StatusCode != HttpStatusCode.OK)
                throw new InvalidOperationException("PushToInformationOutput failed with Http status: " + response.StatusCode.ToString());
        }

        public static string GetTarget_DestinationContentName(InformationOutput informationOutput)
        {
            string destinationContentName = informationOutput.DestinationContentName;
            if (string.IsNullOrEmpty(destinationContentName))
                return "bulkdump.all";
            return informationOutput.DestinationContentName;
        }
    }
}