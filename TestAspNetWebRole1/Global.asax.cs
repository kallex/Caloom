using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace TestAspNetWebRole1
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //ConfigureQueue();
            //ConfigureTableStorage();
            //string tmpStore = StoreExampleData();
            //SendStartupMessage(tmpStore);
        }

        private string StoreExampleData()
        {
            TmpTestEntity testEntity = new TmpTestEntity("Heippa!");
            TableServiceContext ctx = CurrTable.GetDataServiceContext();
            ctx.AddObject(TmpTableName, testEntity);
            var response = ctx.SaveChangesWithRetries();
            return testEntity.PartitionKey;
        }

        private void ConfigureTableStorage()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            tableClient.CreateTableIfNotExist(TmpTableName);
            CurrTable = tableClient;
        }

        private void SendStartupMessage(string tmpStore)
        {
            CloudQueueMessage message = new CloudQueueMessage(tmpStore);
            CurrQueue.AddMessage(message);
        }

        private CloudQueueClient Client;
        private CloudQueue CurrQueue;
        private string TmpTableName = "tmptable";
        private CloudTableClient CurrTable;
        private const string QueueName = "immediatequeue";

        private void ConfigureQueue()
        {
            // Configure Queue Settings
            ServicePointManager.DefaultConnectionLimit = 12;

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
    }

    public class TmpTestEntity22 : TableServiceEntity
    {
        public TmpTestEntity22(string msgData)
        {
            string key = Guid.NewGuid().ToString();
            this.PartitionKey = key;
            this.RowKey = key;
            MessageData = msgData;
        }

        public string MessageData { get; set; }
    }
}
