namespace AaltoGlobalImpact.OIP
{
    public interface IContainerOwner
    {
        string ContainerName { get; }
        string LocationPrefix { get; }
    }
}