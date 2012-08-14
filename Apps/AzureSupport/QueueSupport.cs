using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure.StorageClient;

namespace TheBall
{
    public static class QueueSupport
    {
        public const string DefaultQueueName = "defaultqueue";

        public static CloudQueue CurrDefaultQueue { get; private set; }
        public static CloudQueueClient CurrQueueClient { get; private set; }

        public static void InitializeAfterStorage()
        {
            CurrQueueClient = StorageSupport.CurrStorageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            CloudQueue queue = CurrQueueClient.GetQueueReference(DefaultQueueName);
            CurrDefaultQueue = queue;
            // Create the queue if it doesn't already exist
            queue.CreateIfNotExist();
        }

        public static void PutToDefaultQueue(QueueEnvelope queueEnvelope)
        {
            string xmlString = SerializeToXml(queueEnvelope);
            CloudQueueMessage message = new CloudQueueMessage(xmlString);
            CurrDefaultQueue.AddMessage(message);
        }

        public static QueueEnvelope GetFromDefaultQueue()
        {
            CloudQueueMessage message = CurrDefaultQueue.GetMessage();
            if (message == null)
                return null;
            QueueEnvelope queueEnvelope = DeserializeFromXml(message.AsString);
            CurrDefaultQueue.DeleteMessage(message);
            return queueEnvelope;
        }

        private static string SerializeToXml(QueueEnvelope queueEnvelope)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(QueueEnvelope));
            using (var output = new StringWriter())
            {
                using (var writer = new XmlTextWriter(output) {Formatting = Formatting.Indented})
                {
                    serializer.WriteObject(writer, queueEnvelope);
                }
                return output.GetStringBuilder().ToString();
            }
        }

        private static QueueEnvelope DeserializeFromXml(string xmlData)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(QueueEnvelope));
            using(StringReader reader = new StringReader(xmlData))
            {
                using (var xmlReader = new XmlTextReader(reader))
                    return (QueueEnvelope) serializer.ReadObject(xmlReader);
            }
            
        }
    }
}