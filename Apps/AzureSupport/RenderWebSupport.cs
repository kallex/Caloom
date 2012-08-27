using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace TheBall
{
    public static class RenderWebSupport
    {
        private const string RootTagBegin = "<!-- THEBALL-CONTEXT-ROOT-BEGIN:";
        private const string CollectionTagBegin = "<!-- THEBALL-CONTEXT-COLLECTION-BEGIN:";
        private const string ObjectTagBegin = "<!-- THEBALL-CONTEXT-OBJECT-BEGIN:";
        private const string CommentEnd = " -->";
        private const string ContextTagEnd = "<!-- THEBALL-CONTEXT-END:";
        private const string TheBallPrefix = "<!-- THEBALL-";
        private const string AtomTagBegin = "[!ATOM]";
        private const string AtomTagEnd = "[ATOM!]";
        private const string MemberAtomPattern = @"(?<fulltag>\[!ATOM](?<membername>\w*)\[ATOM!])";
        public static string RenderTemplateWithContent(string templatePage, object content)
        {
            StringReader reader = new StringReader(templatePage);
            StringBuilder result = new StringBuilder(templatePage.Length);
            Stack<StackContextItem> contextStack = new Stack<StackContextItem>();
            int rootTagLen = RootTagBegin.Length;
            int rootTagsTotalLen = RootTagBegin.Length + CommentEnd.Length;
            int collTagLen = CollectionTagBegin.Length;
            int collTagsTotalLen = CollectionTagBegin.Length + CommentEnd.Length;
            int objTagLen = ObjectTagBegin.Length;
            int objTagsTotalLen = ObjectTagBegin.Length + CommentEnd.Length;
            int atomTagLen = AtomTagBegin.Length;
            int atomTagsTotalLen = AtomTagBegin.Length + AtomTagEnd.Length;
            List<ErrorItem> errorList = new List<ErrorItem>();

            for(string line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                try
                {
                    if (IgnoreBindingsTillEnd(line))
                    {
                        break;
                    }

                    if (line.StartsWith(TheBallPrefix) == false && line.Contains("[!ATOM]") == false)
                    {
                        result.AppendLine(line);
                        continue;
                    }
                    if (line.StartsWith(RootTagBegin))
                    {
                        // TODO: Multiple container support; type and instance ID mapping
                        string typeName = line.Substring(rootTagLen, line.Length - rootTagsTotalLen).Trim();
                        StackContextItem rootItem = new StackContextItem(content, content.GetType(), null, true, false);
                        if (contextStack.Count != 0)
                            throw new InvalidDataException("Context stack already has a root item before: " + typeName);
                        contextStack.Push(rootItem);
                    }
                    else if (line.StartsWith(CollectionTagBegin))
                    {
                        string memberName = line.Substring(collTagLen, line.Length - collTagsTotalLen).Trim();
                        StackContextItem currCtx = contextStack.Peek();
                        StackContextItem collItem = null;
                        try
                        {
                            Type type = GetMemberType(currCtx, memberName);
                            object contentValue = GetPropertyValue(currCtx, memberName);
                            collItem = new StackContextItem(contentValue, type, memberName, false, true);
                        } finally
                        {
                            if (collItem == null)
                                collItem = new StackContextItem("Invalid Collection Context", typeof(string), "INVALID", false, true);
                            contextStack.Push(collItem);
                        }
                    }
                    else if (line.StartsWith(ObjectTagBegin))
                    {
                        string memberName = line.Substring(objTagLen, line.Length - objTagsTotalLen).Trim();
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
                                objItem = new StackContextItem(contentValue, type, memberName, false, false);
                            } finally
                            {
                                if(objItem == null)
                                    objItem = new StackContextItem("Invalid Context", typeof(string), "INVALID", false, false);
                                contextStack.Push(objItem);
                            }
                        }
                    }
                    else if (line.StartsWith(ContextTagEnd))
                    {
                        contextStack.Pop();
                    }
                    else // ATOM line
                    {
                        StackContextItem currCtx = contextStack.Peek();
                        bool isCollection = false;
                        if (currCtx.IsCollection == true)
                            isCollection = true;
                        var contentLine = Regex.Replace(line, MemberAtomPattern,
                                      match =>
                                      {
                                          string memberName = match.Groups["membername"].Value;
                                          object value = GetPropertyValue(currCtx, memberName);
                                          return (value ?? (currCtx.MemberName + "." + memberName + " is null")).ToString();
                                      });
                        result.AppendLine(contentLine);
                    }
                } catch(Exception ex)
                {
                    StackContextItem item = contextStack.Count > 0 ? contextStack.Peek() : null;
                    ErrorItem errorItem = new ErrorItem(ex, item, line);
                    errorList.Add(errorItem);
                }
            }
            if(errorList.Count > 0)
            {
                result.Insert(0, RenderErrorListAsHtml(errorList, "Errors - Databinding"));
            }
            return result.ToString();
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
            if(currCtx.Content == null)
                throw new InvalidDataException("Object: " + currCtx.MemberName + " does not have content (was retrieving value: " + propertyName + ")");
            Type type = currCtx.ItemType;
            PropertyInfo pi = type.GetProperty(propertyName);
            return pi.GetValue(currCtx.Content, null);
        }

        private static Type GetMemberType(StackContextItem containingItem, string memberName)
        {
            Type containingType = containingItem.ItemType;
            PropertyInfo pi = containingType.GetProperty(memberName);
            if(pi == null)
                throw new InvalidDataException("Type: " + containingType.Name + " does not contain property: " + memberName);
            return pi.PropertyType;
        }
    }
}