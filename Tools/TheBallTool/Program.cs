using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;

namespace TheBallTool
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string connStr = String.Format("DefaultEndpointsProtocol=http;AccountName=theball;AccountKey={0}",
                                               args[0]);
                bool debugMode = false;


                InformationContext.InitializeFunctionality(3, true);
                StorageSupport.InitializeWithConnectionString(connStr, debugMode);

                if(DataPatcher.DoPatching())
                    return;

                //ProcessErrors(false);
                //return;

                string templateLocation = "livetemplate";
                string privateSiteLocation = "livesite";
                string publicSiteLocation = "livepubsite";
                const string accountNamePart = "oip-account\\";
                const string publicNamePart = "oip-public\\";
                const string groupNamePart = "oip-group\\";
                //DoMapData(webGroup);
                //return;
                string directory = Directory.GetCurrentDirectory();
                if (directory.EndsWith("\\") == false)
                    directory = directory + "\\";
                string[] allFiles =
                    Directory.GetFiles(directory, "*", SearchOption.AllDirectories).Select(
                        str => str.Substring(directory.Length)).Where(str => str.StartsWith("theball-") == false).ToArray();
                string[] groupTemplates =
                    allFiles.Where(file => file.StartsWith(accountNamePart) == false && file.StartsWith(publicNamePart) == false).
                        ToArray();
                string[] publicTemplates =
                    allFiles.Where(file => file.StartsWith(accountNamePart) == false && file.StartsWith(groupNamePart) == false).
                        ToArray();
                string[] accountTemplates =
                    allFiles.Where(file => file.StartsWith(groupNamePart) == false && file.StartsWith(publicNamePart) == false).
                        ToArray();
                UploadAndMoveUnused(accountTemplates, groupTemplates, publicTemplates);
                //UploadAndMoveUnused(accountTemplates, null, null);

                //DeleteAllAccountAndGroupContents(true);
                //RefreshAllAccounts();

                // TODO: The delete above needs to go through first before the refresh one below

                RenderWebSupport.RefreshAllAccountAndGroupTemplates(true, "AaltoGlobalImpact.OIP.Blog", "AaltoGlobalImpact.OIP.Activity", "AaltoGlobalImpact.OIP.AddressAndLocation",
                    "AaltoGlobalImpact.OIP.Image", "AaltoGlobalImpact.OIP.ImageGroup");

                //RunTaskedQueueWorker();


                //FileSystemSupport.UploadTemplateContent(groupTemplates, webGroup, templateLocation, true);
                Console.WriteLine("Starting to sync...");
                //DoSyncs(templateLocation, privateSiteLocation, publicSiteLocation);
                //"grp/default/pub/", true);
                return;
                //doDataTest(connStr);
                //InitLandingPages();
                //Console.WriteLine("Press enter to continue...");
                //Console.ReadLine();
            } 
                catch(InvalidDataException ex)
            {
                Console.WriteLine("Error exit: " + ex.ToString());
            }
        }

        private static void testContentFetch()
        {
            string requesterPath = "grp/89706487-7dc0-4711-911e-17dd0207a000/website/oip-group/oip-layout-blog-summary.phtml";
            VirtualOwner owner = VirtualOwner.FigureOwner(requesterPath);
            //var blogs = Blog.RetrieveCollectionFromOwnerContent(owner);
            DefaultViewSupport.RefreshDefaultViews(requesterPath, typeof(Blog));
        }

        private static void DebugSomething()
        {
            CloudBlob blob =
                StorageSupport.CurrActiveContainer.GetBlob(
                    "grp/89706487-7dc0-4711-911e-17dd0207a000/website/oip-group/oip-layout-activities-summary.phtml");
            RenderWebSupport.RefreshContent(blob, false);
        }

        private static void RefreshAllAccounts()
        {
            var accountIDs = TBRAccountRoot.GetAllAccountIDs();
            foreach(var accountID in accountIDs)
            {
                var accountRoot = TBRAccountRoot.RetrieveFromDefaultLocation(accountID);
                accountRoot.Account.StoreAndPropagate();
            }

        }

        private static void ProcessErrors(bool useWorker)
        {
            if (useWorker)
            {
                List<QueueEnvelope> envelopes = new List<QueueEnvelope>();
                List<CloudQueueMessage> messages = new List<CloudQueueMessage>();
                CloudQueueMessage message = null;
                QueueEnvelope envelope = ErrorSupport.RetrieveRetryableEnvelope(out message);
                while (envelope != null)
                {
                    //WorkerSupport.ProcessMessage(envelope, false);
                    //QueueSupport.CurrErrorQueue.DeleteMessage(message);
                    messages.Add(message);
                    envelope.CurrentRetryCount++;
                    envelopes.Add(envelope);
                    envelope = ErrorSupport.RetrieveRetryableEnvelope(out message);
                }
                envelopes.ForEach(QueueSupport.PutToDefaultQueue);
                messages.ForEach(msg => QueueSupport.CurrErrorQueue.DeleteMessage(msg));
            }
            else
            {
                CloudQueueMessage message = null;
                QueueEnvelope envelope = ErrorSupport.RetrieveRetryableEnvelope(out message);
                while (envelope != null)
                {
                    WorkerSupport.ProcessMessage(envelope, false);
                    QueueSupport.CurrErrorQueue.DeleteMessage(message);
                    envelope = ErrorSupport.RetrieveRetryableEnvelope(out message);
                }
            }
        }

        private static OperationRequest PushTestQueue()
        {
            OperationRequest operationRequest = new OperationRequest
                                            {
                                                SubscriberNotification = new Subscription
                                                                             {
                                                                                 //SubscriberRelativeLocation =
                                                                                 //    "acc/17e18f1d-c5bd-4955-af5a-a62d1092710a/website/oip-account/oip-layout-account-welcome.phtml",
                                                                                 SubscriberRelativeLocation =
                                                                                 "grp/fd686fbd-deb5-4631-899d-5da4314c27b8/website/oip-group/oip-layout-blog-summary.phtml",
                                                                                 SubscriptionType =
                                                                                     SubscribeSupport.
                                                                                     SubscribeType_WebPageToSource
                                                                             }
                                            };
            return operationRequest;
        }

        private static void RunTaskedQueueWorker()
        {
            Task[] tasks = new Task[]
                               {
                                   Task.Factory.StartNew(() => {}), 
                                   Task.Factory.StartNew(() => {}), 
                                   Task.Factory.StartNew(() => {}), 
                                   //Task.Factory.StartNew(() => {}), 
                                   //Task.Factory.StartNew(() => {}), 
                               };
            bool IsStopped = false;
            int initialCount = 0;
            while (true)
            {
                try
                {
                    Task.WaitAny(tasks);
                    if (IsStopped)
                    {
                        Task.WaitAll(tasks);
                        foreach(var task in tasks)
                        {
                            if(task.Exception != null)
                                ErrorSupport.ReportException(task.Exception);
                        }
                        break;
                    }
                    int availableIx;
                    Task availableTask = WorkerSupport.GetFirstCompleted(tasks, out availableIx);
                    CloudQueueMessage message;
                    QueueEnvelope envelope = QueueSupport.GetFromDefaultQueue(out message);
                    if (envelope != null)
                    {
                        Task executing = Task.Factory.StartNew(() => WorkerSupport.ProcessMessage(envelope));
                        tasks[availableIx] = executing;
                        QueueSupport.CurrDefaultQueue.DeleteMessage(message);
                        if (availableTask.Exception != null)
                            ErrorSupport.ReportException(availableTask.Exception);
                    }
                    else 
                    {
                        if(message != null)
                        {
                            QueueSupport.CurrDefaultQueue.DeleteMessage(message);
                            ErrorSupport.ReportMessageError(message);
                        }
                        Thread.Sleep(1000);
                    }
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
                catch(Exception ex)
                {
                    ErrorSupport.ReportException(ex);
                    throw;
                }
            }
        }

        private static void RunQueueWorker(QueueEnvelope givenEnvelope)
        {
            bool loop = true;
            while (loop)
            {
                loop = givenEnvelope == null; 
                QueueEnvelope envelope;
                if (givenEnvelope == null)
                {

                    CloudQueueMessage message = null;
                    envelope = QueueSupport.GetFromDefaultQueue(out message);
                    QueueSupport.CurrDefaultQueue.DeleteMessage(message);
                }
                else
                {
                    envelope = givenEnvelope;
                }
                try
                {
                    WorkerSupport.ProcessMessage(envelope);
                } catch(Exception ex)
                {
                    string msg = ex.Message;
                }
                //Thread.Sleep(5000);
            }
        }

        private static void UploadAndMoveUnused(string[] accountTemplates, string[] groupTemplates, string[] publicTemplates)
        {
            string[] accountUnusedFiles = null;
            if(accountTemplates != null)
                accountUnusedFiles = FileSystemSupport.UploadTemplateContent(accountTemplates, TBSystem.CurrSystem, RenderWebSupport.DefaultAccountTemplates, true);
            string[] groupUnusedFiles = null;
            if(groupTemplates != null)
                groupUnusedFiles = FileSystemSupport.UploadTemplateContent(groupTemplates, TBSystem.CurrSystem, RenderWebSupport.DefaultGroupTemplates, true);
            string[] publicUnusedFiles = null;
            if(publicTemplates != null)
                publicUnusedFiles = FileSystemSupport.UploadTemplateContent(publicTemplates, TBSystem.CurrSystem, RenderWebSupport.DefaultPublicTemplates, true);
            if(accountTemplates != null && groupTemplates != null && publicTemplates != null)
            {
                string[] everyWhereUnusedFiles =
                    accountUnusedFiles.Intersect(groupUnusedFiles).Intersect(publicUnusedFiles).ToArray();
                //FileSystemSupport.MoveUnusedTxtFiles(everyWhereUnusedFiles);
            }
        }

        private static void DeleteAllAccountAndGroupContents(bool useWorker)
        {
            var accountIDs = TBRAccountRoot.GetAllAccountIDs();
            var groupIDs = TBRGroupRoot.GetAllGroupIDs();
            List<string> referenceLocations = new List<string>();
            foreach (var accountID in accountIDs)
            {
                string referenceLocation = "acc/" + accountID + "/";
                referenceLocations.Add(referenceLocation);
            }
            foreach (var groupID in groupIDs)
            {
                string referenceLocation = "grp/" + groupID + "/";
                referenceLocations.Add(referenceLocation);
            }
            if(useWorker)
            {
                referenceLocations.ForEach(refLoc =>
                                               {
                                                   VirtualOwner owner = VirtualOwner.FigureOwner(refLoc);
                                                   QueueSupport.PutToOperationQueue(
                                                       new OperationRequest
                                                           {
                                                               DeleteOwnerContent = new DeleteOwnerContentOperation
                                                                                        {
                                                                                            ContainerName = owner.ContainerName,
                                                                                            LocationPrefix = owner.LocationPrefix
                                                                                        }
                                                           }
                                                       );
                                               });                
            }
            else
            {
                referenceLocations.ForEach(refLoc => StorageSupport.DeleteContentsFromOwner(refLoc));
            }
        }

        private static void TestEmail()
        {
            EmailSupport.SendEmail("no-reply-theball@msunit.citrus.fi", "kalle.launiala@citrus.fi", "The Ball - Says Hello!",
            "Text testing...");
        }

        private static void DoMapData(IContainerOwner owner)
        {
            MapContainer mapContainer =
                MapContainer.RetrieveMapContainer(
                    "livesite/oip-layouts/oip-layout-default-view.phtml/AaltoGlobalImpact.OIP/MapContainer/38b16ead-5851-484f-a367-bb215eb8e490",
                    owner);
            MapMarker marker1 = MapMarker.CreateDefault();
            marker1.LocationText = "8446198.6713314,2759433.3836466";
            MapMarker marker2 = MapMarker.CreateDefault();
            marker2.LocationText = "10000,10000";
            MapMarker marker3 = MapMarker.CreateDefault();
            marker3.LocationText = "0,0";
            //mapContainer.MapMarkers = MapMarkerCollection.CreateDefault();
            mapContainer.MapMarkers.CollectionContent.Add(marker1);
            mapContainer.MapMarkers.CollectionContent.Add(marker2);
            mapContainer.MapMarkers.CollectionContent.Add(marker3);
            StorageSupport.StoreInformation(mapContainer, owner);
        }

        static void DoSyncs(string templateLocation, string privateSiteLocation, string publicSiteLocation)
        {
           RenderWebSupport.SyncTemplatesToSite(StorageSupport.CurrActiveContainer.Name,
                String.Format("grp/f8e1d8c6-0000-467e-b487-74be4ad099cd/{0}/", templateLocation),
                StorageSupport.CurrActiveContainer.Name,
                                String.Format("grp/f8e1d8c6-0000-467e-b487-74be4ad099cd/{0}/", privateSiteLocation), false, true);
           RenderWebSupport.SyncTemplatesToSite(StorageSupport.CurrActiveContainer.Name,
                String.Format("grp/f8e1d8c6-0000-467e-b487-74be4ad099cd/{0}/", privateSiteLocation),
                StorageSupport.CurrAnonPublicContainer.Name,
                                String.Format("grp/default/{0}/", publicSiteLocation), true, true);
        }

        private static void AddLoginToAccount(string loginUrlID, string accountID)
        {
            TBRAccountRoot accountRoot = TBRAccountRoot.RetrieveFromDefaultLocation(accountID);

            TBLoginInfo loginInfo = TBLoginInfo.CreateDefault();
            loginInfo.OpenIDUrl = loginUrlID;

            accountRoot.Account.Logins.CollectionContent.Add(loginInfo);
            accountRoot.Account.StoreAndPropagate();
        }

        private static void TestDriveDynamicCreation()
        {
            object test = RenderWebSupport.GetOrInitiateContentObject(new List<RenderWebSupport.ContentItem>(),
                                                                      "AaltoGlobalImpact.OIP.InformationSource",
                                                                      "vilperi", false);
        }

        private const string FixedGroupID = "05DF28FD-58A7-46A7-9830-DA3F51AAF6AF";

        private static TBCollaboratingGroup InitializeDefaultOIPWebGroup()
        {
            TBRGroupRoot groupRoot = TBRGroupRoot.RetrieveFromDefaultLocation(FixedGroupID);
            if(groupRoot == null)
            {
                groupRoot = TBRGroupRoot.CreateDefault();
                groupRoot.ID = FixedGroupID;
                groupRoot.UpdateRelativeLocationFromID();
                groupRoot.Group.JoinToGroup("kalle.launiala@citrus.fi", "moderator");
                groupRoot.Group.JoinToGroup("jeroen@caloom.com", "moderator");
                StorageSupport.StoreInformation(groupRoot);
            }
            return groupRoot.Group;
        }

        private static void InitLandingPages()
        {
            var sourceBlob = StorageSupport.CurrTemplateContainer.GetBlobReference("oip-layouts/oip-anon-landing-page.phtml");
            var targetBlob =
                StorageSupport.CurrAnonPublicContainer.GetBlobReference("default/oip-anon-landing-page.phtml");
            targetBlob.CopyFromBlob(sourceBlob);
        }

        private static void doDataTest(string connStr)
        {
            StorageSupport.InitializeWithConnectionString(connStr);
            AccountContainer container = null;
            try
            {
                container = AccountContainer.RetrieveAccountContainer("AcctContainer123");
            }
            catch (StorageException sEx)
            {

            }
            if (container == null)
            {
                container = AccountContainer.CreateDefault();
                container.ID = "AC123";
                container.RelativeLocation = "AcctContainer123";
                container.AccountIndex.Title = "Account demo index";
                container.AccountIndex.Introduction = "Account introduction";
                container.AccountIndex.Summary = "Account demo summary";
                var memberColl = container.AccountModule.Roles.MemberInGroups.CollectionContent;
                memberColl.Add(new ReferenceToInformation { Title = "The Ball Test Yle.fi", URL = "http://www.yle.fi" });
                memberColl.Add(new ReferenceToInformation { Title = "The Ball Test Aalto.fi", URL = "http://www.aalto.fi" });
                var moderatorColl = container.AccountModule.Roles.ModeratorInGroups.CollectionContent;
                moderatorColl.Add(new ReferenceToInformation { Title = "The Ball Test Yle.fi 2", URL = "http://www.yle.fi" });
                moderatorColl.Add(new ReferenceToInformation { Title = "The Ball Test Aalto.fi 2", URL = "http://www.aalto.fi" });
                StorageSupport.StoreInformation(container);
            }

            CloudBlobClient publicClient = new CloudBlobClient("http://theball.blob.core.windows.net/");
            string templatePath = "anon-webcontainer/oip-layouts/oip-layout-account.phtml";
            CloudBlob blob = publicClient.GetBlobReference(templatePath);
            string templateContent = blob.DownloadText();
            string finalHtml = RenderWebSupport.RenderTemplateWithContent(templateContent, container);
            string finalPath = "theball-reference/account-rendered.html";
            StorageSupport.CurrAnonPublicContainer.UploadBlobText(finalPath, finalHtml);
        }

        private static void doDataTest33(string connStr)
        {
            StorageSupport.InitializeWithConnectionString(connStr);
            BlogContainer container = null;
            try
            {
                container = BlogContainer.RetrieveBlogContainer("BlogContainer123");
            }
            catch (StorageException sEx)
            {

            }
            if (container == null)
            {
                container = BlogContainer.CreateDefault();
                container.ID = "BC123";
                container.RelativeLocation = "BlogContainer123";
                StorageSupport.StoreInformation(container);
            }

            CloudBlobClient publicClient = new CloudBlobClient("http://theball.blob.core.windows.net/");
            string templatePath = "anon-webcontainer/oip-layouts/oip-layout-blog.html";
            CloudBlob blob = publicClient.GetBlobReference(templatePath);
            string templateContent = blob.DownloadText();
            string finalHtml = RenderWebSupport.RenderTemplateWithContent(templateContent, container);
            string finalPath = "theball-reference/blog-rendered.html";
            StorageSupport.CurrAnonPublicContainer.UploadBlobText(finalPath, finalHtml);

/*            StorageSupport.InitializeWithConnectionString(connStr);
            TBReferenceEvent dummyData = null;
            try
            {
                dummyData = TBReferenceEvent.RetrieveTBReferenceEvent("EventTestRelativeLoc134");
            } catch(StorageException sEx)
            {
                
            }
            if(dummyData == null)
            {
                dummyData = TBReferenceEvent.CreateDefault();
                dummyData.ID = "EventTestID134";
                dummyData.RelativeLocation = "EventTestRelativeLoc134";
                dummyData.Title = "Otsikko";
                dummyData.Description = "Kuvaus";
                dummyData.EnoughToAttend = false;
                dummyData.AttendeeCount = 12;
                dummyData.DueTime = DateTime.Today;
                StorageSupport.StoreInformation(dummyData);
            } 

            CloudBlobClient publicClient = new CloudBlobClient("http://theball.blob.core.windows.net/");
            string templatePath = "anon-webcontainer/theball-reference/example1-layout-default.html";
            CloudBlob blob = publicClient.GetBlobReference(templatePath);
            string templateContent = blob.DownloadText();
            string finalHtml = RenderWebSupport.RenderTemplateWithContent(templateContent, dummyData);
            string finalPath = "theball-reference/example1-rendered.html";
            StorageSupport.CurrAnonPublicContainer.UploadBlobText(finalPath, finalHtml);*/
        }


        private static void doTestxyz()
        {
            //StorageSupport.InitializeWithConnectionString(connStr);
            VirtualOwner owner = new VirtualOwner("acc", "0c560c69-c3a7-4363-b125-ba1660d21cf4");
            AddressAndLocationCollection locationCollection = new AddressAndLocationCollection();
            locationCollection.SetLocationAsOwnerContent(owner, "MasterCollection");
            IInformationCollection informationCollection = locationCollection;
            informationCollection.RefreshContent();
            return;
/*            AboutAGIApplications target = new AboutAGIApplications()
                                              {
                                                  ID = "TargetID1",
                                                  RelativeLocation = "RelativeNew7",
                                                  ForAllPeople = new IconTitleDescription
                                                                     {
                                                                         Description = "KukkanenZ",
                                                                         Title = "OtsikkoX"
                                                                     }
                                              };
            StorageSupport.StoreInformation(target);*/
            AboutAGIApplications target = AboutAGIApplications.RetrieveAboutAGIApplications("RelativeNew5");
            //target = AboutAGIApplications.RetrieveAboutAGIApplications("RelativeNew2");
            try
            {
                //StorageSupport.StoreInformation(target);
            } catch(Exception ex)
            {
                
            }
            //AboutAGIApplications subscriber = new AboutAGIApplications()
            //                                      {
            //                                          ID = "Subscriber1",
            //                                          RelativeLocation = "SubscriberFixed1"
            //                                      };
            AboutAGIApplications subscriber = AboutAGIApplications.RetrieveAboutAGIApplications("RelativeNew1");
            try
            {
                StorageSupport.StoreInformation(subscriber);
            }
            catch (Exception ex)
            {
            }
            //SubscribeSupport.AddSubscriptionToObject(target, subscriber, "UpdateSubscriberBasedOnObject");
            //SubscribeSupport.AddSubscriptionToItem(target, "ForAllPeople.Title", subscriber, "UpdateSubscriberBasedOnItem");
            StorageSupport.StoreInformation(target);
        }


        private static void ReportInfo(string text)
        {
            Console.WriteLine(text);
        }

        private static string replaceEvaluator(Match match)
        {
            string fileName = match.Groups[0].Value;
            string fileContent = File.ReadAllText(fileName);
            return fileContent;
        }
    }
}
//AddLoginToAccount("https://www.google.com/accounts/o8/id?id=AItOawkXb-XQERsvhNkZVlEEiCSOuP1y82uHCQc", "fbbaaded-6615-4083-8ea8-92b2aa162861");
//TestDriveQueueWorker();
//TestDriveDynamicCreation();
//return;
//bool result = EmailSupport.SendEmail("kalle.launiala@gmail.com", "kalle.launiala@citrus.fi", "The Ball - Says Hello!",
//                       "Text testing...");
