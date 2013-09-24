using Microsoft.WindowsAzure.StorageClient;

namespace TheBall.CORE
{
    public class SetOwnerWebRedirectImplementation
    {
        public static void ExecuteMethod_SetRedirection(IContainerOwner owner, string redirectPath)
        {
            CloudBlob redirectBlob = StorageSupport.GetOwnerBlobReference(owner,
                                                                  InstanceConfiguration.RedirectFromFolderFileName);
            if (string.IsNullOrEmpty(redirectPath))
                redirectBlob.DeleteIfExists();
            else
                redirectBlob.UploadText(redirectPath);
            
        }
    }
}