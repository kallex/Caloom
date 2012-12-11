using System;
using System.Collections.Generic;
using System.IO;

namespace AaltoGlobalImpact.OIP
{
    partial class AccountContainer : IBeforeStoreHandler
    {
        //void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
        //{
        //    if (sourceMaster == null)
        //        throw new ArgumentNullException("sourceMaster");
        //    if (GetType() != sourceMaster.GetType())
        //        throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
        //    IInformationObject iObject = this;
        //    if(iObject.IsIndependentMaster == false)
        //        throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
        //    if(ID != sourceMaster.ID)
        //        throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");

        //}
        public void PerformBeforeStoreUpdate()
        {
            this.AccountIndex.Icon = AccountModule.Profile.ProfileImage;
        }
    }
}