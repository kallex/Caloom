using System;
using System.IO;
using System.Web;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class CreateGroupInfo : IAddOperationProvider
    {
        public bool PerformAddOperation(string commandName, InformationSourceCollection sources, string requesterLocation, HttpFileCollection files)
        {
            if(GroupName == "")
                throw new InvalidDataException("Group name must be given");
            AccountContainer container = (AccountContainer) sources.GetDefaultSource().RetrieveInformationObject();
            TBRAccountRoot accountRoot = TBRAccountRoot.GetOwningAccountRoot(container);
            TBAccount account = accountRoot.Account;
            if(account.Emails.CollectionContent.Count == 0)
                throw new InvalidDataException("Account needs to have at least one email address to create a group");
            TBRGroupRoot groupRoot = TBRGroupRoot.CreateNewWithGroup();
            TBCollaboratingGroup grp = groupRoot.Group;
            grp.Title = GroupName;
            StorageSupport.StoreInformation(groupRoot);
            foreach (var accountEmail in account.Emails.CollectionContent)
                grp.JoinToGroup(accountEmail.EmailAddress, "Initiator");
            StorageSupport.StoreInformation(groupRoot);
            RenderWebSupport.RefreshGroupTemplates(grp.ID, true);
            this.GroupName = "";
            return true;
        }
    }
}