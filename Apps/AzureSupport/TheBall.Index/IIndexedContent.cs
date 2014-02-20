using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Documents;

namespace TheBall.Index
{
    public interface IIndexedDocument
    {
        Document GetLuceneDocument(string indexName);
    }
}
