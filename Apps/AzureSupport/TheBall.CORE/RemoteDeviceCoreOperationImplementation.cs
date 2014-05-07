using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AzureSupport;
using Microsoft.WindowsAzure.StorageClient;

namespace TheBall.CORE
{
    public class RemoteDeviceCoreOperationImplementation
    {
        public static DeviceOperationData GetTarget_DeviceOperationData(Stream inputStream)
        {
            return JSONSupport.GetObjectFromStream<DeviceOperationData>(inputStream);
        }

        public static DeviceMembership GetTarget_CurrentDevice()
        {
            return InformationContext.CurrentExecutingForDevice;
        }

        public static void ExecuteMethod_PerformOperation(DeviceMembership currentDevice, DeviceOperationData deviceOperationData)
        {
            switch (deviceOperationData.OperationRequestString)
            {
                case "SYNCCOPYCONTENT":
                    performSyncQueryForCopyContent(currentDevice, deviceOperationData);
                    break;
                case "DELETEREMOTEDEVICE":
                    currentDevice.DeleteInformationObject();
                    break;
                case "GETCONTENTMD5LIST":
                    getContentMD5List(deviceOperationData);
                    break;
                case "COPYSYNCEDCONTENTTOOWNER":
                    copySyncedContentToOwner(deviceOperationData);
                    break;
                default:
                    throw new NotImplementedException("Not implemented RemoteDevice operation for request string: " + deviceOperationData.OperationRequestString);
            }
            deviceOperationData.OperationResult = true;
        }

        private static void copySyncedContentToOwner(DeviceOperationData deviceOperationData)
        {
            string folderPrefix = deviceOperationData.OperationParameters[0];
            if (!isValidFolderName(folderPrefix))
                throw new InvalidDataException("Invalid data for remote folder name");
            var currentDevice = InformationContext.CurrentExecutingForDevice;
            string deviceInputRoot = getDeviceInputRoot(currentDevice.ID);
            string syncSourceRootFolder = deviceInputRoot + folderPrefix;
            var owner = InformationContext.CurrentOwner;
            ContentItemLocationWithMD5[] syncContentList = owner.GetOwnerBlobListing(syncSourceRootFolder, true)
                                                                .Cast<CloudBlockBlob>()
                                                                .Select(blob => new ContentItemLocationWithMD5
                                                                    {
                                                                        ContentLocation = StorageSupport.RemoveOwnerPrefixIfExists(blob.Name)
                                                                            .Substring(syncSourceRootFolder.Length),
                                                                        ContentMD5 = blob.Properties.ContentMD5
                                                                    }).ToArray();
            string syncTargetRootFolder = folderPrefix;
            SyncSupport.SynchronizeSourceListToTargetFolder(syncSourceRootFolder, syncContentList, syncTargetRootFolder);
        }

        static bool isValidFolderName(string folderName)
        {
            return folderName.StartsWith("DEV_") || folderName == "wwwsite";
        }

        private static void getContentMD5List(DeviceOperationData deviceOperationData)
        {
            var folders = deviceOperationData.OperationParameters;
            bool hasInvalidFolderNames = folders.Any(folder => !isValidFolderName(folder));
            if(hasInvalidFolderNames)
                throw new InvalidDataException("Invalid parameter for remote folder name");
            var owner = InformationContext.CurrentOwner;
            var md5List = folders.SelectMany(folder => owner.GetOwnerBlobListing(folder, true)
                                                            .Cast<CloudBlockBlob>()
                                                            .Select(blob => new ContentItemLocationWithMD5
                                                                {
                                                                    ContentLocation = StorageSupport.RemoveOwnerPrefixIfExists(blob.Name),
                                                                    ContentMD5 = blob.Properties.ContentMD5
                                                                })).ToArray();
            deviceOperationData.OperationSpecificContentData = md5List;
            deviceOperationData.OperationResult = true;
        }

        public static void ExecuteMethod_SerializeDeviceOperationDataToOutput(Stream outputStream, DeviceOperationData deviceOperationData)
        {
            JSONSupport.SerializeToJSONStream(deviceOperationData, outputStream);
        }

        private static void performSyncQueryForCopyContent(DeviceMembership currentDevice, DeviceOperationData deviceOperationData)
        {
            List<ContentItemLocationWithMD5> itemsToCopy = new List<ContentItemLocationWithMD5>();
            List<ContentItemLocationWithMD5> itemsDeleted = new List<ContentItemLocationWithMD5>();
            SyncSupport.CopySourceToTargetMethod copySourceToTarget = (location, blobLocation) => itemsToCopy.Add(new ContentItemLocationWithMD5
                {
                    ContentLocation = StorageSupport.RemoveOwnerPrefixIfExists(location),
                    ItemDatas = new ItemData[] {new ItemData {DataName = "OPTODO", ItemTextData = "COPY"}}
                });
            SyncSupport.DeleteObsoleteTargetMethod deleteObsoleteTarget = (location) =>
                {
                    itemsDeleted.Add(new ContentItemLocationWithMD5
                        {
                            ContentLocation = location,
                            ItemDatas = new ItemData[] { new ItemData { DataName = "OPDONE", ItemTextData = "DELETED"}}
                        });
                    SyncSupport.DeleteObsoleteTarget(location);
                };

            string syncTargetRootFolder = getDeviceInputRoot(currentDevice.ID) + deviceOperationData.OperationParameters[0];
            if (syncTargetRootFolder.EndsWith("/") == false)
                syncTargetRootFolder += "/";
            SyncSupport.SynchronizeSourceListToTargetFolder(SyncSupport.SourceIsRelativeRoot, deviceOperationData.OperationSpecificContentData, syncTargetRootFolder,
                                                            copySourceToTarget, deleteObsoleteTarget);
            deviceOperationData.OperationSpecificContentData = itemsToCopy.Union(itemsDeleted).ToArray();
        }


        static string getDeviceInputRoot(string deviceID)
        {
            return String.Format("TheBall.CORE/DeviceMembership/{0}_Input/", deviceID);
        }
        /*
                    ItemsToCopy = deviceOperationData.OperationSpecificContentData.Where(item => item.ItemDatas.Any(iData => iData.DataName == "OPTODO" && iData.ItemTextData == "COPY")).ToArray(),
                    ItemsDeleted = deviceOperationData.OperationSpecificContentData.Where(item => item.ItemDatas.Any(iData => iData.DataName == "OPDONE" && iData.ItemTextData == "DELETED")).ToArray()
         * */

    }
}