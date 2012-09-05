using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AaltoGlobalImpact.OIP;

namespace TheBall
{
    public static class FileSystemSupport
    {
        public static void UploadTemplateContent(string[] allFiles, IContainerOwner owner, string targetLocation, bool clearOldTarget)
        {
            if (clearOldTarget)
            {
                StorageSupport.DeleteBlobsFromOwnerTarget(owner, targetLocation);
            }
            var processedDict = allFiles.Where(file => file.EndsWith(".txt")).ToDictionary(file => Path.GetFullPath(file), file => false);
            List<ErrorItem> errorList = new List<ErrorItem>();
            var fixedContent = allFiles //.Where(fileName => fileName.EndsWith(".txt") == false)
                .Select(fileName =>
                        new
                            {
                                FileName
                            =
                            fileName,
                                TextContent
                            =
                            GetFixedContent(fileName, errorList, processedDict),
                                BinaryContent = GetBinaryContent(fileName)

                            })
                .ToArray();
            MoveUnusedTxtFiles(processedDict);
            foreach (var content in fixedContent)
            {
                if (content.FileName.EndsWith(".txt"))
                    continue;
                string webtemplatePath = Path.Combine(targetLocation, content.FileName).Replace("\\", "/");
                Console.WriteLine("Uploading: " + webtemplatePath);
                string blobInformationType = webtemplatePath.EndsWith(".phtml")
                                                 ? StorageSupport.InformationType_WebTemplateValue
                                                 : StorageSupport.InformationType_GenericContentValue;
                if (webtemplatePath.EndsWith("oip-layout-register.phtml") || webtemplatePath.EndsWith("oip-layout-blog-more.phtml") || webtemplatePath.Contains("oip-layout-blog"))
                    blobInformationType = StorageSupport.InformationType_GenericContentValue;
                if (content.TextContent != null)
                {
                    StorageSupport.UploadOwnerBlobText(owner, webtemplatePath, content.TextContent, blobInformationType);
                }
                else
                {
                    StorageSupport.UploadOwnerBlobBinary(owner, webtemplatePath, content.BinaryContent);
                }
            }
        }

        private static void MoveUnusedTxtFiles(Dictionary<string, bool> processedDict)
        {
            string[] filesToMove = processedDict.Keys.ToArray();
            foreach (string fileToMove in filesToMove)
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
                if (File.Exists(destinationFile))
                    File.Delete(destinationFile);
                File.Move(fileToMove, destinationFile);
            }
        }

        private static byte[] GetBinaryContent(string fileName)
        {
            return File.ReadAllBytes(fileName);
        }

        private static string GetFixedContent(string fileName, List<ErrorItem> errorList, Dictionary<string, bool> processedDict)
        {
            if (fileName.EndsWith(".phtml") == false)
                //return File.ReadAllText(fileName);
                return null;
            string fixedContent = FixContent(fileName, errorList, processedDict);
            if (errorList.Count > 0)
            {
                fixedContent = RenderWebSupport.RenderErrorListAsHtml(errorList, "Errors - Combining Files") +
                               fixedContent;
                errorList.Clear();
            }
            return fixedContent;
        }

        private static string FixContent(string fileName, List<ErrorItem> errorList, Dictionary<string, bool> processedDict)
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
                                            //processedDict[incFile] = true;)
                                            if (processedDict.ContainsKey(dictKey))
                                                processedDict.Remove(dictKey);
                                            if (fileContent.Contains("<?php"))
                                                fileContent = FixContent(incFile, errorList, processedDict);
                                            if (String.IsNullOrEmpty(bindObject) == false)
                                            {
                                                // DO Binding Object
                                                fileContent = Environment.NewLine + "<!-- THEBALL-CONTEXT-OBJECT-BEGIN:" + bindObject +
                                                              " -->" + Environment.NewLine
                                                              + fileContent + Environment.NewLine +
                                                              "<!-- THEBALL-CONTEXT-END:" + bindObject +
                                                              " -->" + Environment.NewLine;
                                            }
                                            if (String.IsNullOrEmpty(bindCollection) == false)
                                            {
                                                fileContent = Environment.NewLine + "<!-- THEBALL-CONTEXT-COLLECTION-BEGIN:" + bindCollection +
                                                              " -->" + Environment.NewLine
                                                              + fileContent + Environment.NewLine +
                                                              "<!-- THEBALL-CONTEXT-END:" + bindCollection +
                                                              " -->" + Environment.NewLine;
                                            }
                                            if (String.IsNullOrEmpty(bindRoot) == false)
                                            {
                                                fileContent = Environment.NewLine + "<!-- THEBALL-CONTEXT-ROOT-BEGIN:" + bindRoot +
                                                              " -->" + Environment.NewLine
                                                              + fileContent + Environment.NewLine +
                                                              "<!-- THEBALL-CONTEXT-END:" + bindRoot +
                                                              " -->" + Environment.NewLine;

                                            }
                                            if (fileContent.StartsWith("<!--"))
                                            {
                                                fileContent = Environment.NewLine + "<!-- ========== Begin: " + incFile + " ========== -->" + Environment.NewLine
                                                              + fileContent + Environment.NewLine +
                                                              "<!-- ========== End: " + incFile + " ========== -->" + Environment.NewLine;
                                            }
                                        }
                                        else
                                        {
                                            errorList.Add(new ErrorItem
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

        private static void ReportProblem(string text)
        {
            Console.WriteLine(text);
        }

    }
}