using TheBall.CORE;
using TheBall.Interface.INT;

namespace TheBall.Interface
{
    public class FinalizeConnectionAfterGroupAuthorizationImplementation
    {
        private static IContainerOwner Owner
        {
            get { return InformationContext.CurrentOwner; }
        }

        public static Connection GetTarget_Connection(string connectionId)
        {
            return Connection.RetrieveFromOwnerContent(Owner, connectionId);

        }

        public static ConnectionCommunicationData GetTarget_ConnectionCommunicationData(Connection connection)
        {
            return new ConnectionCommunicationData
                {
                    ActiveSideConnectionID = connection.ID,
                    ProcessRequest = "FINALIZECONNECTION"
                };
        }

        public static void ExecuteMethod_CallDeviceServiceForFinalizing(Connection connection, ConnectionCommunicationData connectionCommunicationData)
        {
            connectionCommunicationData.ProcessParametersString = connection.Description;
            var result = DeviceSupport
                .ExecuteRemoteOperation<ConnectionCommunicationData>(
                    connection.DeviceID,
                    "TheBall.Interface.ExecuteRemoteCalledConnectionOperation", connectionCommunicationData);
            connectionCommunicationData.ReceivingSideConnectionID = result.ReceivingSideConnectionID;

        }

        public static void ExecuteMethod_UpdateConnectionWithCommunicationData(Connection connection, ConnectionCommunicationData connectionCommunicationData)
        {
            connection.OtherSideConnectionID = connectionCommunicationData.ReceivingSideConnectionID;
        }

        public static void ExecuteMethod_StoreObject(Connection connection)
        {
            connection.StoreInformation();
        }

        public static CreateConnectionStructuresParameters CallCreateConnectionStructures_GetParameters(Connection connection)
        {
            return new CreateConnectionStructuresParameters {ConnectionID = connection.ID};
        }
    }
}