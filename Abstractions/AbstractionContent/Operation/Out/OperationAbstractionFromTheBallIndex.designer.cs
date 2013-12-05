 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

		namespace TheBall.Index { 
				public class IndexInformationObjectParameters 
		{
				public IContainerOwner Owner ;
				public IInformationObject InformationObject ;
				}
		
		public class IndexInformationObject 
		{
				private static void PrepareParameters(IndexInformationObjectParameters parameters)
		{
					}
				public static void Execute(IndexInformationObjectParameters parameters)
		{
						PrepareParameters(parameters);
					IndexInformationObjectImplementation.ExecuteMethod_PerformIndexing(parameters.Owner, parameters.InformationObject);		
				}
				}
		 } 