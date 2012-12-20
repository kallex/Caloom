namespace TheBall.CORE
{
    public interface IContainerOwner
    {
        string ContainerName { get; }
        string LocationPrefix { get; }
    }

    public static class ExtIContainerOwner
    {
        public static bool IsAccountContainer(this IContainerOwner owner)
        {
            return owner.ContainerName == "acc";
        }

        public static bool IsGroupContainer(this IContainerOwner owner)
        {
            return owner.ContainerName == "grp" || owner.ContainerName == "dev";
        }
    }
}