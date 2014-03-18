using System.Linq;
using TheBall.CORE;

namespace TheBall.Interface
{
    public class UpdateConnectionOtherSideCategoriesImplementation
    {
        private static IContainerOwner Owner
        {
            get { return InformationContext.CurrentOwner; }
        }
        public static Connection GetTarget_Connection(string connectionId)
        {
            return Connection.RetrieveFromOwnerContent(Owner, connectionId);
        }

        public static Category[] ExecuteMethod_GetOtherSideCategories(Connection connection)
        {
            ConnectionCommunicationData connectionData = new ConnectionCommunicationData
                {
                    ActiveSideConnectionID = connection.ID,
                    ReceivingSideConnectionID = connection.OtherSideConnectionID,
                    ProcessRequest = "GETCATEGORIES"
                };
            var result = DeviceSupport.ExecuteRemoteOperation<ConnectionCommunicationData>(connection.DeviceID,
                                                                                           "TheBall.Interface.ExecuteRemoteCalledConnectionOperation", connectionData);
            return result.CategoryCollectionData.Select(getCategoryFromCategoryInfo).ToArray();
        }

        private static Category getCategoryFromCategoryInfo(CategoryInfo catInfo)
        {
            return new Category
                {
                    ID = catInfo.CategoryID,
                    NativeCategoryID = catInfo.NativeCategoryID,
                    NativeCategoryDomainName = catInfo.NativeCategoryDomainName,
                    NativeCategoryObjectName = catInfo.NativeCategoryObjectName,
                    NativeCategoryTitle = catInfo.NativeCategoryTitle,
                    IdentifyingCategoryName = catInfo.IdentifyingCategoryName,
                    ParentCategoryID = catInfo.ParentCategoryID
                };
        }

        /*
         * 
         *             <InterfaceItem name="NativeCategoryID" logicalDataType="Text_Short" />
                    <InterfaceItem name="NativeCategoryDomainName" logicalDataType="Text_Short"/>
                    <InterfaceItem name="NativeCategoryObjectName" logicalDataType="Text_Short"/>
                    <InterfaceItem name="NativeCategoryTitle" logicalDataType="Text_Short"/>
                    <InterfaceItem name="IdentifyingCategoryName" logicalDataType="Text_Short" />
                    <InterfaceItem name="ParentCategoryID" logicalDataType="Text_Short"/>

         * 
         * */

        public static void ExecuteMethod_UpdateOtherSideCategories(Connection connection, Category[] getOtherSideCategoriesOutput)
        {
            connection.OtherSideCategories.Clear();
            connection.OtherSideCategories.AddRange(getOtherSideCategoriesOutput);
        }

        public static void ExecuteMethod_StoreObject(Connection connection)
        {
            connection.StoreInformation();
        }
    }
}