using System;

namespace AaltoGlobalImpact.OIP
{
    partial class Group
    {
        public static Comparison<Group> CompareByGroupName = NameComparer;

        private static int NameComparer(Group x, Group y)
        {
            return String.Compare(x.GroupName, y.GroupName);
        }

        public void UpdateReferenceToInformation(string groupRootID)
        {
            ReferenceToInformation.URL = string.Format("/auth/grp/{0}/website/oip-group/oip-layout-groups-edit.phtml",
                                          groupRootID);
            ReferenceToInformation.Title = GroupName;
        }
    }
}