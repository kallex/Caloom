namespace TheBall.Support.DeviceClient
{
    public partial class DeviceOperationData
    {
        public string OperationRequestString { get; set; }
        public string[] OperationParameters { get; set; }
        public string[] OperationReturnValues { get; set; }
        public bool OperationResult { get; set; }
        public ContentItemLocationWithMD5[] OperationSpecificContentData { get; set; }
    }
}