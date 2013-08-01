using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security;
using System.Web;
using TheBall.CORE;
using AaltoGlobalImpact.OIP;

namespace TheBall
{
    public static class ModifyInformationSupport
    {
        public static void ExecuteOwnerWebPOST(IContainerOwner containerOwner, NameValueCollection form, HttpFileCollection fileContent)
        {
            bool isCancelButton = form["btnCancel"] != null;
            if (isCancelButton)
                return;

            string contentSourceInfo = form["ContentSourceInfo"];
            string[] contentSourceInfos = contentSourceInfo.Split(',');
            NameValueCollection fileEntries = new NameValueCollection();
            NameValueCollection fieldEntries = new NameValueCollection();
            NameValueCollection objectEntries = new NameValueCollection();
            foreach (var key in form.AllKeys)
            {
                var value = form[key];
                if (key.StartsWith("File_"))
                    fileEntries.Add(key, value);
                else if (key.StartsWith("Object_"))
                    objectEntries.Add(key, value);
                else
                    fieldEntries.Add(key, value);
            
            }
            foreach (var key in fileContent.AllKeys)
            {
                if (key.StartsWith("File_") && fileEntries.AllKeys.Contains(key) == false)
                    fileEntries.Add(key, "");
            }
            foreach (string sourceInfo in contentSourceInfos)
            {
                string relativeLocation;
                string oldETag;
                retrieveDataSourceInfo(sourceInfo, out relativeLocation, out oldETag);
                VirtualOwner verifyOwner = VirtualOwner.FigureOwner(relativeLocation);
                if (verifyOwner.IsSameOwner(containerOwner) == false)
                    throw new SecurityException("Mismatch in ownership of data submission");
                IInformationObject rootObject = StorageSupport.RetrieveInformation(relativeLocation, oldETag,
                                                                                   containerOwner);
                if (oldETag != rootObject.ETag)
                {
                    throw new InvalidDataException("Information under editing was modified during display and save");
                }
                // TODO: Proprely validate against only the object under the editing was changed (or its tree below)
                SetFieldValues(rootObject, fieldEntries);
                SetBinaryContent(rootObject, fileEntries, fileContent, containerOwner);
                SetObjectLinks(rootObject, objectEntries);

                /* Operation bridge model below - not used/needed with field assignment solution */
                /*
                var removeMediaList = form["cmdRemoveMedia"];
                if (String.IsNullOrWhiteSpace(removeMediaList) == false)
                {
                    string[] removeList = removeMediaList.Split(',');
                    foreach (string contentInfo in removeList)
                    {
                        SetBinaryContent(containerOwner, contentInfo, rootObject, null);
                    }
                }
                 * */
                rootObject.StoreInformationMasterFirst(containerOwner, false);
            }

        }

        private static void SetObjectLinks(IInformationObject rootObject, NameValueCollection objectEntries)
        {
            foreach (var objectKey in objectEntries.AllKeys)
            {
                string objectInfo = objectKey.Substring(7); // Substring("Object_".Length);
                int firstIX = objectInfo.IndexOf('_');
                if (firstIX < 0)
                    throw new InvalidDataException("Invalid field data on binary content");
                string containerID = objectInfo.Substring(0, firstIX);
                string containerField = objectInfo.Substring(firstIX + 1);
                string objectIDCommaSeparated = objectEntries[objectKey] ?? "";
                string[] objectIDList = objectIDCommaSeparated.Split(',');
                rootObject.SetObjectContent(containerID, containerField, objectIDList);
            }
        }

        private static void SetBinaryContent(IInformationObject rootObject, NameValueCollection fileEntries, HttpFileCollection fileContent, IContainerOwner containerOwner)
        {
            foreach (string fileKey in fileEntries.AllKeys)
            {
                HttpPostedFile postedFile = null;
                if (fileContent.AllKeys.Contains(fileKey))
                {
                    postedFile = fileContent[fileKey];
                }
                //if (String.IsNullOrWhiteSpace(postedFile.FileName))
                //    continue;
                string contentInfo = fileKey.Substring(5); // Substring("File_".Length);
                SetBinaryContent(containerOwner, contentInfo, rootObject, postedFile);
            }
        }

        private static void SetFieldValues(IInformationObject rootObject, NameValueCollection fieldEntries)
        {
            rootObject.SetValuesToObjects(fieldEntries);
        }

        private static void retrieveDataSourceInfo(string sourceInfo, out string relativeLocation, out string oldETag)
        {
            string[] infoParts = sourceInfo.Split(':');
            relativeLocation = infoParts[0];
            oldETag = infoParts[1];
        }

        private static void SetBinaryContent(IContainerOwner containerOwner, string contentInfo, IInformationObject rootObject,
                                HttpPostedFile postedFile)
        {
            int firstIX = contentInfo.IndexOf('_');
            if (firstIX < 0)
                throw new InvalidDataException("Invalid field data on binary content");
            string containerID = contentInfo.Substring(0, firstIX);
            string containerField = contentInfo.Substring(firstIX + 1);
            rootObject.SetMediaContent(containerOwner, containerID, containerField, postedFile);
        }


        
    }
}