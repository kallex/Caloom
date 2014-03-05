using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AaltoGlobalImpact.OIP;
using TheBall.CORE;

namespace TheBall
{
    public class BlobStorageContent
    {
        private byte[] _binaryContent;
        public string FileName { get; set; }

        public byte[] BinaryContent
        {
            get { return _binaryContent;  }
            set
            {
                _binaryContent = value;
                updateMD5();
            }
        }

        public string MD5Hash { get; private set; }

        private void updateMD5()
        {
            if (_binaryContent == null)
                MD5Hash = null;
            MD5 md5 = MD5.Create();
            byte[] md5result = md5.ComputeHash(_binaryContent);
            MD5Hash = Convert.ToBase64String(md5result);
        }
    }

    public delegate string InformationTypeResolver(BlobStorageContent blobStorageContent);

    public static class FileSystemSupport
    {
        public static string[] UploadTemplateContent(string[] allFiles, IContainerOwner owner, string targetLocation, bool clearOldTarget,
            Action<BlobStorageContent> preprocessor = null, Predicate<BlobStorageContent> contentFilterer = null)
        {
            if (clearOldTarget)
            {
                StorageSupport.DeleteBlobsFromOwnerTarget(owner, targetLocation);
            }
            var processedDict = allFiles.Where(file => file.EndsWith(".txt")).Where(File.Exists).ToDictionary(file => Path.GetFullPath(file), file => false);
            List<ErrorItem> errorList = new List<ErrorItem>();
            var fixedContent = allFiles.Where(fileName => fileName.EndsWith(".txt") == false)
                .Select(fileName =>
                        new BlobStorageContent
                        {
                            FileName = fileName,
                            BinaryContent = File.ReadAllBytes(fileName)
                            // BinaryContent = GetBlobContent(fileName, errorList, processedDict)
                        })
                .ToArray();
            if (preprocessor != null)
            {
                foreach (var content in fixedContent)
                {
                    preprocessor(content);
                }
            }
            foreach (var content in fixedContent)
            {
                if (contentFilterer != null && contentFilterer(content) == false)
                {
                    // TODO: Properly implement delete above
                    continue;
                }
                string webtemplatePath = Path.Combine(targetLocation, content.FileName).Replace("\\", "/");
                Console.WriteLine("Uploading: " + webtemplatePath);
                StorageSupport.UploadOwnerBlobBinary(owner, webtemplatePath, content.BinaryContent);
            }
            return processedDict.Keys.ToArray();
        }


#if superseded

        private static string GetBlobInformationType(BlobStorageContent content)
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
            else 
                blobInformationType = StorageSupport.InformationType_GenericContentValue;
            if (webtemplatePath.EndsWith("oip-layout-register.phtml")) // || webtemplatePath.EndsWith("oip-layout-blog-more.phtml") || webtemplatePath.Contains("oip-layout-blog"))
                blobInformationType = StorageSupport.InformationType_GenericContentValue;
            return blobInformationType;
        }

        public static void MoveUnusedTxtFiles(string[] filesToMove)
        {
            //string[] filesToMove = processedDict.Keys.ToArray();
            foreach (string fileToMove in filesToMove)
            {
                if (fileToMove.Contains("oip-") == false)
                {
                    Console.WriteLine("Ignoring unused: " + fileToMove);
                    continue;
                }

                string destinationFile = fileToMove.Replace(@"caloomhtml\UI\docs\", @"caloomhtml\UI\notusedtxt\");
                Console.WriteLine("Moving unused file to: " + destinationFile);
                //continue;
                string destDir = Path.GetDirectoryName(destinationFile);
                if (Directory.Exists(destDir) == false)
                    Directory.CreateDirectory(destDir);
                if (File.Exists(destinationFile))
                    File.Delete(destinationFile);
                File.Move(fileToMove, destinationFile);
            }
        }

        private static byte[] GetBlobContent(string fileName, List<ErrorItem> errorList, Dictionary<string, bool> processedDict)
        {
            byte[] fixedContent = GetFixedContent(fileName, errorList, processedDict);
            if (fixedContent != null)
                return fixedContent;
            return GetBinaryContent(fileName);
        }



        private static byte[] GetBinaryContent(string fileName)
        {
            return File.ReadAllBytes(fileName);
        }

        public static byte[] GetFixedContent(string fileName, List<ErrorItem> errorList, Dictionary<string, bool> processedDict)
        {
            if (fileName.EndsWith(".phtml") == false && fileName.EndsWith(".html") == false)
                //return File.ReadAllText(fileName);
                return null;
            string fixedContent = FixContent(fileName, errorList, processedDict);
            if (errorList.Count > 0)
            {
                fixedContent = RenderWebSupport.RenderErrorListAsHtml(errorList, "Errors - Combining Files") +
                               fixedContent;
                errorList.Clear();
            }
            return Encoding.UTF8.GetBytes(fixedContent);
        }

        private static string FixContent(string fileName, List<ErrorItem> errorList, Dictionary<string, bool> processedDict)
        {
            string content = File.ReadAllText(fileName, Encoding.UTF8);
            //string pattern = @"<\?php\sinclude\s*'(?<incfile>.*)'.*\?>";
            //string pattern = @"<\?php\sinclude\s*'(?<incfile>[^']*)'[^>]*\?>";
            string pattern =
                @"<\?php\sinclude\s*'(?<incfile>[^']*)'[^>]*\?>(<!--\s*UseInformationObject:(?<bindingobject>[^\s]*)\s*-->|<!--\s*UseInformationObjectAsCollection:(?<bindingcollection>[^\s]*)\s*-->|<!--\s*UseInformationObjectAsRoot:(?<bindingroot>[^\s]*)\s*-->|<!--\s*UseInformationObjectAsDynamicRoot:(?<bindingdynamicroot>[^\s]*)\s*-->|)";
            content = Regex.Replace(content,
                                    pattern,
                                    match =>
                                    {
                                        string incFile = match.Groups["incfile"].Value;
                                        string bindObject = match.Groups["bindingobject"].Value;
                                        string bindCollection = match.Groups["bindingcollection"].Value;
                                        string bindRoot = match.Groups["bindingroot"].Value;
                                        string bindDynamicRoot = match.Groups["bindingdynamicroot"].Value;
                                        string currPath = Path.GetDirectoryName(fileName);
                                        incFile = Path.Combine(currPath, incFile);
                                        string fileContent;
                                        if (File.Exists(incFile))
                                        {
                                            //bool isAccount = incFile.Contains("oip-module-account.txt");
                                            //if (isAccount)
                                            //    isAccount = false;
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
                                            if (String.IsNullOrEmpty(bindDynamicRoot) == false)
                                            {
                                                fileContent = Environment.NewLine + "<!-- THEBALL-CONTEXT-DYNAMICROOT-BEGIN:" + bindDynamicRoot +
                                                              " -->" + Environment.NewLine
                                                              + fileContent + Environment.NewLine +
                                                              "<!-- THEBALL-CONTEXT-END:" + bindDynamicRoot +
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
#endif
    }
}