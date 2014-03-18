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
            CreateProcessParameters processParameters = new CreateProcessParameters
                {
                    ExecutingOperationName = "AaltoGlobalImpact.OIP.ListConnectionPackageContents",
                    InitialArguments = new SemanticInformationItem[] {new SemanticInformationItem("ConnectionID", connection.ID)},
                    ProcessDescription = "Process to list package contents"
                };
            var result = CreateProcess.Execute(processParameters);
            return result.CreatedProcess;
        }

        public static Process GetTarget_ProcessToProcessReceivedData(Connection connection)
        {
            CreateProcessParameters processParameters = new CreateProcessParameters
            {
                ExecutingOperationName = "AaltoGlobalImpact.OIP.ProcessConnectionReceivedData",
                InitialArguments = new SemanticInformationItem[] { new SemanticInformationItem("ConnectionID", connection.ID) },
                ProcessDescription = "Process to list package contents"
            };
            var result = CreateProcess.Execute(processParameters);
            return result.CreatedProcess;
        }

        public static Process GetTarget_ProcessToUpdateThisSideCategories(Connection connection)
        {
            CreateProcessParameters processParameters = new CreateProcessParameters
            {
                ExecutingOperationName = "AaltoGlobalImpact.OIP.UpdateConnectionThisSideCategories",
                InitialArguments = new SemanticInformationItem[] { new SemanticInformationItem("ConnectionID", connection.ID) },
                ProcessDescription = "Process to list package contents"
            };
            var result = CreateProcess.Execute(processParameters);
            return result.CreatedProcess;
        }

        public static void ExecuteMethod_SetConnectionProcesses(Connection connection, Process processToListPackageContents, Process processToProcessReceivedData, Process processToUpdateThisSideCategories)
        {
            connection.ProcessIDToListPackageContents = processToListPackageContents.ID;
            connection.ProcessIDToProcessReceived = processToProcessReceivedData.ID;
            connection.ProcessIDToUpdateThisSideCategories = processToUpdateThisSideCategories.ID;
        }

        public static void ExecuteMethod_StoreObject(Connection connection)
        {
            connection.StoreInformation();
        }

        public static CreateConnectionStructuresReturnValue Get_ReturnValue(Connection connection)
        {
            return new CreateConnectionStructuresReturnValue {UpdatedConnection = connection};
        }
    }
}