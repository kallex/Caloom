using System.Linq;
using TheBall;
using TheBall.CORE;

namespace AaltoGlobalImpact.OIP
{
    public class UpdateGroupInformationChangeToMembersImplementation
    {
        public static GroupContainer GetTarget_GroupContainer(string groupId)
        {
            VirtualOwner owner = new VirtualOwner("grp", groupId);
            return GroupContainer.RetrieveFromOwnerContent(owner, "default");
        }

        public static string[] GetTarget_AccountIDs(GroupContainer groupContainer)
        {
            return
                groupContainer.Collaborators.CollectionContent.Select(collaborator => collaborator.AccountID).ToArray();
        }

        public static void ExecuteMethod_UpdateAccountGroupSummaryContainers(string groupId, GroupContainer groupContainer, string[] accountIDs)
        {
            foreach (string accountID in accountIDs)
            {
                int retryCount = 3;
                VirtualOwner accountOwner = new VirtualOwner("acc", accountID);
                while (retryCount-- > 0)
                {
                    try
                    {
                        GroupSummaryContainer summaryContainer =
                            GroupSummaryContainer.RetrieveFromOwnerContent(accountOwner, "default");
                        var groupToUpdate =
                            summaryContainer.GroupCollection.CollectionContent.FirstOrDefault(grp => grp.ID == groupId);
                        if (groupToUpdate != null)
                        {
                            summaryContainer.GroupCollection.CollectionContent.Remove(groupToUpdate);
                            summaryContainer.GroupCollection.CollectionContent.Add(groupContainer.GroupProfile);
                            summaryContainer.StoreInformation(accountOwner);
                        }
                        break; // break while
                    }
                    catch
                    {
                        
                    }
                }
            }
        }
    }
}