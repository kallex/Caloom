 

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
		 } 