using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using AzureSupport;

namespace ContentSyncTool
{
    public partial class DeviceOperationData
    {
        public string OperationRequestString { get; set; }
        public string[] OperationParameters { get; set; }
        public string[] OperationReturnValues { get; set; }
        public bool OperationResult { get; set; }
        public ContentItemLocationWithMD5[] OperationSpecificContentData { get; set; }
    }

    public partial class ContentItemLocationWithMD5
    {
        public string ContentLocation { get; set; }
        public string ContentMD5 { get; set; }
        public ItemData[] ItemDatas { get; set; }
    }

    public partial class ItemData
    {
        public string DataName { get; set; }
        public string ItemTextData { get; set; }
    }


    public static class DeviceSupport
    {
        public const string OperationPrefixStr = "OP-";

        public static TReturnType ExecuteRemoteOperation<TReturnType>(UserSettings.Device device, string operationName, object operationParameters)
        {
            return (TReturnType)executeRemoteOperation<TReturnType>(device, operationName, operationParameters);
        }        
        public static void ExecuteRemoteOperationVoid(UserSettings.Device device, string operationName, object operationParameters)
        {
            executeRemoteOperation<object>(device, operationName, operationParameters);
        }

        private static object executeRemoteOperation<TReturnType>(UserSettings.Device device, string operationName, object operationParameters)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(device.ConnectionURL);
            request.Method = "POST";
            AesManaged aes = new AesManaged();
            aes.KeySize = 256;
            aes.GenerateIV();
            aes.Key = device.AESKey;
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
            return getObjectFromResponseStream<TReturnType>(response, device.AESKey);
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

        public static void PushContentToDevice(UserSettings.Device device, string localContentFileName, string destinationContentName)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(device.ConnectionURL);
            request.Method = "POST";
            AesManaged aes = new AesManaged();
            aes.KeySize = 256;
            aes.GenerateIV();
            aes.Key = device.AESKey;
            var ivBase64 = Convert.ToBase64String(aes.IV);
            request.Headers.Add("Authorization", "DeviceAES:" + ivBase64 + ":" + device.EstablishedTrustID + ":" + destinationContentName);
            var requestStream = request.GetRequestStream();
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var cryptoStream = new CryptoStream(requestStream, encryptor, CryptoStreamMode.Write);
            var fileStream = File.OpenRead(localContentFileName);
            fileStream.CopyTo(cryptoStream);
            cryptoStream.Close();
            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
                throw new InvalidOperationException("PushToInformationOutput failed with Http status: " + response.StatusCode.ToString());
            
        }
    }
}