using System;
using System.IO;
using System.Linq;
using System.Web;
using TheBall;
using TheBall.CORE;

namespace AaltoGlobalImpact.OIP
{
    partial class AddImageGroupInfo : IAddOperationProvider
    {
        public bool PerformAddOperation(string commandName, InformationSourceCollection sources, string requesterLocation, HttpFileCollection files)
        {
            if(ImageGroupTitle == "")
                throw new InvalidDataException("Image group title is mandatory");
            throw new NotSupportedException();
        }
    }

    partial class AddAddressAndLocationInfo : IAddOperationProvider
    {
        public bool PerformAddOperation(string commandName, InformationSourceCollection sources, string requesterLocation, HttpFileCollection files)
        {
            if (LocationName == "")
                throw new InvalidDataException("Location name is mandatory");
            throw new NotSupportedException();
        }
    }
}