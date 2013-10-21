namespace TheBall.CORE
{
    public class SetInformationOutputValidationAndActiveStatusImplementation
    {
        public static InformationOutput GetTarget_InformationOutput(IContainerOwner owner, string informationOutputId)
        {
            return InformationOutput.RetrieveFromOwnerContent(owner, informationOutputId);
        }

        public static void ExecuteMethod_SetInputValidAndActiveValue(bool isValidAndActive, InformationOutput informationOutput)
        {
            informationOutput.IsValidatedAndActive = isValidAndActive;
        }

        public static void ExecuteMethod_StoreObject(InformationOutput informationOutput)
        {
            informationOutput.StoreInformation();
        }
    }
}