 

using System;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;

		namespace Caloom.Housing { 
		
		public class CreateHouse 
		{
				public static void Execute()
		{
						
					House HouseRoot = CreateHouseImplementation.GetTarget_HouseRoot();	
				CreateHouseImplementation.ExecuteMethod_StoreObjects(HouseRoot);		
				}
				}
		 } 