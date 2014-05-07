using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;

namespace TheBall
{
    public static class EncryptionSupport
    {
        private const string KeyBlobName = "SysInternal/AESKey";
        private const string IVBlobName = "SysInternal/AESIV";

        private static AesManaged CurrProvider;

        static EncryptionSupport()
        {
            CurrProvider = new AesManaged();
            RetrieveOrCreateEncDataToDefaultBlob();
        }

        public static string EncryptStringToBase64(string plainText)
        {
            var encryptor = CurrProvider.CreateEncryptor();

            // Create the streams used for encryption. 
            byte[] encrypted;
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt, Encoding.UTF8))
                    {

                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
            return Convert.ToBase64String(encrypted);
        }

        public static string DecryptStringFromBase64(string cipherText)
        {
            var decryptor = CurrProvider.CreateDecryptor();

            byte[] cipherData = Convert.FromBase64String(cipherText);
            string plainText;
            using (MemoryStream msDecrypt = new MemoryStream(cipherData))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt, Encoding.UTF8))
                    {

                        // Read the decrypted bytes from the decrypting stream 
                        // and place them in a string.
                        plainText = srDecrypt.ReadToEnd();
                    }
                }
            }
            return plainText;
        }

        
        public static byte[] EncryptData(byte[] plainText)
        {
            var encryptor = CurrProvider.CreateEncryptor();

            // Create the streams used for encryption. 
            byte[] encrypted;
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (BinaryWriter swEncrypt = new BinaryWriter(csEncrypt))
                    {

                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
            return encrypted;
        }

        public static byte[] DecryptData(byte[] cipherText)
        {
            var decryptor = CurrProvider.CreateDecryptor();

            byte[] plainText;
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        csDecrypt.CopyTo(memoryStream);
                        plainText = memoryStream.ToArray();
                    }
                }
            }
            return plainText;
        }

        private static void RetrieveOrCreateEncDataToDefaultBlob()
        {
            CloudBlob keyBlob = StorageSupport.CurrActiveContainer.GetBlob(KeyBlobName);
            try
            {
                CurrProvider.Key = keyBlob.DownloadByteArray();
            } catch(StorageException storageException)
            {
                if(storageException.ErrorCode == StorageErrorCode.BlobNotFound)
                {
                    CurrProvider.KeySize = 128;
                    CurrProvider.GenerateKey();
                    keyBlob.UploadByteArray(CurrProvider.Key);
                }
                else
                {
                    throw;
                }
            }
            CloudBlob ivBlob = StorageSupport.CurrActiveContainer.GetBlob(IVBlobName);
            try
            {
                CurrProvider.IV = ivBlob.DownloadByteArray();
            } catch(StorageException storageException)
            {
                if (storageException.ErrorCode == StorageErrorCode.BlobNotFound)
                {
                    CurrProvider.GenerateIV();
                    ivBlob.UploadByteArray(CurrProvider.IV);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}