 

				using System;

		namespace Caloom.Web.HTML5 {
						public partial class TestClass {
							public int TestInt { get; set; }
					public HierarchyItem TestHierarchy { get; set; }
									}
						public partial class HierarchyItem {
							public long TestLong { get; set; }
					public HierarchyItem RecursiveHierarchy { get; set; }
									}
					}
		