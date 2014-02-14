using System;
using System.Security.Cryptography;
using System.Text;

namespace TheBall.Index
{
    public class QueryIndexedInformationImplementation
    {
        public static string GetTarget_QueryID(QueryRequest queryRequest)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(queryRequest.QueryString));
            var hexStr = BitConverter.ToString(hash).Replace("-", "").ToLower();
            return hexStr;
        }

        public static QueryIndexedInformationReturnValue Get_ReturnValue(string queryId)
        {
            return new QueryIndexedInformationReturnValue {QueryTrackableID = queryId};
        }

        public static string GetTarget_QueryQueueName(string indexName)
        {
            return IndexSupport.GetQueryRequestQueueName(indexName);
        }

        public static void ExecuteMethod_QueueQueryRequest(QueryRequest queryRequest, string queryQueueName)
        {
            // TODO: Actual query request
        }
    }
}