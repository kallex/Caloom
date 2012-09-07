using System;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class TBCollaboratingGroup : IContainerOwner
    {
        public string ContainerName
        {
            get { return "grp"; }
        }

        public string LocationPrefix
        {
            get { return ID; }
        }


        public void JoinToGroup(string emailAddress, string role)
        {
            if (this.Roles.CollectionContent.Find(member => member.Email.EmailAddress == emailAddress) != null)
                return;
            string emailRootID = TBREmailRoot.GetIDFromEmailAddress(emailAddress);
            TBREmailRoot emailRoot = TBREmailRoot.RetrieveFromDefaultLocation(emailRootID);
            TBAccount account = emailRoot.Account;
            account.JoinGroup(this, role);
            account.StoreAndPropagate();
            TBEmail email = account.GetAccountEmail(emailAddress);
            this.Roles.CollectionContent.Add(new TBCollaboratorRole()
            {
                Email = email,
                Role = role
            });
        }
    }
}