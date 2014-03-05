using System;
using TheBall.CORE;

namespace TheBall
{
    public class InformationObjectSupport
    {
        public static string GetInformationObjectType(string contentFullPath)
        {
            if (contentFullPath.StartsWith("acc/") || contentFullPath.StartsWith("grp/") || contentFullPath.StartsWith(SystemSupport.SystemOwnerRoot))
                return figureOwnedObjectType(contentFullPath);
            return figureRootedObjectType(contentFullPath);
        }

        private static string figureRootedObjectType(string contentFullPath)
        {
            int firstIX = contentFullPath.IndexOf('/');
            if(firstIX < 0)
                throw new ArgumentException("Invalid object full path, missing two / characters");
            int secondIX = contentFullPath.IndexOf('/', firstIX + 1);
            if (secondIX < 0)
                throw new ArgumentException("Invalid object full path, missing two / characters");
            return constructTypeName(contentFullPath, firstIX, secondIX);
        }

        private static string constructTypeName(string contentFullPath, int firstIX, int secondIX)
        {
            int firstContentEnd = firstIX;
            int firstContentLength = firstIX;
            int secondContentStart = firstContentEnd + 1;
            int secondContentEnd = secondIX;
            int secondContentLength = secondContentEnd - secondContentStart;
            return contentFullPath.Substring(0, firstContentLength) + "."
                   + contentFullPath.Substring(secondContentStart, secondContentLength);
        }

        private static string figureOwnedObjectType(string contentFullPath)
        {
            int secondIX = contentFullPath.IndexOf('/');
            if (secondIX < 0)
                throw new ArgumentException("Invalid object full path, missing two / characters");
            int firstIX = contentFullPath.LastIndexOf('/', secondIX-1);
            if (firstIX < 0)
                throw new ArgumentException("Invalid object full path, missing two / characters");
            return constructTypeName(contentFullPath, firstIX, secondIX);
        }
    }
}