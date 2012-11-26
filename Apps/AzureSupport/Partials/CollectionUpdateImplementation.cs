using System.Collections.Generic;
using System.Linq;

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

        internal static void Update_MapContainer_MarkerSourceLocations(MapContainer mapContainer, AddressAndLocationCollection localCollection, AddressAndLocationCollection masterCollection)
        {
            mapContainer.MapMarkers.CollectionContent.RemoveAll(
                marker => marker.MarkerSource == MapMarker.MarkerSourceLocationValue);
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
            var mapMarkersFromLocation = masterCollection.CollectionContent.SelectMany(blog => blog.LocationCollection.CollectionContent).
                Select(loc =>
                {
                    MapMarker marker = MapMarker.CreateDefault();
                    marker.Location = loc.Location;
                    marker.MarkerSource = MapMarker.MarkerSourceBlogValue;
                    marker.IconUrl = "../oip-additions/oip-assets/oip-images/oip-markers/OIP-marker-2.png";
                    marker.SetLocationTextFromLocation(loc.Location);
                    return marker;

                }).ToArray();
            mapContainer.MapMarkers.CollectionContent.AddRange(mapMarkersFromLocation);
        }

        internal static void Update_MapContainer_MarkerSourceActivities(MapContainer mapContainer, ActivityCollection localCollection, ActivityCollection masterCollection)
        {
            mapContainer.MapMarkers.CollectionContent.RemoveAll(
                marker => marker.MarkerSource == MapMarker.MarkerSourceActivityValue);
            var mapMarkersFromLocation = masterCollection.CollectionContent.SelectMany(activity => activity.LocationCollection.CollectionContent).
                Select(loc =>
                {
                    MapMarker marker = MapMarker.CreateDefault();
                    marker.Location = loc.Location;
                    marker.MarkerSource = MapMarker.MarkerSourceActivityValue;
                    marker.IconUrl = "../oip-additions/oip-assets/oip-images/oip-markers/OIP-marker-meeting.png";
                    marker.SetLocationTextFromLocation(loc.Location);
                    return marker;

                }).ToArray();
            mapContainer.MapMarkers.CollectionContent.AddRange(mapMarkersFromLocation);
        }
    }
}