namespace TheBall.CORE
{
    partial class SemanticInformationItem
    {
        public SemanticInformationItem(IInformationObject informationObject)
        {
            ItemFullType = informationObject.GetType().FullName;
            ItemValue = informationObject.RelativeLocation;
        }
    }
}