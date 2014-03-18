namespace TheBall.Interface
{
    public partial class CategoryInfo
    {
        public Category ToCategory()
        {
            return new Category
            {
                ID = CategoryID,
                NativeCategoryID = NativeCategoryID,
                NativeCategoryDomainName = NativeCategoryDomainName,
                NativeCategoryObjectName = NativeCategoryObjectName,
                NativeCategoryTitle = NativeCategoryTitle,
                IdentifyingCategoryName = IdentifyingCategoryName,
                ParentCategoryID = ParentCategoryID
            };
            
        }

        public static CategoryInfo FromCategory(Category category)
        {
            return new CategoryInfo
            {
                CategoryID = category.ID,
                NativeCategoryID = category.NativeCategoryID,
                NativeCategoryDomainName = category.NativeCategoryDomainName,
                NativeCategoryObjectName = category.NativeCategoryObjectName,
                NativeCategoryTitle = category.NativeCategoryTitle,
                IdentifyingCategoryName = category.IdentifyingCategoryName,
                ParentCategoryID = category.ParentCategoryID
            };
        }
    }
}