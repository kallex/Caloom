using System.Linq;

namespace AaltoGlobalImpact.OIP
{
    public static class CollectionUpdateImplementation
    {

        internal static void Update_AccountModule_LocationCollection(AccountModule accountModule, AddressAndLocationCollection localCollection, AddressAndLocationCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
        }

        internal static void Update_RecentBlogSummary_RecentBlogCollection(RecentBlogSummary recentBlogSummary, BlogCollection localCollection, BlogCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent.OrderByDescending(blog => blog.Published).Take(3).ToList();
        }

        internal static void Update_LocationContainer_Locations(LocationContainer locationContainer, AddressAndLocationCollection localCollection, AddressAndLocationCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
        }

        internal static void Update_GroupContainer_Activities(GroupContainer groupContainer, ActivityCollection localCollection, ActivityCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
        }

        internal static void Update_BlogIndexGroup_BlogByLocation(BlogIndexGroup blogIndexGroup, BlogCollection localCollection, BlogCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
        }

        internal static void Update_BlogIndexGroup_BlogByAuthor(BlogIndexGroup blogIndexGroup, BlogCollection localCollection, BlogCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
        }

        internal static void Update_BlogIndexGroup_BlogByCategory(BlogIndexGroup blogIndexGroup, BlogCollection localCollection, BlogCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
        }

        internal static void Update_ActivitySummaryContainer_ActivityCollection(ActivitySummaryContainer activitySummaryContainer, ActivityCollection localCollection, ActivityCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
        }

        internal static void Update_BlogIndexGroup_BlogByDate(BlogIndexGroup blogIndexGroup, BlogCollection localCollection, BlogCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
        }

        internal static void Update_ImageGroupContainer_ImageGroups(ImageGroupContainer imageGroupContainer, ImageGroupCollection localCollection, ImageGroupCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
        }

        internal static void Update_Group_ImageSets(Group group, ImageGroupCollection localCollection, ImageGroupCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
        }

        internal static void Update_Activity_ImageSets(Activity activity, ImageGroupCollection localCollection, ImageGroupCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
        }
    }
}