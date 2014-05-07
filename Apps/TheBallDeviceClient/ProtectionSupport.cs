using System.Security.Cryptography;

namespace TheBall.Support.DeviceClient
{
    public static class ProtectionSupport
    {
        public static byte[] Protect(byte[] dataToProtect)
        {
            return ProtectedData.Protect(dataToProtect, null, DataProtectionScope.CurrentUser);
        }

        public static byte[] Unprotect(byte[] dataToUnprotect)
        {
            return ProtectedData.Unprotect(dataToUnprotect, null, DataProtectionScope.CurrentUser);
        }
    }
}