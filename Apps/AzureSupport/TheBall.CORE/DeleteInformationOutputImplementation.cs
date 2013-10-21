namespace TheBall.CORE
{
    public class DeleteInformationOutputImplementation
    {
        public static InformationOutput GetTarget_InformationOutput(IContainerOwner owner, string informationOutputId)
        {
            return InformationOutput.RetrieveFromOwnerContent(owner, informationOutputId);
        }

        public static void ExecuteMethod_DeleteInformationOutput(InformationOutput informationOutput)
        {
            informationOutput.DeleteInformationObject();
        }
    }
}