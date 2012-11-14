using System;
using System.IO;
using System.Web;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class AddBlogPostInfo : IAddOperationProvider
    {
        public bool PerformAddOperation(string commandName, InformationSourceCollection sources, string requesterLocation, HttpFileCollection files)
        {
            if(Title == "")
                throw new InvalidDataException("Blog title is mandatory");
            Blog blog = Blog.CreateDefault();
            VirtualOwner owner = VirtualOwner.FigureOwner(this);
            blog.SetLocationAsOwnerContent(owner, blog.ID);
            blog.Title = Title;
            blog.ReferenceToInformation.Title = blog.Title;
            blog.ReferenceToInformation.URL = DefaultViewSupport.GetDefaultViewURL(blog);
            blog.Published = DateTime.Now;
            StorageSupport.StoreInformationMasterFirst(blog, owner, true);
            DefaultViewSupport.CreateDefaultViewRelativeToRequester(requesterLocation, blog, owner);
            //BlogContainer blogContainer = BlogContainer.RetrieveFromOwnerContent(owner, "default");
            //blogContainer.AddNewBlogPost(blog);
            //StorageSupport.StoreInformation(blogContainer);
            Title = "";
            return true;
        }
    }
}