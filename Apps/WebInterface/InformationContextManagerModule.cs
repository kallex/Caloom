using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Web;
using AzureSupport;
using TheBall;
using TheBall.CORE;

namespace WebInterface
{
    public class InformationContextManagerModule : IHttpModule
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
            //context.LogRequest += new EventHandler(OnLogRequest);
            context.BeginRequest += ContextOnBeginRequest;
            context.EndRequest += ContextOnEndRequest;
            context.PreRequestHandlerExecute += ContextOnPreRequestHandlerExecute;
        }

        private void ContextOnPreRequestHandlerExecute(object sender, EventArgs eventArgs)
        {
            var request = HttpContext.Current.Request;
            if (request.Path.StartsWith("/websocket/"))
                return;
            var reqDetails = InformationContext.Current.RequestResourceUsage.RequestDetails;
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                reqDetails.UserID =
                    HttpContext.Current.User.Identity.Name;
            }
            else
            {
                reqDetails.UserID = "anonymous";
            }
        }

        private void ContextOnBeginRequest(object sender, EventArgs eventArgs)
        {
            var request = HttpContext.Current.Request;
            if (request.Path.StartsWith("/websocket/"))
                return;
            WebSupport.InitializeContextStorage(HttpContext.Current.Request);
            InformationContext.StartResourceMeasuringOnCurrent();
            SetRequestLoggingData(request, InformationContext.Current.RequestResourceUsage.RequestDetails);
            var application = (HttpApplication)sender;
            application.Response.Filter = new ContentLengthFilter(application.Response.Filter);
        }

        private void SetRequestLoggingData(HttpRequest request, HTTPActivityDetails requestDetails)
        {
            requestDetails.RemoteIPAddress = request.UserHostAddress;
            requestDetails.RemoteEndpointUserName = "-";
            requestDetails.UTCDateTime = DateTime.UtcNow;
            requestDetails.RequestLine = String.Format("{0} {1} {2}", request.HttpMethod, request.Url.AbsoluteUri, request.ServerVariables["SERVER_PROTOCOL"]);
        }

        private void ContextOnEndRequest(object sender, EventArgs eventArgs)
        {
            var request = HttpContext.Current.Request;
            if (request.Path.StartsWith("/websocket/"))
                return;
            var application = (HttpApplication)sender;
            var response = application.Response;
            var contentLengthFilter = (ContentLengthFilter)response.Filter;
            var contentLength = contentLengthFilter.BytesWritten;
            Debug.WriteLine("Content length sent: " + contentLength);
            InformationContext.AddNetworkOutputByteAmountToCurrent(contentLength);
            var reqDetails = InformationContext.Current.RequestResourceUsage.RequestDetails;
            reqDetails.HTTPStatusCode = response.StatusCode;
            reqDetails.ReturnedContentLength = contentLength;
            InformationContext.ProcessAndClearCurrent();
        }

        #endregion

        public void OnLogRequest(Object source, EventArgs e)
        {
            //custom logging logic can go here
        }

        public class ContentLengthFilter : Stream
        {
            private readonly Stream _responseFilter;

            public int BytesWritten { get; set; }

            public ContentLengthFilter(Stream responseFilter)
            {
                _responseFilter = responseFilter;
            }

            public override void Flush()
            {
                _responseFilter.Flush();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return _responseFilter.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _responseFilter.SetLength(value);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return _responseFilter.Read(buffer, offset, count);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                BytesWritten += count;
                _responseFilter.Write(buffer, offset, count);
            }

            public override bool CanRead
            {
                get { return _responseFilter.CanRead; }
            }

            public override bool CanSeek
            {
                get { return _responseFilter.CanSeek; }
            }

            public override bool CanWrite
            {
                get { return _responseFilter.CanWrite; }
            }

            public override long Length
            {
                get { return _responseFilter.Length; }
            }

            public override long Position
            {
                get { return _responseFilter.Position; }
                set { _responseFilter.Position = value; }
            }
        }
    }
}
