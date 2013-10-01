using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AzureSupport;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    public class SetCategoryHierarchyAndOrderInNodeSummaryImplementation
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

        public static NodeSummaryContainer GetTarget_NodeSummaryContainer()
        {
            var owner = InformationContext.Current.Owner;
            return NodeSummaryContainer.RetrieveFromOwnerContent(owner, "default");
        }

        public static void ExecuteMethod_SetCategoryOrder(ParentToChildren[] hierarchy, NodeSummaryContainer nodeSummaryContainer)
        {
            List<string> flattenedIDList = new List<string>();
            flattenHierarchyIDList(hierarchy, flattenedIDList);
            var flattenedArray = flattenedIDList.ToArray();
            string commaSeparatedIDs = String.Join(",", flattenedArray);
            nodeSummaryContainer.NodeSourceCategories.SelectedIDCommaSeparated = commaSeparatedIDs;
            var newList =
                nodeSummaryContainer.NodeSourceCategories.CollectionContent.OrderBy(
                    cat => flattenedIDList.IndexOf(cat.ID)).ToList();
            nodeSummaryContainer.NodeSourceCategories.CollectionContent = newList;
        }

        private static void flattenHierarchyIDList(ParentToChildren[] hierarchy, List<string> flattenedIdList)
        {
            foreach (var item in hierarchy)
            {
                flattenedIdList.Add(item.id);
                if(item.children != null)
                    flattenHierarchyIDList(item.children, flattenedIdList);
            }
        }

        public static void ExecuteMethod_StoreObject(NodeSummaryContainer nodeSummaryContainer)
        {
            nodeSummaryContainer.StoreInformation();
        }
    }
}