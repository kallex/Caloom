using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security;
using System.Web;
using System.Linq;

namespace TheBall.CORE
{
    public class CreateSpecifiedInformationObjectWithValuesImplementation
    {
        public static void ExecuteMethod_CatchInvalidDomains(string objectDomainName)
        {
            if (SystemSupport.ReservedDomainNames.Contains(objectDomainName))
                throw new SecurityException("Creation of system namespace objects is not permitted");
        }

        public static IInformationObject GetTarget_CreatedObject(IContainerOwner owner, string objectDomainName, string objectName)
        {
            string objectTypeName = objectDomainName + "." + objectName;
            Type objectType = Type.GetType(objectTypeName);
            IInformationObject iObj = (IInformationObject) Activator.CreateInstance(objectType);
            var relativeLocation = StorageSupport.GetOwnerContentLocation(owner, objectDomainName + "/" + objectName + "/" + iObj.ID);
            iObj.RelativeLocation = relativeLocation;
            return iObj;
        }

        public static void ExecuteMethod_StoreInitialObject(IInformationObject createdObject)
        {
            createdObject.StoreInformationMasterFirst(InformationContext.CurrentOwner, true);
        }

        public static SetObjectTreeValuesParameters SetObjectValues_GetParameters(NameValueCollection httpFormData, HttpFileCollection httpFileData, IInformationObject createdObject)
        {
            return new SetObjectTreeValuesParameters
                {
                    RootObject = createdObject,
                    HttpFormData = httpFormData,
                    HttpFileData = httpFileData
                };
        }
    }
}