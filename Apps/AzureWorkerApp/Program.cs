using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using AaltoGlobalImpact.OIP;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;

namespace AzureWorkerApp
{
    class Program
    {
        static void Main(string[] args)
        {
        }


        CloudQueueClient Client;
        bool IsStopped;
        private CloudQueue CurrQueue;
        private CloudTableClient CurrTable;
        private LocalResource LocalStorageResource;
        private CloudBlobContainer AnonWebContainer;

        public void Run()
        {
            while (!IsStopped)
            {
                try
                {
                    CloudQueueMessage message;
                    QueueEnvelope envelope = QueueSupport.GetFromDefaultQueue(out message);
                    if (envelope != null)
                    {
                        QueueSupport.CurrDefaultQueue.DeleteMessage(message);
                        ProcessMessage(envelope);
                    }
                    else
                    {
                        if (message != null)
                        {
                            QueueSupport.CurrDefaultQueue.DeleteMessage(message);
                            ErrorSupport.ReportMessageError(message);
                        }
                        Thread.Sleep(1000);
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
                catch (Exception ex)
                {
                    ErrorSupport.ReportException(ex);
                    throw;
                }
            }
        }

        private void ProcessMessage(QueueEnvelope envelope)
        {
            if (envelope.UpdateWebContentOperation != null)
                ProcessUpdateWebContent(envelope.UpdateWebContentOperation);
        }

        private void ProcessUpdateWebContent(UpdateWebContentOperation operation)
        {
            string sourceContainerName = operation.SourceContainerName;
            string sourcePathRoot = operation.SourcePathRoot;
            string targetContainerName = operation.TargetContainerName;
            string targetPathRoot = operation.TargetPathRoot;
            WorkerSupport.WebContentSync(sourceContainerName, sourcePathRoot, targetContainerName, targetPathRoot, RenderWebSupport.RenderingSyncHandler);
        }

        public bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;
            ServicePointManager.UseNagleAlgorithm = false;
            string connStr = null;
            const string ConnStrFileName = @"C:\work\abs\ConnectionStringStorage\theballconnstr.txt";
            if (File.Exists(ConnStrFileName))
                connStr = File.ReadAllText(ConnStrFileName);
            //else
            //    connStr = CloudConfigurationManager.GetSetting("StorageConnectionString");
            StorageSupport.InitializeWithConnectionString(connStr);
            CurrQueue = QueueSupport.CurrDefaultQueue;
            IsStopped = false;
            return true;
        }

        public void OnStop()
        {
            // Close the connection to Service Bus Queue
            IsStopped = true;
        }

    }
}
