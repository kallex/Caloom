using System;
using System.Collections.Generic;
using System.IO;
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

        public InformationSource GetDefaultSource(string informationObjectTypeName)
        {
            return
                CollectionContent.FirstOrDefault(
                    source => source.IsInformationObjectSource && String.IsNullOrEmpty(source.SourceName) && source.SourceInformationObjectType == informationObjectTypeName);
        }

        public bool HasAnySourceChanged()
        {
            return CollectionContent.Any(source => source.HasSourceChanged());
        }

        public void SubscribeTargetToSourceChanges(CloudBlob subscriber)
        {
            if(subscriber.CanContainExternalMetadata() == false)
                throw new InvalidDataException("Subscriber candidate cannot contain metadata: " + subscriber.Name);
            foreach(var source in CollectionContent.Where(src => src.IsInformationObjectSource))
            {
                SubscribeSupport.AddSubscriptionToObject(source.SourceLocation, subscriber.Name,
                                                         SubscribeSupport.SubscribeType_WebPageToSource);
            }
        }

        public void SetDefaultSource(InformationSource defaultSource)
        {
            InformationSource currentDefaultSource = GetDefaultSource(defaultSource.SourceInformationObjectType);
            if (currentDefaultSource != null)
                CollectionContent.Remove(currentDefaultSource);
            CollectionContent.Add(defaultSource);
            currentDefaultSource = GetDefaultSource(defaultSource.SourceInformationObjectType);
            if(defaultSource != currentDefaultSource)
                throw new InvalidDataException("Invalid default source given to add (not maching the GetDefaultSource conditions)");
        }
    }
}