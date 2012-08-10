using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TheBall;

namespace TheBallTool
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] phtmlFiles = Directory.GetFiles(".", "*", SearchOption.AllDirectories);
            var fixedContent = phtmlFiles //.Where(fileName => fileName.EndsWith(".txt") == false)
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
            string connStr = String.Format("DefaultEndpointsProtocol=http;AccountName=theball;AccountKey={0}", args[0]);
            //var container = AzureSupport.ConfigureAnonWebBlobStorage(connStr, true);
            var container = AzureSupport.ConfigurePrivateTemplateBlobStorage(connStr, true);
            foreach (var content in fixedContent)
            {
                if (content.TextContent != null)
                    container.UploadBlobText(content.FileName.Replace(".phtml", ".html"), content.TextContent);
                else
                    container.UploadBlobBinary(content.FileName, content.BinaryContent);
            }
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
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

        private static string FixContent(string fileName)
        {
            string content = File.ReadAllText(fileName);
            string pattern = @"<\?php\sinclude\s*'(?<incfile>.*)'\s*\?>";
            content = Regex.Replace(content,
                                    pattern,
                                    match =>
                                        {
                                            string incFile = match.Groups["incfile"].Value;
                                            string currPath = Path.GetDirectoryName(fileName);
                                            incFile = Path.Combine(currPath, incFile);
                                            string fileContent;
                                            if (File.Exists(incFile))
                                                fileContent = File.ReadAllText(incFile);
                                            else
                                            {
                                                fileContent = "MISSING FILE MISSING FILE MISSING FILE: " + incFile;
                                                ReportProblem(fileContent);
                                            }
                                            if (fileContent.Contains("<?php"))
                                                fileContent = FixContent(incFile);
                                            return fileContent;
                                        });
            return content;
        }

        private static void ReportProblem(string fileContent)
        {
            Console.WriteLine(fileContent);
        }

        private static string replaceEvaluator(Match match)
        {
            string fileName = match.Groups[0].Value;
            string fileContent = File.ReadAllText(fileName);
            return fileContent;
        }
    }
}
