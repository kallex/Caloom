using System.Collections.Generic;
using System.IO;
using Lucene.Net.Documents;
using LuceneSupport;
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

        public static void ExecuteMethod_PerformIndexing(IContainerOwner owner, IndexingRequest indexingRequest, string luceneIndexFolder)
        {
            string indexName = indexingRequest.IndexName;
            List<Document> documents = new List<Document>();
            foreach (var objLocation in indexingRequest.ObjectLocations)
            {
                IInformationObject iObj = StorageSupport.RetrieveInformation(objLocation, null, owner);
                IIndexedDocument iDoc = iObj as IIndexedDocument;
                if (iDoc != null)
                {
                    var luceneDoc = iDoc.GetLuceneDocument(indexName);
                    luceneDoc.RemoveFields("ObjectDomainName");
                    luceneDoc.RemoveFields("ObjectName");
                    luceneDoc.RemoveFields("ObjectID");
                    luceneDoc.RemoveFields("ID");
                    luceneDoc.Add(new Field("ObjectDomainName", iObj.SemanticDomainName, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));
                    luceneDoc.Add(new Field("ObjectName", iObj.Name, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));
                    luceneDoc.Add(new Field("ObjectID", iObj.ID, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));
                    luceneDoc.Add(new Field("ID", iObj.ID, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));
                    documents.Add(luceneDoc);
                }
            }
            FieldIndexSupport.AddDocuments(luceneIndexFolder, documents.ToArray());
        }

        public static void ExecuteMethod_DeleteIndexingRequest(IndexingRequest indexingRequest)
        {
            indexingRequest.DeleteInformationObject();
        }

    }
}