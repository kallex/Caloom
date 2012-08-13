 

using System;

		namespace AaltoGlobalImpact.OIP { 
				public class UpdatePageContentParameters 
		{
				public string changedInformation ;
				}
		
		public class UpdatePageContent 
		{
				private static void PrepareParameters(UpdatePageContentParameters parameters)
		{
					}
				public static void Execute(UpdatePageContentParameters parameters)
		{
						PrepareParameters(parameters);
					UpdatePageContentImplementation.ExecuteMethod_UpdatePage();		
				}
				}
		} 