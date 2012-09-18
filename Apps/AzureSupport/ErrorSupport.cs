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
            SystemError error = new SystemError
                                    {
                                        ErrorTitle = "Cloud Message: " + message.Id,
                                        OccurredAt = DateTime.UtcNow,
                                        SystemErrorItems = new SystemErrorItemCollection()
                                    };
            error.SystemErrorItems.CollectionContent.Add(new SystemErrorItem()
                                                             {
                                                                 ShortDescription = "Message content",
                                                                 LongDescription = message.AsString
                                                             });
            return error;
        }

        public static SystemError GetErrorFromExcetion(Exception exception)
        {
            SystemError error = new SystemError
                                    {
                                        ErrorTitle = "Exception: " + exception.GetType().Name,
                                        OccurredAt = DateTime.UtcNow,
                                        SystemErrorItems = new SystemErrorItemCollection()
                                    };
            error.SystemErrorItems.CollectionContent.Add(new SystemErrorItem()
                                                             {
                                                                 ShortDescription = exception.Message,
                                                                 LongDescription = exception.ToString()
                                                             });
            return error;
        }

        public static void ReportEnvelopeWithException(QueueEnvelope envelope, Exception exception)
        {
            SystemError error = GetErrorFromExcetion(exception);
            error.MessageContent = envelope;
            ReportError(error);
        }

        public static QueueEnvelope RetrieveRetryableEnvelope(out CloudQueueMessage message)
        {
            SystemError error = QueueSupport.GetFromErrorQueue(out message);
            while(error != null)
            {
                if(error.MessageContent == null)
                {
                    QueueSupport.CurrErrorQueue.DeleteMessage(message);
                    error = QueueSupport.GetFromErrorQueue(out message);
                }
                else
                {
                    return error.MessageContent;
                }
            }
            return null;
        }
    }
}