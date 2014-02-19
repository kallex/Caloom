using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheBall.CORE;

namespace TheBall.Index
{
    public static class IndexSupport
    {
        public static string GetQueryRequestQueueName(string indexName)
        {
            validateIndexName(indexName);
            return "index-" + indexName + "-query";
        }

        public static string GetIndexRequestQueueName(string indexName)
        {
            validateIndexName(indexName);
            return "index-" + indexName + "-index";
        }

        private const string ValidationError = "IndexName needs to be lower-case, alphanumeric, starting with alphabet";
        private static void validateIndexName(string indexName)
        {
            if(String.IsNullOrEmpty(indexName))
                throw new ArgumentException(ValidationError, "indexName");
            if(indexName != indexName.ToLower())
                throw new ArgumentException(ValidationError, "indexName");
            if(indexName.All(char.IsLetterOrDigit) == false)
                throw new ArgumentException(ValidationError, "indexName");
            if(char.IsLetter(indexName[0]) == false)
                throw new ArgumentException(ValidationError, "indexName");
        }

        public const int IndexDriveStorageSizeInMB = 1024*100; // 100 GB

        public static void PutQueryRequestToQueue(string indexName, IContainerOwner owner, string requestID)
        {
            var queueName = GetQueryRequestQueueName(indexName);
            string ownerstring = owner.ToParseableString();
            string messageText = ownerstring + ":" + requestID;
            QueueSupport.PutMessageToQueue(queueName, messageText);
        }

        public static void PutIndexingRequestToQueue(string indexName, IContainerOwner owner, string requestID)
        {
            var queueName = GetIndexRequestQueueName(indexName);
            string ownerString = owner.ToParseableString();
            string messageText = ownerString + ":" + requestID;
            QueueSupport.PutMessageToQueue(queueName, messageText);
        }
    }
}
