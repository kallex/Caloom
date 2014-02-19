using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AaltoGlobalImpact.OIP;
using TheBall;
using TheBall.CORE;

namespace TheBall.CORE
{
    public static partial class OwnerInitializer
    {
        public static void InitializeAndConnectMastersAndCollections(this IContainerOwner owner)
        {
            Type myType = typeof(OwnerInitializer);
            var myMethods = myType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
            foreach (var myMethod in myMethods.Where(method => method.Name.StartsWith("DOMAININIT_")))
            {
                myMethod.Invoke(null, new object[] { owner });
            }
            owner.ReconnectMastersAndCollectionsForOwner();
        }
    }

    public static partial class ExtIContainerOwner
    {
        public static string ToParseableString(this IContainerOwner owner)
        {
            return owner.ContainerName + "/" + owner.LocationPrefix;
        }

        public static string PrefixWithOwnerLocation(this IContainerOwner owner, string location)
        {
            string ownerLocationPrefix = owner.ContainerName + "/" + owner.LocationPrefix + "/";
            if (location.StartsWith(ownerLocationPrefix))
                return location;
            return ownerLocationPrefix + location;
        }

        public static bool IsAccountContainer(this IContainerOwner owner)
        {
            return owner.ContainerName == "acc";
        }

        public static bool IsGroupContainer(this IContainerOwner owner)
        {
            return owner.ContainerName == "grp" || owner.ContainerName == "dev";
        }

        public static void ReconnectMastersAndCollectionsForOwner(this IContainerOwner owner)
        {
            //string myLocalAccountID = "0c560c69-c3a7-4363-b125-ba1660d21cf4";
            //string acctLoc = "acc/" + myLocalAccountID + "/";

            string ownerLocation = owner.ContainerName + "/" + owner.LocationPrefix + "/";

            var informationObjects = StorageSupport.CurrActiveContainer.GetInformationObjects(ownerLocation, name => name.Contains("TheBall.CORE/RequestResourceUsage") == false, 
                                                                                              nonMaster =>
                                                                                              nonMaster.
                                                                                                  IsIndependentMaster ==
                                                                                              false && (nonMaster is TBEmailValidation == false)).ToArray();
            foreach (var iObj in informationObjects)
            {
                try
                {
                    iObj.ReconnectMastersAndCollections(true);
                }
                catch (Exception ex)
                {
                    bool ignoreException = false;
                    if (ignoreException == false)
                        throw;
                }
            }
        }

    }
}