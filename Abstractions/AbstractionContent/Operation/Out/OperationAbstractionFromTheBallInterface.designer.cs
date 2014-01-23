 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

		namespace TheBall.Interface { 
				public class ExecuteOperationParameters 
		{
				public string OwnerLocation ;
				public string OperationDomain ;
				public string OperationName ;
				public byte[] OperationParameters ;
				public string CallerProvidedInfo ;
				}
		
		public class ExecuteOperation 
		{
				private static void PrepareParameters(ExecuteOperationParameters parameters)
		{
					}
				public static void Execute(ExecuteOperationParameters parameters)
		{
						PrepareParameters(parameters);
					}
				}
				public class UpdateStatusSummaryParameters 
		{
				public TheBall.CORE.IContainerOwner Owner ;
				public DateTime StartTime ;
				public DateTime EndTime ;
				public string[] ChangedIDList ;
				public int RemoveExpiredEntriesSeconds ;
				}
		
		public class UpdateStatusSummary 
		{
				private static void PrepareParameters(UpdateStatusSummaryParameters parameters)
		{
					}
				public static void Execute(UpdateStatusSummaryParameters parameters)
		{
						PrepareParameters(parameters);
					InformationChangeItem ChangeItem = UpdateStatusSummaryImplementation.GetTarget_ChangeItem(parameters.Owner, parameters.StartTime, parameters.EndTime, parameters.ChangedIDList);	
				UpdateStatusSummaryImplementation.ExecuteMethod_StoreObject(ChangeItem);		
				string[] EnsureUpdateOnStatusSummaryOutput = UpdateStatusSummaryImplementation.ExecuteMethod_EnsureUpdateOnStatusSummary(parameters.Owner, parameters.RemoveExpiredEntriesSeconds, ChangeItem);		
				UpdateStatusSummaryImplementation.ExecuteMethod_RemoveExpiredEntries(parameters.Owner, EnsureUpdateOnStatusSummaryOutput);		
				}
				}
		 } 