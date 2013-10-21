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
            var negotiationResult =
                SecurityNegotiationManager.PerformEKEInitiatorAsBob(authenticatedAsActiveDevice.NegotiationURL,
                                                                    authenticatedAsActiveDevice.SharedSecret, authenticatedAsActiveDevice.AuthenticationDescription);
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