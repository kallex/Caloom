using System.Linq;
using TheBall;
using TheBall.CORE;

namespace AaltoGlobalImpact.OIP
{
    public class ListConnectionPackageContentsImplementation
    {
        public static string GetTarget_ConnectionID(Process process)
        {
            string connectionID = process.InitialArguments.First(arg => arg.ItemFullType == "ConnectionID").ItemValue;
            return connectionID;
        }

        public static PickCategorizedContentToConnectionParameters CallPickCategorizedContentConnection_GetParameters(string connectionId)
        {
            return new PickCategorizedContentToConnectionParameters {ConnectionID = connectionId};
        }

        public static string[] CallPickCategorizedContentConnection_GetOutput(PickCategorizedContentToConnectionReturnValue operationReturnValue, string connectionId)
        {
            return operationReturnValue.ContentLocations;
        }

        public static void ExecuteMethod_SetContentsAsProcessOutput(Process process, string[] callPickCategorizedContentConnectionOutput)
        {
/*
 *                 var contentLocation = processItem.Outputs.First(item => item.ItemFullType == "ContentLocation").ItemValue;
                var contentMD5 = processItem.Outputs.First(item => item.ItemFullType == "ContentMD5").ItemValue;
*/
            process.ProcessItems.Clear();
            foreach (string contentLocation in callPickCategorizedContentConnectionOutput)
            {
                SemanticInformationItem semanticItemForLocation = new SemanticInformationItem
                    {
                        ItemFullType = "ContentLocation",
                        ItemValue = contentLocation
                    };
                var blob = StorageSupport.GetOwnerBlobReference(InformationContext.CurrentOwner, contentLocation);
                blob.FetchAttributes();
                SemanticInformationItem semanticItemForMD5 = new SemanticInformationItem
                    {
                        ItemFullType = "ContentMD5",
                        ItemValue = blob.Properties.ContentMD5
                    };
                ProcessItem processItem = new ProcessItem();
                processItem.Outputs.Add(semanticItemForLocation);
                processItem.Outputs.Add(semanticItemForMD5);
                process.ProcessItems.Add(processItem);
            }
        }
    }
}