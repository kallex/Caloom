using SecuritySupport;

namespace TheBall.CORE
{
    public class PerformNegotiationAndValidateAuthenticationAsActiveDeviceImplementation
    {
        public static AuthenticatedAsActiveDevice GetTarget_AuthenticatedAsActiveDevice(IContainerOwner owner, string authenticatedAsActiveDeviceId)
        {
            return AuthenticatedAsActiveDevice.RetrieveFromOwnerContent(owner, authenticatedAsActiveDeviceId);
        }

        public static void ExecuteMethod_NegotiateWithTarget(AuthenticatedAsActiveDevice authenticatedAsActiveDevice)
        {
            //SecurityNegotiationManager
        }

        public static void ExecuteMethod_StoreObject(AuthenticatedAsActiveDevice authenticatedAsActiveDevice)
        {
            authenticatedAsActiveDevice.StoreInformation();
        }
    }
}