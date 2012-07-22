using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace CaloomWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        // The name of your queue
        const string QueueName = "immediatequeue";

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        CloudQueueClient Client;
        bool IsStopped;
        private CloudQueue CurrQueue;
        private string TmpTableName = "tmptable";
        private CloudTableClient CurrTable;

        public override void Run()
        {
            while (!IsStopped)
            {
                try
                {
                    // Receive the message
                    CloudQueueMessage receivedMessage = null;
                    receivedMessage = CurrQueue.GetMessage();
                        //Client.Receive();)

                    if (receivedMessage != null)
                    {
                        // Process the message
                        Trace.WriteLine("Processing", receivedMessage.Id.ToString());
                        string key = receivedMessage.AsString;
                        CurrQueue.DeleteMessage(receivedMessage);
                        TableServiceContext ctx = CurrTable.GetDataServiceContext();
                        TmpTestEntity entity =
                            ctx.CreateQuery<TmpTestEntity>(TmpTableName).Where(
                                tt => tt.PartitionKey == key && tt.RowKey == key).FirstOrDefault();

                    }
                }
                catch (MessagingException e)
                {
                    if (!e.IsTransient)
                    {
                        Trace.WriteLine(e.Message);
                        throw;
                    }

                    Thread.Sleep(10000);
                }
                catch (OperationCanceledException e)
                {
                    if (!IsStopped)
                    {
                        Trace.WriteLine(e.Message);
                        throw;
                    }
                }
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            ConfigureQueue();
            ConfigureTableStorage();
            IsStopped = false;
            return base.OnStart();
        }

        private void ConfigureTableStorage()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            tableClient.CreateTableIfNotExist(TmpTableName);
            CurrTable = tableClient;
        }

        private void ConfigureQueue()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the queue client
            Client = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            CloudQueue queue = Client.GetQueueReference(QueueName);
            CurrQueue = queue;
            // Create the queue if it doesn't already exist
            queue.CreateIfNotExist();
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            IsStopped = true;
            base.OnStop();
        }
    }
    public class TmpTestEntity : TableServiceEntity
    {
        public TmpTestEntity()
        {}

        public TmpTestEntity(string msgData)
        {
            string key = Guid.NewGuid().ToString();
            this.PartitionKey = key;
            this.RowKey = key;
        }

        public string MessageData { get; set; }
    }

}
