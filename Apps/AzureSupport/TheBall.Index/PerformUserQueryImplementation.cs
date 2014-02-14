using System.Web;
using AzureSupport;
using TheBall.CORE;

namespace TheBall.Index
{
    public class PerformUserQueryImplementation
    {
        public static UserQuery GetTarget_QueryObject()
        {
            var request = HttpContext.Current.Request;
            var stream = request.GetBufferlessInputStream();
            var result = JSONSupport.GetObjectFromStream<UserQuery>(stream);
            return result;
        }

        public static QueryIndexedInformationParameters PerformQuery_GetParameters(UserQuery queryObject)
        {
            return new QueryIndexedInformationParameters
                {
                    QueryRequest = new QueryRequest { OwnerPrefix = InformationContext.CurrentOwner.GetOwnerPrefix(),
                        QueryString = queryObject.QueryString
                    }, IndexName = "default",
                };
        }

        public static string PerformQuery_GetOutput(QueryIndexedInformationReturnValue operationReturnValue, UserQuery queryObject)
        {
            return operationReturnValue.QueryTrackableID;
        }

        public static QueryToken GetTarget_ResponseContentObject(string performQueryOutput)
        {
            return new QueryToken {ID = performQueryOutput};
        }

        public static void ExecuteMethod_WriteContentToHttpResponse(QueryToken responseContentObject)
        {
            var httpContext = HttpContext.Current;
            var jsonString = JSONSupport.SerializeToJSONString(responseContentObject);
            httpContext.Response.Write(jsonString);
            httpContext.Response.ContentType = "application/json";
        }
    }
}