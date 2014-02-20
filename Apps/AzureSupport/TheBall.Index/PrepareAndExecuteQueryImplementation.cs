using System;
using System.Security.Cryptography;
using System.Text;
using TheBall.CORE;

namespace TheBall.Index
{
    public class PrepareAndExecuteQueryImplementation
    {
        public static QueryRequest GetTarget_RequestObject(string queryString, string defaultFieldName, string indexName)
        {
            string queryRequestID = IndexSupport.GetRequestID(indexName, queryString, defaultFieldName);
            var owner = InformationContext.CurrentOwner;
            QueryRequest queryRequest = QueryRequest.RetrieveFromOwnerContent(owner, queryRequestID);
            if (queryRequest == null)
            {
                queryRequest = new QueryRequest();
                queryRequest.ID = queryRequestID;
                queryRequest.SetLocationAsOwnerContent(owner, queryRequest.ID);
                queryRequest.IndexName = indexName;
                queryRequest.QueryString = queryString;
                queryRequest.DefaultFieldName = defaultFieldName;
                queryRequest.LastCompletionTime = DateTime.MinValue.ToUniversalTime();
            }
            queryRequest.LastRequestTime = DateTime.UtcNow;
            queryRequest.LastCompletionDurationMs = 0;
            queryRequest.IsQueryCompleted = false;
            return queryRequest;
        }

        public static void ExecuteMethod_StoreObject(QueryRequest requestObject)
        {
            requestObject.StoreInformation();
        }

        public static void ExecuteMethod_PutQueryRequestToQueryQueue(string indexName, QueryRequest requestObject)
        {
            var owner = InformationContext.CurrentOwner;
            string activeContainerName = StorageSupport.CurrActiveContainer.Name;
            IndexSupport.PutQueryRequestToQueue(activeContainerName, indexName, owner, requestObject.ID);
        }

        public static PrepareAndExecuteQueryReturnValue Get_ReturnValue(QueryRequest requestObject)
        {
            return new PrepareAndExecuteQueryReturnValue
                {
                    ActiveRequest = requestObject
                };
        }

    }
}