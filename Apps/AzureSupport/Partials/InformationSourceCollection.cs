using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class InformationSourceCollection
    {
        public IInformationObject[] FetchAllInformationObjects()
        {
            return CollectionContent.Where(source => source.IsInformationObjectSource).Select(
                source => source.RetrieveInformationObject()).ToArray();
        }

        public InformationSource GetDefaultSource()
        {
            return
                CollectionContent.FirstOrDefault(
                    source => source.IsInformationObjectSource && String.IsNullOrEmpty(source.SourceName));
        }

        public bool HasAnySourceChanged()
        {
            return CollectionContent.Any(source => source.HasSourceChanged());
        }

        public void SubscribeTargetToSourceChanges(CloudBlob renderTarget)
        {
            foreach(var source in CollectionContent.Where(src => src.IsInformationObjectSource))
            {
                SubscribeSupport.AddSubscriptionToObject(source.SourceLocation, renderTarget.Name,
                                                         SubscribeSupport.SubscribeType_WebPageToSource);
            }
        }
    }
}