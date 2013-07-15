 

using System;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;

		namespace Caloom.Schools { 
				public class CreateProductParameters 
		{
				public string Title ;
				}
		
		public class CreateProduct 
		{
				private static void PrepareParameters(CreateProductParameters parameters)
		{
					}
				public static void Execute(CreateProductParameters parameters)
		{
						PrepareParameters(parameters);
					Product ProductRoot = CreateProductImplementation.GetTarget_ProductRoot(parameters.Title);	
				CreateProductImplementation.ExecuteMethod_StoreObjects(ProductRoot);		
				}
				}
		 } 