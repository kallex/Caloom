namespace TheBall.CORE
{
    public class CreateInformationInputImplementation
    {
        public static InformationInput GetTarget_CreatedInformationInput(IContainerOwner owner, string inputDescription, string locationUrl)
        {
            InformationInput informationInput = new InformationInput();
            informationInput.SetLocationAsOwnerContent(owner, informationInput.ID);
            informationInput.Description = inputDescription;
            informationInput.LocationURL = locationUrl;
            return informationInput;
        }

        public static void ExecuteMethod_StoreObject(InformationInput createdInformationInput)
        {
            createdInformationInput.StoreInformation();
        }

        public static CreateInformationInputReturnValue Get_ReturnValue(InformationInput createdInformationInput)
        {
            return new CreateInformationInputReturnValue {InformationInput = createdInformationInput};
            
        }
    }
}