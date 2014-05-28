using System.Collections.Generic;
using System.IO;
using TheBall.CORE;
using TheBall.Interface.INT;

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
            if(string.IsNullOrEmpty(connectionCommunicationData.ReceivingSideConnectionID))
                throw new InvalidDataException("ReceivingSideConnectionID be initialized to retrieve the connection");
            return Connection.RetrieveFromOwnerContent(Owner, connectionCommunicationData.ReceivingSideConnectionID);
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