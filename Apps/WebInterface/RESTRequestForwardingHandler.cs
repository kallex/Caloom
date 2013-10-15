using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;
using AaltoGlobalImpact.OIP;
using DotNetOpenAuth.OpenId.RelyingParty;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.StorageClient.Protocol;
using TheBall;

namespace WebInterface
{
    public class RESTRequestForwardingHandler : IHttpHandler
    {
        /// <summary>
        /// You will need to configure this handler in the web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new NotImplementedException("Not securely implemented yet! - Issues with keepalive by Azure clients");
            string user = context.User.Identity.Name;
            HttpRequest request = context.Request;
            verifySignature(request.Headers);

            var urlPathSplit = request.RawUrl.Split(new[] {"/REST/blob/"}, StringSplitOptions.None);
            string path = urlPathSplit[1];
            var newUrl = "https://caloomdemo.blob.core.windows.net/" + path;
            HttpWebRequest newRequest = (HttpWebRequest)WebRequest.Create(newUrl);
            constructNewSignature(request.HttpMethod, request, newUrl);
            newRequest.ContentType = request.ContentType;
            newRequest.ContentLength = request.ContentLength;
            newRequest.UserAgent = request.UserAgent;
            newRequest.KeepAlive = request.Headers["Connection"] == "Keep-Alive";
            if (request.HttpMethod != "GET"
                        && request.HttpMethod != "HEAD"
                        && request.ContentLength > 0)
            {
                var oldStream = request.GetBufferedInputStream();
                var newStream = newRequest.GetRequestStream();
                oldStream.CopyTo(newStream);
                newStream.Close();
            }
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
            //newRequest.KeepAlive = false; //THIS DOES THE TRICK
            //newRequest.ProtocolVersion = HttpVersion.Version10;
            Debug.WriteLine("Starting request with uri: " + path);
            var newResponse = (HttpWebResponse)newRequest.GetResponse();
            var response = context.Response;
            response.ClearHeaders();
            response.ContentType = newResponse.ContentType;
            if(String.IsNullOrEmpty(newResponse.ContentEncoding) == false)
                response.ContentEncoding = Encoding.GetEncoding(newResponse.ContentEncoding);
            response.StatusCode = (int) newResponse.StatusCode;
            var newRespStream = newResponse.GetResponseStream();
            var respStream = response.OutputStream;
            foreach (var key in newResponse.Headers.AllKeys)
            {
                response.Headers.Set(key, newResponse.Headers[key]);
            }
            Debug.Write("Fetching content...");
//            if(newRequest.KeepAlive)
//                response.Headers.Set("Connection", "close");
            MemoryStream memStream = new MemoryStream();
            newRespStream.CopyTo(memStream);
            var byteContent = memStream.ToArray();
            Debug.WriteLine("... fetched: " + byteContent.Length);
            string strContent = Encoding.UTF8.GetString(byteContent);
            //newRespStream.CopyTo(respStream);
            BinaryWriter writer = new BinaryWriter(respStream);
            writer.Write(byteContent);
            Debug.WriteLine("Served with keepalive: " + newRequest.KeepAlive);
            Debug.WriteLine(response.IsClientConnected);
            response.Flush();
            Debug.WriteLine(response.IsClientConnected);
            //response.Close();
            Thread.Sleep(10000);
            Debug.WriteLine("Served request... with connection: " + newResponse.Headers["Connection"]);
            //response.End();
        }

        private void constructNewSignature(string method, HttpRequest request, string newUrl, string ifMatch = "", string md5 = "")
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

        #endregion
    }

    public class MyAzureStrategy : CanonicalizationStrategy
    {
        public override string CanonicalizeHttpRequest(HttpWebRequest request, string accountName)
        {
            throw new NotImplementedException();
        }

        public static string GetCanonicalizedResourceHelper(string url, string azureAccountName)
        {
            return GetCanonicalizedResourceVersion2(new Uri(url), azureAccountName);
        }
    }
}
