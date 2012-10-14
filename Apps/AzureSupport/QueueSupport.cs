using System;
using System.IO;
using System.Linq;
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
        public const string StatisticQueueName = "statisticqueue";

        public static CloudQueue CurrDefaultQueue { get; private set; }
        public static CloudQueue CurrErrorQueue { get; private set; }
        public static CloudQueueClient CurrQueueClient { get; private set; }
        public static CloudQueue CurrStatisticsQueue { get; private set; }

        public static void InitializeAfterStorage(bool debugMode = false)
        {
            CurrQueueClient = StorageSupport.CurrStorageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            string dbgModePrefix = debugMode ? "dbg" : "";
            CloudQueue queue = CurrQueueClient.GetQueueReference(dbgModePrefix + DefaultQueueName);
            // Create the queue if it doesn't already exist
            queue.CreateIfNotExist();
            CurrDefaultQueue = queue;

            queue = CurrQueueClient.GetQueueReference(ErrorQueueName);
            // Create the queue if it doesn't already exist
            queue.CreateIfNotExist();
            CurrErrorQueue = queue;

            queue = CurrQueueClient.GetQueueReference(dbgModePrefix + StatisticQueueName);
            // Create the queue if it doesn't already exist
            queue.CreateIfNotExist();
            CurrStatisticsQueue = queue;
        }

        public static void ReportStatistics(string message, TimeSpan? toLive = null)
        {
            if(toLive.HasValue == false)
                toLive = TimeSpan.FromDays(7);
            CurrStatisticsQueue.AddMessage(new CloudQueueMessage(message), toLive.Value);
        }

        public static void PutToDefaultQueue(QueueEnvelope queueEnvelope)
        {
            string xmlString = queueEnvelope.SerializeToXml();
            CloudQueueMessage message = new CloudQueueMessage(xmlString);
            CurrDefaultQueue.AddMessage(message);
        }

        public static QueueEnvelope GetFromDefaultQueue(out CloudQueueMessage message)
        {
            message = CurrDefaultQueue.GetMessage(TimeSpan.FromMinutes(5));
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

        public static void PutToOperationQueue(params OperationRequest[] operationRequests)
        {
            if (operationRequests == null)
                throw new ArgumentNullException("operationRequests");
            if (operationRequests.Length == 0)
                return;
            QueueEnvelope envelope = new QueueEnvelope();
            if (operationRequests.Length == 1)
                envelope.SingleOperation = operationRequests[0];
            else
            {
                envelope.OrderDependentOperationSequence = OperationRequestCollection.CreateDefault();
                envelope.OrderDependentOperationSequence.CollectionContent.
                    AddRange(operationRequests.Where(oper => oper != null));
            }
            PutToDefaultQueue(envelope);
        }
    }
}