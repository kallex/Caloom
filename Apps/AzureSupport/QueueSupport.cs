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
        public const string ErrorQueueName = "errorqueue";

        public static CloudQueue CurrDefaultQueue { get; private set; }
        public static CloudQueue CurrErrorQueue { get; private set; }
        public static CloudQueueClient CurrQueueClient { get; private set; }

        public static void InitializeAfterStorage()
        {
            CurrQueueClient = StorageSupport.CurrStorageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            CloudQueue queue = CurrQueueClient.GetQueueReference(DefaultQueueName);
            // Create the queue if it doesn't already exist
            queue.CreateIfNotExist();
            CurrDefaultQueue = queue;

            queue = CurrQueueClient.GetQueueReference(ErrorQueueName);
            // Create the queue if it doesn't already exist
            queue.CreateIfNotExist();
            CurrErrorQueue = queue;
        }

        public static void PutToDefaultQueue(QueueEnvelope queueEnvelope)
        {
            string xmlString = queueEnvelope.SerializeToXml();
            CloudQueueMessage message = new CloudQueueMessage(xmlString);
            CurrDefaultQueue.AddMessage(message);
        }

        public static QueueEnvelope GetFromDefaultQueue(out CloudQueueMessage message)
        {
            message = CurrDefaultQueue.GetMessage();
            if (message == null)
                return null;
            try
            {
                QueueEnvelope queueEnvelope = QueueEnvelope.DeserializeFromXml(message.AsString);
                return queueEnvelope;
            } catch
            {
                return null;
            }
        }

        public static void PutToErrorQueue(SystemError error)
        {
            string xmlString = error.SerializeToXml();
            CloudQueueMessage message = new CloudQueueMessage(xmlString);
            CurrErrorQueue.AddMessage(message);
        }

        public static SystemError GetFromErrorQueue(out CloudQueueMessage message)
        {
            message = CurrErrorQueue.GetMessage();
            if (message == null)
                return null;
            try
            {
                SystemError error = SystemError.DeserializeFromXml(message.AsString);
                return error;
            } catch
            {
                return null;
            }
        }
    }
}