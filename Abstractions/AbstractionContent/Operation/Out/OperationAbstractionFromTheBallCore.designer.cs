 

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
					IInformationObject ObjectToDelete = DeleteSpecifiedInformationObjectImplementation.GetTarget_ObjectToDelete(parameters.Owner, parameters.ObjectDomainName, parameters.ObjectName, parameters.ObjectID);	
				DeleteSpecifiedInformationObjectImplementation.ExecuteMethod_DeleteObject(ObjectToDelete);		
				}
				}
		 } 