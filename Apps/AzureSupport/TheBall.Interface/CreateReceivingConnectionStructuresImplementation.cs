using System.Collections.Generic;
using System.IO;
using TheBall.CORE;

namespace TheBall.Interface
{
    public class CreateReceivingConnectionStructuresImplementation
    {
        private static IContainerOwner Owner
        {
            get { return InformationContext.CurrentOwner; }
        }

        public static Connection GetTarget_ThisSideConnection(ConnectionCommunicationData connectionCommunicationData)
        {
            if(string.IsNullOrEmpty(connectionCommunicationData.ReceivingSideConnectionID) == false)
                throw new InvalidDataException("ReceivingSideConnectionID must not be initialized at creation");
            Connection connection = new Connection();
            connection.SetLocationAsOwnerContent(Owner, connection.ID);
            connectionCommunicationData.ReceivingSideConnectionID = connection.ID;
            return connection;
        }

        public static Process GetTarget_ProcessToListPackageContents(ConnectionCommunicationData connectionCommunicationData)
        {
            Process process = new Process
                {
                    ExecutingOperation = new SemanticInformationItem("AaltoGlobalImpact.OIP.ListConnectionPackageContents", null),
                };
            process.InitialArguments.Add(new SemanticInformationItem("ConnectionID", connectionCommunicationData.ReceivingSideConnectionID));
            return process;
        }

        public static Process GetTarget_ProcessToProcessReceivedData(ConnectionCommunicationData connectionCommunicationData)
        {
            Process process = new Process
                {
                    ExecutingOperation = new SemanticInformationItem("AaltoGlobalImpact.OIP.ProcessConnectionReceivedData", null),
                };
            process.InitialArguments.Add(new SemanticInformationItem("ConnectionID", connectionCommunicationData.ReceivingSideConnectionID));
            return process;
        }

        public static Process GetTarget_ProcessToUpdateThisSideCategories(ConnectionCommunicationData connectionCommunicationData)
        {
            Process process = new Process
                {
                    ExecutingOperation = new SemanticInformationItem("AaltoGlobalImpact.OIP.UpdateConnectionThisSideCategories", null)
                };
            process.InitialArguments.Add(new SemanticInformationItem("ConnectionID", connectionCommunicationData.ReceivingSideConnectionID));
            return process;
        }

        public static void ExecuteMethod_SetConnectionProcesses(Connection thisSideConnection, Process processToListPackageContents, Process processToProcessReceivedData, Process processToUpdateThisSideCategories)
        {
            thisSideConnection.ProcessIDToListPackageContents = processToListPackageContents.ID;
            thisSideConnection.ProcessIDToProcessReceived = processToProcessReceivedData.ID;
            thisSideConnection.ProcessIDToUpdateThisSideCategories = processToUpdateThisSideCategories.ID;
        }

        public static void ExecuteMethod_StoreObjects(Connection thisSideConnection, Process processToListPackageContents, Process processToProcessReceivedData, Process processToUpdateThisSideCategories)
        {
            thisSideConnection.StoreInformation();
        }
    }
}