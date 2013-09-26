using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using AaltoGlobalImpact.OIP;
using SecuritySupport;
using TheBall;
using TheBall.CORE;

namespace WebTemplateManager
{
    internal class Program
    {
        private static void Main(string[] arguments)
        {
            try
            {
                /*
                Console.WriteLine("Running test EKE...");
                TheBallEKE.TestExecution();
                Console.WriteLine("Running test EKE complete.");
                //return;
                //SecurityNegotiationManager.EchoClient().Wait();
                SecurityNegotiationManager.EchoClient();
                Console.ReadLine(); // Enter to exit
                //return;*/

                if (arguments.Length != 5 || arguments[1].Length != 4)
                {
                    Console.WriteLine("Usage: WebTemplateManager.exe <containername> <-pub name/-pri name/-sys name> grp<groupID>/acc<acctID>/sys<templatename> <connection string>");
                    return;
                }
                Debugger.Launch();

                string currContainerName = arguments[0];
                ValidateContainerName(currContainerName);
                string pubPriPrefixWithDash = arguments[1];
                string templateName = arguments[2];
                if(String.IsNullOrWhiteSpace(templateName))
                    throw new ArgumentException("Template name must be given");
                string connStr = arguments[4];
                string grpacctIDorTemplateName = arguments[3];
                if(pubPriPrefixWithDash != "-pub" && pubPriPrefixWithDash != "-pri" && pubPriPrefixWithDash != "-sys")
                    throw new ArgumentException("-pub or -pri misspelled or missing");
                string pubPriPrefix = pubPriPrefixWithDash.Substring(1);

                bool debugMode = false;
                StorageSupport.InitializeWithConnectionString(connStr, debugMode);
                InformationContext.InitializeFunctionality(3, true);
                InformationContext.Current.InitializeCloudStorageAccess(currContainerName);

                IContainerOwner owner;
                bool isAccount = false;
                bool isSystem = false;
                string sysTemplateOwner = null;
                if (pubPriPrefix == "pub" || pubPriPrefix == "pri")
                {
                    string ownerPrefix = grpacctIDorTemplateName.Substring(0, 3);
                    string ownerID = grpacctIDorTemplateName.Substring(3);
                    owner = VirtualOwner.FigureOwner(ownerPrefix + "/" + ownerID);
                }
                else // sys
                {
                    isSystem = true;
                    owner = TBSystem.CurrSystem;
                    sysTemplateOwner = grpacctIDorTemplateName.Substring(3);
                    if (sysTemplateOwner != "account" && sysTemplateOwner != "group")
                        throw new NotSupportedException("Other templates than account or group are not supported");
                    isAccount = sysTemplateOwner == "account";
                }
                

                //string connStr = String.Format("DefaultEndpointsProtocol=http;AccountName=theball;AccountKey={0}",
                //                               args[0]);
                //connStr = "UseDevelopmentStorage=true";

                string directory = Directory.GetCurrentDirectory();
                if (directory.EndsWith("\\") == false)
                    directory = directory + "\\";
                string[] allFiles =
                    Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
                             .Select(str => str.Substring(directory.Length))
                             .ToArray();
                if (pubPriPrefix == "pub" && templateName == "legacy")
                {
                    FileSystemSupport.UploadTemplateContent(allFiles, owner,
                                                            RenderWebSupport.DefaultPublicWwwTemplateLocation, true,
                                                            Preprocessor, ContentFilterer, InformationTypeResolver);
                    RenderWebSupport.RenderWebTemplate(owner.LocationPrefix, true, "AaltoGlobalImpact.OIP.Blog",
                                                       "AaltoGlobalImpact.OIP.Activity");
                }
                else
                {
                    string templateLocation = isSystem ? sysTemplateOwner + "/" + templateName : templateName;

                    FileSystemSupport.UploadTemplateContent(allFiles, owner, templateLocation, true);
                    if (isSystem)
                    {
                        if (isAccount)
                            RenderWebSupport.RefreshAllAccountTemplates(templateName);
                        else
                            RenderWebSupport.RefreshAllGroupTemplates(templateName);
                    }
                }
            }
            catch(Exception exception)
            {
                Console.WriteLine("EXCEPTION: " + exception.ToString());
            }
        }

        private static void ValidateContainerName(string currContainerName)
        {
            string[] validContainers = Properties.Settings.Default.AllowedContainerNames.Split(',');
            if(validContainers.Contains(currContainerName) == false)
                throw new InvalidDataException("Given container name not among app.config approved ones: " + currContainerName);
        }

        private static void Preprocessor(BlobStorageContent content)
        {
            if (content.FileName.EndsWith("_DefaultView.html"))
                ReplaceHtmlExtensionWithPHtml(content);
            if (content.FileName.EndsWith("oip-layout-landing.html"))
                ReplaceHtmlExtensionWithPHtml(content);
        }

        private static void ReplaceHtmlExtensionWithPHtml(BlobStorageContent content)
        {
            content.FileName = content.FileName.Substring(0, content.FileName.LastIndexOf(".html")) + ".phtml";
        }

        private static bool ContentFilterer(BlobStorageContent content)
        {
            string fileName = content.FileName;
            if (fileName.EndsWith("readme.txt"))
                return false;
            if (fileName.Contains("_DefaultView.phtml"))
            {
                bool isBlogDefaultView = fileName.EndsWith(".Blog_DefaultView.phtml");
                bool isActivityDefaultView = fileName.EndsWith(".Activity_DefaultView.phtml");
                if (isBlogDefaultView == false && isActivityDefaultView == false)
                    return false;
            }
            return true;
        }

        private static string InformationTypeResolver(BlobStorageContent content)
        {
            string webtemplatePath = content.FileName;
            string blobInformationType;
            if (webtemplatePath.EndsWith(".phtml"))
            {
                //if (webtemplatePath.Contains("/oip-viewtemplate/"))
                if (webtemplatePath.EndsWith("_DefaultView.phtml"))
                    blobInformationType = StorageSupport.InformationType_RuntimeWebTemplateValue;
                else
                    blobInformationType = StorageSupport.InformationType_WebTemplateValue;
            }
            else if (webtemplatePath.EndsWith(".html"))
            {
                string htmlContent = Encoding.UTF8.GetString(content.BinaryContent);
                bool containsMarkup = htmlContent.Contains("THEBALL-CONTEXT");
                if (containsMarkup == false)
                    blobInformationType = StorageSupport.InformationType_GenericContentValue;
                else
                {
                    blobInformationType = webtemplatePath.EndsWith("_DefaultView.html")
                                              ? StorageSupport.InformationType_RuntimeWebTemplateValue
                                              : StorageSupport.InformationType_WebTemplateValue;
                }
            }
            else
                blobInformationType = StorageSupport.InformationType_GenericContentValue;
            return blobInformationType;
        }

    }
}
