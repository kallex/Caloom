using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Lucene.Net.Analysis.Standard;
using LuceneSupport;
using TheBall.CORE;
using Version = Lucene.Net.Util.Version;

namespace TheBall.Index
{
    public class QueryIndexedInformationImplementation
    {
        /*
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
         * */

        public static QueryRequest GetTarget_QueryRequest(IContainerOwner owner, string queryRequestId)
        {
            return QueryRequest.RetrieveFromOwnerContent(owner, queryRequestId);
        }

        public static string GetTarget_LuceneIndexFolder(IContainerOwner owner, string indexName, string indexStorageRootPath)
        {
            string fullPath = Path.Combine(indexStorageRootPath, owner.ToFolderName(), indexName);
            return fullPath;
        }

        public static void ExecuteMethod_PerformQueryRequest(QueryRequest queryRequest, string luceneIndexFolder)
        {
            var queryString = queryRequest.QueryString;
            var defaultFieldName = queryRequest.DefaultFieldName;
            var resultDocuments = FieldIndexSupport.PerformQuery(luceneIndexFolder, queryString, defaultFieldName, new StandardAnalyzer(Version.LUCENE_30));
            queryRequest.QueryResultObjects.Clear();
            foreach (var resultDoc in resultDocuments)
            {
                var doc = resultDoc.Doc;
                string objectDomainName = doc.Get("ObjectDomainName");
                string objectName = doc.Get("ObjectName");
                string objectID = doc.Get("ObjectID");
                QueryResultItem item = new QueryResultItem
                    {
                        ObjectDomainName = objectDomainName,
                        ObjectName = objectName,
                        ObjectID = objectID,
                        Rank = resultDoc.Score
                    };
                queryRequest.QueryResultObjects.Add(item);
            }
            queryRequest.IsQueryCompleted = true;
        }

        public static void ExecuteMethod_SaveQueryRequest(QueryRequest queryRequest)
        {
            queryRequest.StoreInformation();
        }
    }
}