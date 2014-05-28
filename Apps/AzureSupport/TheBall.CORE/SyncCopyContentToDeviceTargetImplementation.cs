using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.StorageClient;
using TheBall.CORE.INT;

namespace TheBall.CORE
{
    public class SyncCopyContentToDeviceTargetImplementation
    {
        public static AuthenticatedAsActiveDevice GetTarget_AuthenticatedAsActiveDevice(string authenticatedAsActiveDeviceId)
        {
            return AuthenticatedAsActiveDevice.RetrieveFromOwnerContent(InformationContext.CurrentOwner, authenticatedAsActiveDeviceId);
        }

        public static string GetTarget_ContentRootLocation(AuthenticatedAsActiveDevice authenticatedAsActiveDevice)
        {
            var contentRootLocation = StorageSupport.GetOwnerContentLocation(InformationContext.CurrentOwner, authenticatedAsActiveDevice.RelativeLocation) + "/";
            if (contentRootLocation.EndsWith("/") == false)
                contentRootLocation += "/";
            return contentRootLocation;
        }

        public static ContentItemLocationWithMD5[] GetTarget_ThisSideContentMD5List(string contentRootLocation)
        {
            var blobList = InformationContext.CurrentOwner.GetOwnerBlobListing(contentRootLocation, true);
            int contentRootLength = contentRootLocation.Length;
            List<ContentItemLocationWithMD5> list = new List<ContentItemLocationWithMD5>();
            foreach (CloudBlockBlob blob in blobList)
            {
                string relativeLocation = blob.Name.Substring(contentRootLength);
                list.Add(new ContentItemLocationWithMD5
                    {
                        ContentLocation = relativeLocation, ContentMD5 = blob.Properties.ContentMD5
                    });
            }
            return list.ToArray();
        }

        public static SyncCopyContentToDeviceTarget.CallPrepareTargetAndListItemsToCopyReturnValue ExecuteMethod_CallPrepareTargetAndListItemsToCopy(AuthenticatedAsActiveDevice authenticatedAsActiveDevice, ContentItemLocationWithMD5[] thisSideContentMd5List)
        {
            DeviceOperationData deviceOperationData = new DeviceOperationData
                {
                    OperationRequestString = "SYNCCOPYCONTENT",
                    OperationSpecificContentData = thisSideContentMd5List,
                    OperationParameters = new string[] { SyncSupport.RelativeRootFolderValue}
                };
            deviceOperationData = DeviceSupport.ExecuteRemoteOperation<DeviceOperationData>(authenticatedAsActiveDevice.ID,
                                                                                            "TheBall.CORE.RemoteDeviceCoreOperation", deviceOperationData);
            var returnValue = new SyncCopyContentToDeviceTarget.CallPrepareTargetAndListItemsToCopyReturnValue
                {
                    ItemsToCopy = deviceOperationData.OperationSpecificContentData.Where(item => item.ItemDatas.Any(iData => iData.DataName == "OPTODO" && iData.ItemTextData == "COPY")).ToArray(),
                    ItemsDeleted = deviceOperationData.OperationSpecificContentData.Where(item => item.ItemDatas.Any(iData => iData.DataName == "OPDONE" && iData.ItemTextData == "DELETED")).ToArray()
                };
            return returnValue;
        }

        public static void ExecuteMethod_CopyItemsToCopyToTargetDevice(AuthenticatedAsActiveDevice authenticatedAsActiveDevice, SyncCopyContentToDeviceTarget.CallPrepareTargetAndListItemsToCopyReturnValue callPrepareTargetAndListItemsToCopyOutput)
        {
            var itemsToCopy = callPrepareTargetAndListItemsToCopyOutput.ItemsToCopy;
            foreach(var itemToCopy in itemsToCopy)
            {
                string ownerRelatedLocation = StorageSupport.RemoveOwnerPrefixIfExists(itemToCopy.ContentLocation);
                DeviceSupport.PushContentToDevice(authenticatedAsActiveDevice, ownerRelatedLocation, ownerRelatedLocation);
            }
        }

        public static SyncCopyContentToDeviceTargetReturnValue Get_ReturnValue(SyncCopyContentToDeviceTarget.CallPrepareTargetAndListItemsToCopyReturnValue callPrepareTargetAndListItemsToCopyOutput)
        {
            return new SyncCopyContentToDeviceTargetReturnValue
                {
                    CopiedItems = callPrepareTargetAndListItemsToCopyOutput.ItemsToCopy,
                    DeletedItems = callPrepareTargetAndListItemsToCopyOutput.ItemsDeleted
                };
        }
    }
}