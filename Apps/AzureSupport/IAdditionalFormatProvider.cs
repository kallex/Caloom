namespace AaltoGlobalImpact.OIP
{
    public interface IAdditionalFormatProvider
    {
        AdditionalFormatContent[] GetAdditionalContentToStore();
        string[] GetAdditionalFormatExtensions();
    }
}