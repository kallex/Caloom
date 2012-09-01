using System;
using System.Collections.Generic;
using System.Linq;

namespace AaltoGlobalImpact.OIP
{
    partial class InformationSourceCollection
    {
        public IInformationObject[] FetchAllInformationObjects()
        {
            return CollectionContent.Where(source => source.IsInformationObjectSource).Select(
                source => source.RetrieveInformationObject()).ToArray();
        }
    }
}