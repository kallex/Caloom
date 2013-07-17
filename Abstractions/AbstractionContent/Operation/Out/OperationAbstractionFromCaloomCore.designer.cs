 

using System;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;

		namespace Caloom.CORE { 
				public class CreateAndUpdateProductParameters 
		{
				public string Title ;
				public string Excerpt ;
				public string Description ;
				}
		
		public class CreateAndUpdateProduct 
		{
				private static void PrepareParameters(CreateAndUpdateProductParameters parameters)
		{
					}
				public static void Execute(CreateAndUpdateProductParameters parameters)
		{
						PrepareParameters(parameters);
					string CreateProductOutput;
		{ // Local block to allow local naming
			
			var operationReturnValue = CreateProduct.Execute();
			CreateProductOutput = CreateAndUpdateProductImplementation.CreateProduct_GetOutput(operationReturnValue);						
		} // Local block closing
				
		{ // Local block to allow local naming
			UpdateProductParameters operationParameters = CreateAndUpdateProductImplementation.UpdateProduct_GetParameters(parameters.Title, parameters.Excerpt, parameters.Description, CreateProductOutput);
			var operationReturnValue = UpdateProduct.Execute(operationParameters);
									
		} // Local block closing
				}
				}

		    public class CreateProduct 
		{
				public static CreateProductReturnValue Execute()
		{
						
					Product ProductRoot = CreateProductImplementation.GetTarget_ProductRoot();	
				CreateProductImplementation.ExecuteMethod_StoreObjects(ProductRoot);		
				CreateProductReturnValue returnValue = CreateProductImplementation.Get_ReturnValue();
		return returnValue;
				}
				}

		    public class CreateProductReturnValue 
		{
				public string ProductID ;
				}
				public class UpdateProductParameters 
		{
				public string ProductID ;
				public string Title ;
				public string Excerpt ;
				public string Description ;
				}
		
		public class UpdateProduct 
		{
				private static void PrepareParameters(UpdateProductParameters parameters)
		{
					}
				public static UpdateProductReturnValue Execute(UpdateProductParameters parameters)
		{
						PrepareParameters(parameters);
					Product ProductRoot = UpdateProductImplementation.GetTarget_ProductRoot(parameters.ProductID);	
				UpdateProductImplementation.ExecuteMethod_UpdateProductInfo(parameters.Title, parameters.Excerpt, parameters.Description, ProductRoot);		
				UpdateProductImplementation.ExecuteMethod_StoreObjects(ProductRoot);		
				UpdateProductReturnValue returnValue = UpdateProductImplementation.Get_ReturnValue();
		return returnValue;
				}
				}

		    public class UpdateProductReturnValue 
		{
				public string ProductID ;
				}
				public class SetSubProductUsageParameters 
		{
				public string UsingProductID ;
				public string SubProductBeingUsedID ;
				public double AmountOfUsage ;
				}
		
		public class SetSubProductUsage 
		{
				private static void PrepareParameters(SetSubProductUsageParameters parameters)
		{
					}
				public static void Execute(SetSubProductUsageParameters parameters)
		{
						PrepareParameters(parameters);
					Product UsingProduct = SetSubProductUsageImplementation.GetTarget_UsingProduct(parameters.UsingProductID);	
				Product SubProductBeingUsed = SetSubProductUsageImplementation.GetTarget_SubProductBeingUsed(parameters.SubProductBeingUsedID);	
				SetSubProductUsageImplementation.ExecuteMethod_SetSubProductUsage(parameters.AmountOfUsage, UsingProduct, SubProductBeingUsed);		
				SetSubProductUsageImplementation.ExecuteMethod_StoreObjects(UsingProduct, SubProductBeingUsed);		
				}
				}

		    public class RemoveSubProductUsageParameters 
		{
				public string UsingProductID ;
				public string SubProductBeingUsedID ;
				}
		
		public class RemoveSubProductUsage 
		{
				private static void PrepareParameters(RemoveSubProductUsageParameters parameters)
		{
					}
				public static void Execute(RemoveSubProductUsageParameters parameters)
		{
						PrepareParameters(parameters);
					Product UsingProduct = RemoveSubProductUsageImplementation.GetTarget_UsingProduct(parameters.UsingProductID);	
				RemoveSubProductUsageImplementation.ExecuteMethod_RemoveSubProductUsage(parameters.SubProductBeingUsedID, UsingProduct);		
				RemoveSubProductUsageImplementation.ExecuteMethod_StoreObjects(UsingProduct);		
				}
				}
		} 