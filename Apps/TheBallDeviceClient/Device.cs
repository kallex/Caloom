using System;

namespace TheBall.Support.DeviceClient
{
    [Serializable]
    public class Device
    {
        public string ConnectionURL;
        public byte[] AESKey;
        public string EstablishedTrustID;
    }
}