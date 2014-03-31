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
            var exactMatchSourceIDs = connection.CategoryLinks.Where(catLink => catLink.LinkingType == INT.Category.LINKINGTYPE_ONE).Select(catLink => catLink.SourceCategoryID).ToArray();
            var exactMatchIDs = transferCategories
                .Where(tCat => exactMatchSourceIDs.Contains(tCat.ID))
                .Select(tCat => tCat.NativeCategoryID).OrderBy(str => str)
                .ToList();
            var result =
                sourceCategoryList
                    .Where(cat => matchesOrParentMatches(cat, exactMatchIDs, childrenInclusiveIDs, sourceCategoryDict))
                    .ToArray();
            return result.ToDictionary(cat => cat.ID);
        }

        private static bool matchesOrParentMatches(Category cat, List<string> exactMatchIDs, List<string> childrenInclusiveIDs, Dictionary<string, Category> categoryDict)
        {
            if (exactMatchIDs != null && exactMatchIDs.BinarySearch(cat.ID) >= 0)
                return true;
            if (childrenInclusiveIDs.BinarySearch(cat.ID) >= 0)
                return true;
            if (cat.ParentCategoryID != null && categoryDict.ContainsKey(cat.ParentCategoryID))
            {
                Category parentCategory = categoryDict[cat.ParentCategoryID];
                return matchesOrParentMatches(parentCategory, null, childrenInclusiveIDs, categoryDict);
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
            var locationCategoriesTuplesWithMedia = binaryFiles.CollectionContent
                                                               .Select(getBinaryFileTuple)
                                                               .Union(linkToContents.CollectionContent.Select(getLinkToContentTuple))
                                                               .Union(embeddedContents.CollectionContent.Select(getEmbeddedContentTuple))
                                                               .Union(images.CollectionContent.Select(getImageTuple))
                                                               .Union(textContents.CollectionContent.Select(getTextContentTuple)).ToArray();
                    
                        


            string[] locations = getLocationsOfObjectsThatBelongToCategory(categoriesToTransfer,
                                    locationCategoriesTuplesWithMedia);
            return locations;
        }

        private static string[] getLocationsOfObjectsThatBelongToCategory(Dictionary<string, Category> categoriesToTransfer, Tuple<string, List<Category>, List<MediaContent>>[] locationCategoriesTuplesWithMedia)
        {
            var resultLocations = locationCategoriesTuplesWithMedia.Where(tuple => tuple.Item2.Any(cat => categoriesToTransfer.ContainsKey(cat.ID)))
                                                        .SelectMany(tuple =>
                                                            {
                                                                List<string> joiningValues = new List<string>();
                                                                joiningValues.Add(tuple.Item1);
                                                                joiningValues.AddRange(tuple.Item3.Select(mc => mc.RelativeLocation));
                                                                return joiningValues.ToArray();
                                                            }).ToArray();
            return resultLocations;
        }

        public static PickCategorizedContentToConnectionReturnValue Get_ReturnValue(string[] contentToTransferLocations)
        {
            return new PickCategorizedContentToConnectionReturnValue { ContentLocations = contentToTransferLocations };
        }

        private static Tuple<string, List<Category>, List<MediaContent>> getBinaryFileTuple(BinaryFile binaryFile)
        {
            List<MediaContent> mediaContents = new List<MediaContent>();
            if(binaryFile.Data != null)
                mediaContents.Add(binaryFile.Data);
            return new Tuple<string, List<Category>, List<MediaContent>>(binaryFile.RelativeLocation, binaryFile.Categories != null ?
                binaryFile.Categories.CollectionContent : new List<Category>(), mediaContents);
        }

        private static Tuple<string, List<Category>, List<MediaContent>> getLinkToContentTuple(LinkToContent linkTo)
        {
            List<MediaContent> mediaContents = new List<MediaContent>();
            if(linkTo.ImageData != null)
                mediaContents.Add(linkTo.ImageData);
            return new Tuple<string, List<Category>, List<MediaContent>>(linkTo.RelativeLocation,
                linkTo.Categories != null ? linkTo.Categories.CollectionContent : new List<Category>(),
                mediaContents);
        }

        private static Tuple<string, List<Category>, List<MediaContent>> getEmbeddedContentTuple(EmbeddedContent embedded)
        {
            List<MediaContent> mediaContents = new List<MediaContent>();
            return new Tuple<string, List<Category>, List<MediaContent>>(embedded.RelativeLocation,
                embedded.Categories != null ? embedded.Categories.CollectionContent : new List<Category>(),
                mediaContents);
        }

        private static Tuple<string, List<Category>, List<MediaContent>> getImageTuple(Image image)
        {
            List<MediaContent> mediaContents = new List<MediaContent>();
            if(image.ImageData != null)
                mediaContents.Add(image.ImageData);
            return new Tuple<string, List<Category>, List<MediaContent>>(image.RelativeLocation,
                image.Categories != null ? image.Categories.CollectionContent : new List<Category>(),
                mediaContents);
        }

        private static Tuple<string, List<Category>, List<MediaContent>> getTextContentTuple(TextContent textContent)
        {
            List<MediaContent> mediaContents = new List<MediaContent>();
            if(textContent.ImageData != null)
                mediaContents.Add(textContent.ImageData);
            return new Tuple<string, List<Category>, List<MediaContent>>(textContent.RelativeLocation,
                textContent.Categories != null ? textContent.Categories.CollectionContent : new List<Category>(),
                mediaContents);
        }


        /*
         * 
         *                                                       linkToContents.CollectionContent
                                                                           .Select(linkTo => new Tuple<string, List<Category>>(linkTo.RelativeLocation, 
                                                                               linkTo.Categories != null ? linkTo.Categories.CollectionContent : new List<Category>())))

         * 
         *                                                       .Union(embeddedContents.CollectionContent
                                                                             .Select(embedded => new Tuple<string, List<Category>>(embedded.RelativeLocation, 
                                                                                 embedded.Categories != null ? embedded.Categories.CollectionContent : new List<Category>())))
                                                      .Union(images.CollectionContent
                                                                   .Select(image => new Tuple<string, List<Category>>(image.RelativeLocation, 
                                                                       image.Categories != null ? image.Categories.CollectionContent : new List<Category>())))
                                                      .Union(textContents.CollectionContent
                                                                         .Select(txtC => new Tuple<string, List<Category>>(txtC.RelativeLocation, 
                                                                             txtC.Categories != null ? txtC.Categories.CollectionContent : new List<Category>()))).ToArray();

         * */

    }
}