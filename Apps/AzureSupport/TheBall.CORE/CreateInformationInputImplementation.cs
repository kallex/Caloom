namespace TheBall.CORE
{
    public class CreateInformationInputImplementation
    {
        public static InformationInput GetTarget_CreatedInformationInput(IContainerOwner owner, string inputDescription, string locationUrl, string localContentName)
        {
            InformationInput informationInput = new InformationInput();
            informationInput.SetLocationAsOwnerContent(owner, informationInput.ID);
            informationInput.InputDescription = inputDescription;
            informationInput.LocationURL = locationUrl;
            informationInput.LocalContentName = localContentName;
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