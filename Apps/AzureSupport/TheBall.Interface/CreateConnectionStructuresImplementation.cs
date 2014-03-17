using TheBall.CORE;

namespace TheBall.Interface
{
    public class CreateConnectionStructuresImplementation
    {
        private static IContainerOwner Owner
        {
            get { return InformationContext.CurrentOwner; }
        }

        public static Connection GetTarget_Connection(string connectionId)
        {
            return Connection.RetrieveFromOwnerContent(Owner, connectionId);
        }

        public static Process GetTarget_ProcessToListPackageContents(Connection connection)
        {
            Process process = new Process
            {
                ExecutingOperation = new SemanticInformationItem("AaltoGlobalImpact.OIP.ListConnectionPackageContents", null),
            };
            process.InitialArguments.Add(new SemanticInformationItem("ConnectionID", connection.ID));
            return process;
        }

        public static Process GetTarget_ProcessToProcessReceivedData(Connection connection)
        {
            Process process = new Process
            {
                ExecutingOperation = new SemanticInformationItem("AaltoGlobalImpact.OIP.ProcessConnectionReceivedData", null),
            };
            process.InitialArguments.Add(new SemanticInformationItem("ConnectionID", connection.ID));
            return process;
        }

        public static Process GetTarget_ProcessToUpdateThisSideCategories(Connection connection)
        {
            Process process = new Process
            {
                ExecutingOperation = new SemanticInformationItem("AaltoGlobalImpact.OIP.UpdateConnectionThisSideCategories", null)
            };
            process.InitialArguments.Add(new SemanticInformationItem("ConnectionID", connection.ID));
            return process;
        }

        public static void ExecuteMethod_SetConnectionProcesses(Connection connection, Process processToListPackageContents, Process processToProcessReceivedData, Process processToUpdateThisSideCategories)
        {
            connection.ProcessIDToListPackageContents = processToListPackageContents.ID;
            connection.ProcessIDToProcessReceived = processToProcessReceivedData.ID;
            connection.ProcessIDToUpdateThisSideCategories = processToUpdateThisSideCategories.ID;
        }

        public static void ExecuteMethod_StoreObjects(Connection connection, Process processToListPackageContents, Process processToProcessReceivedData, Process processToUpdateThisSideCategories)
        {
            connection.StoreInformation();
            processToListPackageContents.StoreInformation();
            processToProcessReceivedData.StoreInformation();
            processToUpdateThisSideCategories.StoreInformation();
        }

        public static CreateConnectionStructuresReturnValue Get_ReturnValue(Connection connection)
        {
            return new CreateConnectionStructuresReturnValue {UpdatedConnection = connection};
        }
    }
}