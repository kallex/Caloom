using System.IO;
using AzureSupport;

namespace TheBall.Interface
{
    public class PerformConnectionOperationImplementation
    {
        public static ConnectionCommunicationData GetTarget_ConnectionCommunicationData(Stream inputStream)
        {
            return JSONSupport.GetObjectFromStream<ConnectionCommunicationData>(inputStream);
        }

        public static void ExecuteMethod_PerformOperation(ConnectionCommunicationData connectionCommunicationData)
        {
            switch (connectionCommunicationData.ProcessRequest)
            {
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