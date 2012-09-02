using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure.StorageClient;

namespace TheBall
{
    public static class RenderWebSupport
    {
        private const string InformationStorageKey = "InformationStorage";
        private static int RootTagLen;
        private static int RootTagsTotalLen;
        private static int CollTagLen;
        private static int CollTagsTotalLen;
        private static int ObjTagLen;
        private static int ObjTagsTotalLen;
        private static int AtomTagLen;
        private static int AtomTagsTotalLen;
        private const string FormHiddenFieldTag = "<!-- THEBALL-FORM-HIDDEN-FIELDS -->";
        private const string RootTagBegin = "<!-- THEBALL-CONTEXT-ROOT-BEGIN:";
        private const string CollectionTagBegin = "<!-- THEBALL-CONTEXT-COLLECTION-BEGIN:";
        private const string ObjectTagBegin = "<!-- THEBALL-CONTEXT-OBJECT-BEGIN:";
        private const string CommentEnd = " -->";
        private const string ContextTagEnd = "<!-- THEBALL-CONTEXT-END:";
        private const string TheBallPrefix = "<!-- THEBALL-";
        private const string AtomTagBegin = "[!ATOM]";
        private const string AtomTagEnd = "[ATOM!]";
        private const string MemberAtomPattern = @"(?<fulltag>\[!ATOM](?<membername>\w*)\[ATOM!])";

        private static Regex ContextRootRegex;

        static RenderWebSupport()
        {
            RootTagLen = RootTagBegin.Length;
            RootTagsTotalLen = RootTagBegin.Length + CommentEnd.Length;
            CollTagLen = CollectionTagBegin.Length;
            CollTagsTotalLen = CollectionTagBegin.Length + CommentEnd.Length;
            ObjTagLen = ObjectTagBegin.Length;
            ObjTagsTotalLen = ObjectTagBegin.Length + CommentEnd.Length;
            AtomTagLen = AtomTagBegin.Length;
            AtomTagsTotalLen = AtomTagBegin.Length + AtomTagEnd.Length;
            ContextRootRegex = new Regex(@"THEBALL-CONTEXT-ROOT-BEGIN:(?<rootType>[\w_\.]*)(?:\:(?<rootName>[\w_]*)|)", RegexOptions.Compiled);
        }

        public class ContentItem
        {
            public InformationSource Source;
            public string RootName;
            public string RootType;
            public object RootObject;
            public bool WasMissing = false;
            public bool WasNeeded = false;
        }

        public static void RenderTemplateWithContentToBlob(CloudBlob template, CloudBlob renderTarget)
        {
            InformationSourceCollection sources = renderTarget.GetBlobInformationSources();
            if(sources == null)
            {
                sources = CreateDefaultSources(template);
            }
            string templateContent = template.DownloadText();
            List<ContentItem> existingRoots = GetExistingRoots(sources);
            string renderResult = RenderTemplateWithContentRoots(templateContent, existingRoots);
            UpdateMismatchedRootsToSources(sources, existingRoots, renderTarget);
            renderTarget.SetBlobInformationSources(sources);
            renderTarget.UploadBlobText(renderResult, StorageSupport.InformationType_GenericContentValue);
        }

        private static void UpdateMismatchedRootsToSources(InformationSourceCollection sources, List<ContentItem> existingRoots, CloudBlob renderTarget)
        {
            var newSources =
                existingRoots.Where(root => root.WasMissing).Select(root => GetMissingRootAsNewSource(root, renderTarget.Name)).ToArray();
            sources.CollectionContent.AddRange(newSources);
            foreach (var item in existingRoots.Where(root => root.WasNeeded == false))
                sources.CollectionContent.Remove(item.Source);
            // Don't delete the missing blobs just now

        }

        private static InformationSource GetMissingRootAsNewSource(ContentItem root, string masterLocation)
        {
            InformationSource source = root.Source ?? InformationSource.CreateDefault();
            IInformationObject informationObject = (IInformationObject) root.RootObject;
            informationObject.SetLocationRelativeToRoot(masterLocation);
            CloudBlob blob = StorageSupport.StoreInformation(informationObject);
            source.SetBlobValuesToSource(blob);
            source.SetInformationObjectValuesToSource(root.RootName, informationObject.GetType().FullName);
            return source;
        }

        private static List<ContentItem> GetExistingRoots(InformationSourceCollection sources)
        {
            return
                sources.CollectionContent.Where(
                    source => source.SourceType == StorageSupport.InformationType_InformationObjectValue).Select(
                        GetRootFromSource).ToList();
        }

        private static ContentItem GetRootFromSource(InformationSource source)
        {
            ContentItem contentItem = new ContentItem
                                          {
                                              Source = source,
                                              RootName = source.SourceName,
                                              RootType = source.SourceInformationObjectType,
                                          };
            return contentItem;
        }

        private static InformationSourceCollection CreateDefaultSources(CloudBlob template)
        {
            InformationSourceCollection sources = InformationSourceCollection.CreateDefault();
            InformationSource source = InformationSource.CreateDefault();
            source.SetBlobValuesToSource(template);
            sources.CollectionContent.Add(source);
            return sources;
        }

        public static string RenderTemplateWithContent(string templatePage, object content)
        {
            List<ContentItem> contentRoots = new List<ContentItem>();
            contentRoots.Add(new ContentItem
                                 {
                                     RootName = "",
                                     RootObject = content,
                                     RootType = content.GetType().FullName,
                                     WasMissing = false
                                 });
            return RenderTemplateWithContentRoots(templatePage, contentRoots);
        }

        public static string RenderTemplateWithContentRoots(string templatePage, List<ContentItem> contentRoots)
        {
            StringBuilder result = new StringBuilder(templatePage.Length);
            Stack<StackContextItem> contextStack = new Stack<StackContextItem>();
            List<ErrorItem> errorList = new List<ErrorItem>();

            string[] lines = templatePage.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            int endIndexExclusive = lines.Length;
            ProcessLinesScope(lines, 0, ref endIndexExclusive, result, contentRoots, contextStack, errorList);
            if(errorList.Count > 0)
            {
                result.Insert(0, RenderErrorListAsHtml(errorList, "Errors - Databinding"));
            }
            return result.ToString();
        }

        private static void ProcessLinesScope(string[] lines, int startIndex, ref int endIndexExclusive, StringBuilder result, List<ContentItem> contentRoots, Stack<StackContextItem> contextStack, List<ErrorItem> errorList)
        {
            bool hasEmptyStackToBegin = contextStack.Count == 0;
            for (int currLineIX = startIndex; currLineIX < endIndexExclusive; currLineIX++)
            {
                string line = lines[currLineIX];
                try
                {
                    if (IgnoreBindingsTillEnd(line))
                    {
                        break;
                    }

                    bool hasStackContext = ProcessLine(lines, ref currLineIX, line, result, contentRoots, contextStack, errorList);
                    // If stack is empty and we had context to begin with, stop here
                    if (hasStackContext == false && hasEmptyStackToBegin == false)
                    {
                        endIndexExclusive = currLineIX + 1;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    StackContextItem item = contextStack.Count > 0 ? contextStack.Peek() : null;
                    ErrorItem errorItem = new ErrorItem(ex, item, line);
                    errorList.Add(errorItem);
                }
            }
        }

        private static bool ProcessLine(string[] lines, ref int currLineIx, string line, StringBuilder result, List<ContentItem> contentRoots, Stack<StackContextItem> contextStack, List<ErrorItem> errorList)
        {
            if (line.StartsWith(TheBallPrefix) == false && line.Contains("[!ATOM]") == false)
            {
                result.AppendLine(line);
                return contextStack.Count > 0;
            }
            if (line.StartsWith(RootTagBegin))
            {
                object content;
                StackContextItem rootItem;
                Match match = ContextRootRegex.Match(line);
                string rootType = match.Groups["rootType"].Value;
                string rootName = match.Groups["rootName"].Value;
                try
                {
                    //string typeName = line.Substring(RootTagLen, line.Length - RootTagsTotalLen).Trim();
                    content = GetOrInitiateContentObject(contentRoots, rootType, rootName);
                    StackContextItem parent = contextStack.Count > 0 ? contextStack.Peek() : null;
                    rootItem = new StackContextItem(content, parent, content.GetType(), null, true, false);
                }
                catch
                {
                    content = new object();
                    rootItem = new StackContextItem("Invalid Stack Root Item", null, typeof(string), "INVALID", false, true);
                    errorList.Add(new ErrorItem(new Exception("Invalid stack root: " + rootType), rootItem, line));
                }
                if (contextStack.Count != 0)
                    throw new InvalidDataException("Context stack already has a root item before: " + content.GetType().FullName);
                contextStack.Push(rootItem);
            }
            else if (line.StartsWith(CollectionTagBegin))
            {
                string memberName = line.Substring(CollTagLen, line.Length - CollTagsTotalLen).Trim();
                Stack<StackContextItem> collStack = new Stack<StackContextItem>();
                StackContextItem currCtx = contextStack.Peek();
                StackContextItem collItem = null;
                try
                {
                    Type type = GetMemberType(currCtx, memberName);
                    object contentValue = GetPropertyValue(currCtx, memberName);
                    StackContextItem parent = currCtx;
                    collItem = new StackContextItem(contentValue, parent, type, memberName, false, true);
                } catch(Exception ex)
                {
                    if (collItem == null)
                        collItem = new StackContextItem("Invalid Collection Context", contextStack.Peek(), typeof(string), "INVALID", false, true);
                }
                collStack.Push(collItem);
                int scopeStartIx = currLineIx + 1;
                int scopeEndIx = lines.Length; // Candidate, the lower call will adjust properly
                // Get scope length
                ProcessLinesScope(lines, scopeStartIx, ref scopeEndIx, new StringBuilder(), contentRoots, collStack,
                                  new List<ErrorItem>());
                bool isFirstRound = true;
                while (collItem.IsNotFullyProcessed)
                {
                    var currErrorList = isFirstRound ? errorList : new List<ErrorItem>();
                    collStack.Push(collItem);
                    ProcessLinesScope(lines, scopeStartIx, ref scopeEndIx, result, contentRoots, collStack,
                                        currErrorList);
                    if(collStack.Count > 0)
                        throw new InvalidDataException("Collection stack should be empty at this point");
                    isFirstRound = false;
                    collItem.CurrCollectionItem++;
                }
                // Jump to the end tag (as the next loop will progress by one)
                currLineIx = scopeEndIx - 1;
            }
            else if (line.StartsWith(ObjectTagBegin))
            {
                string memberName = line.Substring(ObjTagLen, line.Length - ObjTagsTotalLen).Trim();
                StackContextItem currCtx = contextStack.Peek();
                if (memberName == "*")
                {
                    // Put top item again to stack
                    contextStack.Push(currCtx);
                }
                else
                {
                    StackContextItem objItem = null;
                    try
                    {
                        Type type = GetMemberType(currCtx, memberName);
                        object contentValue = GetPropertyValue(currCtx, memberName);
                        StackContextItem parent = currCtx;
                        objItem = new StackContextItem(contentValue, parent, type, memberName, false, false);
                    } finally
                    {
                        if(objItem == null)
                            objItem = new StackContextItem("Invalid Context", contextStack.Peek(), typeof(string), "INVALID", false, false);
                        contextStack.Push(objItem);
                    }
                }
            }
            else if (line.StartsWith(ContextTagEnd))
            {
                StackContextItem popItem = contextStack.Pop();
                if (contextStack.Count == 0)
                    return false;
            }
            else if(line.Contains(FormHiddenFieldTag))
            {
                result.AppendLine(line);
                ProcessATOMLine(
                    "<input id=\"RootObjectRelativeLocation\" name=\"RootObjectRelativeLocation\" type=\"hidden\" value=\"[!ATOM]RelativeLocation[ATOM!]\" />",
                    result, contextStack);
                ProcessATOMLine(
                    "<input id=\"RootObjectType\" name=\"RootObjectType\" type=\"hidden\" value=\"[!ATOM]SemanticDomainName[ATOM!].[!ATOM]Name[ATOM!]\" />",
                    result, contextStack);
                ProcessATOMLine(
                    "<input id=\"RootObjectETag\" name=\"RootObjectETag\" type=\"hidden\" value=[!ATOM]ETag[ATOM!] />",
                    result, contextStack);
            }
            else // ATOM line
            {
                ProcessATOMLine(line, result, contextStack);
            }
            return contextStack.Count > 0;
        }

        public static object GetOrInitiateContentObject(List<ContentItem> contentRoots, string rootType, string rootName)
        {
            ContentItem contentItem =
                contentRoots.FirstOrDefault(item => item.RootType == rootType && item.RootName == rootName);
            if(contentItem == null)
            {
                Type createdType = Assembly.GetExecutingAssembly().GetType(rootType);
                object result = createdType.InvokeMember("CreateDefault", BindingFlags.InvokeMethod, null, null, null);
                contentItem = new ContentItem
                                  {
                                      RootName = rootName,
                                      RootType = rootType,
                                      RootObject = result,
                                      WasMissing = true
                                  };
                contentRoots.Add(contentItem);
            } else
            {
                if(contentItem.RootObject == null)
                {
                    try
                    {
                        contentItem.RootObject = contentItem.Source.RetrieveInformationObject();
                    } catch // If the rootobject fetch fails, it's due to serialization error most likely
                    {
                        
                    }
                    if (contentItem.RootObject == null)
                    {
                        contentItem.WasMissing = true;
                        Type createdType = Assembly.GetExecutingAssembly().GetType(rootType);
                        object result = createdType.InvokeMember("CreateDefault", BindingFlags.InvokeMethod, null, null,
                                                                 null);
                        contentItem.RootObject = result;
                    }
                }
            }
            contentItem.WasNeeded = true;
            return contentItem.RootObject;
        }

        private static void ProcessATOMLine(string line, StringBuilder result, Stack<StackContextItem> contextStack)
        {
            StackContextItem currCtx = contextStack.Peek();
            var contentLine = Regex.Replace(line, MemberAtomPattern,
                                            match =>
                                                {
                                                    string memberName = match.Groups["membername"].Value;
                                                    object value = GetPropertyValue(currCtx, memberName);
                                                    return (value ?? (currCtx.MemberName + "." + memberName + " is null")).ToString();
                                                });
            result.AppendLine(contentLine);
        }

        public static string RenderErrorListAsHtml(List<ErrorItem> errorList, string errorLabel)
        {
            StringBuilder errorHtml = new StringBuilder();
            errorHtml.AppendLine("<h1>" + errorLabel + "</h1>");
            errorHtml.AppendLine("<p><table border=\"1\"><tr><th>Context</th><th>Error</th><th>Line</th></tr>");
            var tableList = errorList.
                Where(errorItem => errorItem.CurrentStackItem != null || errorItem.CurrentContextName != null).
                Select(errorItem =>
                           {
                               string currentContextName;
                               if (errorItem.CurrentStackItem != null)
                               {
                                   currentContextName = errorItem.CurrentStackItem.ItemType.Name + "." +
                                                       errorItem.CurrentStackItem.MemberName;
                               }
                               else
                               {
                                   currentContextName = errorItem.CurrentContextName;
                               }
                               string errorMessage = errorItem.ErrorException != null
                                                         ? errorItem.ErrorException.Message
                                                         : errorItem.ErrorMessage;
                               return new
                                          {
                                              Name = currentContextName,
                                              ErrorMessage = errorMessage,
                                              CurrLine = errorItem.CurrentLine.Trim()
                                          };

                           });
            foreach(var tableRow in tableList.Distinct())
            {
                errorHtml.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>",
                                       tableRow.Name, tableRow.ErrorMessage, tableRow.CurrLine);
            }
            errorHtml.AppendLine("</table></p>");
            return errorHtml.ToString();
        }

        private static bool IgnoreBindingsTillEnd(string line)
        {
            return line.StartsWith("<!-- IGNOREBINDINGS FROM THIS POINT ONWARDS -->");
        }

        private static object GetPropertyValue(StackContextItem currCtx, string propertyName)
        {
            if(currCtx.CurrContent == null)
                throw new InvalidDataException("Object: " + currCtx.MemberName + " does not have content (was retrieving value: " + propertyName + ")");
            //Type type = currCtx.ItemType;
            Type type = currCtx.CurrContent.GetType();
            PropertyInfo pi = type.GetProperty(propertyName);
            return pi.GetValue(currCtx.CurrContent, null);
        }

        private static Type GetMemberType(StackContextItem containingItem, string memberName)
        {
            Type containingType = containingItem.ItemType;
            PropertyInfo pi = containingType.GetProperty(memberName);
            if(pi == null)
                throw new InvalidDataException("Type: " + containingType.Name + " does not contain property: " + memberName);
            return pi.PropertyType;
        }

        public static bool RenderingSyncHandler(CloudBlob source, CloudBlob target, WorkerSupport.SyncOperationType operationtype)
        {
            // Don't delete informationobject types of target folders
            if (operationtype == WorkerSupport.SyncOperationType.Delete)
            {
                if(target.GetBlobInformationType() == StorageSupport.InformationType_InformationObjectValue)
                    return true;
                return false;
            }
            if (operationtype == WorkerSupport.SyncOperationType.Copy)
            {
                // Custom rendering for web templates
                if(source.GetBlobInformationType() == StorageSupport.InformationType_WebTemplateValue)
                {
                    if(source.Name.Contains("map"))
                    {
                        bool isMap = true;
                    }
                    RenderWebSupport.RenderTemplateWithContentToBlob(source, target);
                    return true;
                }
                // Don't copy source dir information objects
                if(source.GetBlobInformationType() == StorageSupport.InformationType_InformationObjectValue)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public static void RefreshContent(CloudBlob webPageBlob)
        {
            InformationSourceCollection sources = webPageBlob.GetBlobInformationSources();
            InformationSource templateSource = sources.CollectionContent.First(src => src.IsWebTemplateSource);
            CloudBlob templateBlob =
                StorageSupport.CurrActiveContainer.GetBlockBlobReference(templateSource.SourceLocation);
            RenderTemplateWithContentToBlob(templateBlob, webPageBlob);
        }
    }
}