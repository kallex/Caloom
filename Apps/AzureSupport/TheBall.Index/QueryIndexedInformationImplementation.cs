using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using DiagnosticsUtils;
using Lucene.Net.Analysis.Standard;
using LuceneSupport;
using TheBall.CORE;
using Version = Lucene.Net.Util.Version;

namespace TheBall.Index
{
    public class QueryIndexedInformationImplementation
    {
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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
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
            stopwatch.Stop();
            queryRequest.LastCompletionDurationMs = (long) Math.Ceiling(stopwatch.Elapsed.TotalMilliseconds);
            queryRequest.LastCompletionTime = DateTime.UtcNow;
            queryRequest.IsQueryCompleted = true;
        }

        public static void ExecuteMethod_SaveQueryRequest(QueryRequest queryRequest)
        {
            queryRequest.StoreInformation();
        }
    }
}