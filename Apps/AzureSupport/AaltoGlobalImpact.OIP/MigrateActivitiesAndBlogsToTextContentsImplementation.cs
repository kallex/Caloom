using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheBall;
using TheBall.CORE;

namespace AaltoGlobalImpact.OIP
{
    public class MigrateActivitiesAndBlogsToTextContentsImplementation
    {
        static IContainerOwner Owner
        {
            get { return InformationContext.CurrentOwner; }
        }

        public static void ExecuteMethod_ExecuteProcess(Process process)
        {
            bool isInitialRun = process.ProcessItems.Count == 0;
            if (isInitialRun)
                performInitialMigration(process);
            else
                performUpdatingMigration(process);
        }

        private static void performUpdatingMigration(Process process)
        {
            performInitialMigration(process);
        }

        private static void performInitialMigration(Process process)
        {
            var argDict = process.InitialArguments.ToDictionary(item => item.ItemFullType, item => item.ItemValue);
            string inputRoot = argDict["InputRoot"];
            string blogFullName = typeof (Blog).FullName;
            var blogs = StorageSupport.CurrActiveContainer
                                      .GetInformationObjects(Owner, inputRoot, fullName =>
                                                                               InformationObjectSupport.IsContentGivenType(fullName, blogFullName))
                                      .Cast<Blog>();
            string activityFullName = typeof (Activity).FullName;
            var activities = StorageSupport.CurrActiveContainer
                                           .GetInformationObjects(Owner, inputRoot, fullName =>
                                                                                    InformationObjectSupport.IsContentGivenType(fullName, activityFullName))
                                           .Cast<Activity>();
            var targetCategoryCollection =
                CategoryCollection.RetrieveFromOwnerContent(Owner, "MasterCollection");
            var targetAddressAndLocationCollection =
                AddressAndLocationCollection.RetrieveFromOwnerContent(Owner, "MasterCollection");
            foreach (var blog in blogs)
            {
                TextContent textContent = getOrCreateInformationObjectsTextContent(process, blog);
                MediaContent blogMediaContent = getBlogMediaContent(blog);
                setTextContentImage(textContent, blogMediaContent);
                textContent.Title = blog.Title;
                textContent.Published = blog.Published;
                textContent.Author = blog.Author;
                textContent.SubTitle = blog.SubTitle;
                textContent.Excerpt = blog.Excerpt;
                textContent.Body = blog.Body;
                textContent.RawHtmlContent = blog.Body;
                textContent.Categories = getCategoriesByTitleFilteringToOne(targetCategoryCollection.CollectionContent,
                                                              blog.CategoryCollection.GetIDSelectedArray());
                textContent.Locations = getLocationsByWhitespaceTrimmedLocationName(targetAddressAndLocationCollection.CollectionContent,
                                                                             blog.LocationCollection.GetIDSelectedArray());
                textContent.StoreInformation();
            }
            foreach (var activity in activities)
            {
                TextContent textContent = getOrCreateInformationObjectsTextContent(process, activity);
                MediaContent activityMediaContent = getActivityMediaContent(activity);
                setTextContentImage(textContent, activityMediaContent);
                textContent.Title = activity.ActivityName;
                textContent.Published = activity.StartingTime;
                textContent.Author = activity.ContactPerson;
                textContent.Excerpt = activity.Excerpt;
                textContent.Body = activity.Description;
                textContent.RawHtmlContent = activity.Description;
                textContent.Categories = getCategoriesByTitleFilteringToOne(targetCategoryCollection.CollectionContent,
                                                              activity.CategoryCollection.GetIDSelectedArray());
                textContent.Locations = getLocationsByWhitespaceTrimmedLocationName(targetAddressAndLocationCollection.CollectionContent,
                                                                             activity.LocationCollection.GetIDSelectedArray());
                textContent.StoreInformation();
            }
        }

        private static void setTextContentImage(TextContent textContent, MediaContent legacyContent)
        {
            var owner = VirtualOwner.FigureOwner(textContent);
            var existingImageData = textContent.ImageData;
            if (existingImageData != null && legacyContent != null)
            {
                // If actual content is equal, return without doing anything
                if(existingImageData.GetMD5FromStorage() == legacyContent.GetMD5FromStorage())
                    return;
                var legacyData = legacyContent.GetContentData();
                if (legacyData == null)
                {
                    existingImageData.ClearCurrentContent(owner);
                    textContent.ImageData = null;
                    return;
                }
                string fileNameWithExtension = "data" + legacyContent.FileExt;
                existingImageData.SetMediaContent(fileNameWithExtension,
                    legacyContent.GetContentData());
            }
            if (existingImageData != null && legacyContent == null)
            {
                existingImageData.ClearCurrentContent(owner);
                textContent.ImageData = null;
                return;
            }
            if (existingImageData == null && legacyContent != null)
            {
                var legacyData = legacyContent.GetContentData();
                if (legacyData == null)
                    return;
                string fileNameWithExtension = "data" + legacyContent.FileExt;
                var newImageData = new MediaContent();
                newImageData.SetLocationAsOwnerContent(owner, newImageData.ID);
                newImageData.SetMediaContent(fileNameWithExtension,
                    legacyContent.GetContentData());
                textContent.ImageData = newImageData;
            }
        }

        private static MediaContent getActivityMediaContent(Activity activity)
        {
            return activity.ProfileImage.ImageData;
        }

        private static MediaContent getBlogMediaContent(Blog blog)
        {
            return blog.ProfileImage.ImageData;
        }

        private static AddressAndLocationCollection getLocationsByWhitespaceTrimmedLocationName(List<AddressAndLocation> currentLocations, AddressAndLocation[] sourceLocations)
        {
            AddressAndLocationCollection result = new AddressAndLocationCollection();
            var sourceLocationNames = sourceLocations.Select(loc => loc.Location.LocationName.Trim());
            var locationsToSet = sourceLocationNames.Select(name => currentLocations.FirstOrDefault(loc => loc.Location.LocationName.Trim() == name))
                                                    .Where(loc => loc != null);
            result.CollectionContent.AddRange(locationsToSet);
            return result;
        }

        private static CategoryCollection getCategoriesByTitleFilteringToOne(List<Category> currentCategories, Category[] sourceCategories)
        {
            CategoryCollection result = new CategoryCollection();
            var sourceTitles = sourceCategories.Select(cat => cat.CategoryName).ToArray();
            var categoriesToSet = sourceTitles.Select(title => currentCategories.FirstOrDefault(cat => cat.Title == title))
                                              .Where(cat => cat != null);
            result.CollectionContent.AddRange(categoriesToSet);
            if (result.CollectionContent.Count > 1)
            {
                bool removeProject = false;
                bool removeNews = false;
                if (result.CollectionContent.Any(cat => cat.Title == "Events"))
                {
                    removeProject = true;
                    removeNews = true;
                }
                if (result.CollectionContent.Any(cat => cat.Title == "Projects"))
                {
                    removeNews = true;
                }
                result.CollectionContent.RemoveAll(cat =>
                    {
                        if ((cat.Title == "News" && removeNews) || (cat.Title == "Projects" && removeProject))
                            return true;
                        return false;
                    });
            }
            return result;
        }

        private static TextContent getOrCreateInformationObjectsTextContent(Process process, IInformationObject sourceObject)
        {
            var processItems = process.ProcessItems;
            var matchingProcessItem = processItems.FirstOrDefault(processItem => processItem.Inputs.Any(sourceObject.IsObjectsSemanticItem));
            TextContent result = null;
            if (matchingProcessItem != null)
            {
                var matchingOutput = matchingProcessItem.Outputs.FirstOrDefault(semanticItem => semanticItem.ItemFullType == typeof (TextContent).FullName);
                if (matchingOutput != null)
                {
                    var textContentLocation = matchingOutput.ItemValue;
                    result = TextContent.RetrieveTextContent(textContentLocation, Owner);
                }
                if (result == null)
                {
                    processItems.Remove(matchingProcessItem);
                    matchingProcessItem = null;
                }
            } 
            if(matchingProcessItem == null)
            {
                matchingProcessItem = new ProcessItem();
                matchingProcessItem.Inputs.Add(new SemanticInformationItem(sourceObject));
                TextContent textContent = new TextContent();
                textContent.SetLocationAsOwnerContent(Owner, textContent.ID);
                textContent.GeneratedByProcessID = process.ID;
                matchingProcessItem.Outputs.Add(new SemanticInformationItem(textContent));
                processItems.Add(matchingProcessItem);
                result = textContent;
            }
            return result;
        }
    }
}