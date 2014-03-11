namespace TheBall.CORE
{
    partial class SemanticInformationItem
    {
        public SemanticInformationItem(IInformationObject informationObject)
        {
            ItemFullType = informationObject.GetType().FullName;
            ItemValue = informationObject.RelativeLocation;
        }

        public SemanticInformationItem(string itemFullType, string itemValue)
        {
            ItemFullType = itemFullType;
            ItemValue = itemValue;
        }
    }
}