using System.Diagnostics;
using TheBall;
using TheBall.CORE;
using TheBall.Interface;
using System.Linq;

namespace AaltoGlobalImpact.OIP
{
    public class UpdateConnectionThisSideCategoriesImplementation
    {
        private static IContainerOwner Owner
        {
            get { return InformationContext.CurrentOwner; }
        }
        
        public static NodeSummaryContainer GetTarget_CurrentCategoryContainer()
        {
            NodeSummaryContainer nodeSummaryContainer = NodeSummaryContainer.RetrieveFromOwnerContent(Owner, "default");
            return nodeSummaryContainer;
        }

        public static Category[] GetTarget_ActiveCategories(NodeSummaryContainer currentCategoryContainer)
        {
            return currentCategoryContainer.NodeSourceCategories.GetIDSelectedArray();
        }

        public static Connection GetTarget_Connection(string connectionId)
        {
            return Connection.RetrieveFromOwnerContent(Owner, connectionId);
        }

        public static void ExecuteMethod_UpdateThisSideCategories(Connection connection, Category[] activeCategories)
        {
            var activeCategoryIDs = activeCategories.Select(cat => cat.ID).ToArray();
            var thisSideCategories = connection.ThisSideCategories;
            var thisSideCategoryIDs = thisSideCategories.Select(cat => cat.NativeCategoryID).ToArray();
            thisSideCategories.RemoveAll(cat => activeCategoryIDs.Contains(cat.NativeCategoryID) == false);
            var missingCategoryIDs = activeCategoryIDs.Except(thisSideCategoryIDs);
            var missingCategories = activeCategories.Where(cat => missingCategoryIDs.Contains(cat.ID)).ToArray();
            thisSideCategories.AddRange(missingCategories.Select(categoryToInterfaceCategory));
            foreach (var nativeCategory in activeCategories)
            {
                var matchinCategory = thisSideCategories.First(cat => cat.NativeCategoryID == nativeCategory.ID);
                TheBall.Interface.Category parentCategory = null;
                if (string.IsNullOrEmpty(nativeCategory.ParentCategoryID) == false)
                {
                    parentCategory = thisSideCategories.First(cat => cat.NativeCategoryID == nativeCategory.ParentCategoryID);
                }
                if (parentCategory != null)
                {
                    matchinCategory.ParentCategoryID = parentCategory.ID;
                }
            }
            Debug.Assert(thisSideCategories.Count == activeCategories.Length);
            var finalList = activeCategories.Select(activeCat => thisSideCategories.First(cat => cat.ParentCategoryID == activeCat.ID)).ToList();
            connection.ThisSideCategories = finalList;
        }

        private static TheBall.Interface.Category categoryToInterfaceCategory(Category cat)
        {
            return new TheBall.Interface.Category
                {
                    NativeCategoryDomainName = cat.SemanticDomainName,
                    NativeCategoryObjectName = cat.Name,
                    NativeCategoryID = cat.ID,
                    IdentifyingCategoryName = cat.CategoryName
                };
        }

        public static void ExecuteMethod_StoreObject(Connection connection)
        {
            connection.StoreInformation();
        }
    }
}