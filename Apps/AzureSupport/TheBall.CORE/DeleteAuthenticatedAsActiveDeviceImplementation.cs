namespace TheBall.CORE
{
    public class DeleteAuthenticatedAsActiveDeviceImplementation
    {
        public static AuthenticatedAsActiveDevice GetTarget_AuthenticatedAsActiveDevice(IContainerOwner owner, string authenticatedAsActiveDeviceId)
        {
            return AuthenticatedAsActiveDevice.RetrieveFromOwnerContent(owner, authenticatedAsActiveDeviceId);
        }

        public static void ExecuteMethod_DeleteAuthenticatedAsActiveDevice(AuthenticatedAsActiveDevice authenticatedAsActiveDevice)
        {
            authenticatedAsActiveDevice.DeleteInformationObject();
        }
    }
}