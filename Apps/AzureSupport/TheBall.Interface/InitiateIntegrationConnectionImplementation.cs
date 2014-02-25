using TheBall.CORE;

namespace TheBall.Interface
{
    public class InitiateIntegrationConnectionImplementation
    {
        private static IContainerOwner Owner
        {
            get { return InformationContext.CurrentOwner; }
        }
        public static Connection GetTarget_Connection(string description)
        {
            Connection connection = new Connection();
            connection.SetLocationAsOwnerContent(Owner, connection.ID);
            connection.Description = description;
            return connection;
        }

        public static DeviceMembership GetTarget_DeviceForConnection(Connection connection)
        {
            throw new System.NotImplementedException();
        }

        public static void ExecuteMethod_StoreConnection(Connection connection)
        {
            throw new System.NotImplementedException();
        }

        public static void ExecuteMethod_NegotiateDeviceConnection(DeviceMembership deviceForConnection)
        {
            throw new System.NotImplementedException();
        }
    }
}