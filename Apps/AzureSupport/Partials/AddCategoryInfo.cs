using System;
using System.IO;
using System.Web;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class AddCategoryInfo : IAddOperationProvider
    {
        public bool PerformAddOperation(string commandName, InformationSourceCollection sources, string requesterLocation, HttpFileCollection files)
        {
            if(CategoryName == "")
                throw new InvalidDataException("Category name is mandatory");
            Category category = Category.CreateDefault();
            VirtualOwner owner = VirtualOwner.FigureOwner(this);
            category.SetLocationAsOwnerContent(owner, category.ID);
            category.CategoryName = CategoryName;
            StorageSupport.StoreInformationMasterFirst(category, owner, true);
            DefaultViewSupport.CreateDefaultViewRelativeToRequester(requesterLocation, category, owner);
            //BlogContainer blogContainer = BlogContainer.RetrieveFromOwnerContent(owner, "default");
            //blogContainer.AddNewBlogPost(blog);
            //StorageSupport.StoreInformation(blogContainer);
            CategoryName = "";
            return true;
        }
    }
}