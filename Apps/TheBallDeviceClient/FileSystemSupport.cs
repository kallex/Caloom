using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace TheBall.Support.DeviceClient
{
    public static class FileSystemSupport
    {
        public static ContentItemLocationWithMD5[] GetContentRelativeFromRoot(string rootFolder)
        {
            int relativeNameStartingIX = rootFolder.EndsWith("/") ? rootFolder.Length : rootFolder.Length + 1;
            List<ContentItemLocationWithMD5> contentItems = new List<ContentItemLocationWithMD5>();
            DirectoryInfo dirInfo = new DirectoryInfo(rootFolder);
            var fileInfos = dirInfo.GetFiles("*", SearchOption.AllDirectories);
            Console.WriteLine("Getting MD5 for {0} files...", fileInfos.Length);
            int totalTODO = fileInfos.Length;
            int currDone = 0;
            int currDots = 0;
            foreach (var fileInfo in fileInfos)
            {
                ContentItemLocationWithMD5 contentItem = new ContentItemLocationWithMD5
                    {
                        ContentLocation = fileInfo.FullName.Substring(relativeNameStartingIX).Replace('\\','/' ),
                        ContentMD5 = getMD5(fileInfo)
                    };
                contentItems.Add(contentItem);
                currDone++;
                int currProgress = (currDone*10)/totalTODO;
                if (currDots < currProgress)
                {
                    Console.Write(".");
                    currDots++;
                }
            }
            Console.WriteLine(" done.");
            return contentItems.ToArray();
        }

        private static MD5 md5 = new MD5Cng();

        private static string getMD5(FileInfo fileInfo)
        {
            var fileData = File.ReadAllBytes(fileInfo.FullName);
            var md5Hash = md5.ComputeHash(fileData);
            return Convert.ToBase64String(md5Hash);
        }
    }
}
