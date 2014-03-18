using System.Collections.Generic;
using System.IO;
using System.Linq;
using AzureSupport;

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
                case "GETCATEGORIES":
                    {
                        Connection thisSideConnection = Connection.RetrieveFromOwnerContent(InformationContext.CurrentOwner,
                                                                                            connectionCommunicationData.ReceivingSideConnectionID);
                        connectionCommunicationData.CategoryCollectionData = getCategoryCollectionData(thisSideConnection.ThisSideCategories);
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

        private static CategoryInfo[] getCategoryCollectionData(List<Category> thisSideCategories)
        {
            return thisSideCategories.Select(getCategoryInfo).ToArray();
        }

        private static CategoryInfo getCategoryInfo(Category category)
        {
            return new CategoryInfo
                {
                    CategoryID = category.ID,
                    NativeCategoryID = category.NativeCategoryID,
                    NativeCategoryDomainName = category.NativeCategoryDomainName,
                    NativeCategoryObjectName = category.NativeCategoryObjectName,
                    NativeCategoryTitle = category.NativeCategoryTitle,
                    IdentifyingCategoryName = category.IdentifyingCategoryName,
                    ParentCategoryID = category.ParentCategoryID
                };
        }

        public static void ExecuteMethod_SerializeCommunicationDataToOutput(Stream outputStream, ConnectionCommunicationData connectionCommunicationData)
        {
            JSONSupport.SerializeToJSONStream(connectionCommunicationData, outputStream);
        }
    }
}