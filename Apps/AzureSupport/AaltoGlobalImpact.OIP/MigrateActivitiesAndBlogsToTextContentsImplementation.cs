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
            foreach (var blog in blogs)
            {
                TextContent textContent = getOrCreateInformationObjectsTextContent(process, blog);
                textContent.Title = blog.Title;
                textContent.Published = blog.Published;
                textContent.Author = blog.Author;
                textContent.SubTitle = blog.SubTitle;
                textContent.Excerpt = blog.Excerpt;
                textContent.Body = blog.Body;
                textContent.RawHtmlContent = blog.Body;
                textContent.Categories = getCategoriesByTitleFilteringToOne(targetCategoryCollection.CollectionContent,
                                                              blog.CategoryCollection.GetIDSelectedArray());
                textContent.StoreInformation();
            }
            foreach (var activity in activities)
            {
                TextContent textContent = getOrCreateInformationObjectsTextContent(process, activity);
                textContent.SetLocationAsOwnerContent(Owner, textContent.ID);
                textContent.Title = activity.ActivityName;
                textContent.Published = activity.StartingTime;
                textContent.Author = activity.ContactPerson;
                textContent.Excerpt = activity.Excerpt;
                textContent.Body = activity.Description;
                textContent.RawHtmlContent = activity.Description;
                textContent.Categories = getCategoriesByTitleFilteringToOne(targetCategoryCollection.CollectionContent,
                                                              activity.CategoryCollection.GetIDSelectedArray());
                textContent.StoreInformation();
            }
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