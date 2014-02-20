using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using AaltoGlobalImpact.OIP;
using AzureSupport;
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
        public static ConcurrentDictionary<string, CloudQueue> Queues = new ConcurrentDictionary<string, CloudQueue>();

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

        public static CloudQueue RegisterQueue(string queueName, bool createIfNotExist = true)
        {
            var queue = CurrQueueClient.GetQueueReference(queueName);
            bool addResult = Queues.TryAdd(queueName, queue);
            if(addResult == false)
                throw new InvalidOperationException("Cannot add already existing queue: " + queueName);
            if (createIfNotExist)
                queue.CreateIfNotExist();
            return queue;
        }

        public static CloudQueue UnregisterQueue(string queueName)
        {
            CloudQueue queue;
            bool removeResult = Queues.TryRemove(queueName, out queue);
            if(removeResult == false)
                throw new InvalidOperationException("Cannot remove non-existent queue: " + queueName);
            return queue;
        }

        public static CloudQueue GetQueue(string queueName)
        {
            CloudQueue queue;
            var getResult = Queues.TryGetValue(queueName, out queue);
            if(getResult == false)
                throw new InvalidOperationException("No queue with name registered: " + queueName);
            return queue;
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
            envelope.ActiveContainerName = StorageSupport.CurrActiveContainer.Name;
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

        public static void WriteObjectAsJSONToQueue<T>(string queueName, T objectToWrite)
        {
            var queue = GetQueue(queueName);
            var jsonString = JSONSupport.SerializeToJSONString(objectToWrite);
            CloudQueueMessage message = new CloudQueueMessage(jsonString);
            queue.AddMessage(message);
        }



        public static void GetJSONObjectsFromQueue<T>(string queueName, out MessageObject<T>[] messageObjects, int maxMessagesToRetrieve)
        {
            if (maxMessagesToRetrieve > 32)
                throw new ArgumentException("Max messages to retrieve is 32", "maxMessagesToRetrieve");
            var queue = GetQueue(queueName);
            var messages = queue.GetMessages(maxMessagesToRetrieve);
            List<MessageObject<T>> resultData = new List<MessageObject<T>>();
            foreach (var message in messages)
            {
                var jsonString = message.AsString;
                var contentObject = JSONSupport.GetObjectFromString<T>(jsonString);
                MessageObject<T> messageObject = new MessageObject<T>
                    {
                        Message = message,
                        RetrievedObject = contentObject
                    };
                resultData.Add(messageObject);
            }
            messageObjects = resultData.ToArray();
        }

        public class MessageObject<T>
        {
            public CloudQueueMessage Message;
            public T RetrievedObject;
        }

        public static void PutMessageToQueue(string queueName, string messageText)
        {
            var queue = GetQueue(queueName);
            CloudQueueMessage message = new CloudQueueMessage(messageText);
            queue.AddMessage(message);
        }

        public static void GetMessagesFromQueue(string queueName, out MessageObject<string>[] messageObjects, int maxCount = 32, bool deleteOnRetrieval = true)
        {
            var queue = GetQueue(queueName);
            //List<CloudQueueMessage> messageList = new List<CloudQueueMessage>();
            var messages = queue.GetMessages(maxCount);
            //messageList.AddRange(messages);
            messageObjects = messages.Select(msg => new MessageObject<string> { Message = msg, RetrievedObject = msg.AsString }).ToArray();
            if (deleteOnRetrieval)
            {
                foreach(var msgObj in messageObjects)
                    queue.DeleteMessage(msgObj.Message);
            }
        }
    }
}