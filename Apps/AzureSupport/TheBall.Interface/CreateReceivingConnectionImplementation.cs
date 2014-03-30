using TheBall.CORE;

namespace TheBall.Interface
{
    public class CreateReceivingConnectionImplementation
    {
        private static IContainerOwner Owner
        {
            get { return InformationContext.CurrentOwner; }
        }

        public static Connection GetTarget_Connection(string description, string otherSideConnectionId)
        {
            Connection connection = new Connection();
            connection.SetLocationAsOwnerContent(Owner, connection.ID);
            connection.Description = description;
            connection.IsActiveParty = false;
            connection.OtherSideConnectionID = otherSideConnectionId;
            connection.DeviceID = InformationContext.CurrentExecutingForDevice.ID;
            return connection;
        }

        public static void ExecuteMethod_StoreConnection(Connection connection)
        {
            connection.StoreInformation();
        }

        public static CreateReceivingConnectionReturnValue Get_ReturnValue(Connection connection)
        {
            return new CreateReceivingConnectionReturnValue
                {
                    ConnectionID = connection.ID
                };
        }
    }
}