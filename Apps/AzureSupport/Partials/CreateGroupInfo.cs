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
            //throw new NotImplementedException("Old implementation not converted to managed group structures");
            //AccountContainer container = (AccountContainer) sources.GetDefaultSource(typeof(AccountContainer).FullName).RetrieveInformationObject();
            VirtualOwner owner = VirtualOwner.FigureOwner(this);
            if(owner.ContainerName != "acc")
                throw new NotSupportedException("Group creation only supported from account");
            string accountID = owner.LocationPrefix;
            TBRAccountRoot accountRoot = TBRAccountRoot.RetrieveFromDefaultLocation(accountID);
            TBAccount account = accountRoot.Account;
            if (account.Emails.CollectionContent.Count == 0)
                throw new InvalidDataException("Account needs to have at least one email address to create a group");
            CreateGroup.Execute(new CreateGroupParameters { AccountID = accountID, GroupName = this.GroupName });
            this.GroupName = "";
            return true;
        }
    }
}