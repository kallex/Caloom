using System.Linq;
using TheBall.CORE;

namespace TheBall.Interface
{
    public class SynchronizeConnectionCategoriesImplementation
    {
        private static IContainerOwner Owner
        {
            get { return InformationContext.CurrentOwner; }
        }
        public static Connection GetTarget_Connection(string connectionId)
        {
            return Connection.RetrieveFromOwnerContent(Owner, connectionId);
        }

        public static Category[] ExecuteMethod_SyncCategoriesWithOtherSideCategories(Connection connection)
        {
            ConnectionCommunicationData connectionData = new ConnectionCommunicationData
                {
                    ActiveSideConnectionID = connection.ID,
                    ReceivingSideConnectionID = connection.OtherSideConnectionID,
                    ProcessRequest = "SYNCCATEGORIES",
                    CategoryCollectionData = connection.ThisSideCategories.Select(CategoryInfo.FromCategory).ToArray()
                };
            var result = DeviceSupport.ExecuteRemoteOperation<ConnectionCommunicationData>(connection.DeviceID,
                                                                                           "TheBall.Interface.ExecuteRemoteCalledConnectionOperation", connectionData);
            return result.CategoryCollectionData.Select(catInfo => catInfo.ToCategory()).ToArray();
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

        public static void ExecuteMethod_ExecuteProcessToUpdateThisSideCategories(string connectionID)
        {
            ExecuteConnectionProcess.Execute(new ExecuteConnectionProcessParameters
                {
                    ConnectionID = connectionID,
                    ConnectionProcessToExecute = "UpdateConnectionThisSideCategories"
                });
        }
    }
}