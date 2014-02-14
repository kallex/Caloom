namespace TheBall.CORE
{
    public interface IContainerOwner
    {
        string ContainerName { get; }
        string LocationPrefix { get; }
    }

    public static class IContainerOwnerExt
    {
        public static string GetOwnerPrefix(this IContainerOwner containerOwner)
        {
            return containerOwner.ContainerName + "/" + containerOwner.LocationPrefix;
        }
    }

}