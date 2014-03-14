using TheBall.CORE;

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

        public static void ExecuteMethod_CallDeviceServiceForFinalizing(ConnectionCommunicationData connectionCommunicationData)
        {
            throw new System.NotImplementedException();
        }

        public static void ExecuteMethod_UpdateConnectionWithCommunicationData(Connection connection, ConnectionCommunicationData connectionCommunicationData)
        {
            connection.OtherSideConnectionID = connectionCommunicationData.ReceivingSideConnectionID;
        }

        public static void ExecuteMethod_StoreObject(Connection connection)
        {
            connection.StoreInformation();
        }
    }
}