namespace TheBall.CORE
{
    public class CreateAuthenticatedAsActiveDeviceImplementation
    {
        public static string GetTarget_NegotiationURL(string targetBallHostName, string targetGroupId)
        {
            string protocol = targetBallHostName.StartsWith("localdev:") || targetBallHostName.StartsWith("localhost:") ? "ws" : "wss";
            return string.Format("{2}://{0}/websocket/NegotiateDeviceConnection?groupID={1}", targetBallHostName,
                                 targetGroupId, protocol);
        }

        public static string GetTarget_ConnectionURL(string targetBallHostName, string targetGroupId)
        {
            string protocol = targetBallHostName.StartsWith("localdev:") || targetBallHostName.StartsWith("localhost:") ? "http" : "https";
            return string.Format("{2}://{0}/auth/grp/{1}/DEV", targetBallHostName,
                                 targetGroupId, protocol);
        }

        public static AuthenticatedAsActiveDevice GetTarget_AuthenticatedAsActiveDevice(IContainerOwner owner, string authenticationDeviceDescription, string sharedSecret, string negotiationUrl, string connectionUrl)
        {
            AuthenticatedAsActiveDevice activeDevice = new AuthenticatedAsActiveDevice();
            activeDevice.SetLocationAsOwnerContent(owner, activeDevice.ID);
            activeDevice.AuthenticationDescription = authenticationDeviceDescription;
            activeDevice.SharedSecret = sharedSecret;
            activeDevice.NegotiationURL = negotiationUrl;
            activeDevice.ConnectionURL = connectionUrl;
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