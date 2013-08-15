using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Security;
using System.Web;
using System.Linq;

namespace TheBall.CORE
{
    public class CreateSpecifiedInformationObjectWithValuesImplementation
    {
        public static IInformationObject GetTarget_CreatedObject(IContainerOwner owner, string objectDomainName, string objectName)
        {
            string objectTypeName = objectDomainName + "." + objectName;
            Type objectType = Type.GetType(objectTypeName);
            IInformationObject iObj = (IInformationObject) Activator.CreateInstance(objectType);
            var relativeLocation = StorageSupport.GetBlobOwnerAddress(owner, objectDomainName + "/" + objectName + "/" + iObj.ID);
            iObj.RelativeLocation = relativeLocation;
            return iObj;
        }

        public static NameValueCollection GetTarget_FieldValues(NameValueCollection httpFormData, IInformationObject createdObject)
        {
            NameValueCollection fieldEntries = new NameValueCollection();
            string objectID = createdObject.ID;
            foreach (var key in httpFormData.AllKeys)
            {
                var value = httpFormData[key];
                if (key.StartsWith("File_") == false && key.StartsWith("Object_") == false)
                    fieldEntries.Add(objectID + "_" + key, value);
            }
            return fieldEntries;
        }

        public static NameValueCollection GetTarget_ObjectLinkValues(NameValueCollection httpFormData, IInformationObject createdObject)
        {
            NameValueCollection objectEntries = new NameValueCollection();
            string objectID = createdObject.ID;
            foreach (var key in httpFormData.AllKeys)
            {
                var value = httpFormData[key];
                if (key.StartsWith("Object_"))
                    objectEntries.Add(key.Replace("Object_", "Object_" + objectID + "_"), value);
            }
            return objectEntries;
        }

        public static Dictionary<string, HttpPostedFile> GetTarget_BinaryContentFiles(NameValueCollection httpFormData, HttpFileCollection httpFileData, IInformationObject createdObject)
        {
            Dictionary<string, HttpPostedFile> resultDict = new Dictionary<string, HttpPostedFile>(httpFileData.Count);
            string objectID = createdObject.ID;

            foreach (var key in httpFormData.AllKeys)
            {
                var value = httpFormData[key];
                if (key.StartsWith("File_"))
                {
                    HttpPostedFile httpFile = null;
                    if (httpFileData.AllKeys.Contains(key))
                        httpFile = httpFileData[key];
                    string dictKey = key.Replace("File_", "File_" + objectID + "_");
                    resultDict.Add(dictKey, httpFile);
                }
            }
            foreach (var key in httpFileData.AllKeys)
            {
                if (key.StartsWith("File_"))
                {
                    string dictKey = key.Replace("File_", "File_" + objectID + "_");
                    if (resultDict.ContainsKey(dictKey) == false)
                        resultDict.Add(dictKey, httpFileData[key]);
                }
            }
            return resultDict;
        }


        public static void ExecuteMethod_SetFieldValues(IInformationObject createdObject, NameValueCollection fieldValues)
        {
            ModifyInformationSupport.SetFieldValues(createdObject, fieldValues);
        }

        public static void ExecuteMethod_SetObjectLinks(IInformationObject createdObject, NameValueCollection objectLinkValues)
        {
            ModifyInformationSupport.SetObjectLinks(createdObject, objectLinkValues);
        }

        public static void ExecuteMethod_StoreInitialObject(IContainerOwner owner, IInformationObject createdObject)
        {
            createdObject.StoreInformationMasterFirst(owner, true);
        }

        public static void ExecuteMethod_SetBinaryContent(IContainerOwner owner, IInformationObject createdObject, Dictionary<string, HttpPostedFile> binaryContentFiles)
        {
            foreach (var fileKey in binaryContentFiles.Keys)
            {
                string contentInfo = fileKey.Substring(5); // Substring("File_".Length);
                ModifyInformationSupport.SetBinaryContent(owner, contentInfo, createdObject,
                                                          binaryContentFiles[fileKey]);
            }
        }

        public static void ExecuteMethod_StoreCompleteObject(IContainerOwner owner, IInformationObject createdObject)
        {
            createdObject.StoreInformationMasterFirst(owner, false);
        }

        public static void ExecuteMethod_CatchInvalidDomains(string objectDomainName)
        {
            if (SystemSupport.ReservedDomainNames.Contains(objectDomainName))
                throw new SecurityException("Creation of system namespace objects is not permitted");
        }
    }
}