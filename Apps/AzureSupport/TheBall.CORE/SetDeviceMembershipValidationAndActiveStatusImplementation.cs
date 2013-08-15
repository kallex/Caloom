namespace TheBall.CORE
{
    public class SetDeviceMembershipValidationAndActiveStatusImplementation
    {
        public static DeviceMembership GetTarget_DeviceMembership(IContainerOwner owner, string deviceMembershipId)
        {
            return DeviceMembership.RetrieveFromDefaultLocation(deviceMembershipId, owner);
        }

        public static void ExecuteMethod_SetDeviceValidAndActiveValue(bool isValidAndActive, DeviceMembership deviceMembership)
        {
            deviceMembership.IsValidatedAndActive = isValidAndActive;
        }

        public static void ExecuteMethod_StoreObject(DeviceMembership deviceMembership)
        {
            deviceMembership.StoreInformation();
        }
    }
}