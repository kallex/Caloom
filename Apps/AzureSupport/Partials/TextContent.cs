using System;
using Lucene.Net.Documents;
using LuceneSupport;
using TheBall.Index;

namespace AaltoGlobalImpact.OIP
{
    partial class TextContent : IAdditionalFormatProvider, IBeforeStoreHandler, IIndexedDocument
    {
        AdditionalFormatContent[] IAdditionalFormatProvider.GetAdditionalContentToStore(string masterBlobETag)
        {
            return this.GetFormattedContentToStore(masterBlobETag, AdditionalFormatSupport.WebUIFormatExtensions);
        }

        string[] IAdditionalFormatProvider.GetAdditionalFormatExtensions()
        {
            return this.GetFormatExtensions(AdditionalFormatSupport.WebUIFormatExtensions);
        }

        void IBeforeStoreHandler.PerformBeforeStoreUpdate()
        {
            /* Don't need to modify from default time */
            if (Published == default(DateTime))
                Published = Published.ToUniversalTime();
        }

        Document IIndexedDocument.GetLuceneDocument(string indexName)
        {
            Document document = new Document();
            document.Add(FieldIndexSupport.GetField("DocType", "Text"));
            document.Add(FieldIndexSupport.GetField("Title", Title));
            document.Add(FieldIndexSupport.GetField("Excerpt", Excerpt, analyzed: true));
            document.Add(FieldIndexSupport.GetField("Body", Body, analyzed: true));
            return document;
        }
    }
}