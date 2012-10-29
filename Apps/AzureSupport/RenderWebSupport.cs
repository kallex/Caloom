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
        private const string DynamicRootTagBegin = "<!-- THEBALL-CONTEXT-DYNAMICROOT-BEGIN:";
        private const string CollectionTagBegin = "<!-- THEBALL-CONTEXT-COLLECTION-BEGIN:";
        private const string ObjectTagBegin = "<!-- THEBALL-CONTEXT-OBJECT-BEGIN:";
        private const string CommentEnd = " -->";
        private const string ContextTagEnd = "<!-- THEBALL-CONTEXT-END:";
        private const string TheBallPrefix = "<!-- THEBALL-";
        private const string AtomTagBegin = "[!ATOM]";
        private const string AtomTagEnd = "[ATOM!]";
        private const string MemberAtomPattern = @"(?<fulltag>\[!ATOM](?<membername>[^[]*)\[ATOM!])";
        private const string SpanTagCollectionBeginFormat = @"<span id='{0}' data-id='{0}' data-etag='{1}' class='theballcollection'>";
        private const string SpanTagCollectionItemBeginFormat = @"<span id='{0}' data-id='{0}' data-etag='{1}' class='theballcollitem'>";
        private const string SpanTagItemBeginFormat = @"<span id='{0}' data-id='{0}' data-etag='{1}' class='theballitem'>";
        //private const string SpanTagClosing = @"</span>";
        private const string SpanTagClosing = @"";

        public const string DefaultWebTemplateLocation = "webtemplate";
        public const string DefaultWebSiteLocation = "website";
        public const string DefaultPublicWebTemplateLocation = "publictemplate";

        public const string DefaultPublicWebSiteLocation = "publicsite";
        public const string DefaultAccountTemplates = DefaultWebTemplateLocation + "/account";
        public const string DefaultGroupTemplates = DefaultWebTemplateLocation + "/group";
        public const string DefaultPublicTemplates = DefaultWebTemplateLocation + "/public";
        public const string DefaultGroupViewLocation = "oip-group";
        public const string DefaultAccountViewLocation = "oip-account";
        public const string DefaultPublicViewLocation = "oip-public";
        public const string DefaultGroupID = "9798daca-afc4-4046-a99b-d0d88bb364e0";
        // https://theball.blob.core.windows.net/00000000-0000-0000-0000-000000000000/grp/9798daca-afc4-4046-a99b-d0d88bb364e0/AaltoGlobalImpact.OIP/AboutContainer/default

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
            ContextRootRegex = new Regex(@"THEBALL-CONTEXT-(?<bindingType>DYNAMIC|)ROOT-BEGIN:(?<rootType>[\w_\.]*)(?:\:(?<rootName>[\w_]*)|)", RegexOptions.Compiled);
        }

        public class ContentItem
        {
            public InformationSource Source;
            public string RootName;
            public string RootType;
            public bool IsDynamicRoot;
            public object RootObject;
            public bool WasMissing = false;
            public bool WasNeeded = false;
        }

        public static void RenderTemplateWithContentToBlob(CloudBlob template, CloudBlob renderTarget, InformationSource setAsDefaultSource = null)
        {
            InformationSourceCollection sources = renderTarget.GetBlobInformationSources();
            if(sources == null)
            {
                sources = CreateDefaultSources(template);
            }
            if (setAsDefaultSource != null)
            {
                sources.SetDefaultSource(setAsDefaultSource);
            }
            string templateContent = template.DownloadText();
            List<ContentItem> existingRoots = GetExistingRoots(sources);
            string renderResult = RenderTemplateWithContentRoots(templateContent, existingRoots);
            bool rerenderRequired = UpdateMismatchedRootsToSources(sources, existingRoots, renderTarget);
            renderTarget.SetBlobInformationSources(sources);
            renderTarget.UploadBlobText(renderResult, StorageSupport.InformationType_RenderedWebPage);
            if(rerenderRequired)
            {
                RenderTemplateWithContentToBlob(template, renderTarget);
            } else
            {
                sources.SubscribeTargetToSourceChanges(renderTarget);
            }
        }

        /// <summary>
        /// Updates mismatched (new and removed) roots to sources
        /// </summary>
        /// <param name="sources">Information sources</param>
        /// <param name="existingRoots">Current roots</param>
        /// <param name="renderTarget">Render target</param>
        /// <returns>True, if existing sources were spotted and re-rendering is required</returns>
        private static bool UpdateMismatchedRootsToSources(InformationSourceCollection sources, List<ContentItem> existingRoots, CloudBlob renderTarget)
        {
            var newSources =
                existingRoots.Where(root => root.WasMissing).Select(root =>
                                                                        {
                                                                            bool foundExistingSource;
                                                                            var source = GetMissingRootAsNewSource(
                                                                                root, renderTarget.Name,
                                                                                out foundExistingSource);
                                                                            // If error, returns null
                                                                            if (source == null)
                                                                                return null;
                                                                            return new
                                                                                       {
                                                                                           Source = source,
                                                                                           FoundExistingSource =
                                                                                               foundExistingSource
                                                                                       };
                                                                        }
                    ).Where(res => res != null).ToArray();
            sources.CollectionContent.AddRange(newSources.Select(item => item.Source));
            foreach (var item in existingRoots.Where(root => root.WasNeeded == false))
                sources.CollectionContent.Remove(item.Source);
            // Don't delete the missing blobs just now
            return newSources.Any(source => source.FoundExistingSource);
        }

        private static InformationSource GetMissingRootAsNewSource(ContentItem root, string masterLocation, out bool foundExistingSource)
        {
            InformationSource source = root.Source ?? InformationSource.CreateDefault();
            IInformationObject informationObject = (IInformationObject) root.RootObject;
            if (informationObject == null)
            {
                foundExistingSource = false;
                return null;
            }
            string sourceContentLocation = informationObject.GetLocationRelativeToContentRoot(masterLocation,
                                                                                              root.RootName);
            CloudBlob blob;
            IInformationObject existingObject = StorageSupport.RetrieveInformationWithBlob(sourceContentLocation,
                                                                                           root.RootType, out blob);
            if(existingObject == null)
            {
                informationObject.SetLocationRelativeToContentRoot(masterLocation, root.RootName);
                blob = StorageSupport.StoreInformation(informationObject);
                foundExistingSource = false;
            }
            else
            {
                informationObject = existingObject;
                root.RootObject = existingObject;
                foundExistingSource = true;
            }
            source.SetBlobValuesToSource(blob);
            source.SetInformationObjectValuesToSource(root.RootName, informationObject.GetType().FullName);
            source.IsDynamic = root.IsDynamicRoot;
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
                                              IsDynamicRoot = source.IsDynamic
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
                    if (item == null)
                        errorItem.CurrentContextName = "No active context";
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
            if (line.StartsWith(RootTagBegin) || line.StartsWith(DynamicRootTagBegin))
            {
                result.AppendLine(line);
                object content;
                StackContextItem rootItem;
                Match match = ContextRootRegex.Match(line);
                string rootType = match.Groups["rootType"].Value;
                string rootName = match.Groups["rootName"].Value;
                string bindingType = match.Groups["bindingType"].Value;
                bool isDynamicRoot = bindingType == "DYNAMIC";
                try
                {
                    //string typeName = line.Substring(RootTagLen, line.Length - RootTagsTotalLen).Trim();
                    content = GetOrInitiateContentObject(contentRoots, rootType, rootName, isDynamicRoot);
                    StackContextItem parent = contextStack.Count > 0 ? contextStack.Peek() : null;
                    rootItem = new StackContextItem(content, parent, content.GetType(), null, true, false, rootName);
                }
                catch
                {
                    rootItem = new StackContextItem("Invalid Stack Root Item", null, typeof(string), "INVALID", true, false, "");
                    errorList.Add(new ErrorItem(new Exception("Invalid stack root: " + rootType), rootItem, line));
                }
                result.AppendLine(GetSpanTag(rootItem.CurrContent, SpanTagItemBeginFormat));
                // NOTE! We support multiple roots now, but root has to be first in stack
                //if (contextStack.Count != 0)
                //    throw new InvalidDataException("Context stack already has a root item before: " + content.GetType().FullName);
                contextStack.Push(rootItem);
            }
            else if (line.StartsWith(CollectionTagBegin))
            {
                result.AppendLine(line);
                requireExistingContext(contextStack, line);
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
                    StackContextItem item = contextStack.Count > 0 ? contextStack.Peek() : null;
                    ErrorItem errorItem = new ErrorItem(ex, item, line);
                    if (item == null)
                        errorItem.CurrentContextName = "No active context";
                    errorList.Add(errorItem);
                    if (collItem == null)
                        collItem = new StackContextItem("Invalid Collection Context", contextStack.Peek(), typeof(string), "INVALID", false, true);
                }
                collStack.Push(collItem);
                int scopeStartIx = currLineIx + 1;
                int scopeEndIx = lines.Length; // Candidate, the lower call will adjust properly
                // Get scope length
                result.AppendLine(GetSpanTag(collItem.CollectionContainer, SpanTagCollectionBeginFormat));
                ProcessLinesScope(lines, scopeStartIx, ref scopeEndIx, new StringBuilder(), contentRoots, collStack,
                                  new List<ErrorItem>());
                // Scope goes to end context tag, so it pops the item back from scope - let's push it back
                // ... or not, commented below for a while
                //scopeEndIx--;
                //collStack.Push(collItem);
                bool isFirstRound = true;
                while (collItem.IsNotFullyProcessed)
                {
                    var currErrorList = isFirstRound ? errorList : new List<ErrorItem>();
                    collStack.Push(collItem);
                    string spanTag = GetSpanTag(collItem.CurrContent, SpanTagCollectionItemBeginFormat);
                    result.AppendLine(spanTag);
                    ProcessLinesScope(lines, scopeStartIx, ref scopeEndIx, result, contentRoots, collStack,
                                        currErrorList);
                    //result.AppendLine(SpanTagClosing);
                    if(collStack.Count > 0)
                        throw new InvalidDataException("Collection stack should be empty at this point");
                    isFirstRound = false;
                    collItem.CurrCollectionItem++;
                }
                result.AppendLine(SpanTagClosing);
                // Jump to the end tag (as the next loop will progress by one)
                currLineIx = scopeEndIx - 1;
            }
            else if (line.StartsWith(ObjectTagBegin))
            {
                result.AppendLine(line);
                requireExistingContext(contextStack, line);
                string memberName = line.Substring(ObjTagLen, line.Length - ObjTagsTotalLen).Trim();
                StackContextItem currItem = PushMemberObjectToStack(contextStack, memberName);
                result.AppendLine(GetSpanTag(currItem.CurrContent, SpanTagItemBeginFormat));
            }
            else if (line.StartsWith(ContextTagEnd))
            {
                result.AppendLine(SpanTagClosing);
                result.AppendLine(line);
                StackContextItem popItem = contextStack.Pop();
                if (contextStack.Count == 0)
                    return false;
            }
            else if(line.Contains(FormHiddenFieldTag))
            {
                requireExistingContext(contextStack, line);
                result.AppendLine(line);
                /*
                ProcessATOMLine(
                    "<input id=\"RootObjectRelativeLocation\" name=\"RootObjectRelativeLocation\" type=\"hidden\" value=\"[!ATOM]RelativeLocation[ATOM!]\" />",
                    result, contextStack);
                ProcessATOMLine(
                    "<input id=\"RootObjectType\" name=\"RootObjectType\" type=\"hidden\" value=\"[!ATOM]SemanticDomainName[ATOM!].[!ATOM]Name[ATOM!]\" />",
                    result, contextStack);
                ProcessATOMLine(
                    "<input id=\"RootSourceName\" name=\"RootSourceName\" type=\"hidden\" value=[!ATOM]ETag[ATOM!] />",
                    result, contextStack);*/
                StackContextItem currContext = contextStack.Peek().GetContextRoot();
                result.AppendLine(
                    String.Format(
                        "<input id=\"RootSourceName\" name=\"RootSourceName\" type=\"hidden\" value=\"{0}\" />",
                        currContext.RootName));

            }
            else // ATOM line
            {
                ProcessATOMLine(line, result, contextStack);
            }
            return contextStack.Count > 0;
        }

        private static string GetSpanTag(object content, string spanTagFormatForIDandETag)
        {
            return "";
            IInformationObject informationObject = content as IInformationObject;
            string currID;
            string currEtag;
            if(informationObject != null)
            {
                currID = informationObject.ID;
                if(informationObject.ETag != null)
                    currEtag = informationObject.ETag.Substring(1, informationObject.ETag.Length - 2);
                else
                    currEtag = "";
            } else
            {
                currID = "";
                currEtag = "";
            }
            return String.Format(spanTagFormatForIDandETag, currID, currEtag);
        }

        private static StackContextItem PushMemberObjectToStack(Stack<StackContextItem> contextStack, string memberName)
        {
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
            return contextStack.Peek();
        }

        private static void requireExistingContext(Stack<StackContextItem> contextStack, string line)
        {
            if (contextStack.Count == 0)
                throw new InvalidDataException("No context (stack) available before using it: " + line);
        }

        public static object GetOrInitiateContentObject(List<ContentItem> contentRoots, string rootType, string rootName, bool isDynamicRoot)
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
                                      WasMissing = true,
                                      IsDynamicRoot = isDynamicRoot
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
                        if(createdType == null)
                            throw new InvalidDataException("Invalid RootObjectType: " + rootType);
                        object result = createdType.InvokeMember("CreateDefault", BindingFlags.InvokeMethod, null, null,
                                                                 null);
                        contentItem.RootObject = result;
                    }
                }
            }
            contentItem.WasNeeded = true;
            // Even if used elsewhere, the use of dynamic root switches whole source item as dynamic
            if (isDynamicRoot)
            {
                contentItem.IsDynamicRoot = true;
                if(contentItem.Source != null)
                    contentItem.Source.IsDynamic = true;
            }
                
            return contentItem.RootObject;
        }

        private static void ProcessATOMLine(string line, StringBuilder result, Stack<StackContextItem> contextStack)
        {
            requireExistingContext(contextStack, line);
            StackContextItem currCtx = contextStack.Peek();
            var contentLine = Regex.Replace(line, MemberAtomPattern,
                                            match =>
                                                {
                                                    string memberName = match.Groups["membername"].Value;
                                                    var workCtx = currCtx;
                                                    if(memberName.Contains("."))
                                                    {
                                                        string[] referenceList = memberName.Split('.');
                                                        Stack<StackContextItem> workStack = new Stack<StackContextItem>(referenceList.Length);
                                                        workStack.Push(currCtx);
                                                        int currIx;
                                                        for(currIx = 0; currIx < referenceList.Length - 1; currIx++)
                                                            PushMemberObjectToStack(workStack, referenceList[currIx]);
                                                        workCtx = workStack.Peek();
                                                        memberName = referenceList[currIx];
                                                    }
                                                    object value = GetPropertyValue(workCtx, memberName);
                                                    return (value ?? (workCtx.MemberName + "." + memberName + " is null")).ToString();
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
                                       tableRow.Name, tableRow.ErrorMessage, tableRow.CurrLine).AppendLine();
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
            if(pi == null)
                throw new InvalidDataException(String.Format("No InformationItem '{0}' found in InformationObject '{1}'", propertyName, type.Name));
            return pi.GetValue(currCtx.CurrContent, null);
        }

        private static Type GetMemberType(StackContextItem containingItem, string memberName)
        {
            Type containingType = containingItem.ItemType;
            PropertyInfo pi = containingType.GetProperty(memberName);
            if(pi == null)
                throw new InvalidDataException("InformationObject: " + containingType.Name + " does not contain InformationItem with name: " + memberName);
            return pi.PropertyType;
        }

        public static bool CopyAsIsSyncHandler(CloudBlob source, CloudBlob target, WorkerSupport.SyncOperationType operationtype)
        {
            return false;
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

        public static void RefreshContent(CloudBlob webPageBlob, bool skipIfSourcesIntact = false)
        {
            InformationSourceCollection sources = webPageBlob.GetBlobInformationSources();
            if(skipIfSourcesIntact)
            {
                bool sourcesIntact = sources.HasAnySourceChanged() == false;
                if (sourcesIntact)
                    return;
            }
            InformationSource templateSource = sources.CollectionContent.First(src => src.IsWebTemplateSource);
            CloudBlob templateBlob =
                StorageSupport.CurrActiveContainer.GetBlockBlobReference(templateSource.SourceLocation);
            RenderTemplateWithContentToBlob(templateBlob, webPageBlob);
        }

        public static OperationRequest RefreshDefaultViews(string viewLocation, string fullTypeName, bool useQueuedWorker)
        {
                OperationRequest operationRequest = new OperationRequest
                                                        {
                                                            RefreshDefaultViewsOperation = new RefreshDefaultViewsOperation
                                                                                               {
                                                                                                   ViewLocation = viewLocation,
                                                                                                   TypeNameToRefresh = fullTypeName
                                                                                               }
                                                        };
            if(useQueuedWorker)
            {
                return operationRequest;
            }
            WorkerSupport.RefreshDefaultViews(operationRequest.RefreshDefaultViewsOperation);
            return null;
        }
        
        public static OperationRequest SyncTemplatesToSite(string sourceContainerName, string sourcePathRoot, string targetContainerName, string targetPathRoot, bool useQueuedWorker, bool renderWhileSync)
        {
            if (useQueuedWorker)
            {
                OperationRequest envelope = new OperationRequest
                {
                    UpdateWebContentOperation = new UpdateWebContentOperation
                    {
                        SourceContainerName =
                            sourceContainerName,
                        SourcePathRoot = sourcePathRoot,
                        TargetContainerName =
                            targetContainerName,
                        TargetPathRoot = targetPathRoot,
                        RenderWhileSync = renderWhileSync
                    }
                };
                //QueueSupport.PutToDefaultQueue(envelope);
                return envelope;
            }
            else
            {
                WorkerSupport.WebContentSync(sourceContainerName, sourcePathRoot, targetContainerName, targetPathRoot, renderWhileSync ? (WorkerSupport.PerformCustomOperation)RenderWebSupport.RenderingSyncHandler : (WorkerSupport.PerformCustomOperation)RenderWebSupport.CopyAsIsSyncHandler);
                return null;
            }
        }


        public static void RefreshAllAccountAndGroupTemplates(bool useWorker, params string[] viewTypesToRefresh)
        {
            refreshAllGroupTemplates(useWorker, viewTypesToRefresh);
            refreshAllAccountTemplates(useWorker, viewTypesToRefresh);
        }

        private static void refreshAllGroupTemplates(bool useWorker, params string[] viewTypesToRefresh)
        {
            string[] groupIDs = TBRGroupRoot.GetAllGroupIDs();
            foreach (var grpID in groupIDs)
            {
                RefreshGroupTemplates(grpID, useWorker, viewTypesToRefresh);
            }
     //       RenderWebSupport.SyncTemplatesToSite(StorageSupport.CurrActiveContainer.Name,
     //String.Format("grp/f8e1d8c6-0000-467e-b487-74be4ad099cd/{0}/", privateSiteLocation),
     //StorageSupport.CurrAnonPublicContainer.Name,
     //                String.Format("grp/default/{0}/", publicSiteLocation), true, true);

        }

        public static void RefreshGroupTemplates(string grpID, bool useWorker, params string[] viewTypesToRefresh)
        {
            string syscontentRoot = "sys/AAA/";
            string currContainerName = StorageSupport.CurrActiveContainer.Name;
            string anonContainerName = StorageSupport.CurrAnonPublicContainer.Name;
            string groupTemplateLocation = "grp/" + grpID + "/" + DefaultWebTemplateLocation;
            string groupSiteLocation = "grp/" + grpID + "/" + DefaultWebSiteLocation;
            string groupSiteViewLocation = groupSiteLocation + "/" + DefaultGroupViewLocation;
            string groupPublicTemplateLocation = "grp/" + grpID + "/" + DefaultPublicWebTemplateLocation;
            string groupPublicSiteLocation = "grp/" + grpID + "/" + DefaultPublicWebSiteLocation;
            string groupPublicViewLocation = groupPublicSiteLocation + "/" + DefaultPublicViewLocation;
            string defaultPublicSiteLocation = "grp/default/" + DefaultPublicWebSiteLocation;
                
            // Sync to group local template
            List<OperationRequest> operationRequests = new List<OperationRequest>();
            var localGroupTemplates = SyncTemplatesToSite(currContainerName, syscontentRoot + DefaultGroupTemplates, currContainerName, groupTemplateLocation, useWorker, false);
            // Render local template
            var renderLocalTemplates = SyncTemplatesToSite(currContainerName, groupTemplateLocation, currContainerName, groupSiteLocation, useWorker, true);
            // Sync public pages to group local template
            var publicGroupTemplates = SyncTemplatesToSite(currContainerName, syscontentRoot + DefaultPublicTemplates, currContainerName, groupPublicTemplateLocation, useWorker, false);
            // Render local template
            var renderPublicTemplates = SyncTemplatesToSite(currContainerName, groupPublicTemplateLocation, currContainerName, groupPublicSiteLocation, useWorker, true);
            operationRequests.Add(localGroupTemplates);
            operationRequests.Add(renderLocalTemplates);
            operationRequests.Add(publicGroupTemplates);
            operationRequests.Add(renderPublicTemplates);
            foreach(string viewTypeToRefresh in viewTypesToRefresh)
            {
                OperationRequest refreshOp = RefreshDefaultViews(groupSiteViewLocation, viewTypeToRefresh, useWorker);
                operationRequests.Add(refreshOp);
                refreshOp = RefreshDefaultViews(groupPublicViewLocation, viewTypeToRefresh, useWorker);
                operationRequests.Add(refreshOp);
            }
            // Publish group public content
            var publishPublicContent = SyncTemplatesToSite(currContainerName, groupPublicSiteLocation, anonContainerName, groupPublicSiteLocation, useWorker, false);
            OperationRequest publishDefault = null;
            if (grpID == DefaultGroupID)
            {
                publishDefault = SyncTemplatesToSite(currContainerName, groupPublicSiteLocation, anonContainerName, defaultPublicSiteLocation, useWorker, false);
            }
            operationRequests.Add(publishPublicContent);
            if(useWorker)
            {
                //QueueSupport.PutToOperationQueue(localGroupTemplates, renderLocalTemplates);
                QueueSupport.PutToOperationQueue(operationRequests.ToArray());
            }
        }

        private static void refreshAllAccountTemplates(bool useWorker, params string[] viewTypesToRefresh)
        {
            string[] accountIDs = TBRAccountRoot.GetAllAccountIDs();
            foreach(var acctID in accountIDs)
            {
                RefreshAccountTemplates(acctID, useWorker, viewTypesToRefresh);
            }
        }

        public static void RefreshAccountTemplates(string acctID, bool useWorker, params string[] viewTypesToRefresh)
        {
            string currContainerName = StorageSupport.CurrActiveContainer.Name;
            string syscontentRoot = "sys/AAA/";
            string acctTemplateLocation = "acc/" + acctID + "/" + DefaultWebTemplateLocation;
            string acctSiteLocation = "acc/" + acctID + "/" + DefaultWebSiteLocation;
            string acctViewLocation = acctSiteLocation + "/" + DefaultAccountViewLocation;

            // Sync to account local template
            var accountLocalTemplate = SyncTemplatesToSite(currContainerName, syscontentRoot + DefaultAccountTemplates, currContainerName, acctTemplateLocation, useWorker, false);
            // Render local template
            var renderLocalTemplate = SyncTemplatesToSite(currContainerName, acctTemplateLocation, currContainerName, acctSiteLocation, useWorker, true);
            List<OperationRequest> operationRequests = new List<OperationRequest>();
            operationRequests.Add(accountLocalTemplate);
            operationRequests.Add(renderLocalTemplate);
            foreach(string viewTypeToRefresh in viewTypesToRefresh)
            {
                OperationRequest refreshOp = RefreshDefaultViews(acctViewLocation, viewTypeToRefresh, useWorker);
                operationRequests.Add(refreshOp);
            }
            if(useWorker)
            {
                QueueSupport.PutToOperationQueue(operationRequests.ToArray());
            }
        }

        public static string GetUrlFromRelativeLocation(string relativeLocation)
        {
            if (relativeLocation.StartsWith("grp/"))
                return "/auth/" + relativeLocation;
            if(relativeLocation.StartsWith("acc/"))
            {
                string result = "/auth/account" + relativeLocation.Substring(4 + StorageSupport.GuidLength);
                return result;
            }
            return relativeLocation;
        }
    }
}