using System;
using System.Web;
using Lucene.Net.Documents;
using LuceneSupport;
using TheBall;
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
            string currentAccountID = "";
            string currentAccountEmail = "";
            string currentAccountName = "";
            var utcNow = DateTime.UtcNow;
            if (InformationContext.CurrentAccount != null)
            {
                currentAccountID = InformationContext.CurrentAccount.AccountID;
                currentAccountEmail = InformationContext.CurrentAccount.AccountEmail;
                currentAccountName = InformationContext.CurrentAccount.AccountName;
            }
            if (ETag == null)
            {
                Created = utcNow;
                OriginalAuthorAccountID = currentAccountID;
                OriginalAuthorEmail = currentAccountEmail;
                OriginalAuthorName = currentAccountName;
            }
            LastModified = utcNow;
            LastAuthorAccountID = currentAccountID;
            LastAuthorEmail = currentAccountEmail;
            LastAuthorName = currentAccountName;
        }

        Document IIndexedDocument.GetLuceneDocument(string indexName)
        {
            Document document = new Document();
            document.Add(FieldIndexSupport.GetField("DocType", "Comment"));
            document.Add(FieldIndexSupport.GetField("OriginalAuthorAccountID", OriginalAuthorAccountID ?? ""));
            document.Add(FieldIndexSupport.GetField("OriginalAuthorEmail", OriginalAuthorEmail ?? ""));
            document.Add(FieldIndexSupport.GetField("OriginalAuthorName", OriginalAuthorName ?? ""));
            document.Add(FieldIndexSupport.GetField("LastAuthorAccountID", LastAuthorAccountID ?? ""));
            document.Add(FieldIndexSupport.GetField("LastAuthorEmail", LastAuthorEmail ?? ""));
            document.Add(FieldIndexSupport.GetField("LastAuthorName", LastAuthorName ?? ""));
            document.Add(FieldIndexSupport.GetField("CommentText", CommentText ?? "", analyzed: true));
            return document;
        }

    }
}