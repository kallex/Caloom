using System.Collections.Generic;
using System.IO;
using System.Linq;
using AzureSupport;
using TheBall.Interface.INT;

namespace TheBall.Interface
{
    public class ExecuteRemoteCalledConnectionOperationImplementation
    {
        public static ConnectionCommunicationData GetTarget_ConnectionCommunicationData(Stream inputStream)
        {
            return JSONSupport.GetObjectFromStream<ConnectionCommunicationData>(inputStream);
        }

        public static void ExecuteMethod_PerformOperation(ConnectionCommunicationData connectionCommunicationData)
        {
            switch (connectionCommunicationData.ProcessRequest)
            {
                case "PROCESSPUSHEDCONTENT":
                    {
                        ExecuteConnectionProcess.Execute(new ExecuteConnectionProcessParameters
                            {
                                ConnectionID = connectionCommunicationData.ReceivingSideConnectionID,
                                ConnectionProcessToExecute = "ProcessReceived"
                            });
                        break;
                    }
                case "SYNCCATEGORIES":
                    {
                        ExecuteConnectionProcess.Execute(new ExecuteConnectionProcessParameters
                            {
                                ConnectionID = connectionCommunicationData.ReceivingSideConnectionID,
                                ConnectionProcessToExecute = "UpdateConnectionThisSideCategories"
                            });
                        Connection thisSideConnection = Connection.RetrieveFromOwnerContent(InformationContext.CurrentOwner,
                                                                                            connectionCommunicationData.ReceivingSideConnectionID);
                        thisSideConnection.OtherSideCategories.Clear();
                        thisSideConnection.OtherSideCategories.AddRange(connectionCommunicationData.CategoryCollectionData.Select(catInfo => catInfo.ToCategory()));
                        thisSideConnection.CategoryLinks.Clear();
                        thisSideConnection.CategoryLinks.AddRange(connectionCommunicationData.LinkItems.Select(catLinkItem => new CategoryLink
                            {
                                SourceCategoryID = catLinkItem.SourceCategoryID,
                                TargetCategoryID = catLinkItem.TargetCategoryID,
                                LinkingType = catLinkItem.LinkingType
                            }));
                        thisSideConnection.StoreInformation();
                        connectionCommunicationData.CategoryCollectionData = thisSideConnection.ThisSideCategories.Select(CategoryInfo.FromCategory).ToArray();
                        break;
                    }
                case "FINALIZECONNECTION":
                    var output = CreateReceivingConnection.Execute(new CreateReceivingConnectionParameters
                        {
                            Description = connectionCommunicationData.ProcessParametersString,
                            OtherSideConnectionID = connectionCommunicationData.ActiveSideConnectionID
                        });
                    connectionCommunicationData.ReceivingSideConnectionID = output.ConnectionID;
                    CreateReceivingConnectionStructures.Execute(new CreateReceivingConnectionStructuresParameters
                        {
                            ConnectionCommunicationData = connectionCommunicationData
                        });
                    break;
                case "DELETEREMOTECONNECTION":
                    DeleteConnectionWithStructures.Execute(new DeleteConnectionWithStructuresParameters
                        {
                            ConnectionID = connectionCommunicationData.ReceivingSideConnectionID,
                            IsLaunchedByRemoteDelete = true
                        });
                    connectionCommunicationData.ReceivingSideConnectionID = null;
                    break;
                default:
                    break;
            }
        }

        public static void ExecuteMethod_SerializeCommunicationDataToOutput(Stream outputStream, ConnectionCommunicationData connectionCommunicationData)
        {
            JSONSupport.SerializeToJSONStream(connectionCommunicationData, outputStream);
        }
    }
}