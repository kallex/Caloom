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
            localCollection.CollectionContent = masterCollection.CollectionContent.OrderByDescending(blog => blog.Published).Take(3).ToList();
            if(localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
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

        internal static void Update_ActivitySummaryContainer_ActivityCollection(ActivitySummaryContainer activitySummaryContainer, ActivityCollection localCollection, ActivityCollection masterCollection)
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
            var mapMarkersFromLocation = masterCollection.CollectionContent.SelectMany(blog => blog.LocationCollection.GetIDSelectedArray()).
                Select(loc =>
                {
                    MapMarker marker = MapMarker.CreateDefault();
                    marker.Location = loc.Location;
                    marker.MarkerSource = MapMarker.MarkerSourceBlogValue;
                    marker.IconUrl = "../oip-additions/oip-assets/oip-images/oip-markers/OIP-marker-2.png";
                    marker.SetLocationTextFromLocation(loc.Location);
                    return marker;
                }).ToArray();
            //var groupedMarkers = 
            mapContainer.MapMarkers.CollectionContent.AddRange(mapMarkersFromLocation);
        }

        class LocationSpot
        {
            public string LocationText;
            public Location Location;
            public List<CategoryActivity> CategorizedActivites = new List<CategoryActivity>();
            public CategoryActivity GetOrInitiateCategoryWithName(string categoryName)
            {
                var result = CategorizedActivites.FirstOrDefault(catAct => catAct.CategoryName == categoryName);
                if (result == null)
                {
                    result = new CategoryActivity { CategoryName = categoryName, };
                    CategorizedActivites.Add(result);
                }
                return result;
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
        }

        internal static void Update_MapContainer_MarkerSourceActivities(MapContainer mapContainer, ActivityCollection localCollection, ActivityCollection masterCollection)
        {
            mapContainer.MapMarkers.CollectionContent.RemoveAll(
                marker => marker.MarkerSource == MapMarker.MarkerSourceActivityValue);
            var mapMarkersFromLocation = masterCollection.CollectionContent.SelectMany(activity => activity.LocationCollection.GetIDSelectedArray()).
                Select(loc =>
                {
                    MapMarker marker = MapMarker.CreateDefault();
                    marker.Location = loc.Location;
                    marker.MarkerSource = MapMarker.MarkerSourceActivityValue;
                    marker.IconUrl = "../oip-additions/oip-assets/oip-images/oip-markers/OIP-marker-meeting.png";
                    marker.SetLocationTextFromLocation(loc.Location);
                    return marker;

                }).ToArray();
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
                    marker.IconUrl = "../oip-additions/oip-assets/oip-images/oip-markers/OIP-marker-meeting.png";
                    marker.LocationText = locSpot.LocationText;
                    marker.SetLocationTextFromLocation(locSpot.Location);
                    marker.PopupTitle = "Activities";
                    StringBuilder strBuilder = new StringBuilder();
                    foreach(var act in catItem.Activities)
                    {
                        strBuilder.AppendFormat("<a href=\"{0}\">{1}</a><br>", 
                            act.ReferenceToInformation.URL, act.ReferenceToInformation.Title.Replace("'", ""));
                    }
                    marker.PopupContent = strBuilder.ToString();
                    markers.Add(marker);
                }
            }

            mapContainer.MapMarkers.CollectionContent.AddRange(markers);
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
    }
}