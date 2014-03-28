using System;
using System.Collections.Generic;
using System.Linq;
using TheBall;
using TheBall.CORE;
using TheBall.Interface;
using INT = TheBall.Interface;

namespace AaltoGlobalImpact.OIP
{
    public class PickCategorizedContentToConnectionImplementation
    {
        public static Connection GetTarget_Connection(string connectionId)
        {
            Connection connection = Connection.RetrieveFromOwnerContent(InformationContext.CurrentOwner,
                                                                        connectionId);
            return connection;
        }

        public static Dictionary<string, Category> GetTarget_CategoriesToTransfer(Connection connection)
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
            var childrenInclusiveSourceIDs = connection.CategoryLinks.Where(catLink => catLink.LinkingType == INT.Category.LINKINGTYPE_INCLUDECHILDREN).Select(catLink => catLink.SourceCategoryID).ToArray();
            var childrenInclusiveIDs = transferCategories
                .Where(tCat => childrenInclusiveSourceIDs.Contains(tCat.ID))
                .Select(tCat => tCat.NativeCategoryID).OrderBy(str => str)
                .ToList();
            var matchIDs = transferCategories
                .Select(tCat => tCat.NativeCategoryID).OrderBy(str => str)
                .ToList();
            var result =
                sourceCategoryList
                    .Where(cat => matchesOrParentMatches(cat, matchIDs, childrenInclusiveIDs, sourceCategoryDict))
                    .ToArray();
            return result.ToDictionary(cat => cat.ID);
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

        public static string[] GetTarget_ContentToTransferLocations(Dictionary<string, Category> categoriesToTransfer)
        {
            BinaryFileCollection binaryFiles =
                BinaryFileCollection.RetrieveFromOwnerContent(InformationContext.CurrentOwner,
                                                              "MasterCollection");
            LinkToContentCollection linkToContents =
                LinkToContentCollection.RetrieveFromOwnerContent(InformationContext.CurrentOwner,
                                                                 "MasterCollection");
            EmbeddedContentCollection embeddedContents =
                EmbeddedContentCollection.RetrieveFromOwnerContent(InformationContext.CurrentOwner,
                                                                   "MasterCollection");
            ImageCollection images =
                ImageCollection.RetrieveFromOwnerContent(InformationContext.CurrentOwner,
                                                         "MasterCollection");

            TextContentCollection textContents =
                TextContentCollection.RetrieveFromOwnerContent(InformationContext.CurrentOwner,
                                                               "MasterCollection");
            var locationCategoriesTuples = binaryFiles.CollectionContent
                                                      .Select(bf => new Tuple<string, List<Category>>(bf.RelativeLocation, bf.Categories.CollectionContent))
                                                      .Union(linkToContents.CollectionContent
                                                                           .Select(linkTo => new Tuple<string, List<Category>>(linkTo.RelativeLocation, 
                                                                               linkTo.Categories != null ? linkTo.Categories.CollectionContent : new List<Category>())))
                                                      .Union(embeddedContents.CollectionContent
                                                                             .Select(embedded => new Tuple<string, List<Category>>(embedded.RelativeLocation, 
                                                                                 embedded.Categories != null ? embedded.Categories.CollectionContent : new List<Category>())))
                                                      .Union(images.CollectionContent
                                                                   .Select(image => new Tuple<string, List<Category>>(image.RelativeLocation, 
                                                                       image.Categories != null ? image.Categories.CollectionContent : new List<Category>())))
                                                      .Union(textContents.CollectionContent
                                                                         .Select(txtC => new Tuple<string, List<Category>>(txtC.RelativeLocation, 
                                                                             txtC.Categories != null ? txtC.Categories.CollectionContent : new List<Category>()))).ToArray();
                    
                        


            string[] locations = getLocationsOfObjectsThatBelongToCategory(categoriesToTransfer,
                                    locationCategoriesTuples);
            return locations;
        }

        private static string[] getLocationsOfObjectsThatBelongToCategory(Dictionary<string, Category> categoriesToTransfer, Tuple<string, List<Category>>[] locationCategoryTuples)
        {
            var resultLocations = locationCategoryTuples.Where(tuple => tuple.Item2.Any(cat => categoriesToTransfer.ContainsKey(cat.ID)))
                                                        .Select(tuple => tuple.Item1).ToArray();
            return resultLocations;
        }

        public static PickCategorizedContentToConnectionReturnValue Get_ReturnValue(string[] contentToTransferLocations)
        {
            return new PickCategorizedContentToConnectionReturnValue { ContentLocations = contentToTransferLocations };
        }

    }
}