namespace TheBall.CORE
{
    public class SetInformationInputValidationAndActiveStatusImplementation
    {
        public static InformationInput GetTarget_InformationInput(IContainerOwner owner, string informationInputId)
        {
            return InformationInput.RetrieveFromDefaultLocation(informationInputId, owner);
        }

        public static void ExecuteMethod_SetInputValidAndActiveValue(bool isValidAndActive, InformationInput informationInput)
        {
            informationInput.IsValidatedAndActive = isValidAndActive;
        }

        public static void ExecuteMethod_StoreObject(InformationInput informationInput)
        {
            informationInput.StoreInformation();
        }
    }
}