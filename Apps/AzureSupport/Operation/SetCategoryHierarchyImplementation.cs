using System;
using System.Collections.Generic;
using System.Web;
using AzureSupport;

namespace AaltoGlobalImpact.OIP
{
    public class SetCategoryHierarchyImplementation
    {
        public static ParentToChildren[] GetTarget_Hierarchy()
        {
            var request = HttpContext.Current.Request;
            var stream = request.GetBufferlessInputStream();
            var result = JSONSupport.GetObjectFromStream<ParentToChildren[]>(stream);
            return result;
        }

        public static void ExecuteMethod_SetParentCategories(ParentToChildren[] hierarchy)
        {
            // TODO: Implement
        }
    }
}