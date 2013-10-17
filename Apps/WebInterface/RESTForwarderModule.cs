using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.WindowsAzure.StorageClient.Protocol;
using TheBall;

namespace WebInterface
{
    public class RESTForwarderModule : IHttpModule
    {
        /// <summary>
        /// You will need to configure this module in the Web.config file of your
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpModule Members

        public void Dispose()
        {
            //clean-up code here.
        }

        public void Init(HttpApplication context)
        {
            // Below is an example of how you can handle LogRequest event and provide 
            // custom logging implementation for it
            context.LogRequest += new EventHandler(OnLogRequest);
            context.BeginRequest += ContextOnBeginRequest;
            context.PreSendRequestContent += context_PreSendRequestContent;
            context.PreSendRequestHeaders += context_PreSendRequestHeaders;
        }

        void context_PreSendRequestHeaders(object sender, EventArgs e)
        {
            var resp = HttpContext.Current.Response;
        }

        void context_PreSendRequestContent(object sender, EventArgs e)
        {
            var resp = HttpContext.Current.Response;
        }

        private void ContextOnBeginRequest(object sender, EventArgs eventArgs)
        {
            ProcessRequest((HttpApplication) sender, HttpContext.Current);
        }

        #endregion

        public void OnLogRequest(Object source, EventArgs e)
        {
            //custom logging logic can go here
            int i = 0;
        }

        private bool isInitiated = false;

        public void ProcessRequest(HttpApplication app, HttpContext context)
        {
            isInitiated = true;
            //string user = context.User.Identity.Name;
            HttpRequest request = context.Request;
            if (request.Url.DnsSafeHost != "localdev" || request.UserHostAddress != "127.0.0.1") // To safeguard because of direct access to blob storage without credential verification right now
                return;
            verifySignature(request.Headers);

            var urlPathSplit = request.RawUrl.Split(new[] {"/REST/blob/"}, StringSplitOptions.None);
            string path = urlPathSplit.Length > 1 ? urlPathSplit[1] : urlPathSplit[0].Substring(1);
            var newUrl = "https://caloomdemo.blob.core.windows.net/" + path;
            HttpWebRequest newRequest = (HttpWebRequest)WebRequest.Create(newUrl);
            newRequest.ContentType = request.ContentType;
            newRequest.ContentLength = request.ContentLength;
            newRequest.UserAgent = request.UserAgent;
            newRequest.Method = request.HttpMethod;
            newRequest.SendChunked = false;
            foreach (var headerKey in request.Headers.AllKeys)
            {
                switch (headerKey)
                {
                    case "Connection":
                    case "Content-Length":
                    case "Date":
                    case "Expect":
                    case "Host":
                    case "If-Modified-Since":
                    case "Range":
                    case "Transfer-Encoding":
                    case "Proxy-Connection":
                        // Let IIS handle these
                        break;

                    case "Accept":
                    case "Content-Type":
                    case "Referer":
                    case "User-Agent":
                        // Restricted - copied below
                        break;

                    default:
                        newRequest.Headers.Add(headerKey, request.Headers[headerKey]);
                        break;
                }
            }
            string md5 = newRequest.Headers["Content-MD5"] ?? "";
            string ifMatch = newRequest.Headers["If-Match"] ?? "";
            constructNewSignature(request.HttpMethod, newRequest, newUrl, ifMatch, md5);
            if (request.HttpMethod != "GET"
                        && request.HttpMethod != "HEAD"
                        && request.ContentLength > 0)
            {
                var oldStream = request.GetBufferedInputStream();
                var newStream = newRequest.GetRequestStream();
                oldStream.CopyTo(newStream);
                newStream.Close();
            }
            Debug.WriteLine("Starting request with uri: " + path);
//            bool keepAlive = request.Headers["Connection"] == "Keep-Alive";
//            if (keepAlive)
//                newRequest.KeepAlive = true;
            HttpWebResponse newResponse = null;
            try
            {
                //Debugger.Break();
                newResponse = (HttpWebResponse) newRequest.GetResponse();
            }
            catch (WebException ex)
            {
                newResponse = (HttpWebResponse) ex.Response;
            }
            var response = context.Response;
            response.ClearHeaders();
            response.ContentType = newResponse.ContentType;
            //if(String.IsNullOrEmpty(newResponse.Con) == false)
            //    response.ContentEncoding = Encoding.GetEncoding(newResponse.ContentEncoding);
            response.StatusCode = (int) newResponse.StatusCode;
            foreach (var key in newResponse.Headers.AllKeys)
            {
                switch (key)
                {
                    case "Connection":
                    case "Content-Length":
                    case "Date":
                    case "Expect":
                    case "Host":
                    case "If-Modified-Since":
                    case "Range":
                    case "Transfer-Encoding":
                    case "Proxy-Connection":
                        // Let IIS handle these
                        break;
                    default:
                    response.Headers.Set(key, newResponse.Headers[key]);
                        break;
                }

            }
            var newRespStream = newResponse.GetResponseStream();
            var respStream = response.OutputStream;
            //Debug.Write("Fetching content...");
            //MemoryStream memStream = new MemoryStream();
            //newRespStream.CopyTo(memStream);
            //var byteContent = memStream.ToArray();
            //Debug.WriteLine("... fetched: " + byteContent.Length);
            //string strContent = Encoding.UTF8.GetString(byteContent);
            //BinaryWriter writer = new BinaryWriter(respStream);
            //writer.Write(byteContent);
            //writer.Flush();
            //response.AddHeader("Content-Length", (byteContent.Length / 2).ToString());
            newRespStream.CopyTo(respStream);
            response.Flush();
            //response.Close();
            //Thread.Sleep(10000);
            //response.Close();
            //app.CompleteRequest();
            response.End();
            //if(newRequest.KeepAlive == false)
            //response.End();
            //app.a
        }

        private void constructNewSignature(string method, HttpWebRequest request, string newUrl, string ifMatch, string md5)
        {
            var headers = request.Headers;
            string MessageSignature = String.Format("{0}\n\n\n{1}\n{5}\n\n\n\n{2}\n\n\n\n{3}{4}",
                   method,
                   (method == "GET" || method == "HEAD") ? String.Empty : request.ContentLength.ToString(),
                   ifMatch,
                   GetCanonicalizedHeaders(request.Headers),
                   GetCanonicalizedResource(new Uri(newUrl), InstanceConfiguration.AzureAccountName),
                   md5
                   );
            byte[] signatureBytes = Encoding.UTF8.GetBytes(MessageSignature);
            HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String(InstanceConfiguration.AzureStorageKey));
            var hash = hmac.ComputeHash(signatureBytes);
            string authorizationValue = "SharedKey " + InstanceConfiguration.AzureAccountName + ":" +
                                        Convert.ToBase64String(hash);
            headers["Authorization"] = authorizationValue;
        }

        public string GetCanonicalizedHeaders(NameValueCollection headers)
        {
            ArrayList headerNameList = new ArrayList();
            StringBuilder sb = new StringBuilder();
            foreach (string headerName in headers.Keys)
            {
                if (headerName.ToLowerInvariant().StartsWith("x-ms-", StringComparison.Ordinal))
                {
                    headerNameList.Add(headerName.ToLowerInvariant());
                }
            }
            headerNameList.Sort();
            foreach (string headerName in headerNameList)
            {
                StringBuilder builder = new StringBuilder(headerName);
                string separator = ":";
                foreach (string headerValue in GetHeaderValues(headers, headerName))
                {
                    string trimmedValue = headerValue.Replace("\r\n", String.Empty);
                    builder.Append(separator);
                    builder.Append(trimmedValue);
                    separator = ",";
                }
                sb.Append(builder.ToString());
                sb.Append("\n");
            }
            return sb.ToString();
        }

        public ArrayList GetHeaderValues(NameValueCollection headers, string headerName)
        {
            ArrayList list = new ArrayList();
            string[] values = headers.GetValues(headerName);
            if (values != null)
            {
                foreach (string str in values)
                {
                    list.Add(str.TrimStart(null));
                }
            }
            return list;
        }

        private bool IsTableStorage = false;

        public string GetCanonicalizedResource(Uri address, string accountName)
        {
            StringBuilder str = new StringBuilder();
            StringBuilder builder = new StringBuilder("/");
            builder.Append(accountName);
            builder.Append(address.AbsolutePath);
            str.Append(builder.ToString());
            NameValueCollection values2 = new NameValueCollection();
            if (!IsTableStorage)
            {
                NameValueCollection values = HttpUtility.ParseQueryString(address.Query);
                foreach (string str2 in values.Keys)
                {
                    ArrayList list = new ArrayList(values.GetValues(str2));
                    list.Sort();
                    StringBuilder builder2 = new StringBuilder();
                    foreach (object obj2 in list)
                    {
                        if (builder2.Length > 0)
                        {
                            builder2.Append(",");
                        }
                        builder2.Append(obj2.ToString());
                    }
                    values2.Add((str2 == null) ? str2 : str2.ToLowerInvariant(), builder2.ToString());
                }
            }
            ArrayList list2 = new ArrayList(values2.AllKeys);
            list2.Sort();
            foreach (string str3 in list2)
            {
                StringBuilder builder3 = new StringBuilder(string.Empty);
                builder3.Append(str3);
                builder3.Append(":");
                builder3.Append(values2[str3]);
                str.Append("\n");
                str.Append(builder3.ToString());
            }
            return str.ToString();
        }



        private void verifySignature(NameValueCollection headers)
        {
        }

    }

}
