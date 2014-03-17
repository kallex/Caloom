using TheBall.CORE;

namespace TheBall.Interface
{
    public class DeleteConnectionWithStructuresImplementation
    {
        private static IContainerOwner Owner
        {
            get { return InformationContext.CurrentOwner; }
        }
        public static Connection GetTarget_Connection(string connectionId)
        {
            return Connection.RetrieveFromOwnerContent(Owner, connectionId);
        }

        public static void ExecuteMethod_CallDeleteOnOtherEndOrDeleteDevice(bool isLaunchedByRemoteDelete, Connection connection)
        {
            if (isLaunchedByRemoteDelete == false)
            {
                try
                {
                    var result = DeviceSupport
                        .ExecuteRemoteOperation<ConnectionCommunicationData>(
                            connection.DeviceID,
                            "ExecuteRemoteCalledConnectionOperation", new ConnectionCommunicationData
                                {
                                    ActiveSideConnectionID = connection.ID,
                                    ReceivingSideConnectionID = connection.OtherSideConnectionID,
                                    ProcessRequest = "DELETEREMOTECONNECTION"
                                });
                    bool success = result.ReceivingSideConnectionID == null;
                }
                catch
                {

                }
            }
            else
            {
                try
                {
                    if (connection.DeviceID != null)
                    {
                        DeleteDeviceMembership.Execute(new DeleteDeviceMembershipParameters
                            {
                                Owner = InformationContext.CurrentOwner,
                                DeviceMembershipID = connection.DeviceID
                            });
                    }
                }
                catch
                {
                    
                }
            }
        }

        public static void ExecuteMethod_DeleteConnectionIntermediateContent(Connection connection)
        {
            string targetLocation;
            if (connection.IsActiveParty)
            {
                targetLocation = "TheBall.Interface/Connection/" + connection.ID;
            }
            else
            {
                if (connection.DeviceID != null)
                    targetLocation = "TheBall.CORE/DeviceMembership/" + connection.DeviceID + "_Input/";
                else
                    targetLocation = null;
            }
            if (targetLocation != null)
                StorageSupport.DeleteBlobsFromOwnerTarget(InformationContext.CurrentOwner, targetLocation);
        }

        public static void ExecuteMethod_DeleteConnectionProcesses(Connection connection)
        {
            DeleteProcess.Execute(new DeleteProcessParameters
                {
                    ProcessID = connection.ProcessIDToUpdateThisSideCategories
                });
            DeleteProcess.Execute(new DeleteProcessParameters
                {
                    ProcessID = connection.ProcessIDToListPackageContents
                });
            DeleteProcess.Execute(new DeleteProcessParameters
                {
                    ProcessID = connection.ProcessIDToProcessReceived
                });
        }

        public static void ExecuteMethod_DeleteConnectionObject(Connection connection)
        {
            connection.DeleteInformationObject(Owner);
        }
    }
}