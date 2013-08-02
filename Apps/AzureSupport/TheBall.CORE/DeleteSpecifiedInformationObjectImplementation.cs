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
    }
}