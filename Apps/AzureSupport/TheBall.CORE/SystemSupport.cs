using System.Linq;

namespace TheBall.CORE
{
    public static class SystemSupport
    {
        public static readonly string[] ReservedDomainNames = new string[] {"TheBall.CORE"};

        public static string[] FilterAwayReservedFolders(string[] directories)
        {
            return directories.Where(dir => ReservedDomainNames.Any(resDom => dir.Contains(resDom) == false)).ToArray();
        }
    }
}