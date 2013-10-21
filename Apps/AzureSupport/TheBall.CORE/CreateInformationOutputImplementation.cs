namespace TheBall.CORE
{
    public class CreateInformationOutputImplementation
    {
        public static InformationOutput GetTarget_CreatedInformationOutput(IContainerOwner owner, string outputDescription, string destinationUrl, string destinationContentName, string localContentUrl, string authenticatedDeviceId)
        {
            InformationOutput informationOutput = new InformationOutput();
            informationOutput.SetLocationAsOwnerContent(owner, informationOutput.ID);
            informationOutput.OutputDescription = outputDescription;
            informationOutput.DestinationURL = destinationUrl;
            informationOutput.DestinationContentName = destinationContentName;
            informationOutput.LocalContentURL = localContentUrl;
            informationOutput.AuthenticatedDeviceID = authenticatedDeviceId;
            return informationOutput;
        }

        public static void ExecuteMethod_StoreObject(InformationOutput createdInformationOutput)
        {
            createdInformationOutput.StoreInformation();
        }

        public static CreateInformationOutputReturnValue Get_ReturnValue(InformationOutput createdInformationOutput)
        {
            return new CreateInformationOutputReturnValue() {InformationOutput = createdInformationOutput};
        }
    }
}