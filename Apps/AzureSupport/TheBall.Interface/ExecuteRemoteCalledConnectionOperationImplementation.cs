using System.IO;
using AzureSupport;

namespace TheBall.Interface
{
    public class ExecuteRemoteCalledConnectionOperationImplementation
    {
        public static ConnectionCommunicationData GetTarget_ConnectionCommunicationData(Stream inputStream)
        {
            return JSONSupport.GetObjectFromStream<ConnectionCommunicationData>(inputStream);
        }

        public static void ExecuteMethod_PerformOperation(ConnectionCommunicationData connectionCommunicationData)
        {
            switch (connectionCommunicationData.ProcessRequest)
            {
                case "INITIATEREMOTECONNECTION":
                    var output = CreateReceivingConnection.Execute(new CreateReceivingConnectionParameters
                        {
                            Description = connectionCommunicationData.ProcessParametersString,
                            OtherSideConnectionID = connectionCommunicationData.ActiveSideConnectionID
                        });
                    connectionCommunicationData.ReceivingSideConnectionID = output.ConnectionID;
                    break;
                case "DELETEREMOTECONNECTION":
                    DeleteConnectionWithStructures.Execute(new DeleteConnectionWithStructuresParameters
                        {
                            ConnectionID = connectionCommunicationData.ReceivingSideConnectionID,
                            IsLaunchedByRemoteDelete = true
                        });
                    connectionCommunicationData.ReceivingSideConnectionID = null;
                    break;
                default:
                    break;
            }
        }

        public static void ExecuteMethod_SerializeCommunicationDataToOutput(Stream outputStream, ConnectionCommunicationData connectionCommunicationData)
        {
            JSONSupport.SerializeToJSONStream(connectionCommunicationData, outputStream);
        }
    }
}