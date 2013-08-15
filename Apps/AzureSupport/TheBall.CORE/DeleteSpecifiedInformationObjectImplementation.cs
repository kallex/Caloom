using System;
using System.Security;
using System.Linq;

namespace TheBall.CORE
{
    public class DeleteSpecifiedInformationObjectImplementation
    {
        public static IInformationObject GetTarget_ObjectToDelete(IContainerOwner owner, string objectDomainName, string objectName, string objectId)
        {
            IInformationObject objectToDelete =
                StorageSupport.RetrieveInformationObjectFromDefaultLocation(objectDomainName, objectName, objectId,
                                                                            owner);
            return objectToDelete;
        }

        public static void ExecuteMethod_DeleteObject(IInformationObject objectToDelete)
        {
            if(objectToDelete != null)
                objectToDelete.DeleteInformationObject();
        }

        public static void ExecuteMethod_CatchInvalidDomains(string objectDomainName)
        {
            if (SystemSupport.ReservedDomainNames.Contains(objectDomainName))
                throw new SecurityException("Creation of system namespace objects is not permitted");
        }
    }
}