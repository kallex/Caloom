using System;
using System.Net;
using System.Security.Cryptography;
using AzureSupport;

namespace TheBall.CORE
{
    public static class DeviceSupport
    {
        public const string OperationPrefixStr = "OP-";

        public static TReturnType ExecuteRemoteOperation<TReturnType>(string deviceID, string operationName, object operationParameters)
        {
            return (TReturnType) executeRemoteOperation<TReturnType>(deviceID, operationName, operationParameters);
        }        
        public static void ExecuteRemoteOperationVoid(string deviceID, string operationName, object operationParameters)
        {
            executeRemoteOperation<object>(deviceID, operationName, operationParameters);
        }

        private static object executeRemoteOperation<TReturnType>(string deviceID, string operationName, object operationParameters)
        {
            AuthenticatedAsActiveDevice device = AuthenticatedAsActiveDevice.RetrieveFromOwnerContent(InformationContext.CurrentOwner, deviceID);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(device.ConnectionURL);
            request.Method = "POST";
            AesManaged aes = new AesManaged();
            aes.KeySize = 256;
            aes.GenerateIV();
            aes.Key = device.ActiveSymmetricAESKey;
            var ivBase64 = Convert.ToBase64String(aes.IV);
            request.Headers.Add("Authorization", "DeviceAES:" + ivBase64 
                + ":" + device.EstablishedTrustID 
                + ":" + String.Format("{0}{1}", OperationPrefixStr, operationName));
            var requestStream = request.GetRequestStream();
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (var cryptoStream = new CryptoStream(requestStream, encryptor, CryptoStreamMode.Write))
            {
                JSONSupport.SerializeToJSONStream(operationParameters, cryptoStream);
            }
            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
                throw new InvalidOperationException("PushToInformationOutput failed with Http status: " + response.StatusCode.ToString());
            if (typeof (TReturnType) == typeof (object))
                return null;
            return getObjectFromResponseStream<TReturnType>(response, device.ActiveSymmetricAESKey);
        }

        private static TReturnType getObjectFromResponseStream<TReturnType>(HttpWebResponse response, byte[] aesKey)
        {
            string ivStr = response.Headers["IV"];
            var respStream = response.GetResponseStream();
            AesManaged aes = new AesManaged();
            aes.KeySize = 256;
            aes.IV = Convert.FromBase64String(ivStr);
            aes.Key = aesKey;
            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using (CryptoStream cryptoStream = new CryptoStream(respStream, decryptor, CryptoStreamMode.Read))
            {
                var contentObject = JSONSupport.GetObjectFromStream<TReturnType>(cryptoStream);
                return contentObject;
            }
        }
    }
}