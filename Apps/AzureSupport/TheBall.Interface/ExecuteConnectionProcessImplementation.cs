using System;
using TheBall.CORE;

namespace TheBall.Interface
{
    public class ExecuteConnectionProcessImplementation
    {
        private static IContainerOwner Owner
        {
            get { return InformationContext.CurrentOwner; }
        }
        public static Connection GetTarget_Connection(string connectionId)
        {
            return Connection.RetrieveFromOwnerContent(Owner, connectionId);
        }

        public static void ExecuteMethod_PerformProcessExecution(string connectionProcessToExecute, Connection connection)
        {
            string processID;
            switch (connectionProcessToExecute)
            {
                case "UpdateConnectionThisSideCategories":
                    processID = connection.ProcessIDToUpdateThisSideCategories;
                    break;
                default:
                    throw new NotImplementedException("Connection process execution not implemented for: " + connectionProcessToExecute);
            }
            ExecuteProcess.Execute(new ExecuteProcessParameters
            {
                ProcessID = processID
            });
        }
    }
}