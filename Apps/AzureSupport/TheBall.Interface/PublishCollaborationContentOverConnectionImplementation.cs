using TheBall.CORE;
using TheBall.Interface.INT;

namespace TheBall.Interface
{
    public class PublishCollaborationContentOverConnectionImplementation
    {
        public static Connection GetTarget_Connection(string connectionId)
        {
            return Connection.RetrieveFromOwnerContent(InformationContext.CurrentOwner, connectionId);
        }


        public static void ExecuteMethod_CallOtherSideProcessingForCopiedContent(Connection connection, bool callDeviceSyncToSendContentOutput)
        {
            if (callDeviceSyncToSendContentOutput == false)
                return;
            ConnectionCommunicationData connectionCommunication = new ConnectionCommunicationData
            {
                ActiveSideConnectionID = connection.ID,
                ProcessRequest = "PROCESSPUSHEDCONTENT",
                ReceivingSideConnectionID = connection.OtherSideConnectionID
            };
            DeviceSupport.ExecuteRemoteOperationVoid(connection.DeviceID,
                                                     "TheBall.Interface.ExecuteRemoteCalledConnectionOperation", 
                                                     connectionCommunication);
        }

        public static SyncConnectionContentToDeviceToSendParameters CallSyncConnectionContentToDeviceToSend_GetParameters(Connection connection)
        {
            return new SyncConnectionContentToDeviceToSendParameters {Connection = connection};
        }

        public static bool ExecuteMethod_CallDeviceSyncToSendContent(Connection connection)
        {
            var result = SyncCopyContentToDeviceTarget.Execute(
                new SyncCopyContentToDeviceTargetParameters { AuthenticatedAsActiveDeviceID = connection.DeviceID});
            return result.CopiedItems.Length > 0 || result.DeletedItems.Length > 0;
        }
    }
}