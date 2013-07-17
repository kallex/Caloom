 

using System;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;

		namespace Caloom.Schools { 
		
		public class CreateTrainingModule 
		{
				public static void Execute()
		{
						
					TrainingModule TrainingModuleRoot = CreateTrainingModuleImplementation.GetTarget_TrainingModuleRoot();	
				CreateTrainingModuleImplementation.ExecuteMethod_StoreObjects(TrainingModuleRoot);		
				}
				}
		 } 