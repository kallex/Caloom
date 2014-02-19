using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;
using TheBall.CORE;
using TheBall.Index;

namespace CaloomWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private const int PollCyclePerContainerMilliseconds = 1000;

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        CloudQueueClient Client;
        bool IsStopped;
        bool GracefullyStopped;
        private CloudQueue CurrQueue;
        private CloudTableClient CurrTable;
        private LocalResource LocalStorageResource;
        private CloudBlobContainer AnonWebContainer;
        private bool IsIndexingMaster = false;
        private AttemptToBecomeInfrastructureIndexerReturnValue IndexerInfo;

        private string[] ActiveContainerNames;

        private void stepActiveContainerIX(ref int activeContainerIX)
        {
            activeContainerIX++;
            activeContainerIX %= ActiveContainerNames.Length;
        }

        public class TaskCommunicatingObject
        {
            public bool IsExecutingIndexing;
        }

        public static TaskCommunicatingObject TaskCommObj = new TaskCommunicatingObject();

        public override void Run()
        {
            GracefullyStopped = false;
            //ThreadPool.SetMinThreads(3, 3);
            Task[] tasks = new Task[]
                               {
                                   Task.Factory.StartNew(() => {}), 
                                   Task.Factory.StartNew(() => {}), 
                                   Task.Factory.StartNew(() => {}), 
                                   //Task.Factory.StartNew(() => {}), 
                                   //Task.Factory.StartNew(() => {}), 
                                   //Task.Factory.StartNew(() => {}), 
                               };
            QueueSupport.ReportStatistics("Starting worker: " + CurrWorkerID, TimeSpan.FromDays(1));
            int activeContainerIX = 0;
            int PollCyclePerRound = PollCyclePerContainerMilliseconds/ActiveContainerNames.Length;
            while (!IsStopped)
            {
                try
                {
                    Task.WaitAny(tasks);
                    if (IsStopped)
                        break;
                    int availableIx;
                    Task availableTask = WorkerSupport.GetFirstCompleted(tasks, out availableIx);

                    if (IsIndexingMaster && TaskCommObj.IsExecutingIndexing == false)
                    {
                        lock (TaskCommObj)
                        {
                            TaskCommObj.IsExecutingIndexing = true;
                            bool handledIndexing = PollAndHandleIndexingMessages(tasks, availableIx, availableTask);
                            if (handledIndexing)
                            {
                                Thread.Sleep(PollCyclePerRound);
                                continue;
                            }
                            else // If task wasn't started, we set indexing to false and allow flow to continue
                            {
                                TaskCommObj.IsExecutingIndexing = false;
                            }
                        }
                    }

                    stepActiveContainerIX(ref activeContainerIX);
                    string activeContainerName = ActiveContainerNames[activeContainerIX];
                    InformationContext.Current.InitializeCloudStorageAccess(activeContainerName, true);
                    bool handledSubscriptionChain = PollAndHandleSubscriptionChain(tasks, availableIx, availableTask, activeContainerName);
                    if (handledSubscriptionChain)
                    {
                        // TODO: Fix return value check
                        Thread.Sleep(PollCyclePerRound);
                        continue;
                    }
                    bool handledMessage = PollAndHandleMessage(tasks, availableIx, availableTask);
                    if (handledMessage)
                        continue;
                    Thread.Sleep(PollCyclePerRound);
                }
                catch (AggregateException ae)
                {
                    foreach (var e in ae.Flatten().InnerExceptions)
                    {
                        ErrorSupport.ReportException(e);
                    }
                    Thread.Sleep(10000);
                    // or ...
                    // ae.Flatten().Handle((ex) => ex is MyCustomException);
                }
                    /*
                catch (MessagingException e)
                {
                    if (!e.IsTransient)
                    {
                        Trace.WriteLine(e.Message);
                        throw;
                    }
                    Thread.Sleep(10000);
                }*/
                catch (OperationCanceledException e)
                {
                    if (!IsStopped)
                    {
                        Trace.WriteLine(e.Message);
                        throw;
                    }
                } 
                catch(Exception ex)
                {
                    ErrorSupport.ReportException(ex);
                    throw;
                }
            }
            Task.WaitAll(tasks);
            foreach (var task in tasks.Where(task => task.Exception != null))
            {
                ErrorSupport.ReportException(task.Exception);
            }
            releaseIndexingMasterResourcesForMaster();
            QueueSupport.ReportStatistics("Stopped: " + CurrWorkerID, TimeSpan.FromDays(1));
            GracefullyStopped = true;
        }

        private bool PollAndHandleSubscriptionChain(Task[] tasks, int availableIx, Task availableTask, string activeContainerName)
        {
            var result = SubscribeSupport.GetOwnerChainsInOrderOfSubmission();
            if (result.Length == 0)
                return false;
            string acquiredEtag = null;
            var firstLockedOwner =
                result.FirstOrDefault(
                    lockCandidate => SubscribeSupport.AcquireChainLock(lockCandidate, out acquiredEtag));
            if (firstLockedOwner == null)
                return false;
            var executing = Task.Factory.StartNew(() => WorkerSupport.ProcessOwnerSubscriptionChains(firstLockedOwner, acquiredEtag, activeContainerName));
            tasks[availableIx] = executing;
            if (availableTask.Exception != null)
                ErrorSupport.ReportException(availableTask.Exception);
            return true;
        }

        private bool PollAndHandleIndexingMessages(Task[] tasks, int availableIx, Task availableTask)
        {
            var indexingMessages = IndexSupport.GetIndexingRequestsFromQueue(IndexSupport.DefaultIndexName);
            var queryMessages = IndexSupport.GetQueryRequestsFromQueue(IndexSupport.DefaultIndexName);
            if (indexingMessages.Length > 0 || queryMessages.Length > 0)
            {
                // TODO: Task the stuff
                Task executing = Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            WorkerSupport.ProcessIndexing(indexingMessages, IndexerInfo.CloudDrive.LocalPath);
                            WorkerSupport.ProcessQueries(queryMessages, IndexerInfo.CloudDrive.LocalPath);
                        }
                        finally
                        {
                            lock (TaskCommObj)
                            {
                                TaskCommObj.IsExecutingIndexing = false;
                            }
                        }
                    });
                tasks[availableIx] = executing;
                if(availableTask.Exception != null)
                    ErrorSupport.ReportException(availableTask.Exception);
                return true;
            }
            else
                return false;
        }


        private bool PollAndHandleMessage(Task[] tasks, int availableIx, Task availableTask)
        {
            CloudQueueMessage message;
            QueueEnvelope envelope = QueueSupport.GetFromDefaultQueue(out message);
            if (envelope != null)
            {
                Task executing = Task.Factory.StartNew(() => WorkerSupport.ProcessMessage(envelope));
                tasks[availableIx] = executing;
                QueueSupport.CurrDefaultQueue.DeleteMessage(message);
                if (availableTask.Exception != null)
                    ErrorSupport.ReportException(availableTask.Exception);
                return true;
            }
            else 
            {
                if(message != null)
                {
                    QueueSupport.CurrDefaultQueue.DeleteMessage(message);
                    ErrorSupport.ReportMessageError(message);
                }
                GC.Collect();
                return false;
            }
        }

        protected string CurrWorkerID { get; private set; }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            CurrWorkerID = DateTime.Now.ToString();
            ServicePointManager.DefaultConnectionLimit = 12;
            ServicePointManager.UseNagleAlgorithm = false;
            string connStr = InstanceConfiguration.AzureStorageConnectionString;
            StorageSupport.InitializeWithConnectionString(connStr);
            InformationContext.InitializeFunctionality(3, allowStatic:true);
            ActiveContainerNames = InstanceConfiguration.WorkerActiveContainerName.Split(',');
            //InformationContext.Current.InitializeCloudStorageAccess(InstanceConfiguration.WorkerActiveContainerName);
            CurrQueue = QueueSupport.CurrDefaultQueue;
            tryToBecomeIndexingMaster();
            IsStopped = false;
            return base.OnStart();
        }

        private void releaseIndexingMasterResourcesForMaster()
        {
            if (IsIndexingMaster)
            {
                QueueSupport.ReportStatistics("Unmounting drive... " + RoleEnvironment.CurrentRoleInstance.Id);
                ReleaseIndexerResources.Execute(new ReleaseIndexerResourcesParameters
                {
                    ResourceInfo = IndexerInfo
                });
                QueueSupport.ReportStatistics("Unmounted succesfully... " + RoleEnvironment.CurrentRoleInstance.Id);
            }
        }

        private void tryToBecomeIndexingMaster()
        {
            QueueSupport.ReportStatistics("Trying to become indexing master: " + RoleEnvironment.CurrentRoleInstance.Id);
            AttemptToBecomeInfrastructureIndexerParameters parameters =
                new AttemptToBecomeInfrastructureIndexerParameters
                    {
                        IndexName = "defaultindex"
                    };
            var result = AttemptToBecomeInfrastructureIndexer.Execute(parameters);
            IndexerInfo = result;
            IsIndexingMaster = result.Success;
            QueueSupport.ReportStatistics("Trying result: " + IsIndexingMaster + " from  " + RoleEnvironment.CurrentRoleInstance.Id);
            if (!IsIndexingMaster)
            {
                QueueSupport.ReportStatistics("Error: " + result.Exception.ToString());
            }
            else
            {
                var driveLetter = result.CloudDrive.LocalPath.Substring(0, 1);
                DriveInfo driveInfo = new DriveInfo(driveLetter);
                var availableMegabytes = driveInfo.AvailableFreeSpace/(1024*1024);
                QueueSupport.ReportStatistics("Success: " + result.CloudDrive.LocalPath + " available: " + availableMegabytes + "MB");
            }
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            QueueSupport.ReportStatistics("Stopping: " + CurrWorkerID, TimeSpan.FromDays(1));
            IsStopped = true;
            while(GracefullyStopped == false)
                Thread.Sleep(1000);
            base.OnStop();
        }
    }
}
