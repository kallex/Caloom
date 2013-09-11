namespace TheBall.CORE
{
    public class ProcessFetchedInputsImplementation
    {
        public static InformationInput GetTarget_InformationInput(IContainerOwner owner, string informationInputId)
        {
            throw new System.NotImplementedException();
        }

        public static void ExecuteMethod_VerifyValidInput(InformationInput informationInput)
        {
            throw new System.NotImplementedException();
        }

        public static string GetTarget_InputFetchLocation(InformationInput informationInput)
        {
            throw new System.NotImplementedException();
        }

        public static ProcessFetchedInputs.ProcessInputFromStorageReturnValue ExecuteMethod_ProcessInputFromStorage(string processingOperationName, InformationInput informationInput, string inputFetchLocation)
        {
            throw new System.NotImplementedException();
        }

        public static void ExecuteMethod_StoreObjects(IInformationObject[] processingResultsToStore)
        {
            foreach (var iObj in processingResultsToStore)
                iObj.StoreInformation();
        }

        public static void ExecuteMethod_DeleteObjects(IInformationObject[] processingResultsToDelete)
        {
            foreach(var iObj in processingResultsToDelete)
                iObj.DeleteInformationObject();
        }
    }
}