using System.Collections.Generic;
using System.IO;
using TheBall.CORE;

namespace TheBall.Interface
{
    public class CreateReceivingConnectionStructuresImplementation
    {
        private static IContainerOwner Owner
        {
            get { return InformationContext.CurrentOwner; }
        }

        public static Connection GetTarget_ThisSideConnection(ConnectionCommunicationData connectionCommunicationData)
        {
            if(string.IsNullOrEmpty(connectionCommunicationData.ReceivingSideConnectionID) == false)
                throw new InvalidDataException("ReceivingSideConnectionID must not be initialized at creation");
            Connection connection = new Connection();
            connection.SetLocationAsOwnerContent(Owner, connection.ID);
            connectionCommunicationData.ReceivingSideConnectionID = connection.ID;
            return connection;
        }


        public static CreateConnectionStructuresParameters CallCreateConnectionStructures_GetParameters(Connection thisSideConnection)
        {
            return new CreateConnectionStructuresParameters
                {
                    ConnectionID = thisSideConnection.ID
                };
        }

        public static void ExecuteMethod_StoreObject(Connection thisSideConnection)
        {
            thisSideConnection.StoreInformation();
        }
    }
}