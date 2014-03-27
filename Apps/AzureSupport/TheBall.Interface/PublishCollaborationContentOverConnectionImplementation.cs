using TheBall.CORE;

namespace TheBall.Interface
{
    public class PublishCollaborationContentOverConnectionImplementation
    {
        public static Connection GetTarget_Connection(string connectionId)
        {
            return Connection.RetrieveFromOwnerContent(InformationContext.CurrentOwner, connectionId);
        }

        public static void ExecuteMethod_UpdateOtherSideMD5List(Connection connection)
        {
            ConnectionCommunicationData connectionCommunication = new ConnectionCommunicationData
                {
                    ActiveSideConnectionID = connection.ID,
                    ProcessRequest = "SYNCHRONIZEMD5LIST",
                    ReceivingSideConnectionID = connection.OtherSideConnectionID
                };
            connectionCommunication = DeviceSupport.ExecuteRemoteOperation<ConnectionCommunicationData>(connection.DeviceID,
                                                                              "TheBall.Interface.ExecuteRemoteCalledConnectionOperation", connectionCommunication);
            connection.OtherSideMD5List.Clear();
            connection.OtherSideMD5List.AddRange(connectionCommunication.ProcessResultArray);
        }

        public static void ExecuteMethod_StoreObject(Connection connection)
        {
            connection.StoreInformation();
        }

        public static PackageAndPushCollaborationContentParameters CallPackageAndPushCollaborationContent_GetParameters(string connectionId)
        {
            return new PackageAndPushCollaborationContentParameters
                {
                    ConnectionID = connectionId
                };
        }

        public static void ExecuteMethod_CallOtherSideProcessingForPushedContent(Connection connection)
        {
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

        public static void ExecuteMethod_UpdateThisSideMD5List(Connection connection)
        {
        }
    }
}