using TheBall.CORE.INT;

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

        public static void ExecuteMethod_CallDeleteDeviceOnRemoteSide(AuthenticatedAsActiveDevice authenticatedAsActiveDevice)
        {
            try
            {
                var result = DeviceSupport.ExecuteRemoteOperation<DeviceOperationData>(authenticatedAsActiveDevice.ID,
                                                                                       "TheBall.CORE.RemoteDeviceCoreOperation", new DeviceOperationData {OperationRequestString = "DELETEREMOTEDEVICE"});
            }
            catch
            {
                
            }
        }
    }
}