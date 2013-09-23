namespace AaltoGlobalImpact.OIP
{
    public interface IAdditionalFormatProvider
    {
        AdditionalFormatContent[] GetAdditionalContentToStore(string masterBlobETag);
        string[] GetAdditionalFormatExtensions();
    }
}