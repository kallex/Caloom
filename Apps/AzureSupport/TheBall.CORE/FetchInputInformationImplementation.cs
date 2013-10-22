using System;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using Microsoft.WindowsAzure.StorageClient;

namespace TheBall.CORE
{
    public class FetchInputInformationImplementation
    {
        public static InformationInput GetTarget_InformationInput(IContainerOwner owner, string informationInputId)
        {
            return InformationInput.RetrieveFromDefaultLocation(informationInputId, owner);
        }

        public static void ExecuteMethod_VerifyValidInput(InformationInput informationInput)
        {
            if(informationInput.IsValidatedAndActive == false)
                throw new SecurityException("InformationInput is not active");
        }

        public static string GetTarget_InputFetchLocation(InformationInput informationInput)
        {
            return informationInput.RelativeLocation + "_Input";
        }

        public static string GetTarget_InputFetchName(InformationInput informationInput)
        {
            // TODO: timestamped, incremental and other options supported, now just bulk
            string localContentName = informationInput.LocalContentName;
            if (string.IsNullOrEmpty(localContentName))
                return "bulkdump.all";
            return informationInput.LocalContentName;
        }

        public static void ExecuteMethod_FetchInputToStorage(IContainerOwner owner, string queryParameters, InformationInput informationInput, string inputFetchLocation, string inputFetchName, AuthenticatedAsActiveDevice authenticatedAsActiveDevice)
        {
                string url = string.IsNullOrEmpty(queryParameters)
                                 ? informationInput.LocationURL
                                 : informationInput.LocationURL + queryParameters;
            if (authenticatedAsActiveDevice == null)
            {
                WebRequest getRequest = WebRequest.Create(url);
                var response = getRequest.GetResponse();
                var stream = response.GetResponseStream();
                var targetBlob = StorageSupport.CurrActiveContainer.GetBlob(inputFetchLocation + "/" + inputFetchName,
                                                                            owner);
                targetBlob.UploadFromStream(stream);
            }
            else
            {
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
                request.Method = "GET";
                request.Headers.Add("Authorization", "DeviceAES::" + authenticatedAsActiveDevice.EstablishedTrustID + ":");
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new InvalidOperationException("Authroized fetch failed with non-OK status code");
                string ivStr = response.Headers["IV"];
                string contentRoot = inputFetchLocation;
                string blobName = contentRoot + "/" + inputFetchName;
                var blob = StorageSupport.GetOwnerBlobReference(owner, blobName);
                if (blob.Name != blobName)
                    throw new InvalidDataException("Invalid content name");
                var respStream = response.GetResponseStream();
                AesManaged aes = new AesManaged();
                aes.KeySize = 256;
                aes.IV = Convert.FromBase64String(ivStr);
                aes.Key = authenticatedAsActiveDevice.ActiveSymmetricAESKey;
                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                CryptoStream cryptoStream = new CryptoStream(respStream, decryptor, CryptoStreamMode.Read);
                blob.UploadFromStream(cryptoStream);
            }
        }

        public static AuthenticatedAsActiveDevice GetTarget_AuthenticatedAsActiveDevice(InformationInput informationInput)
        {
            var authenticationID = informationInput.AuthenticatedDeviceID;
            if (string.IsNullOrEmpty(authenticationID))
                return null;
            VirtualOwner owner = VirtualOwner.FigureOwner(informationInput);
            return AuthenticatedAsActiveDevice.RetrieveFromOwnerContent(owner, authenticationID);
        }
    }
}