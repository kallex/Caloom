 

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
				public class CreateInformationInputParameters 
		{
				public IContainerOwner Owner ;
				public string InputDescription ;
				public string LocationURL ;
				}
		
		public class CreateInformationInput 
		{
				private static void PrepareParameters(CreateInformationInputParameters parameters)
		{
					}
				public static CreateInformationInputReturnValue Execute(CreateInformationInputParameters parameters)
		{
						PrepareParameters(parameters);
					InformationInput CreatedInformationInput = CreateInformationInputImplementation.GetTarget_CreatedInformationInput(parameters.Owner, parameters.InputDescription, parameters.LocationURL);	
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
				FetchInputInformationImplementation.ExecuteMethod_FetchInputToStorage(parameters.Owner, parameters.QueryParameters, InformationInput, InputFetchLocation, InputFetchName);		
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
		 } 