using System;
using System.IO;

namespace TheBall.CORE
{
    public class ExportOwnerContentToZipImplementation
    {
        public static string[] GetTarget_IncludedFolders(IContainerOwner owner, string packageRootFolder)
        {
            string[] directories = StorageSupport.GetLogicalDirectories(owner, packageRootFolder);
            directories = SystemSupport.FilterAwayReservedFolders(directories);
            return directories;
        }

        public static ContentPackage PackageOwnerContentToZip_GetOutput(PackageOwnerContentReturnValue operationReturnValue, IContainerOwner owner, string packageRootFolder, string[] includedFolders)
        {
            return operationReturnValue.ContentPackage;
        }

        public static PackageOwnerContentParameters PackageOwnerContentToZip_GetParameters(IContainerOwner owner, string packageRootFolder, string[] includedFolders)
        {
            return new PackageOwnerContentParameters
                {
                    Owner = owner,
                    PackageName = "Full export",
                    Description = "Full export done by ExportOwnerContentToZip",
                    PackageType = "FULLEXPORT",
                    IncludedFolders = includedFolders,
                    PackageRootFolder = packageRootFolder
                };
        }

        public static ExportOwnerContentToZipReturnValue Get_ReturnValue(ContentPackage packageOwnerContentToZipOutput)
        {
            return new ExportOwnerContentToZipReturnValue {ContentPackageID = packageOwnerContentToZipOutput.ID};
        }
    }
}

