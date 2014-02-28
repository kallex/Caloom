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
            return connection.ThisSideCategories.ToArray();
        }

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