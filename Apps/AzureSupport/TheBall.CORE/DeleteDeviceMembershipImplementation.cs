namespace TheBall.CORE
{
    public class DeleteDeviceMembershipImplementation
    {
        public static DeviceMembership GetTarget_DeviceMembership(IContainerOwner owner, string deviceMembershipId)
        {
            return DeviceMembership.RetrieveFromDefaultLocation(deviceMembershipId, owner);
        }

        public static void ExecuteMethod_DeleteDeviceMembership(DeviceMembership deviceMembership)
        {
            deviceMembership.DeleteInformationObject();
        }
    }
}