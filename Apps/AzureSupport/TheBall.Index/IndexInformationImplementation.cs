using System.IO;
using TheBall.CORE;

namespace TheBall.Index
{
    public class IndexInformationImplementation
    {
        public static IndexingRequest GetTarget_IndexingRequest(IContainerOwner owner, string indexingRequestId)
        {
            return IndexingRequest.RetrieveFromOwnerContent(owner, indexingRequestId);
        }

        public static string GetTarget_LuceneIndexFolder(IContainerOwner owner, string indexName, string indexStorageRootPath)
        {
            string fullPath = Path.Combine(indexStorageRootPath, owner.ToFolderName(), indexName);
            return fullPath;
        }

        public static void ExecuteMethod_PerformIndexing(IndexingRequest indexingRequest, string luceneIndexFolder)
        {

        }

        public static void ExecuteMethod_DeleteIndexingRequest(IndexingRequest indexingRequest)
        {
            indexingRequest.DeleteInformationObject();
        }

    }
}