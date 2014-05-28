using System.Web;
using AzureSupport;
using TheBall.CORE;
using System.Linq;
using TheBall.Interface.INT;

namespace TheBall.Interface
{
    public class SetCategoryLinkingForConnectionImplementation
    {
        private static IContainerOwner Owner
        {
            get { return InformationContext.CurrentOwner; }
        }

        public static CategoryLinkParameters GetTarget_CategoryLinkingParameters()
        {
            var request = HttpContext.Current.Request;
            var stream = request.GetBufferlessInputStream();
            var result = JSONSupport.GetObjectFromStream<CategoryLinkParameters>(stream);
            return result;
        }

        public static Connection GetTarget_Connection(CategoryLinkParameters categoryLinkingParameters)
        {
            return Connection.RetrieveFromOwnerContent(Owner, categoryLinkingParameters.ConnectionID);
        }

        public static void ExecuteMethod_SetConnectionLinkingData(Connection connection, CategoryLinkParameters categoryLinkingParameters)
        {
            connection.CategoryLinks.Clear();
            connection.CategoryLinks.AddRange(categoryLinkingParameters.LinkItems.Select(getCategoryLinkFromInterfaceLink));
        }

        private static CategoryLink getCategoryLinkFromInterfaceLink(CategoryLinkItem categoryLinkItem)
        {
            return new CategoryLink
                {
                    SourceCategoryID = categoryLinkItem.SourceCategoryID,
                    TargetCategoryID = categoryLinkItem.TargetCategoryID,
                    LinkingType = categoryLinkItem.LinkingType
                };
        }

        public static void ExecuteMethod_StoreObject(Connection connection)
        {
            connection.StoreInformation();
        }
    }
}