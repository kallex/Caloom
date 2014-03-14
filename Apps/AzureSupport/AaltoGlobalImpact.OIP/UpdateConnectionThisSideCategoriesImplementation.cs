using System.Diagnostics;
using TheBall;
using TheBall.CORE;
using TheBall.Interface;
using System.Linq;
using Process = TheBall.CORE.Process;

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

        public static Connection GetTarget_Connection(Process process)
        {
            string connectionID = process.InitialArguments.First(arg => arg.ItemFullType == "ConnectionID").ItemValue;
            return Connection.RetrieveFromOwnerContent(Owner, connectionID);
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
                var matchingCategory = thisSideCategories.First(cat => cat.NativeCategoryID == nativeCategory.ID);
                matchingCategory.IdentifyingCategoryName = nativeCategory.CategoryName;
                matchingCategory.NativeCategoryTitle = nativeCategory.Title;
                TheBall.Interface.Category parentCategory = null;
                if (string.IsNullOrEmpty(nativeCategory.ParentCategoryID) == false)
                {
                    parentCategory = thisSideCategories.First(cat => cat.NativeCategoryID == nativeCategory.ParentCategoryID);
                }
                if (parentCategory != null)
                {
                    matchingCategory.ParentCategoryID = parentCategory.ID;
                }
            }
            Debug.Assert(thisSideCategories.Count == activeCategories.Length);
            var finalList = activeCategories.Select(activeCat => thisSideCategories.First(cat => cat.NativeCategoryID == activeCat.ID)).ToList();
            connection.ThisSideCategories = finalList;
        }

        private static TheBall.Interface.Category categoryToInterfaceCategory(Category cat)
        {
            return new TheBall.Interface.Category
                {
                    NativeCategoryDomainName = cat.SemanticDomainName,
                    NativeCategoryObjectName = cat.Name,
                    NativeCategoryID = cat.ID,
                    NativeCategoryTitle = cat.Title,
                    IdentifyingCategoryName = cat.CategoryName
                };
        }

        public static void ExecuteMethod_StoreObject(Connection connection)
        {
            connection.StoreInformation();
        }
    }
}