using TheBall.CORE;

namespace TheBall.Index
{
    public class PrepareAndExecuteQueryImplementation
    {
        public static QueryRequest GetTarget_RequestObject(string queryString)
        {
            QueryRequest queryRequest = new QueryRequest();
            var owner = InformationContext.CurrentOwner;
            queryRequest.SetLocationAsOwnerContent(owner, queryRequest.ID);
            queryRequest.QueryString = queryString;
            queryRequest.IsQueryCompleted = false;
            return queryRequest;
        }

        public static void ExecuteMethod_StoreObject(QueryRequest requestObject)
        {
            requestObject.StoreInformation();
        }

        public static void ExecuteMethod_PutQueryRequestToQueryQueue(QueryRequest requestObject)
        {
            var owner = InformationContext.CurrentOwner;
            string activeContainerName = StorageSupport.CurrActiveContainer.Name;
            IndexSupport.PutQueryRequestToQueue(activeContainerName, "defaultindex", owner, requestObject.ID);
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