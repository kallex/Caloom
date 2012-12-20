using System;
using System.Collections.Generic;
using System.Linq;
using TheBall;
using TheBall.CORE;

namespace AaltoGlobalImpact.OIP
{
    public static class ExtIContainerOwner
    {
        public static void ReconnectMastersAndCollectionsForOwner(this IContainerOwner owner)
        {
            //string myLocalAccountID = "0c560c69-c3a7-4363-b125-ba1660d21cf4";
            //string acctLoc = "acc/" + myLocalAccountID + "/";

            string ownerLocation = owner.ContainerName + "/" + owner.LocationPrefix + "/";

            var informationObjects = StorageSupport.CurrActiveContainer.GetInformationObjects(ownerLocation,
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