using System;
using System.Collections.Generic;
using System.Web;
using AzureSupport;
using TheBall;

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
            foreach (var parentItem in hierarchy)
                SetParentsRecursively(parentItem, null);
        }

        private static void SetParentsRecursively(ParentToChildren parentItem, string parentID)
        {
            var owner = InformationContext.Current.Owner;
            int retryCount = 3;
            while (retryCount-- > 0)
            {
                try
                {
                    string currID = parentItem.id;
                    Category cat = Category.RetrieveFromOwnerContent(owner, currID);
                    if (cat.ParentCategoryID != parentID)
                    {
                        cat.ParentCategoryID = parentID;
                        cat.StoreInformation();
                    }
                }
                catch
                {
                    if (retryCount == 0)
                        throw;
                }
            }
            if (parentItem.children == null)
                return;
            foreach(var childItem in parentItem.children)
                SetParentsRecursively(childItem, parentItem.id);
        }
    }
}