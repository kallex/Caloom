using System;
using System.Web;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class ReferenceToInformation
    {
        public static Comparison<ReferenceToInformation> CompareByReferenceTitle = TitleComparer;

        private static int TitleComparer(ReferenceToInformation x, ReferenceToInformation y)
        {
            return String.Compare(x.Title, y.Title);
        }
    }
}