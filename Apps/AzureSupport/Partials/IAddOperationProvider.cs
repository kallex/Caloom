namespace AaltoGlobalImpact.OIP
{
    public interface IAddOperationProvider
    {
        bool PerformAddOperation(InformationSourceCollection sources, string requesterLocation);
    }
}