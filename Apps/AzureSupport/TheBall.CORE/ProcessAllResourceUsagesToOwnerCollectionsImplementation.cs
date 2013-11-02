namespace TheBall.CORE
{
    public class ProcessAllResourceUsagesToOwnerCollectionsImplementation
    {
        public static void ExecuteMethod_ExecuteBatchProcessor(int processBatchSize)
        {
            bool continueProcessing;
            do
            {
                var processResult =
                    ProcessBatchOfResourceUsagesToOwnerCollections.Execute(new ProcessBatchOfResourceUsagesToOwnerCollectionsParameters
                        {
                            ProcessBatchSize = processBatchSize,
                            ProcessIfLess = false
                        });
                continueProcessing = processResult.ProcessedAnything && processResult.ProcessedFullCount;
            } while (continueProcessing);
        }
    }
}