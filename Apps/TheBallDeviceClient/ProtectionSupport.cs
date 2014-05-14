using System.Security.Cryptography;
using System;

namespace TheBall.Support.DeviceClient
{
    public static class ProtectionSupport
    {
        public static byte[] Protect(byte[] dataToProtect)
        {
#if !MONODROID
            return ProtectedData.Protect(dataToProtect, null, DataProtectionScope.CurrentUser);
#else
            throw new NotSupportedException("ProtectedData not supported");
#endif
        }

        public static byte[] Unprotect(byte[] dataToUnprotect)
        {
#if !MONODROID
            return ProtectedData.Unprotect(dataToUnprotect, null, DataProtectionScope.CurrentUser);
#else
            throw new NotSupportedException("ProtectedData not supported");
#endif
        }
    }
}