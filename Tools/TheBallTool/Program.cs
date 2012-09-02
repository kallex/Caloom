using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AaltoGlobalImpact.OIP;
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
                StorageSupport.InitializeWithConnectionString(connStr);

                TBCollaboratingGroup webGroup = InitializeDefaultOIPWebGroup();
                string templateLocation = "livetemplate";
                string privateSiteLocation = "livesite";
                string publicSiteLocation = "livepubsite";
                //DoMapData(webGroup);
                //return;
                //UpdateTemplateContainer(webGroup, templateLocation);
                Console.WriteLine("Starting to sync...");
                DoSyncs(templateLocation, privateSiteLocation, publicSiteLocation);
                //"grp/default/pub/", true);
                return;
                //doDataTest(connStr);
                //InitLandingPages();
                //Console.WriteLine("Press enter to continue...");
                //Console.ReadLine();
            } 
                catch(Exception ex)
            {
                Console.WriteLine("Error exit: " + ex.ToString());
            }
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
            SyncTemplatesToSite(StorageSupport.CurrActiveContainer.Name,
                String.Format("grp/f8e1d8c6-0000-467e-b487-74be4ad099cd/{0}/", templateLocation),
                StorageSupport.CurrActiveContainer.Name,
                                String.Format("grp/f8e1d8c6-0000-467e-b487-74be4ad099cd/{0}/", privateSiteLocation), false);
            SyncTemplatesToSite(StorageSupport.CurrActiveContainer.Name,
                String.Format("grp/f8e1d8c6-0000-467e-b487-74be4ad099cd/{0}/", privateSiteLocation),
                StorageSupport.CurrAnonPublicContainer.Name,
                                String.Format("grp/default/{0}/", publicSiteLocation), true);
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
                                                                      "vilperi");
        }

        private static void SyncTemplatesToSite(string sourceContainerName, string sourcePathRoot, string targetContainerName, string targetPathRoot, bool useQueuedWorker)
        {
            if (useQueuedWorker)
            {
                QueueEnvelope envelope = new QueueEnvelope
                                             {
                                                 UpdateWebContentOperation = new UpdateWebContentOperation
                                                                                 {
                                                                                     SourceContainerName =
                                                                                         sourceContainerName,
                                                                                     SourcePathRoot = sourcePathRoot,
                                                                                     TargetContainerName =
                                                                                         targetContainerName,
                                                                                     TargetPathRoot = targetPathRoot
                                                                                 }
                                             };
                QueueSupport.PutToDefaultQueue(envelope);
            }
            else
            {
                WorkerSupport.WebContentSync(sourceContainerName, sourcePathRoot, targetContainerName, targetPathRoot, RenderWebSupport.RenderingSyncHandler);
            }
        }

        private static void UpdateTemplateContainer(IContainerOwner owner, string templateLocation)
        {
            string[] allFiles = Directory.GetFiles(".", "*", SearchOption.AllDirectories);
            ProcessedDict = allFiles.Where(file => file.EndsWith(".txt")).ToDictionary(file => Path.GetFullPath(file), file => false);
            var fixedContent = allFiles //.Where(fileName => fileName.EndsWith(".txt") == false)
                .Select(fileName =>
                        new
                            {
                                FileName
                            =
                            fileName,
                                TextContent
                            =
                            GetFixedContent(fileName),
                                BinaryContent = GetBinaryContent(fileName)

                            })
                .ToArray();
            MoveUnusedTxtFiles(ProcessedDict);
            foreach (var content in fixedContent)
            {
                //if (content.FileName.Contains("glyph"))
                //    continue;
                //if (content.FileName.Contains("theball-reference") == false)
                //    continue;
                if (content.FileName.EndsWith(".txt"))
                    continue;
                //if (content.FileName.EndsWith(".png"))
                //    continue;
                //if (content.FileName.EndsWith(".jpg"))
                //    continue;
                //if (content.FileName.Contains("oip-") == false && content.FileName.Contains("theball-") == false)
                //    continue;
                //if (content.FileName.Contains(".phtml") == false)
                //    continue;
                string webtemplatePath = Path.Combine(templateLocation, content.FileName).Replace("\\", "/");
                Console.WriteLine("Uploading: " + webtemplatePath);
                string blobInformationType = webtemplatePath.EndsWith(".phtml")
                                                 ? StorageSupport.InformationType_WebTemplateValue
                                                 : StorageSupport.InformationType_GenericContentValue;
                if (content.TextContent != null)
                {
                    StorageSupport.UploadOwnerBlobText(owner, webtemplatePath, content.TextContent, blobInformationType);                    
                }
                else
                {
                    StorageSupport.UploadOwnerBlobBinary(owner, webtemplatePath, content.BinaryContent);
                }
                //    container.UploadBlobText(content.FileName.Replace(".phtml", ".phtml"), content.TextContent);
                //else
                //    container.UploadBlobBinary(content.FileName, content.BinaryContent);
            }
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

        private static void MoveUnusedTxtFiles(Dictionary<string, bool> processedDict)
        {
            string[] filesToMove = processedDict.Keys.ToArray();
            foreach(string fileToMove in filesToMove)
            {
                if (fileToMove.Contains("oip-") == false)
                {
                    Console.WriteLine("Ignoring unused: " + fileToMove);
                    continue;
                }
                string destinationFile = fileToMove.Replace(@"caloomhtml\UI\docs\", @"caloomhtml\UI\notusedtxt\");
                Console.WriteLine("NOT Moving unused file to: " + destinationFile);
                continue;
                string destDir = Path.GetDirectoryName(destinationFile);
                if (Directory.Exists(destDir) == false)
                    Directory.CreateDirectory(destDir);
                if(File.Exists(destinationFile))
                    File.Delete(destinationFile);
                File.Move(fileToMove, destinationFile);
            }
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
                var memberColl = container.AccountModule.AccountRoles.MemberInGroups.CollectionContent;
                memberColl.Add(new ReferenceToInformation { Title = "The Ball Test Yle.fi", URL = "http://www.yle.fi" });
                memberColl.Add(new ReferenceToInformation { Title = "The Ball Test Aalto.fi", URL = "http://www.aalto.fi" });
                var moderatorColl = container.AccountModule.AccountRoles.ModeratorInGroups.CollectionContent;
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


        private static void doTest(string connStr)
        {
            StorageSupport.InitializeWithConnectionString(connStr);
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
            SubscribeSupport.AddSubscriptionToObject(target, subscriber, "UpdateSubscriberBasedOnObject");
            SubscribeSupport.AddSubscriptionToItem(target, "ForAllPeople.Title", subscriber, "UpdateSubscriberBasedOnItem");
            StorageSupport.StoreInformation(target);
        }

        private static byte[] GetBinaryContent(string fileName)
        {
            return File.ReadAllBytes(fileName);
        }

        private static string GetFixedContent(string fileName)
        {
            if (fileName.EndsWith(".phtml") == false)
                //return File.ReadAllText(fileName);
                return null;
            string fixedContent = FixContent(fileName);
            if(ErrorList.Count > 0)
            {
                fixedContent = RenderWebSupport.RenderErrorListAsHtml(ErrorList, "Errors - Combining Files") +
                               fixedContent;
                ErrorList.Clear();
            }
            return fixedContent;
        }

        private static Dictionary<string, bool> ProcessedDict = new Dictionary<string, bool>();
        private static List<ErrorItem> ErrorList = new List<ErrorItem>();

        private static string FixContent(string fileName)
        {
            string content = File.ReadAllText(fileName);
            //string pattern = @"<\?php\sinclude\s*'(?<incfile>.*)'.*\?>";
            //string pattern = @"<\?php\sinclude\s*'(?<incfile>[^']*)'[^>]*\?>";
            string pattern =
                @"<\?php\sinclude\s*'(?<incfile>[^']*)'[^>]*\?>(<!--\s*UseInformationObject:(?<bindingobject>[^\s]*)\s*-->|<!--\s*UseInformationObjectAsCollection:(?<bindingcollection>[^\s]*)\s*-->|<!--\s*UseInformationObjectAsRoot:(?<bindingroot>[^\s]*)\s*-->|)";
            content = Regex.Replace(content,
                                    pattern,
                                    match =>
                                        {
                                            string incFile = match.Groups["incfile"].Value;
                                            string bindObject = match.Groups["bindingobject"].Value;
                                            string bindCollection = match.Groups["bindingcollection"].Value;
                                            string bindRoot = match.Groups["bindingroot"].Value;
                                            string currPath = Path.GetDirectoryName(fileName);
                                            incFile = Path.Combine(currPath, incFile);
                                            string fileContent;
                                            if (File.Exists(incFile))
                                            {
                                                fileContent = File.ReadAllText(incFile);
                                                string dictKey = Path.GetFullPath(incFile);
                                                //ProcessedDict[incFile] = true;)
                                                if (ProcessedDict.ContainsKey(dictKey))
                                                    ProcessedDict.Remove(dictKey);
                                                if (fileContent.Contains("<?php"))
                                                    fileContent = FixContent(incFile);
                                                if(String.IsNullOrEmpty(bindObject) == false)
                                                {
                                                    // DO Binding Object
                                                    fileContent = Environment.NewLine + "<!-- THEBALL-CONTEXT-OBJECT-BEGIN:" + bindObject +
                                                                  " -->" + Environment.NewLine
                                                                  + fileContent + Environment.NewLine +
                                                                  "<!-- THEBALL-CONTEXT-END:" + bindObject +
                                                                  " -->" + Environment.NewLine;
                                                }
                                                if(String.IsNullOrEmpty(bindCollection) == false)
                                                {
                                                    fileContent = Environment.NewLine + "<!-- THEBALL-CONTEXT-COLLECTION-BEGIN:" + bindCollection +
                                                                  " -->" + Environment.NewLine
                                                                  + fileContent + Environment.NewLine +
                                                                  "<!-- THEBALL-CONTEXT-END:" + bindCollection +
                                                                  " -->" + Environment.NewLine;
                                                }
                                                if(String.IsNullOrEmpty(bindRoot) == false)
                                                {
                                                    fileContent = Environment.NewLine + "<!-- THEBALL-CONTEXT-ROOT-BEGIN:" + bindRoot +
                                                                  " -->" + Environment.NewLine
                                                                  + fileContent + Environment.NewLine +
                                                                  "<!-- THEBALL-CONTEXT-END:" + bindRoot +
                                                                  " -->" + Environment.NewLine;
                                                    
                                                }
                                                if(fileContent.StartsWith("<!--"))
                                                {
                                                    fileContent = Environment.NewLine + "<!-- ========== Begin: " + incFile + " ========== -->" + Environment.NewLine
                                                                  + fileContent + Environment.NewLine + 
                                                                  "<!-- ========== End: " + incFile + " ========== -->" + Environment.NewLine;
                                                }
                                            }
                                            else
                                            {
                                                ErrorList.Add(new ErrorItem
                                                                  {
                                                                      CurrentLine = incFile,
                                                                      CurrentContextName = fileName,
                                                                      ErrorMessage = "File missing"
                                                                      
                                                                  });
                                                fileContent = "MISSING FILE MISSING FILE MISSING FILE: " + incFile;
                                                ReportProblem(fileContent);
                                            }
                                            return fileContent;
                                        });
            return content;
        }

        private static void ReportInfo(string text)
        {
            Console.WriteLine(text);
        }

        private static void ReportProblem(string text)
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
