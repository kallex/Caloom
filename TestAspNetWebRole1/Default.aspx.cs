using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace TestAspNetWebRole1
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            ConfigureQueue();
            ConfigureTableStorage();
            string tmpStore = StoreExampleData();
            SendStartupMessage(tmpStore);
        }

        private string StoreExampleData()
        {
            //var testEntity = new TmpTestEntity("Heippa!");
            var testEntity = new TestAnotherEntity("Hi There!");
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

    public class TmpTestEntity : TableServiceEntity
    {
        public TmpTestEntity(string msgData)
        {
            string key = Guid.NewGuid().ToString();
            this.PartitionKey = key;
            this.RowKey = key;
            MessageData = msgData;
        }

        public string MessageData { get; set; }
    }

    public class TestAnotherEntity : TableServiceEntity
    {
        public TestAnotherEntity(string myData)
        {
            string key = Guid.NewGuid().ToString();
            this.PartitionKey = key;
            this.RowKey = key;
            MyData = myData;
            MyDataLength = myData.Length;
        }

        public string MyData { get; set; }
        public int MyDataLength { get; set; }
    }

}
