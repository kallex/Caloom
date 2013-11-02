 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

		namespace TheBall.CORE { 
				public class CreateSpecifiedInformationObjectWithValuesParameters 
		{
				public IContainerOwner Owner ;
				public string ObjectDomainName ;
				public string ObjectName ;
				public NameValueCollection HttpFormData ;
				public System.Web.HttpFileCollection HttpFileData ;
				}
		
		public class CreateSpecifiedInformationObjectWithValues 
		{
				private static void PrepareParameters(CreateSpecifiedInformationObjectWithValuesParameters parameters)
		{
					}
				public static void Execute(CreateSpecifiedInformationObjectWithValuesParameters parameters)
		{
						PrepareParameters(parameters);
					CreateSpecifiedInformationObjectWithValuesImplementation.ExecuteMethod_CatchInvalidDomains(parameters.ObjectDomainName);		
				IInformationObject CreatedObject = CreateSpecifiedInformationObjectWithValuesImplementation.GetTarget_CreatedObject(parameters.Owner, parameters.ObjectDomainName, parameters.ObjectName);	
				CreateSpecifiedInformationObjectWithValuesImplementation.ExecuteMethod_CreateInternalObjects(parameters.HttpFormData, CreatedObject);		
				NameValueCollection FieldValues = CreateSpecifiedInformationObjectWithValuesImplementation.GetTarget_FieldValues(parameters.HttpFormData, CreatedObject);	
				NameValueCollection ObjectLinkValues = CreateSpecifiedInformationObjectWithValuesImplementation.GetTarget_ObjectLinkValues(parameters.HttpFormData, CreatedObject);	
				Dictionary<string, System.Web.HttpPostedFile> BinaryContentFiles = CreateSpecifiedInformationObjectWithValuesImplementation.GetTarget_BinaryContentFiles(parameters.HttpFormData, parameters.HttpFileData, CreatedObject);	
				CreateSpecifiedInformationObjectWithValuesImplementation.ExecuteMethod_SetFieldValues(CreatedObject, FieldValues);		
				CreateSpecifiedInformationObjectWithValuesImplementation.ExecuteMethod_SetObjectLinks(CreatedObject, ObjectLinkValues);		
				CreateSpecifiedInformationObjectWithValuesImplementation.ExecuteMethod_StoreInitialObject(parameters.Owner, CreatedObject);		
				CreateSpecifiedInformationObjectWithValuesImplementation.ExecuteMethod_SetBinaryContent(parameters.Owner, CreatedObject, BinaryContentFiles);		
				CreateSpecifiedInformationObjectWithValuesImplementation.ExecuteMethod_StoreCompleteObject(parameters.Owner, CreatedObject);		
				}
				}
				public class DeleteSpecifiedInformationObjectParameters 
		{
				public IContainerOwner Owner ;
				public string ObjectDomainName ;
				public string ObjectName ;
				public string ObjectID ;
				}
		
		public class DeleteSpecifiedInformationObject 
		{
				private static void PrepareParameters(DeleteSpecifiedInformationObjectParameters parameters)
		{
					}
				public static void Execute(DeleteSpecifiedInformationObjectParameters parameters)
		{
						PrepareParameters(parameters);
					DeleteSpecifiedInformationObjectImplementation.ExecuteMethod_CatchInvalidDomains(parameters.ObjectDomainName);		
				IInformationObject ObjectToDelete = DeleteSpecifiedInformationObjectImplementation.GetTarget_ObjectToDelete(parameters.Owner, parameters.ObjectDomainName, parameters.ObjectName, parameters.ObjectID);	
				DeleteSpecifiedInformationObjectImplementation.ExecuteMethod_DeleteObject(ObjectToDelete);		
				}
				}
				public class CreateDeviceMembershipParameters 
		{
				public IContainerOwner Owner ;
				public string DeviceDescription ;
				public byte[] ActiveSymmetricAESKey ;
				}
		
		public class CreateDeviceMembership 
		{
				private static void PrepareParameters(CreateDeviceMembershipParameters parameters)
		{
					}
				public static CreateDeviceMembershipReturnValue Execute(CreateDeviceMembershipParameters parameters)
		{
						PrepareParameters(parameters);
					DeviceMembership CreatedDeviceMembership = CreateDeviceMembershipImplementation.GetTarget_CreatedDeviceMembership(parameters.Owner, parameters.DeviceDescription, parameters.ActiveSymmetricAESKey);	
				CreateDeviceMembershipImplementation.ExecuteMethod_StoreObject(CreatedDeviceMembership);		
				CreateDeviceMembershipReturnValue returnValue = CreateDeviceMembershipImplementation.Get_ReturnValue(CreatedDeviceMembership);
		return returnValue;
				}
				}
				public class CreateDeviceMembershipReturnValue 
		{
				public DeviceMembership DeviceMembership ;
				}
				public class SetDeviceMembershipValidationAndActiveStatusParameters 
		{
				public IContainerOwner Owner ;
				public string DeviceMembershipID ;
				public bool IsValidAndActive ;
				}
		
		public class SetDeviceMembershipValidationAndActiveStatus 
		{
				private static void PrepareParameters(SetDeviceMembershipValidationAndActiveStatusParameters parameters)
		{
					}
				public static void Execute(SetDeviceMembershipValidationAndActiveStatusParameters parameters)
		{
						PrepareParameters(parameters);
					DeviceMembership DeviceMembership = SetDeviceMembershipValidationAndActiveStatusImplementation.GetTarget_DeviceMembership(parameters.Owner, parameters.DeviceMembershipID);	
				SetDeviceMembershipValidationAndActiveStatusImplementation.ExecuteMethod_SetDeviceValidAndActiveValue(parameters.IsValidAndActive, DeviceMembership);		
				SetDeviceMembershipValidationAndActiveStatusImplementation.ExecuteMethod_StoreObject(DeviceMembership);		
				}
				}
				public class DeleteDeviceMembershipParameters 
		{
				public IContainerOwner Owner ;
				public string DeviceMembershipID ;
				}
		
		public class DeleteDeviceMembership 
		{
				private static void PrepareParameters(DeleteDeviceMembershipParameters parameters)
		{
					}
				public static void Execute(DeleteDeviceMembershipParameters parameters)
		{
						PrepareParameters(parameters);
					DeviceMembership DeviceMembership = DeleteDeviceMembershipImplementation.GetTarget_DeviceMembership(parameters.Owner, parameters.DeviceMembershipID);	
				DeleteDeviceMembershipImplementation.ExecuteMethod_DeleteDeviceMembership(DeviceMembership);		
				}
				}
				public class CreateAndSendEmailValidationForDeviceJoinConfirmationParameters 
		{
				public AaltoGlobalImpact.OIP.TBAccount OwningAccount ;
				public AaltoGlobalImpact.OIP.TBCollaboratingGroup OwningGroup ;
				public DeviceMembership DeviceMembership ;
				}
		
		public class CreateAndSendEmailValidationForDeviceJoinConfirmation 
		{
				private static void PrepareParameters(CreateAndSendEmailValidationForDeviceJoinConfirmationParameters parameters)
		{
					}
				public static void Execute(CreateAndSendEmailValidationForDeviceJoinConfirmationParameters parameters)
		{
						PrepareParameters(parameters);
					string[] OwnerEmailAddresses = CreateAndSendEmailValidationForDeviceJoinConfirmationImplementation.GetTarget_OwnerEmailAddresses(parameters.OwningAccount, parameters.OwningGroup);	
				AaltoGlobalImpact.OIP.TBEmailValidation EmailValidation = CreateAndSendEmailValidationForDeviceJoinConfirmationImplementation.GetTarget_EmailValidation(parameters.OwningAccount, parameters.OwningGroup, parameters.DeviceMembership, OwnerEmailAddresses);	
				CreateAndSendEmailValidationForDeviceJoinConfirmationImplementation.ExecuteMethod_StoreObject(EmailValidation);		
				CreateAndSendEmailValidationForDeviceJoinConfirmationImplementation.ExecuteMethod_SendEmailConfirmation(parameters.DeviceMembership, EmailValidation, OwnerEmailAddresses);		
				}
				}
				public class CreateAuthenticatedAsActiveDeviceParameters 
		{
				public IContainerOwner Owner ;
				public string AuthenticationDeviceDescription ;
				public string TargetBallHostName ;
				public string TargetGroupID ;
				public string SharedSecret ;
				}
		
		public class CreateAuthenticatedAsActiveDevice 
		{
				private static void PrepareParameters(CreateAuthenticatedAsActiveDeviceParameters parameters)
		{
					}
				public static CreateAuthenticatedAsActiveDeviceReturnValue Execute(CreateAuthenticatedAsActiveDeviceParameters parameters)
		{
						PrepareParameters(parameters);
					string NegotiationURL = CreateAuthenticatedAsActiveDeviceImplementation.GetTarget_NegotiationURL(parameters.TargetBallHostName, parameters.TargetGroupID);	
				AuthenticatedAsActiveDevice AuthenticatedAsActiveDevice = CreateAuthenticatedAsActiveDeviceImplementation.GetTarget_AuthenticatedAsActiveDevice(parameters.Owner, parameters.AuthenticationDeviceDescription, parameters.SharedSecret, NegotiationURL);	
				CreateAuthenticatedAsActiveDeviceImplementation.ExecuteMethod_StoreObject(AuthenticatedAsActiveDevice);		
				CreateAuthenticatedAsActiveDeviceReturnValue returnValue = CreateAuthenticatedAsActiveDeviceImplementation.Get_ReturnValue(AuthenticatedAsActiveDevice);
		return returnValue;
				}
				}
				public class CreateAuthenticatedAsActiveDeviceReturnValue 
		{
				public AuthenticatedAsActiveDevice CreatedAuthenticatedAsActiveDevice ;
				}
				public class PerformNegotiationAndValidateAuthenticationAsActiveDeviceParameters 
		{
				public IContainerOwner Owner ;
				public string AuthenticatedAsActiveDeviceID ;
				}
		
		public class PerformNegotiationAndValidateAuthenticationAsActiveDevice 
		{
				private static void PrepareParameters(PerformNegotiationAndValidateAuthenticationAsActiveDeviceParameters parameters)
		{
					}
				public static void Execute(PerformNegotiationAndValidateAuthenticationAsActiveDeviceParameters parameters)
		{
						PrepareParameters(parameters);
					AuthenticatedAsActiveDevice AuthenticatedAsActiveDevice = PerformNegotiationAndValidateAuthenticationAsActiveDeviceImplementation.GetTarget_AuthenticatedAsActiveDevice(parameters.Owner, parameters.AuthenticatedAsActiveDeviceID);	
				PerformNegotiationAndValidateAuthenticationAsActiveDeviceImplementation.ExecuteMethod_NegotiateWithTarget(AuthenticatedAsActiveDevice);		
				PerformNegotiationAndValidateAuthenticationAsActiveDeviceImplementation.ExecuteMethod_StoreObject(AuthenticatedAsActiveDevice);		
				}
				}
				public class DeleteAuthenticatedAsActiveDeviceParameters 
		{
				public IContainerOwner Owner ;
				public string AuthenticatedAsActiveDeviceID ;
				}
		
		public class DeleteAuthenticatedAsActiveDevice 
		{
				private static void PrepareParameters(DeleteAuthenticatedAsActiveDeviceParameters parameters)
		{
					}
				public static void Execute(DeleteAuthenticatedAsActiveDeviceParameters parameters)
		{
						PrepareParameters(parameters);
					AuthenticatedAsActiveDevice AuthenticatedAsActiveDevice = DeleteAuthenticatedAsActiveDeviceImplementation.GetTarget_AuthenticatedAsActiveDevice(parameters.Owner, parameters.AuthenticatedAsActiveDeviceID);	
				DeleteAuthenticatedAsActiveDeviceImplementation.ExecuteMethod_DeleteAuthenticatedAsActiveDevice(AuthenticatedAsActiveDevice);		
				}
				}
				public class CreateInformationOutputParameters 
		{
				public IContainerOwner Owner ;
				public string OutputDescription ;
				public string DestinationURL ;
				public string DestinationContentName ;
				public string LocalContentURL ;
				public string AuthenticatedDeviceID ;
				}
		
		public class CreateInformationOutput 
		{
				private static void PrepareParameters(CreateInformationOutputParameters parameters)
		{
					}
				public static CreateInformationOutputReturnValue Execute(CreateInformationOutputParameters parameters)
		{
						PrepareParameters(parameters);
					InformationOutput CreatedInformationOutput = CreateInformationOutputImplementation.GetTarget_CreatedInformationOutput(parameters.Owner, parameters.OutputDescription, parameters.DestinationURL, parameters.DestinationContentName, parameters.LocalContentURL, parameters.AuthenticatedDeviceID);	
				CreateInformationOutputImplementation.ExecuteMethod_StoreObject(CreatedInformationOutput);		
				CreateInformationOutputReturnValue returnValue = CreateInformationOutputImplementation.Get_ReturnValue(CreatedInformationOutput);
		return returnValue;
				}
				}
				public class CreateInformationOutputReturnValue 
		{
				public InformationOutput InformationOutput ;
				}
				public class SetInformationOutputValidationAndActiveStatusParameters 
		{
				public IContainerOwner Owner ;
				public string InformationOutputID ;
				public bool IsValidAndActive ;
				}
		
		public class SetInformationOutputValidationAndActiveStatus 
		{
				private static void PrepareParameters(SetInformationOutputValidationAndActiveStatusParameters parameters)
		{
					}
				public static void Execute(SetInformationOutputValidationAndActiveStatusParameters parameters)
		{
						PrepareParameters(parameters);
					InformationOutput InformationOutput = SetInformationOutputValidationAndActiveStatusImplementation.GetTarget_InformationOutput(parameters.Owner, parameters.InformationOutputID);	
				SetInformationOutputValidationAndActiveStatusImplementation.ExecuteMethod_SetInputValidAndActiveValue(parameters.IsValidAndActive, InformationOutput);		
				SetInformationOutputValidationAndActiveStatusImplementation.ExecuteMethod_StoreObject(InformationOutput);		
				}
				}
				public class DeleteInformationOutputParameters 
		{
				public IContainerOwner Owner ;
				public string InformationOutputID ;
				}
		
		public class DeleteInformationOutput 
		{
				private static void PrepareParameters(DeleteInformationOutputParameters parameters)
		{
					}
				public static void Execute(DeleteInformationOutputParameters parameters)
		{
						PrepareParameters(parameters);
					InformationOutput InformationOutput = DeleteInformationOutputImplementation.GetTarget_InformationOutput(parameters.Owner, parameters.InformationOutputID);	
				DeleteInformationOutputImplementation.ExecuteMethod_DeleteInformationOutput(InformationOutput);		
				}
				}
				public class CreateAndSendEmailValidationForInformationOutputConfirmationParameters 
		{
				public AaltoGlobalImpact.OIP.TBAccount OwningAccount ;
				public AaltoGlobalImpact.OIP.TBCollaboratingGroup OwningGroup ;
				public InformationOutput InformationOutput ;
				}
		
		public class CreateAndSendEmailValidationForInformationOutputConfirmation 
		{
				private static void PrepareParameters(CreateAndSendEmailValidationForInformationOutputConfirmationParameters parameters)
		{
					}
				public static void Execute(CreateAndSendEmailValidationForInformationOutputConfirmationParameters parameters)
		{
						PrepareParameters(parameters);
					string[] OwnerEmailAddresses = CreateAndSendEmailValidationForInformationOutputConfirmationImplementation.GetTarget_OwnerEmailAddresses(parameters.OwningAccount, parameters.OwningGroup);	
				AaltoGlobalImpact.OIP.TBEmailValidation EmailValidation = CreateAndSendEmailValidationForInformationOutputConfirmationImplementation.GetTarget_EmailValidation(parameters.OwningAccount, parameters.OwningGroup, parameters.InformationOutput, OwnerEmailAddresses);	
				CreateAndSendEmailValidationForInformationOutputConfirmationImplementation.ExecuteMethod_StoreObject(EmailValidation);		
				CreateAndSendEmailValidationForInformationOutputConfirmationImplementation.ExecuteMethod_SendEmailConfirmation(parameters.InformationOutput, EmailValidation, OwnerEmailAddresses);		
				}
				}
				public class PushToInformationOutputParameters 
		{
				public IContainerOwner Owner ;
				public string InformationOutputID ;
				}
		
		public class PushToInformationOutput 
		{
				private static void PrepareParameters(PushToInformationOutputParameters parameters)
		{
					}
				public static void Execute(PushToInformationOutputParameters parameters)
		{
						PrepareParameters(parameters);
					InformationOutput InformationOutput = PushToInformationOutputImplementation.GetTarget_InformationOutput(parameters.Owner, parameters.InformationOutputID);	
				PushToInformationOutputImplementation.ExecuteMethod_VerifyValidOutput(InformationOutput);		
				string DestinationURL = PushToInformationOutputImplementation.GetTarget_DestinationURL(InformationOutput);	
				string DestinationContentName = PushToInformationOutputImplementation.GetTarget_DestinationContentName(InformationOutput);	
				string LocalContentURL = PushToInformationOutputImplementation.GetTarget_LocalContentURL(InformationOutput);	
				AuthenticatedAsActiveDevice AuthenticatedAsActiveDevice = PushToInformationOutputImplementation.GetTarget_AuthenticatedAsActiveDevice(InformationOutput);	
				PushToInformationOutputImplementation.ExecuteMethod_PushToInformationOutput(parameters.Owner, InformationOutput, DestinationURL, DestinationContentName, LocalContentURL, AuthenticatedAsActiveDevice);		
				}
				}
				public class CreateInformationInputParameters 
		{
				public IContainerOwner Owner ;
				public string InputDescription ;
				public string LocationURL ;
				public string LocalContentName ;
				public string AuthenticatedDeviceID ;
				}
		
		public class CreateInformationInput 
		{
				private static void PrepareParameters(CreateInformationInputParameters parameters)
		{
					}
				public static CreateInformationInputReturnValue Execute(CreateInformationInputParameters parameters)
		{
						PrepareParameters(parameters);
					InformationInput CreatedInformationInput = CreateInformationInputImplementation.GetTarget_CreatedInformationInput(parameters.Owner, parameters.InputDescription, parameters.LocationURL, parameters.LocalContentName, parameters.AuthenticatedDeviceID);	
				CreateInformationInputImplementation.ExecuteMethod_StoreObject(CreatedInformationInput);		
				CreateInformationInputReturnValue returnValue = CreateInformationInputImplementation.Get_ReturnValue(CreatedInformationInput);
		return returnValue;
				}
				}
				public class CreateInformationInputReturnValue 
		{
				public InformationInput InformationInput ;
				}
				public class SetInformationInputValidationAndActiveStatusParameters 
		{
				public IContainerOwner Owner ;
				public string InformationInputID ;
				public bool IsValidAndActive ;
				}
		
		public class SetInformationInputValidationAndActiveStatus 
		{
				private static void PrepareParameters(SetInformationInputValidationAndActiveStatusParameters parameters)
		{
					}
				public static void Execute(SetInformationInputValidationAndActiveStatusParameters parameters)
		{
						PrepareParameters(parameters);
					InformationInput InformationInput = SetInformationInputValidationAndActiveStatusImplementation.GetTarget_InformationInput(parameters.Owner, parameters.InformationInputID);	
				SetInformationInputValidationAndActiveStatusImplementation.ExecuteMethod_SetInputValidAndActiveValue(parameters.IsValidAndActive, InformationInput);		
				SetInformationInputValidationAndActiveStatusImplementation.ExecuteMethod_StoreObject(InformationInput);		
				}
				}
				public class DeleteInformationInputParameters 
		{
				public IContainerOwner Owner ;
				public string InformationInputID ;
				}
		
		public class DeleteInformationInput 
		{
				private static void PrepareParameters(DeleteInformationInputParameters parameters)
		{
					}
				public static void Execute(DeleteInformationInputParameters parameters)
		{
						PrepareParameters(parameters);
					InformationInput InformationInput = DeleteInformationInputImplementation.GetTarget_InformationInput(parameters.Owner, parameters.InformationInputID);	
				DeleteInformationInputImplementation.ExecuteMethod_DeleteInformationInput(InformationInput);		
				}
				}
				public class CreateAndSendEmailValidationForInformationInputConfirmationParameters 
		{
				public AaltoGlobalImpact.OIP.TBAccount OwningAccount ;
				public AaltoGlobalImpact.OIP.TBCollaboratingGroup OwningGroup ;
				public InformationInput InformationInput ;
				}
		
		public class CreateAndSendEmailValidationForInformationInputConfirmation 
		{
				private static void PrepareParameters(CreateAndSendEmailValidationForInformationInputConfirmationParameters parameters)
		{
					}
				public static void Execute(CreateAndSendEmailValidationForInformationInputConfirmationParameters parameters)
		{
						PrepareParameters(parameters);
					string[] OwnerEmailAddresses = CreateAndSendEmailValidationForInformationInputConfirmationImplementation.GetTarget_OwnerEmailAddresses(parameters.OwningAccount, parameters.OwningGroup);	
				AaltoGlobalImpact.OIP.TBEmailValidation EmailValidation = CreateAndSendEmailValidationForInformationInputConfirmationImplementation.GetTarget_EmailValidation(parameters.OwningAccount, parameters.OwningGroup, parameters.InformationInput, OwnerEmailAddresses);	
				CreateAndSendEmailValidationForInformationInputConfirmationImplementation.ExecuteMethod_StoreObject(EmailValidation);		
				CreateAndSendEmailValidationForInformationInputConfirmationImplementation.ExecuteMethod_SendEmailConfirmation(parameters.InformationInput, EmailValidation, OwnerEmailAddresses);		
				}
				}
				public class FetchInputInformationParameters 
		{
				public IContainerOwner Owner ;
				public string InformationInputID ;
				public string QueryParameters ;
				}
		
		public class FetchInputInformation 
		{
				private static void PrepareParameters(FetchInputInformationParameters parameters)
		{
					}
				public static void Execute(FetchInputInformationParameters parameters)
		{
						PrepareParameters(parameters);
					InformationInput InformationInput = FetchInputInformationImplementation.GetTarget_InformationInput(parameters.Owner, parameters.InformationInputID);	
				FetchInputInformationImplementation.ExecuteMethod_VerifyValidInput(InformationInput);		
				string InputFetchLocation = FetchInputInformationImplementation.GetTarget_InputFetchLocation(InformationInput);	
				string InputFetchName = FetchInputInformationImplementation.GetTarget_InputFetchName(InformationInput);	
				AuthenticatedAsActiveDevice AuthenticatedAsActiveDevice = FetchInputInformationImplementation.GetTarget_AuthenticatedAsActiveDevice(InformationInput);	
				FetchInputInformationImplementation.ExecuteMethod_FetchInputToStorage(parameters.Owner, parameters.QueryParameters, InformationInput, InputFetchLocation, InputFetchName, AuthenticatedAsActiveDevice);		
				}
				}
				public class ProcessFetchedInputsParameters 
		{
				public IContainerOwner Owner ;
				public string InformationInputID ;
				public string ProcessingOperationName ;
				}
		
		public class ProcessFetchedInputs 
		{
				private static void PrepareParameters(ProcessFetchedInputsParameters parameters)
		{
					}
				public class ProcessInputFromStorageReturnValue 
		{
				public IInformationObject[] ProcessingResultsToStore ;
				public IInformationObject[] ProcessingResultsToDelete ;
				}
				public static void Execute(ProcessFetchedInputsParameters parameters)
		{
						PrepareParameters(parameters);
					InformationInput InformationInput = ProcessFetchedInputsImplementation.GetTarget_InformationInput(parameters.Owner, parameters.InformationInputID);	
				ProcessFetchedInputsImplementation.ExecuteMethod_VerifyValidInput(InformationInput);		
				string InputFetchLocation = ProcessFetchedInputsImplementation.GetTarget_InputFetchLocation(InformationInput);	
				ProcessInputFromStorageReturnValue ProcessInputFromStorageOutput = ProcessFetchedInputsImplementation.ExecuteMethod_ProcessInputFromStorage(parameters.ProcessingOperationName, InformationInput, InputFetchLocation);		
				ProcessFetchedInputsImplementation.ExecuteMethod_StoreObjects(ProcessInputFromStorageOutput.ProcessingResultsToStore);		
				ProcessFetchedInputsImplementation.ExecuteMethod_DeleteObjects(ProcessInputFromStorageOutput.ProcessingResultsToDelete);		
				}
				}
				public class BeginAccountEmailAddressRegistrationParameters 
		{
				public string AccountID ;
				public string EmailAddress ;
				public string RedirectUrlAfterValidation ;
				}
		
		public class BeginAccountEmailAddressRegistration 
		{
				private static void PrepareParameters(BeginAccountEmailAddressRegistrationParameters parameters)
		{
					}
				public static void Execute(BeginAccountEmailAddressRegistrationParameters parameters)
		{
						PrepareParameters(parameters);
					BeginAccountEmailAddressRegistrationImplementation.ExecuteMethod_ValidateUnexistingEmail(parameters.EmailAddress);		
				AaltoGlobalImpact.OIP.TBEmailValidation EmailValidation = BeginAccountEmailAddressRegistrationImplementation.GetTarget_EmailValidation(parameters.AccountID, parameters.EmailAddress, parameters.RedirectUrlAfterValidation);	
				BeginAccountEmailAddressRegistrationImplementation.ExecuteMethod_StoreObject(EmailValidation);		
				BeginAccountEmailAddressRegistrationImplementation.ExecuteMethod_SendEmailConfirmation(EmailValidation);		
				}
				}
				public class UnregisterEmailAddressParameters 
		{
				public string AccountID ;
				public string EmailAddress ;
				}
		
		public class UnregisterEmailAddress 
		{
				private static void PrepareParameters(UnregisterEmailAddressParameters parameters)
		{
					}
				public static void Execute(UnregisterEmailAddressParameters parameters)
		{
						PrepareParameters(parameters);
					AaltoGlobalImpact.OIP.AccountContainer AccountContainerBeforeGroupRemoval = UnregisterEmailAddressImplementation.GetTarget_AccountContainerBeforeGroupRemoval(parameters.AccountID);	
				string EmailAddressID = UnregisterEmailAddressImplementation.GetTarget_EmailAddressID(parameters.EmailAddress, AccountContainerBeforeGroupRemoval);	
				UnregisterEmailAddressImplementation.ExecuteMethod_ExecuteUnlinkEmailAddress(parameters.AccountID, AccountContainerBeforeGroupRemoval, EmailAddressID);		
				}
				}
				public class CreateGroupWithTemplatesParameters 
		{
				public string GroupName ;
				public string AccountID ;
				public string TemplateNameList ;
				public string GroupDefaultRedirect ;
				public string RedirectUrlAfterCreation ;
				}
		
		public class CreateGroupWithTemplates 
		{
				private static void PrepareParameters(CreateGroupWithTemplatesParameters parameters)
		{
					}
				public static void Execute(CreateGroupWithTemplatesParameters parameters)
		{
						PrepareParameters(parameters);
					string ExecuteCreateGroupOutput = CreateGroupWithTemplatesImplementation.ExecuteMethod_ExecuteCreateGroup(parameters.GroupName, parameters.AccountID);		
				CreateGroupWithTemplatesImplementation.ExecuteMethod_CopyGroupTemplates(parameters.TemplateNameList, ExecuteCreateGroupOutput);		
				IContainerOwner GroupAsOwner = CreateGroupWithTemplatesImplementation.GetTarget_GroupAsOwner(ExecuteCreateGroupOutput);	
				
		{ // Local block to allow local naming
			SetOwnerWebRedirectParameters operationParameters = CreateGroupWithTemplatesImplementation.SetDefaultRedirect_GetParameters(parameters.GroupDefaultRedirect, GroupAsOwner);
			SetOwnerWebRedirect.Execute(operationParameters);
									
		} // Local block closing
				CreateGroupWithTemplatesImplementation.ExecuteMethod_InitializeGroupWithDefaultObjects(GroupAsOwner);		
				CreateGroupWithTemplatesImplementation.ExecuteMethod_RedirectToGivenUrl(parameters.RedirectUrlAfterCreation, ExecuteCreateGroupOutput);		
				}
				}
				public class SetOwnerWebRedirectParameters 
		{
				public IContainerOwner Owner ;
				public string RedirectPath ;
				}
		
		public class SetOwnerWebRedirect 
		{
				private static void PrepareParameters(SetOwnerWebRedirectParameters parameters)
		{
					}
				public static void Execute(SetOwnerWebRedirectParameters parameters)
		{
						PrepareParameters(parameters);
					SetOwnerWebRedirectImplementation.ExecuteMethod_SetRedirection(parameters.Owner, parameters.RedirectPath);		
				}
				}
				public class ProcessAllResourceUsagesToOwnerCollectionsParameters 
		{
				public int ProcessBatchSize ;
				}
		
		public class ProcessAllResourceUsagesToOwnerCollections 
		{
				private static void PrepareParameters(ProcessAllResourceUsagesToOwnerCollectionsParameters parameters)
		{
					}
				public static void Execute(ProcessAllResourceUsagesToOwnerCollectionsParameters parameters)
		{
						PrepareParameters(parameters);
					ProcessAllResourceUsagesToOwnerCollectionsImplementation.ExecuteMethod_ExecuteBatchProcessor(parameters.ProcessBatchSize);		
				}
				}
				public class ProcessBatchOfResourceUsagesToOwnerCollectionsParameters 
		{
				public int ProcessBatchSize ;
				public bool ProcessIfLess ;
				}
		
		public class ProcessBatchOfResourceUsagesToOwnerCollections 
		{
				private static void PrepareParameters(ProcessBatchOfResourceUsagesToOwnerCollectionsParameters parameters)
		{
					}
				public static ProcessBatchOfResourceUsagesToOwnerCollectionsReturnValue Execute(ProcessBatchOfResourceUsagesToOwnerCollectionsParameters parameters)
		{
						PrepareParameters(parameters);
					Microsoft.WindowsAzure.StorageClient.CloudBlockBlob[] BatchToProcess = ProcessBatchOfResourceUsagesToOwnerCollectionsImplementation.GetTarget_BatchToProcess(parameters.ProcessBatchSize, parameters.ProcessIfLess);	
				ProcessBatchOfResourceUsagesToOwnerCollectionsImplementation.ExecuteMethod_ProcessBatch(BatchToProcess);		
				ProcessBatchOfResourceUsagesToOwnerCollectionsImplementation.ExecuteMethod_DeleteProcessedItems(BatchToProcess);		
				ProcessBatchOfResourceUsagesToOwnerCollectionsImplementation.ExecuteMethod_ReleaseLock(BatchToProcess);		
				ProcessBatchOfResourceUsagesToOwnerCollectionsReturnValue returnValue = ProcessBatchOfResourceUsagesToOwnerCollectionsImplementation.Get_ReturnValue(parameters.ProcessBatchSize, BatchToProcess);
		return returnValue;
				}
				}
				public class ProcessBatchOfResourceUsagesToOwnerCollectionsReturnValue 
		{
				public bool ProcessedAnything ;
				public bool ProcessedFullCount ;
				}
				public class UpdateUsageMonitoringSummariesParameters 
		{
				public IContainerOwner Owner ;
				public int AmountOfDays ;
				}
		
		public class UpdateUsageMonitoringSummaries 
		{
				private static void PrepareParameters(UpdateUsageMonitoringSummariesParameters parameters)
		{
					}
				public static void Execute(UpdateUsageMonitoringSummariesParameters parameters)
		{
						PrepareParameters(parameters);
					UsageMonitorItem[] SourceItems = UpdateUsageMonitoringSummariesImplementation.GetTarget_SourceItems(parameters.Owner, parameters.AmountOfDays);	
				UpdateUsageMonitoringSummariesImplementation.ExecuteMethod_CreateUsageMonitoringSummaries(parameters.Owner, SourceItems);		
				}
				}
				public class UpdateUsageMonitoringItemsParameters 
		{
				public IContainerOwner Owner ;
				public int MonitoringItemTimeSpanInMinutes ;
				public int MonitoringIntervalInMinutes ;
				}
		
		public class UpdateUsageMonitoringItems 
		{
				private static void PrepareParameters(UpdateUsageMonitoringItemsParameters parameters)
		{
					}
				public static void Execute(UpdateUsageMonitoringItemsParameters parameters)
		{
						PrepareParameters(parameters);
					UpdateUsageMonitoringItemsImplementation.ExecuteMethod_ValidateEqualSplitOfIntervalsInTimeSpan(parameters.MonitoringItemTimeSpanInMinutes, parameters.MonitoringIntervalInMinutes);		
				Microsoft.WindowsAzure.StorageClient.CloudBlockBlob[] CurrentMonitoringItems = UpdateUsageMonitoringItemsImplementation.GetTarget_CurrentMonitoringItems(parameters.Owner);	
				DateTime EndingTimeOfCurrentItems = UpdateUsageMonitoringItemsImplementation.GetTarget_EndingTimeOfCurrentItems(CurrentMonitoringItems);	
				Microsoft.WindowsAzure.StorageClient.CloudBlockBlob[] NewResourceUsageBlobs = UpdateUsageMonitoringItemsImplementation.GetTarget_NewResourceUsageBlobs(parameters.Owner, EndingTimeOfCurrentItems);	
				DateTime StartingTimeOfNewItems = UpdateUsageMonitoringItemsImplementation.GetTarget_StartingTimeOfNewItems(parameters.MonitoringItemTimeSpanInMinutes, EndingTimeOfCurrentItems, NewResourceUsageBlobs);	
				DateTime EndingTimeOfNewItems = UpdateUsageMonitoringItemsImplementation.GetTarget_EndingTimeOfNewItems(parameters.MonitoringItemTimeSpanInMinutes, StartingTimeOfNewItems, NewResourceUsageBlobs);	
				RequestResourceUsageCollection[] ResourcesToIncludeInMonitoring = UpdateUsageMonitoringItemsImplementation.GetTarget_ResourcesToIncludeInMonitoring(NewResourceUsageBlobs, EndingTimeOfNewItems);	
				UsageMonitorItem[] NewMonitoringItems = UpdateUsageMonitoringItemsImplementation.GetTarget_NewMonitoringItems(parameters.Owner, parameters.MonitoringItemTimeSpanInMinutes, parameters.MonitoringIntervalInMinutes, StartingTimeOfNewItems, EndingTimeOfNewItems);	
				UpdateUsageMonitoringItemsImplementation.ExecuteMethod_PopulateMonitoringItems(ResourcesToIncludeInMonitoring, NewMonitoringItems);		
				UpdateUsageMonitoringItemsImplementation.ExecuteMethod_StoreObjects(NewMonitoringItems);		
				}
				}
		 } 