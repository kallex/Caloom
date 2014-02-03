using System.Collections.Generic;
using System.Linq;
using TheBall;
using TheBall.Interface;
using INT = TheBall.Interface;

namespace AaltoGlobalImpact.OIP
{
    public class PackageCategorizedContentToConnectionImplementation
    {
        public static Connection GetTarget_Connection(string connectionId)
        {
            Connection connection = Connection.RetrieveFromOwnerContent(InformationContext.CurrentOwner,
                                                                        connectionId);
            return connection;
        }

        public static Category[] GetTarget_CategoriesToTransfer(Connection connection)
        {
            var transferCategories =
                connection.ThisSideCategories
                          .Where(
                              tCat => tCat.NativeCategoryDomainName == "AaltoGlobalImpact.OIP" &&
                                      tCat.NativeCategoryObjectName == "Category")
                          .ToArray();
            CategoryCollection categoryCollection = CategoryCollection.RetrieveFromOwnerContent(
                InformationContext.CurrentOwner, "MasterCollection");
            var sourceCategoryDict = categoryCollection.CollectionContent.ToDictionary(cat => cat.ID);
            var sourceCategoryList = categoryCollection.CollectionContent;
            var childrenInclusiveIDs = transferCategories
                .Where(tCat => tCat.LinkingType == INT.Category.LINKINGTYPE_INCLUDECHILDREN)
                .Select(tCat => tCat.NativeCategoryID).OrderBy(str => str)
                .ToList();
            var matchIDs = transferCategories
                .Select(tCat => tCat.NativeCategoryID).OrderBy(str => str)
                .ToList();
            var result =
                sourceCategoryList
                    .Where(cat => matchesOrParentMatches(cat, matchIDs, childrenInclusiveIDs, sourceCategoryDict))
                    .ToArray();
            return result;
        }

        private static bool matchesOrParentMatches(Category cat, List<string> matchIDs, List<string> childrenInclusiveIDs, Dictionary<string, Category> categoryDict)
        {
            if (matchIDs.BinarySearch(cat.ID) >= 0)
                return true;
            if (childrenInclusiveIDs.BinarySearch(cat.ID) >= 0)
                return true;
            if (cat.ParentCategoryID != null && categoryDict.ContainsKey(cat.ParentCategoryID))
            {
                Category parentCategory = categoryDict[cat.ParentCategoryID];
                return matchesOrParentMatches(parentCategory, matchIDs, childrenInclusiveIDs, categoryDict);
            }
            return false;
        }

        public static string[] GetTarget_ContentToTransferLocations(Category[] filteredCategories)
        {
            throw new System.NotImplementedException();
        }

        public static PackageCategorizedContentToConnectionReturnValue Get_ReturnValue(string[] filteredContentLocations)
        {
            throw new System.NotImplementedException();
        }
    }
}