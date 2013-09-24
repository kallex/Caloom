using System;
using System.Linq;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;
using TheBall.CORE;

namespace AaltoGlobalImpact.OIP
{
    partial class GroupContainer : IBeforeStoreHandler, IAdditionalFormatProvider
    {
        partial void DoPostStoringExecute(IContainerOwner owner)
        {
            return;
        }

        public void PerformBeforeStoreUpdate()
        {
            this.GroupIndex.Icon = this.GroupProfile.ProfileImage;
            this.LocationCollection.IsCollectionFiltered = true;
        }

        AdditionalFormatContent[] IAdditionalFormatProvider.GetAdditionalContentToStore(string masterBlobETag)
        {
            return this.GetFormattedContentToStore(masterBlobETag, AdditionalFormatSupport.WebUIFormatExtensions);
        }

        string[] IAdditionalFormatProvider.GetAdditionalFormatExtensions()
        {
            return this.GetFormatExtensions(AdditionalFormatSupport.WebUIFormatExtensions);
        }
    }
}