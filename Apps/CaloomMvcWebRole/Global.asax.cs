using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace CaloomMvcWebRole
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            ConfigureQueue();
            ConfigureTableStorage();
            string tmpStore = StoreExampleData();
            SendStartupMessage(tmpStore);
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
}