namespace TheBall.CORE
{
    public class CreateDeviceMembershipImplementation
    {
        public static DeviceMembership GetTarget_CreatedDeviceMembership(IContainerOwner owner, string deviceDescription, byte[] activeSymmetricAesKey)
        {
            DeviceMembership deviceMembership = new DeviceMembership();
            deviceMembership.SetLocationAsOwnerContent(owner, deviceMembership.ID);
            deviceMembership.DeviceDescription = deviceDescription;
            deviceMembership.ActiveSymmetricAESKey = activeSymmetricAesKey;
            return deviceMembership;
        }

        public static void ExecuteMethod_StoreObject(DeviceMembership createdDeviceMembership)
        {
            createdDeviceMembership.StoreInformation();
        }

        public static CreateDeviceMembershipReturnValue Get_ReturnValue(DeviceMembership createdDeviceMembership)
        {
            return new CreateDeviceMembershipReturnValue {DeviceMembership = createdDeviceMembership};
        }
    }
}