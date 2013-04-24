using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TheBall;
using TheBall.CORE;

namespace WebTemplateManager
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("Usage: WebTemplateManager.exe <groupID> <connection string>");
                    return;
                }
                string connStr = args[1];
                string grpID = args[0];
                //string connStr = String.Format("DefaultEndpointsProtocol=http;AccountName=theball;AccountKey={0}",
                //                               args[0]);
                //connStr = "UseDevelopmentStorage=true";
                bool debugMode = false;

                StorageSupport.InitializeWithConnectionString(connStr, debugMode);
                InformationContext.InitializeFunctionality(3, true);
                InformationContext.Current.InitializeCloudStorageAccess(
                    Properties.Settings.Default.CurrentActiveContainerName);

                string directory = Directory.GetCurrentDirectory();
                if (directory.EndsWith("\\") == false)
                    directory = directory + "\\";
                string[] allFiles =
                    Directory.GetFiles(directory, "*", SearchOption.AllDirectories).Select(str => str.Substring(directory.Length)).ToArray();
                VirtualOwner owner = VirtualOwner.FigureOwner("grp/" + grpID);
                FileSystemSupport.UploadTemplateContent(allFiles, owner, RenderWebSupport.DefaultPublicWwwTemplateLocation, true);
                RenderWebSupport.RenderWebTemplate(grpID, true, "AaltoGlobalImpact.OIP.Blog", "AaltoGlobalImpact.OIP.Activity");
            }
            catch
            {
                
            }
        }
    }
}
