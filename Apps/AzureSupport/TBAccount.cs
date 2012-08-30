using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AaltoGlobalImpact.OIP
{
    partial class TBAccount : IContainerOwner
    {
        public string ContainerName
        {
            get { return "acc"; }
        }

        public string LocationPrefix
        {
            get { return this.ID; }
        }
    }
}
