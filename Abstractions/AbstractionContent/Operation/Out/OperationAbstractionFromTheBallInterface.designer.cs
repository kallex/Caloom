 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

		namespace TheBall.Interface { 
		
		public class SetCategoryLinkingForConnection 
		{
				public static void Execute()
		{
						
					CategoryLinkParameters CategoryLinkingParameters = SetCategoryLinkingForConnectionImplementation.GetTarget_CategoryLinkingParameters();	
				Connection Connection = SetCategoryLinkingForConnectionImplementation.GetTarget_Connection(CategoryLinkingParameters);	
				SetCategoryLinkingForConnectionImplementation.ExecuteMethod_SetConnectionLinkingData(Connection, CategoryLinkingParameters);		
				SetCategoryLinkingForConnectionImplementation.ExecuteMethod_StoreObject(Connection);		
				}
				}
				public class PushCollaborationContentParameters 
		{
				public string ConnectionID ;
				}
		
		public class PushCollaborationContent 
		{
				private static void PrepareParameters(PushCollaborationContentParameters parameters)
		{
					}
				public static void Execute(PushCollaborationContentParameters parameters)
		{
						PrepareParameters(parameters);
					Connection Connection = PushCollaborationContentImplementation.GetTarget_Connection(parameters.ConnectionID);	
				string PackageContentListingOperationName = PushCollaborationContentImplementation.GetTarget_PackageContentListingOperationName(Connection);	
				string[] DynamicPackageListingOperationOutput = PushCollaborationContentImplementation.ExecuteMethod_DynamicPackageListingOperation(parameters.ConnectionID, PackageContentListingOperationName);		
				TransferPackage TransferPackage = PushCollaborationContentImplementation.GetTarget_TransferPackage(parameters.ConnectionID);	
				PushCollaborationContentImplementation.ExecuteMethod_AddTransferPackageToConnection(Connection, TransferPackage);		
				PushCollaborationContentImplementation.ExecuteMethod_StoreObject(Connection);		
				string[] PackageTransferPackageContentOutput = PushCollaborationContentImplementation.ExecuteMethod_PackageTransferPackageContent(TransferPackage, DynamicPackageListingOperationOutput);		
				PushCollaborationContentImplementation.ExecuteMethod_SendTransferPackageContent(Connection, TransferPackage, PackageTransferPackageContentOutput);		
				PushCollaborationContentImplementation.ExecuteMethod_SetTransferPackageAsProcessed(TransferPackage);		
				PushCollaborationContentImplementation.ExecuteMethod_StoreObjectComplete(Connection, TransferPackage);		
				}
				}
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
				public DateTime UpdateTime ;
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
					UpdateStatusSummaryImplementation.ExecuteMethod_EnsureUpdateOnStatusSummary(parameters.Owner, parameters.UpdateTime, parameters.ChangedIDList, parameters.RemoveExpiredEntriesSeconds);		
				}
				}
				public class InitiateIntegrationConnectionParameters 
		{
				public string Description ;
				public string TargetBallHostName ;
				public string TargetGroupID ;
				}
		
		public class InitiateIntegrationConnection 
		{
				private static void PrepareParameters(InitiateIntegrationConnectionParameters parameters)
		{
					}
				public static void Execute(InitiateIntegrationConnectionParameters parameters)
		{
						PrepareParameters(parameters);
					Connection Connection = InitiateIntegrationConnectionImplementation.GetTarget_Connection(parameters.Description);	
				TheBall.CORE.AuthenticatedAsActiveDevice DeviceForConnection = InitiateIntegrationConnectionImplementation.GetTarget_DeviceForConnection(parameters.Description, parameters.TargetBallHostName, parameters.TargetGroupID, Connection);	
				InitiateIntegrationConnectionImplementation.ExecuteMethod_StoreConnection(Connection);		
				InitiateIntegrationConnectionImplementation.ExecuteMethod_NegotiateDeviceConnection(DeviceForConnection);		
				}
				}
				public class CreateReceivingConnectionParameters 
		{
				public string Description ;
				public string OtherSideConnectionID ;
				}
		
		public class CreateReceivingConnection 
		{
				private static void PrepareParameters(CreateReceivingConnectionParameters parameters)
		{
					}
				public static CreateReceivingConnectionReturnValue Execute(CreateReceivingConnectionParameters parameters)
		{
						PrepareParameters(parameters);
					Connection Connection = CreateReceivingConnectionImplementation.GetTarget_Connection(parameters.Description, parameters.OtherSideConnectionID);	
				CreateReceivingConnectionImplementation.ExecuteMethod_StoreConnection(Connection);		
				CreateReceivingConnectionReturnValue returnValue = CreateReceivingConnectionImplementation.Get_ReturnValue(Connection);
		return returnValue;
				}
				}
				public class CreateReceivingConnectionReturnValue 
		{
				public string ConnectionID ;
				}
				public class UpdateConnectionOtherSideCategoriesParameters 
		{
				public string ConnectionID ;
				}
		
		public class UpdateConnectionOtherSideCategories 
		{
				private static void PrepareParameters(UpdateConnectionOtherSideCategoriesParameters parameters)
		{
					}
				public static void Execute(UpdateConnectionOtherSideCategoriesParameters parameters)
		{
						PrepareParameters(parameters);
					TheBall.Interface.Connection Connection = UpdateConnectionOtherSideCategoriesImplementation.GetTarget_Connection(parameters.ConnectionID);	
				Category[] GetOtherSideCategoriesOutput = UpdateConnectionOtherSideCategoriesImplementation.ExecuteMethod_GetOtherSideCategories(Connection);		
				UpdateConnectionOtherSideCategoriesImplementation.ExecuteMethod_UpdateOtherSideCategories(Connection, GetOtherSideCategoriesOutput);		
				UpdateConnectionOtherSideCategoriesImplementation.ExecuteMethod_StoreObject(Connection);		
				}
				}
		 } 