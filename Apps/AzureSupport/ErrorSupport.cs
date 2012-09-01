using System;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure.StorageClient;

namespace TheBall
{
    public static class ErrorSupport
    {
        public static void ReportError(SystemError error)
        {
            QueueSupport.PutToErrorQueue(error);
        }

        public static void ReportException(Exception exception)
        {
            // Under NO circumstances the exception reporting shall cause another exception to be thrown unhandled
            try
            {
                SystemError error = GetErrorFromExcetion(exception);
                ReportError(error);
            } catch
            {
                
            }
        }

        public static void ReportMessageError(CloudQueueMessage message)
        {
            SystemError error = GetErrorFromMessage(message);
            ReportError(error);
        }

        private static SystemError GetErrorFromMessage(CloudQueueMessage message)
        {
            SystemError error = SystemError.CreateDefault();
            error.ErrorTitle = "Cloud Message: " + message.Id;
            error.OccurredAt = DateTime.UtcNow;
            error.SystemErrorItems.CollectionContent.Add(new SystemErrorItem()
                                                             {
                                                                 ShortDescription = "Message content",
                                                                 LongDescription = message.AsString
                                                             });
            return error;
        }

        public static SystemError GetErrorFromExcetion(Exception exception)
        {
            SystemError error = SystemError.CreateDefault();
            error.ErrorTitle = "Exception: " + exception.GetType().Name;
            error.OccurredAt = DateTime.UtcNow;
            error.SystemErrorItems.CollectionContent.Add(new SystemErrorItem()
                                                             {
                                                                 ShortDescription = exception.Message,
                                                                 LongDescription = exception.ToString()
                                                             });
            return error;
        }
    }
}