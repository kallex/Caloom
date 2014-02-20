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
            List<string> removeDocumentIDs = new List<string>();
            foreach (var objLocation in indexingRequest.ObjectLocations)
            {
                IInformationObject iObj = StorageSupport.RetrieveInformation(objLocation, null, owner);
                if (iObj == null)
                {
                    var lastSlashIX = objLocation.LastIndexOf('/');
                    var objectID = objLocation.Substring(lastSlashIX + 1);
                    removeDocumentIDs.Add(objectID);
                    continue;
                }
                IIndexedDocument iDoc = iObj as IIndexedDocument;
                if (iDoc != null)
                {
                    var luceneDoc = iDoc.GetLuceneDocument(indexName);
                    if (luceneDoc == null)
                        continue;
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
            FieldIndexSupport.AddAndRemoveDocuments(luceneIndexFolder, documents.ToArray(), removeDocumentIDs.ToArray());
        }

        public static void ExecuteMethod_DeleteIndexingRequest(IndexingRequest indexingRequest)
        {
            indexingRequest.DeleteInformationObject();
        }

    }
}