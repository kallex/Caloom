namespace AaltoGlobalImpact.OIP
{
    partial class BlogContainer
    {
        public void AddNewBlogPost(Blog blog)
        {
            RecentBlogSummary.RecentBlogCollection.CollectionContent.Add(blog);
            BlogIndexGroup.BlogByLocation.CollectionContent.Add(blog);
            BlogIndexGroup.BlogByAuthor.CollectionContent.Add(blog);
            BlogIndexGroup.BlogByDate.CollectionContent.Add(blog);
            BlogIndexGroup.BlogByCategory.CollectionContent.Add(blog);
        }
    }
}