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
            string connStr = String.Format("DefaultEndpointsProtocol=http;AccountName=theball;AccountKey={0}", args[0]);
            doTest2(connStr);
            return;
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
            var container = StorageSupport.ConfigureAnonWebBlobStorage(connStr, true);
            //var container = StorageSupport.ConfigureAnonWebBlobStorage(connStr, true);
            //var container = StorageSupport.ConfigurePrivateTemplateBlobStorage(connStr, true);
            MoveUnusedTxtFiles(ProcessedDict);
            foreach (var content in fixedContent)
            {
                //if (content.FileName.Contains("glyph"))
                //    continue;
                if (content.FileName.EndsWith(".txt"))
                    continue;
                Console.WriteLine("Uploading: " + content.FileName);
                if (content.TextContent != null)
                    container.UploadBlobText(content.FileName.Replace(".phtml", ".html"), content.TextContent);
                else
                    container.UploadBlobBinary(content.FileName, content.BinaryContent);
            }
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
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
                Console.WriteLine("Moving unused file to: " + destinationFile);
                string destDir = Path.GetDirectoryName(destinationFile);
                if (Directory.Exists(destDir) == false)
                    Directory.CreateDirectory(destDir);
                File.Move(fileToMove, destinationFile);
            }
        }

        private static void doTest2(string connStr)
        {
            StorageSupport.InitializeWithConnectionString(connStr);

            CloudBlobClient publicClient = new CloudBlobClient("http://theball.blob.core.windows.net/");
            string blobPath = "anon-webcontainer/oip-layouts/oip-layout-default-edit.html";
            CloudBlob blob = publicClient.GetBlobReference(blobPath);
            string webTemplate = blob.DownloadText();
            BlogContainer container = BlogContainer.CreateDefault();
            container.BlogContainerHeader.Title = "Titteli";
            container.BlogContainerHeader.SubTitle = "Aliotsikko";
            string renderedPage = RenderWebSupport.RenderTemplateWithContent(webTemplate, container);
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
            return FixContent(fileName);
        }

        private static Dictionary<string, bool> ProcessedDict = new Dictionary<string, bool>();

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
