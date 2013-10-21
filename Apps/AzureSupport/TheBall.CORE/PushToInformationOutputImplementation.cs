using System.Security;

namespace TheBall.CORE
{
    public class PushToInformationOutputImplementation
    {
        public static InformationOutput GetTarget_InformationOutput(IContainerOwner owner, string informationOutputId)
        {
            return InformationOutput.RetrieveFromOwnerContent(owner, informationOutputId);
        }

        public static void ExecuteMethod_VerifyValidOutput(InformationOutput informationOutput)
        {
            if (informationOutput.IsValidatedAndActive == false)
                throw new SecurityException("InformationOutput is not active");
        }

        public static string GetTarget_DestinationURL(InformationOutput informationOutput)
        {
            return informationOutput.DestinationURL;
        }

        public static string GetTarget_LocalContentURL(InformationOutput informationOutput)
        {
            return informationOutput.LocalContentURL;
        }

        public static AuthenticatedAsActiveDevice GetTarget_AuthenticatedAsActiveDevice(InformationOutput informationOutput)
        {
            var authenticationID = informationOutput.AuthenticatedDeviceID;
            if (string.IsNullOrEmpty(authenticationID))
                return null;
            VirtualOwner owner = VirtualOwner.FigureOwner(informationOutput);
            return AuthenticatedAsActiveDevice.RetrieveFromOwnerContent(owner, authenticationID);
        }

        public static void ExecuteMethod_PushToInformationOutput(IContainerOwner owner, InformationOutput informationOutput, string destinationUrl, string destinationContentName, string localContentUrl, AuthenticatedAsActiveDevice authenticatedAsActiveDevice)
        {
            // TODO: Information push
        }

        public static string GetTarget_DestinationContentName(InformationOutput informationOutput)
        {
            string destinationContentName = informationOutput.DestinationContentName;
            if (string.IsNullOrEmpty(destinationContentName))
                return "bulkdump.all";
            return informationOutput.DestinationContentName;
        }
    }
}