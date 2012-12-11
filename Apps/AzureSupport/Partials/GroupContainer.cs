using System;
using System.Linq;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class GroupContainer : IBeforeStoreHandler
    {
        partial void DoPostStoringExecute(IContainerOwner owner)
        {
            return;
        }

        public void PerformBeforeStoreUpdate()
        {
            this.GroupIndex.Icon = this.GroupProfile.ProfileImage;
        }
    }
}