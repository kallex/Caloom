using System.Linq;

namespace TheBall.Index
{
    public class FilterAndSubmitIndexingRequestsImplementation
    {
        public static string[] GetTarget_ObjectsToIndex(string[] candidateObjectLocations)
        {
            return candidateObjectLocations.Where(item => item.Contains("/AaltoGlobalImpact.OIP/") &&
                (item.Contains("/TextContent/") || item.Contains("/Category/"))).ToArray();
        }

        public static IndexingRequest GetTarget_IndexingRequest(string[] objectsToIndex)
        {
            IndexingRequest indexingRequest = new IndexingRequest();
            var owner = InformationContext.CurrentOwner;
            indexingRequest.SetLocationAsOwnerContent(owner, indexingRequest.ID);
            indexingRequest.ObjectLocations.AddRange(objectsToIndex);
            return indexingRequest;
        }

        public static void ExecuteMethod_StoreObject(IndexingRequest indexingRequest)
        {
            indexingRequest.StoreInformation();
        }

        public static void ExecuteMethod_PutIndexingRequestToQueue(IndexingRequest indexingRequest)
        {
            string activeContainerName = StorageSupport.CurrActiveContainer.Name;
            IndexSupport.PutIndexingRequestToQueue(activeContainerName, "defaultindex", InformationContext.CurrentOwner, indexingRequest.ID);
        }
    }
}