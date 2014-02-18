 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

		namespace TheBall.Infrastructure { 
				public class CreateCloudDriveParameters 
		{
				public string DriveName ;
				public int SizeInMegabytes ;
				}
		
		public class CreateCloudDrive 
		{
				private static void PrepareParameters(CreateCloudDriveParameters parameters)
		{
					}
				public static CreateCloudDriveReturnValue Execute(CreateCloudDriveParameters parameters)
		{
						PrepareParameters(parameters);
					string DriveBlobName = CreateCloudDriveImplementation.GetTarget_DriveBlobName(parameters.DriveName);	
				CreateCloudDriveReturnValue CreateDriveOutput = CreateCloudDriveImplementation.ExecuteMethod_CreateDrive(parameters.SizeInMegabytes, DriveBlobName);		
				CreateCloudDriveReturnValue returnValue = CreateCloudDriveImplementation.Get_ReturnValue(CreateDriveOutput);
		return returnValue;
				}
				}
				public class CreateCloudDriveReturnValue 
		{
				public Microsoft.WindowsAzure.StorageClient.CloudDrive CloudDrive ;
				public System.Exception Exception ;
				}
				public class MountCloudDriveParameters 
		{
				public Microsoft.WindowsAzure.StorageClient.CloudDrive DriveReference ;
				}
		
		public class MountCloudDrive 
		{
				private static void PrepareParameters(MountCloudDriveParameters parameters)
		{
					}
				public static MountCloudDriveReturnValue Execute(MountCloudDriveParameters parameters)
		{
						PrepareParameters(parameters);
					MountCloudDriveReturnValue MountDriveOutput = MountCloudDriveImplementation.ExecuteMethod_MountDrive(parameters.DriveReference);		
				MountCloudDriveReturnValue returnValue = MountCloudDriveImplementation.Get_ReturnValue(MountDriveOutput);
		return returnValue;
				}
				}
				public class MountCloudDriveReturnValue 
		{
				public string MountedDriveLetter ;
				public System.Exception Exception ;
				}
		 } 