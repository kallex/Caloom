namespace AaltoGlobalImpact.OIP
{
    public interface IPublisher
    {
        void RegisterAndPublishToSubscribersOnAdd();
        void UnregisterAndPublishToSubscibersOnDelete();
        void PublishInformationObjectAdd();
        void PublishInformationObjectChange();
        void PublishInformationItemChange(string itemName);
        void PublishInformationObjectDelete();
    }
}