 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

		namespace TheBall.Index { 
				public class ReleaseIndexerResourcesParameters 
		{
				public AttemptToBecomeInfrastructureIndexerReturnValue ResourceInfo ;
				}
		
		public class ReleaseIndexerResources 
		{
				private static void PrepareParameters(ReleaseIndexerResourcesParameters parameters)
		{
					}
				public static void Execute(ReleaseIndexerResourcesParameters parameters)
		{
						PrepareParameters(parameters);
					ReleaseIndexerResourcesImplementation.ExecuteMethod_ReleaseResources(parameters.ResourceInfo);		
				}
				}
				public class AttemptToBecomeInfrastructureIndexerParameters 
		{
				public string IndexName ;
				}
		
		public class AttemptToBecomeInfrastructureIndexer 
		{
				private static void PrepareParameters(AttemptToBecomeInfrastructureIndexerParameters parameters)
		{
					}
				public static AttemptToBecomeInfrastructureIndexerReturnValue Execute(AttemptToBecomeInfrastructureIndexerParameters parameters)
		{
						PrepareParameters(parameters);
					string IndexDriveName = AttemptToBecomeInfrastructureIndexerImplementation.GetTarget_IndexDriveName(parameters.IndexName);	
				AttemptToBecomeInfrastructureIndexerReturnValue MountIndexDriveOutput = AttemptToBecomeInfrastructureIndexerImplementation.ExecuteMethod_MountIndexDrive(IndexDriveName);		
				string QueryQueueName = AttemptToBecomeInfrastructureIndexerImplementation.GetTarget_QueryQueueName(parameters.IndexName);	
				string IndexRequestQueueName = AttemptToBecomeInfrastructureIndexerImplementation.GetTarget_IndexRequestQueueName(parameters.IndexName);	
				AttemptToBecomeInfrastructureIndexerImplementation.ExecuteMethod_EnsureQueuesIfMountSucceeded(MountIndexDriveOutput.Success, QueryQueueName, IndexRequestQueueName);		
				AttemptToBecomeInfrastructureIndexerReturnValue returnValue = AttemptToBecomeInfrastructureIndexerImplementation.Get_ReturnValue(MountIndexDriveOutput);
		return returnValue;
				}
				}
				public class AttemptToBecomeInfrastructureIndexerReturnValue 
		{
				public bool Success ;
				public Microsoft.WindowsAzure.StorageClient.CloudDrive CloudDrive ;
				public System.Exception Exception ;
				}
				public class IndexInformationParameters 
		{
				public IndexingRequest IndexingRequest ;
				}
		
		public class IndexInformation 
		{
				private static void PrepareParameters(IndexInformationParameters parameters)
		{
					}
				public static void Execute(IndexInformationParameters parameters)
		{
						PrepareParameters(parameters);
					IndexInformationImplementation.ExecuteMethod_PerformIndexing(parameters.IndexingRequest);		
				}
				}
				public class QueryIndexedInformationParameters 
		{
				public QueryRequest QueryRequest ;
				public string IndexName ;
				}
		
		public class QueryIndexedInformation 
		{
				private static void PrepareParameters(QueryIndexedInformationParameters parameters)
		{
					}
				public static QueryIndexedInformationReturnValue Execute(QueryIndexedInformationParameters parameters)
		{
						PrepareParameters(parameters);
					string QueryID = QueryIndexedInformationImplementation.GetTarget_QueryID(parameters.QueryRequest);	
				string QueryQueueName = QueryIndexedInformationImplementation.GetTarget_QueryQueueName(parameters.IndexName);	
				QueryIndexedInformationImplementation.ExecuteMethod_QueueQueryRequest(parameters.QueryRequest, QueryQueueName);		
				QueryIndexedInformationReturnValue returnValue = QueryIndexedInformationImplementation.Get_ReturnValue(QueryID);
		return returnValue;
				}
				}
				public class QueryIndexedInformationReturnValue 
		{
				public string QueryTrackableID ;
				}
		
		public class PerformUserQuery 
		{
				public static void Execute()
		{
						
					UserQuery QueryObject = PerformUserQueryImplementation.GetTarget_QueryObject();	
				QueryRequest PerformQueryOutput;
		{ // Local block to allow local naming
			PrepareAndExecuteQueryParameters operationParameters = PerformUserQueryImplementation.PerformQuery_GetParameters(QueryObject);
			var operationReturnValue = PrepareAndExecuteQuery.Execute(operationParameters);
			PerformQueryOutput = PerformUserQueryImplementation.PerformQuery_GetOutput(operationReturnValue, QueryObject);						
		} // Local block closing
				QueryToken ResponseContentObject = PerformUserQueryImplementation.GetTarget_ResponseContentObject(PerformQueryOutput);	
				PerformUserQueryImplementation.ExecuteMethod_WriteContentToHttpResponse(ResponseContentObject);		
				}
				}
				public class PrepareAndExecuteQueryParameters 
		{
				public string QueryString ;
				}
		
		public class PrepareAndExecuteQuery 
		{
				private static void PrepareParameters(PrepareAndExecuteQueryParameters parameters)
		{
					}
				public static PrepareAndExecuteQueryReturnValue Execute(PrepareAndExecuteQueryParameters parameters)
		{
						PrepareParameters(parameters);
					QueryRequest RequestObject = PrepareAndExecuteQueryImplementation.GetTarget_RequestObject(parameters.QueryString);	
				PrepareAndExecuteQueryImplementation.ExecuteMethod_StoreObject(RequestObject);		
				PrepareAndExecuteQueryImplementation.ExecuteMethod_PutQueryRequestToQueryQueue(RequestObject);		
				PrepareAndExecuteQueryReturnValue returnValue = PrepareAndExecuteQueryImplementation.Get_ReturnValue(RequestObject);
		return returnValue;
				}
				}

		    public class PrepareAndExecuteQueryReturnValue 
		{
				public QueryRequest ActiveRequest ;
				}
		 } 