using System.Security;

namespace TheBall.CORE
{
    public class ProcessFetchedInputsImplementation
    {
        public static InformationInput GetTarget_InformationInput(IContainerOwner owner, string informationInputId)
        {
            return InformationInput.RetrieveFromDefaultLocation(informationInputId, owner);
        }

        public static void ExecuteMethod_VerifyValidInput(InformationInput informationInput)
        {
            if (informationInput.IsValidatedAndActive == false)
                throw new SecurityException("InformationInput is not active");
        }

        public static string GetTarget_InputFetchLocation(InformationInput informationInput)
        {
            return informationInput.RelativeLocation + "_Input";
        }

        public static ProcessFetchedInputs.ProcessInputFromStorageReturnValue ExecuteMethod_ProcessInputFromStorage(string processingOperationName, InformationInput informationInput, string inputFetchLocation)
        {
            var result = new ProcessFetchedInputs.ProcessInputFromStorageReturnValue();
            // TODO: Processing
            return result;
        }

        public static void ExecuteMethod_StoreObjects(IInformationObject[] processingResultsToStore)
        {
            if (processingResultsToStore == null)
                return;
            foreach (var iObj in processingResultsToStore)
                iObj.StoreInformation();
        }

        public static void ExecuteMethod_DeleteObjects(IInformationObject[] processingResultsToDelete)
        {
            if (processingResultsToDelete == null)
                return;
            foreach(var iObj in processingResultsToDelete)
                iObj.DeleteInformationObject();
        }
    }
}