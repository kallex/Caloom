namespace TheBall.Support.DeviceClient
{
    public partial class ContentItemLocationWithMD5
    {
        public string ContentLocation { get; set; }
        public string ContentMD5 { get; set; }
        public ItemData[] ItemDatas { get; set; }
    }
}