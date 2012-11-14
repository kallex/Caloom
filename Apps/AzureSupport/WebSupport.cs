using System.Web;
using TheBall;

namespace AzureSupport
{
    public static class WebSupport
    {
        public static string GetLoginUrl(HttpContext context)
        {
            return context.User.Identity.Name;
        }

        static string GetContainerName(HttpRequest request)
        {
            return request.Url.DnsSafeHost.Replace('.', '-').ToLower();
        }

        public static void InitializeContextStorage(HttpRequest request)
        {
            string containerName = GetContainerName(request);
            InformationContext.Current.InitializeCloudStorageAccess(containerName);
        }
    }
}