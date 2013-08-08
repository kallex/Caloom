using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheBall.CORE
{
    public static class CollectionUpdateImplementation
    {
        public static void Update_InvoiceSummaryContainer_OpenInvoices(InvoiceSummaryContainer invoiceSummaryContainer, InvoiceCollection localCollection, InvoiceCollection masterCollection)
        {
            throw new NotImplementedException();
        }

        public static void Update_InvoiceSummaryContainer_PredictedInvoices(InvoiceSummaryContainer invoiceSummaryContainer, InvoiceCollection localCollection, InvoiceCollection masterCollection)
        {
            throw new NotImplementedException();
        }

        public static void Update_InvoiceSummaryContainer_PaidInvoicesActiveYear(InvoiceSummaryContainer invoiceSummaryContainer, InvoiceCollection localCollection, InvoiceCollection masterCollection)
        {
            throw new NotImplementedException();
        }

        public static void Update_InvoiceSummaryContainer_PaidInvoicesLast12Months(InvoiceSummaryContainer invoiceSummaryContainer, InvoiceCollection localCollection, InvoiceCollection masterCollection)
        {
            throw new NotImplementedException();
        }

        public static void Update_InvoiceFiscalExportSummary_ExportedInvoices(InvoiceFiscalExportSummary invoiceFiscalExportSummary, InvoiceCollection localCollection, InvoiceCollection masterCollection)
        {
            throw new NotImplementedException();
        }
    }
}

namespace AaltoGlobalImpact.OIP
{
    public static class CollectionUpdateImplementation
    {

        internal static void Update_AccountModule_LocationCollection(AccountModule accountModule, AddressAndLocationCollection localCollection, AddressAndLocationCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }

        internal static void Update_RecentBlogSummary_RecentBlogCollection(RecentBlogSummary recentBlogSummary, BlogCollection localCollection, BlogCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent.Where(blog => blog.Published >= DateTime.UtcNow.AddMonths(-3)).OrderByDescending(blog => blog.Published).Take(10).ToList();
            //localCollection.CollectionContent = masterCollection.CollectionContent;
            if(localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
            localCollection.IsCollectionFiltered = false;
        }

        internal static void Update_LocationContainer_Locations(LocationContainer locationContainer, AddressAndLocationCollection localCollection, AddressAndLocationCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }

        internal static void Update_GroupContainer_Activities(GroupContainer groupContainer, ActivityCollection localCollection, ActivityCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }

        internal static void Update_BlogIndexGroup_BlogSourceForSummary(BlogIndexGroup blogIndexGroup, BlogCollection localCollection, BlogCollection masterCollection)
        {
            if(localCollection == null)
                localCollection = BlogCollection.CreateDefault();
            localCollection.CollectionContent.Clear();
            var limitDateTime = DateTime.UtcNow.AddMonths(-3);
            var relevantBlogs =
                masterCollection.CollectionContent.Where(blog => blog.Published >= limitDateTime).OrderByDescending(blog => blog.Published).Take(10).ToArray();
            var blogsByAuthor = relevantBlogs.GroupBy(blog => blog.Author).OrderBy(grpItem => grpItem.Key).ToArray();
            if(blogIndexGroup.GroupedByAuthor == null)
                blogIndexGroup.GroupedByAuthor = GroupedInformationCollection.CreateDefault();
            blogIndexGroup.GroupedByAuthor.CollectionContent.Clear();
            blogIndexGroup.GroupedByAuthor.CollectionContent.AddRange(blogsByAuthor.Select(blogGroupToGroupedInformation));

            if (blogIndexGroup.GroupedByCategory == null)
                blogIndexGroup.GroupedByCategory = GroupedInformationCollection.CreateDefault();
            if (blogIndexGroup.GroupedByDate == null)
                blogIndexGroup.GroupedByDate = GroupedInformationCollection.CreateDefault();
            if (blogIndexGroup.GroupedByLocation == null)
                blogIndexGroup.GroupedByLocation = GroupedInformationCollection.CreateDefault();

            var archiveSeq = masterCollection.CollectionContent.OrderByDescending(blog => blog.Published)
                    .ThenBy(blog => blog.Title)
                    .Select(blog => blog.ReferenceToInformation);
            if(blogIndexGroup.FullBlogArchive == null)
                blogIndexGroup.FullBlogArchive = ReferenceCollection.CreateDefault();
            blogIndexGroup.FullBlogArchive.CollectionContent.Clear();
            blogIndexGroup.FullBlogArchive.CollectionContent.AddRange(archiveSeq);

        }

        private static GroupedInformation blogGroupToGroupedInformation(IGrouping<string, Blog> grp)
        {
            var result = GroupedInformation.CreateDefault();
            result.GroupName = grp.Key;
            result.ReferenceCollection.CollectionContent.AddRange(grp.Select(blog => blog.ReferenceToInformation));
            return result;
        }

        /*
        internal static void Update_BlogIndexGroup_BlogByLocation(BlogIndexGroup blogIndexGroup, BlogCollection localCollection, BlogCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }

        internal static void Update_BlogIndexGroup_BlogByAuthor(BlogIndexGroup blogIndexGroup, BlogCollection localCollection, BlogCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }

        internal static void Update_BlogIndexGroup_BlogByCategory(BlogIndexGroup blogIndexGroup, BlogCollection localCollection, BlogCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }


        internal static void Update_BlogIndexGroup_BlogByDate(BlogIndexGroup blogIndexGroup, BlogCollection localCollection, BlogCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }
         */

        internal static void Update_ActivitySummaryContainer_ActivityCollection(ActivitySummaryContainer activitySummaryContainer, ActivityCollection localCollection, ActivityCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }

        internal static void Update_ImageGroupContainer_ImageGroups(ImageGroupContainer imageGroupContainer, ImageGroupCollection localCollection, ImageGroupCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }

        internal static void Update_Group_ImageSets(Group group, ImageGroupCollection localCollection, ImageGroupCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }

        internal static void Update_Activity_ImageSets(Activity activity, ImageGroupCollection localCollection, ImageGroupCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }

        internal static void Update_GroupContainer_LocationCollection(GroupContainer groupContainer, AddressAndLocationCollection localCollection, AddressAndLocationCollection masterCollection)
        {
            if (localCollection == null)
            {
                groupContainer.LocationCollection = AddressAndLocationCollection.CreateDefault();
                localCollection = groupContainer.LocationCollection;
            }
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }


        internal static void Update_Blog_LocationCollection(Blog blog, AddressAndLocationCollection localCollection, AddressAndLocationCollection masterCollection)
        {
            if (localCollection == null)
            {
                blog.LocationCollection = AddressAndLocationCollection.CreateDefault();
                localCollection = blog.LocationCollection;
            }
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }

        internal static void Update_Activity_LocationCollection(Activity activity, AddressAndLocationCollection localCollection, AddressAndLocationCollection masterCollection)
        {
            if (localCollection == null)
            {
                activity.LocationCollection = AddressAndLocationCollection.CreateDefault();
                localCollection = activity.LocationCollection;
            }
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }

        internal static void Update_GroupContainer_ImageGroupCollection(GroupContainer groupContainer, ImageGroupCollection localCollection, ImageGroupCollection masterCollection)
        {
            if(localCollection == null)
            {
                groupContainer.ImageGroupCollection = ImageGroupCollection.CreateDefault();
                localCollection = groupContainer.ImageGroupCollection;
            }
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if(localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }

        internal static void Update_Blog_ImageGroupCollection(Blog blog, ImageGroupCollection localCollection, ImageGroupCollection masterCollection)
        {
            if (localCollection == null)
            {
                blog.ImageGroupCollection = ImageGroupCollection.CreateDefault();
                localCollection = blog.ImageGroupCollection;
            }
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }

        internal static void Update_Activity_ImageGroupCollection(Activity activity, ImageGroupCollection localCollection, ImageGroupCollection masterCollection)
        {
            if (localCollection == null)
            {
                activity.ImageGroupCollection = ImageGroupCollection.CreateDefault();
                localCollection = activity.ImageGroupCollection;
            }
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }


        internal static void Update_MapContainer_MarkerSourceLocations(MapContainer mapContainer, AddressAndLocationCollection localCollection, AddressAndLocationCollection masterCollection)
        {
            mapContainer.MapMarkers.CollectionContent.RemoveAll(
                marker => marker.MarkerSource == MapMarker.MarkerSourceLocationValue);
            return;
            var mapMarkersFromLocation = masterCollection.CollectionContent.
                Select(loc =>
                           {
                               MapMarker marker = MapMarker.CreateDefault();
                               marker.Location = loc.Location;
                               marker.MarkerSource = MapMarker.MarkerSourceLocationValue;
                               marker.IconUrl = "../oip-additions/oip-assets/oip-images/oip-markers/OIP-marker-1.png";
                               marker.SetLocationTextFromLocation(loc.Location);
                               return marker;

                           }).ToArray();
            mapContainer.MapMarkers.CollectionContent.AddRange(mapMarkersFromLocation);
        }

        internal static void Update_MapContainer_MarkerSourceBlogs(MapContainer mapContainer, BlogCollection localCollection, BlogCollection masterCollection)
        {
            mapContainer.MapMarkers.CollectionContent.RemoveAll(
                marker => marker.MarkerSource == MapMarker.MarkerSourceBlogValue);
            var locationBlogs =
                masterCollection.CollectionContent.Select(
                    blog => new { Blog = blog, Locations = blog.LocationCollection.GetIDSelectedArray() });
            Dictionary<string, LocationSpot> locDict = new Dictionary<string, LocationSpot>();
            foreach (var locBlog in locationBlogs)
            {
                foreach (var location in locBlog.Locations)
                {
                    string key = location.Location.GetLocationText();
                    LocationSpot locSpot;
                    locDict.TryGetValue(key, out locSpot);
                    if (locSpot == null)
                    {
                        locSpot = new LocationSpot { LocationText = key, Location = location.Location };
                        locDict.Add(key, locSpot);
                    }
                    locSpot.AddBlog(locBlog.Blog);
                }
            }
            List<MapMarker> markers = new List<MapMarker>();
            foreach (var dictItem in locDict)
            {
                var locSpot = dictItem.Value;
                MapMarker marker = MapMarker.CreateDefault();
                marker.Location = locSpot.Location;
                marker.MarkerSource = MapMarker.MarkerSourceBlogValue;
                marker.IconUrl = GetIconUrlForCategory("News");
                marker.CategoryName = GetMarkerCategoryName("News");
                marker.LocationText = locSpot.LocationText;
                marker.SetLocationTextFromLocation(locSpot.Location);
                marker.PopupTitle = "News";
                StringBuilder strBuilder = new StringBuilder();
                foreach (var blogItem in locSpot.Blogs)
                {
                    ReferenceToInformation referenceToInformation = blogItem.ReferenceToInformation;
                    appendMarkerLink(strBuilder, referenceToInformation);
                }
                marker.PopupContent = strBuilder.ToString();
                markers.Add(marker);
            }

            mapContainer.MapMarkers.CollectionContent.AddRange(markers);
        }

        private static void appendMarkerLink(StringBuilder strBuilder, ReferenceToInformation referenceToInformation)
        {
            if (referenceToInformation == null || referenceToInformation.URL == null || referenceToInformation.Title == null)
                return;
            strBuilder.AppendFormat("<a class=\"oipmapmarkerlink\" href=\"javascript:void(0)\" onclick=\"OipOpenArticle(\\'{0}\\');\" >{1}</a><br>",
                                    referenceToInformation.URL, referenceToInformation.Title.Replace("'", ""));
            //strBuilder.AppendFormat("<a class=\"oipmapmarkerlink\" href=\"javascript:void(0)\">{1}</a><br>",
            //                        referenceToInformation.URL, referenceToInformation.Title.Replace("'", ""));
        }

        class LocationSpot
        {
            public static string[] CategoryNames = new string[] {"Projects", "Events", "Blogs"};
            public string LocationText;
            public Location Location;
            public List<Blog> Blogs = new List<Blog>();
            public List<CategoryActivity> CategorizedActivites = new List<CategoryActivity>();
            public CategoryActivity GetOrInitiateCategoryWithName(string categoryName)
            {
                if (CategoryNames.Contains(categoryName) == false)
                    categoryName = null;
                var result = CategorizedActivites.FirstOrDefault(catAct => catAct.CategoryName == categoryName);
                if (result == null)
                {
                    result = new CategoryActivity { CategoryName = categoryName, };
                    CategorizedActivites.Add(result);
                }
                return result;
            }
            
            public void AddBlog(Blog blog)
            {
                Blogs.Add(blog);
            }

            public void AddActivity(Activity activity)
            {
                var categories = activity.CategoryCollection.GetIDSelectedArray();
                if(categories.Length == 0)
                {
                    var defaultCategoryItem = GetOrInitiateCategoryWithName(null);
                    defaultCategoryItem.Activities.Add(activity);
                }
                foreach(var category in categories)
                {
                    var categoryItem = GetOrInitiateCategoryWithName(category.CategoryName);
                    categoryItem.Activities.Add(activity);
                }

            }
        }

        class CategoryActivity
        {
            public string CategoryName;
            public List<Activity> Activities = new List<Activity>();

            public string GetCategoryTitle()
            {
                if (String.IsNullOrEmpty(CategoryName) || LocationSpot.CategoryNames.Contains(CategoryName) == false)
                    return "Activities";
                return CategoryName;
            }
        }

        internal static void Update_MapContainer_MarkerSourceActivities(MapContainer mapContainer, ActivityCollection localCollection, ActivityCollection masterCollection)
        {
            mapContainer.MapMarkers.CollectionContent.RemoveAll(
                marker => marker.MarkerSource == MapMarker.MarkerSourceActivityValue);
            var locationActivities =
                masterCollection.CollectionContent.Select(
                    activity => new {Activity = activity, Locations = activity.LocationCollection.GetIDSelectedArray()});
            Dictionary<string, LocationSpot> locDict = new Dictionary<string, LocationSpot>();
            foreach(var locAct in locationActivities)
            {
                foreach(var location in locAct.Locations)
                {
                    string key = location.Location.GetLocationText();
                    LocationSpot locSpot;
                    locDict.TryGetValue(key, out locSpot);
                    if(locSpot == null)
                    {
                        locSpot = new LocationSpot {LocationText = key, Location = location.Location};
                        locDict.Add(key, locSpot);
                    }
                    locSpot.AddActivity(locAct.Activity);
                }
            }
            List<MapMarker> markers = new List<MapMarker>();
            foreach(var dictItem in locDict)
            {
                var locSpot = dictItem.Value;
                foreach(var catItem in locSpot.CategorizedActivites)
                {
                    MapMarker marker = MapMarker.CreateDefault();
                    marker.Location = locSpot.Location;
                    marker.MarkerSource = MapMarker.MarkerSourceActivityValue;
                    marker.IconUrl = GetIconUrlForCategory(catItem.CategoryName);
                    marker.CategoryName = GetMarkerCategoryName(catItem.CategoryName);
                    //marker.IconUrl = "../oip-additions/oip-assets/oip-images/oip-markers/OIP-marker-meeting.png";
                    marker.LocationText = locSpot.LocationText;
                    marker.SetLocationTextFromLocation(locSpot.Location);
                    marker.PopupTitle = catItem.GetCategoryTitle();
                    StringBuilder strBuilder = new StringBuilder();
                    foreach(var act in catItem.Activities)
                    {
                        ReferenceToInformation referenceToInformation = act.ReferenceToInformation;
                        appendMarkerLink(strBuilder, referenceToInformation);
                    }
                    marker.PopupContent = strBuilder.ToString();
                    markers.Add(marker);
                }
            }

            mapContainer.MapMarkers.CollectionContent.AddRange(markers);
        }

        private static string GetMarkerCategoryName(string categoryName)
        {
            if (categoryName != "News" && LocationSpot.CategoryNames.Contains(categoryName) == false)
                return "Activities";
            return categoryName;
        }

        private static string GetIconUrlForCategory(string categoryName)
        {
            if (categoryName != "News" && LocationSpot.CategoryNames.Contains(categoryName) == false)
                categoryName = "Activities";
            return string.Format("../oip-additions/oip-assets/oip-images/oip-markers/map-marker-{0}.png", categoryName);
        }


        internal static void Update_Group_CategoryCollection(Group group, CategoryCollection localCollection, CategoryCollection masterCollection)
        {
            if (localCollection == null)
            {
                group.CategoryCollection = CategoryCollection.CreateDefault();
                localCollection = group.CategoryCollection;
            }
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }

        internal static void Update_CategoryContainer_Categories(CategoryContainer categoryContainer, CategoryCollection localCollection, CategoryCollection masterCollection)
        {
            if (localCollection == null)
            {
                categoryContainer.Categories = CategoryCollection.CreateDefault();
                localCollection = categoryContainer.Categories;
            }
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }

        internal static void Update_Blog_CategoryCollection(Blog blog, CategoryCollection localCollection, CategoryCollection masterCollection)
        {
            if (localCollection == null)
            {
                blog.CategoryCollection = CategoryCollection.CreateDefault();
                localCollection = blog.CategoryCollection;
            }
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }

        internal static void Update_Activity_CategoryCollection(Activity activity, CategoryCollection localCollection, CategoryCollection masterCollection)
        {
            if (localCollection == null)
            {
                activity.CategoryCollection = CategoryCollection.CreateDefault();
                localCollection = activity.CategoryCollection;
            }
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }


        const string NodeSourceTypeBlog = "BLOG";
        const string NodeSourceTypeActivity = "ACTIVITY";
        const string NodeSourceTypeTextContent = "TEXTCONTENT";
        internal static void Update_NodeSummaryContainer_NodeSourceBlogs(NodeSummaryContainer nodeSummaryContainer, BlogCollection localCollection, BlogCollection masterCollection)
        {
            var nodes = nodeSummaryContainer.Nodes;
            nodes.CollectionContent.RemoveAll(node => node.TechnicalSource == NodeSourceTypeBlog);
            var blogNodes = masterCollection.CollectionContent.Select(getNodeFromBlog).ToArray();
            nodes.CollectionContent.AddRange(blogNodes);
            cleanUpRenderedNodes(nodes);
        }

        private static void cleanUpRenderedNodes(RenderedNodeCollection nodes)
        {
            nodes.CollectionContent.RemoveAll(node => node.ActualContentUrl == null);
            nodes.CollectionContent.RemoveAll(node => node.Categories.CollectionContent == null ||
                                                      node.Categories.CollectionContent.Count == 0);
            // Note the node1 and node2 are opposite parameters, because we want descending sort
            nodes.CollectionContent.Sort((node1, node2) => String.CompareOrdinal(node2.MainSortableText, node1.MainSortableText));
        }

        internal static void Update_NodeSummaryContainer_NodeSourceActivities(NodeSummaryContainer nodeSummaryContainer, ActivityCollection localCollection, ActivityCollection masterCollection)
        {
            const string DefaultAuthorValue = "Aalto Global Impact";
            var nodes = nodeSummaryContainer.Nodes;
            nodes.CollectionContent.RemoveAll(node => node.TechnicalSource == NodeSourceTypeActivity);
            var activityNodes = masterCollection.CollectionContent.Select(getNodeFromActivity).ToArray();
            nodes.CollectionContent.AddRange(activityNodes);
            nodes.CollectionContent.ForEach(node =>
                {
                    if (node.Authors.CollectionContent.Count == 0)
                        node.Authors.CollectionContent.Add(getShortTextObject(DefaultAuthorValue));
                    else
                    {
                    var firstAuthorObject = node.Authors.CollectionContent[0];
                        if (String.IsNullOrWhiteSpace(firstAuthorObject.Content))
                        {
                            firstAuthorObject.Content = DefaultAuthorValue;
                        }
                    }
                });
            cleanUpRenderedNodes(nodes);
        }

        internal static RenderedNode getNodeFromBlog(Blog blog)
        {
            RenderedNode node = RenderedNode.CreateDefault();
            node.TechnicalSource = NodeSourceTypeBlog;
            node.Title = blog.Title;
            node.Excerpt = blog.Excerpt;
            node.ImageBaseUrl = blog.ProfileImage.ImageData.ContentUrlBase;
            node.ActualContentUrl = blog.ReferenceToInformation.URL;
            node.TimestampText = getTimeStampText(blog.Published);
            node.Categories.CollectionContent.AddRange(getCategoryCollectionTexts(blog.CategoryCollection));
            node.Locations.CollectionContent.AddRange(getLocationCollectionTexts(blog.LocationCollection));
            node.Authors.CollectionContent.Add(getShortTextObject(blog.Author));
            node.MainSortableText = blog.Published.ToString("s");
            return node;
        }

        internal static ShortTextObject getShortTextObject(string source)
        {
            ShortTextObject shortText = ShortTextObject.CreateDefault();
            shortText.Content = source;
            return shortText;
        }

        internal static IEnumerable<ShortTextObject> getCategoryCollectionTexts(CategoryCollection categoryCollection)
        {
            return categoryCollection.GetIDSelectedArray()
                                     .Select(category =>
                                         {
                                             var textShort = ShortTextObject.CreateDefault();
                                             textShort.Content = category.CategoryName;
                                             return textShort;
                                         })
                                     .OrderBy(text => text.Content);
        }

        internal static IEnumerable<ShortTextObject> getLocationCollectionTexts(AddressAndLocationCollection locationCollection)
        {
            return locationCollection.GetIDSelectedArray()
                                     .Select(location =>
                                         {
                                             var textShort = ShortTextObject.CreateDefault();
                                             textShort.Content = location.Location.LocationName;
                                             return textShort;
                                         })
                                     .OrderBy(text => text.Content);
        }

        internal static RenderedNode getNodeFromActivity(Activity activity)
        {
            RenderedNode node = RenderedNode.CreateDefault();
            node.TechnicalSource = NodeSourceTypeActivity;
            node.Title = activity.ActivityName;
            node.Excerpt = activity.Excerpt;
            node.ImageBaseUrl = activity.ProfileImage.ImageData.ContentUrlBase;
            node.ActualContentUrl = activity.ReferenceToInformation.URL;
            node.TimestampText = getTimeStampText(activity.StartingTime);
            node.Categories.CollectionContent.AddRange(getCategoryCollectionTexts(activity.CategoryCollection));
            node.Locations.CollectionContent.AddRange(getLocationCollectionTexts(activity.LocationCollection));
            node.Authors.CollectionContent.Add(getShortTextObject(activity.ContactPerson));
            node.MainSortableText = activity.StartingTime.ToString("s");
            return node;
        }

        private static string getTimeStampText(DateTime dateTime)
        {
            if (dateTime.TimeOfDay == TimeSpan.Zero)
                return dateTime.ToString("D");
            return dateTime.ToString("f");
        }
        /*
        internal static void Update_Category_ParentCategories(Category category, CategoryCollection localCollection, CategoryCollection masterCollection)
        {
            if (localCollection == null)
            {
                category.ParentCategories = CategoryCollection.CreateDefault();
                localCollection = category.ParentCategories;
            }
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }*/

        internal static void Update_TextContent_Locations(TextContent textContent, AddressAndLocationCollection localCollection, AddressAndLocationCollection masterCollection)
        {
            // TODO: Remove objects, that are no longer available in master
        }

        internal static void Update_TextContent_Categories(TextContent textContent, CategoryCollection localCollection, CategoryCollection masterCollection)
        {
            // TODO: Remove objects, that are no longer available in master
        }

        internal static void Update_NodeSummaryContainer_NodeSourceTextContent(NodeSummaryContainer nodeSummaryContainer, TextContentCollection localCollection, TextContentCollection masterCollection)
        {
            var nodes = nodeSummaryContainer.Nodes;
            nodes.CollectionContent.RemoveAll(node => node.TechnicalSource == NodeSourceTypeTextContent);
            var textContentNodes = masterCollection.CollectionContent.Select(getNodeFromTextContent).ToArray();
            nodes.CollectionContent.AddRange(textContentNodes);
            /*
            nodes.CollectionContent.ForEach(node =>
            {
                if (node.Authors.CollectionContent.Count == 0)
                    node.Authors.CollectionContent.Add(getShortTextObject(DefaultAuthorValue));
                else
                {
                    var firstAuthorObject = node.Authors.CollectionContent[0];
                    if (String.IsNullOrWhiteSpace(firstAuthorObject.Content))
                    {
                        firstAuthorObject.Content = DefaultAuthorValue;
                    }
                }
            });*/
            cleanUpRenderedNodes(nodes);
        }

        internal static RenderedNode getNodeFromTextContent(TextContent textContent)
        {
            RenderedNode node = RenderedNode.CreateDefault();
            node.TechnicalSource = NodeSourceTypeTextContent;
            node.Title = textContent.Title;
            node.Excerpt = textContent.Excerpt;
            if(textContent.ImageData != null)
                node.ImageBaseUrl = textContent.ImageData.ContentUrlBase;
            node.ActualContentUrl = "../" + textContent.SemanticDomainName + "/" + textContent.Name + "/" + textContent.ID;
            if(textContent.Categories != null)
                node.Categories.CollectionContent.AddRange(getCategoryCollectionTexts(textContent.Categories));
            if(textContent.Locations != null)
                node.Locations.CollectionContent.AddRange(getLocationCollectionTexts(textContent.Locations));
            node.Authors.CollectionContent.Add(getShortTextObject(textContent.Author));
            node.MainSortableText = textContent.SortOrderNumber.ToString();
            return node;
        }

    }
}