namespace TheBall.CORE
{
    public class CreateAuthenticatedAsActiveDeviceImplementation
    {
        public static string GetTarget_NegotiationURL(string targetBallHostName, string targetGroupId)
        {
            return string.Format("wss://{0}/websocket/NegotiateDeviceConnection?groupID={1}", targetBallHostName,
                                 targetGroupId);
        }

        public static AuthenticatedAsActiveDevice GetTarget_AuthenticatedAsActiveDevice(IContainerOwner owner, string authenticationDeviceDescription, string sharedSecret, string negotiationUrl)
        {
            AuthenticatedAsActiveDevice activeDevice = new AuthenticatedAsActiveDevice();
            activeDevice.SetLocationAsOwnerContent(owner, activeDevice.ID);
            activeDevice.AuthenticationDescription = authenticationDeviceDescription;
            activeDevice.SharedSecret = sharedSecret;
            activeDevice.NegotiationURL = negotiationUrl;
            return activeDevice;
        }

        public static void ExecuteMethod_StoreObject(AuthenticatedAsActiveDevice authenticatedAsActiveDevice)
        {
            authenticatedAsActiveDevice.StoreInformation();
        }

        public static CreateAuthenticatedAsActiveDeviceReturnValue Get_ReturnValue(AuthenticatedAsActiveDevice authenticatedAsActiveDevice)
        {
            return new CreateAuthenticatedAsActiveDeviceReturnValue
                {
                    CreatedAuthenticatedAsActiveDevice = authenticatedAsActiveDevice
                };
        }
    }
}