using System;
using System.Web;
using Lucene.Net.Documents;
using LuceneSupport;
using TheBall.Index;

namespace AaltoGlobalImpact.OIP
{
    partial class Comment : IAdditionalFormatProvider, IBeforeStoreHandler, IIndexedDocument
    {
        AdditionalFormatContent[] IAdditionalFormatProvider.GetAdditionalContentToStore(string masterBlobETag)
        {
            return this.GetFormattedContentToStore(masterBlobETag);
        }

        string[] IAdditionalFormatProvider.GetAdditionalFormatExtensions()
        {
            return this.GetFormatExtensions(AdditionalFormatSupport.WebUIFormatExtensions);
        }

        void IBeforeStoreHandler.PerformBeforeStoreUpdate()
        {
            if (ETag == null)
            {
                Published = DateTime.UtcNow;
            }
            Modified = DateTime.UtcNow;
        }

        Document IIndexedDocument.GetLuceneDocument(string indexName)
        {
            Document document = new Document();
            document.Add(FieldIndexSupport.GetField("DocType", "Comment"));
            document.Add(FieldIndexSupport.GetField("AuthorAccountID", AuthorAccountID ?? ""));
            document.Add(FieldIndexSupport.GetField("AuthorEmail", AuthorEmail ?? ""));
            document.Add(FieldIndexSupport.GetField("AuthorName", AuthorName ?? ""));
            document.Add(FieldIndexSupport.GetField("CommentText", CommentText ?? "", analyzed: true));
            return document;
        }

    }
}