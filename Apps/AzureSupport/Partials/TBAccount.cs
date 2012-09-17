using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TheBall;

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

        public TBEmail GetAccountEmail(string emailAddress)
        {
            TBEmail result = Emails.CollectionContent.FirstOrDefault(candidate => candidate.EmailAddress == emailAddress);
            if(result == null)
                throw new InvalidDataException("Account does not contain email: " + emailAddress);
            return result;
        }


        public void JoinGroup(TBCollaboratingGroup collaboratingGroup, string role)
        {
            if (this.GroupRoleCollection.CollectionContent.Find(member => member.GroupID == collaboratingGroup.ID) != null)
                return;
            this.GroupRoleCollection.CollectionContent.Add(new TBAccountCollaborationGroup()
                                                               {
                                                                   GroupID = collaboratingGroup.ID,
                                                                   GroupRole = role
                                                               });
        }

        public void StoreAndPropagate()
        {
            TBRAccountRoot accountRoot = TBRAccountRoot.RetrieveFromDefaultLocation(this.ID);
            accountRoot.Account = this;
            StorageSupport.StoreInformation(accountRoot);
            AccountContainer accountContainer = AccountContainer.RetrieveFromOwnerContent(this, "default");
            if(accountContainer == null)
            {
                accountContainer = AccountContainer.CreateDefault();
                accountContainer.SetLocationAsOwnerContent(this, "default");
            }
            accountContainer.AccountModule.Security.LoginInfoCollection = this.Logins;
            foreach(var loginItem in this.Logins.CollectionContent)
            {
                string loginRootID = TBLoginInfo.GetLoginIDFromLoginURL(loginItem.OpenIDUrl);
                TBRLoginRoot loginRoot = TBRLoginRoot.RetrieveFromDefaultLocation(loginRootID);
                loginRoot.Account = this;
                StorageSupport.StoreInformation(loginRoot);
                // TODO: Remove invalid group role logins at this stage
                foreach(var groupRoleItem in this.GroupRoleCollection.CollectionContent)
                {
                    string loginGroupID = TBRLoginGroupRoot.GetLoginGroupID(groupRoleItem.GroupID, loginRootID);
                    TBRLoginGroupRoot loginGroupRoot = TBRLoginGroupRoot.RetrieveFromDefaultLocation(loginGroupID);
                    if(loginGroupRoot == null)
                    {
                        loginGroupRoot = TBRLoginGroupRoot.CreateDefault();
                        loginGroupRoot.ID = loginGroupID;
                        loginGroupRoot.UpdateRelativeLocationFromID();
                    }
                    loginGroupRoot.GroupID = groupRoleItem.GroupID;
                    loginGroupRoot.Role = groupRoleItem.GroupRole;
                    StorageSupport.StoreInformation(loginGroupRoot);
                }
            }
            accountContainer.AccountModule.Security.EmailCollection = this.Emails;
            foreach(var emailItem in this.Emails.CollectionContent)
            {
                string emailRootID = TBREmailRoot.GetIDFromEmailAddress(emailItem.EmailAddress);
                TBREmailRoot emailRoot = TBREmailRoot.RetrieveFromDefaultLocation(emailRootID);
                if(emailRoot == null)
                {
                    emailRoot = TBREmailRoot.CreateDefault();
                    emailRoot.ID = emailRootID;
                    emailRoot.UpdateRelativeLocationFromID();
                }
                emailRoot.Account = this;
                StorageSupport.StoreInformation(emailRoot);
            }
            var roles = accountContainer.AccountModule.Roles;
            roles.MemberInGroups.CollectionContent.Clear();
            roles.ModeratorInGroups.CollectionContent.Clear();
            foreach(var groupRoleItem in this.GroupRoleCollection.CollectionContent)
            {
                var groupRoot = TBRGroupRoot.RetrieveFromDefaultLocation(groupRoleItem.GroupID);
                if (groupRoot == null)
                    continue;
                var grp = groupRoot.Group;
                ReferenceToInformation reference = ReferenceToInformation.CreateDefault();
                reference.URL = string.Format("/auth/grp/{0}/website/oip-group/oip-layout-groups-edit.phtml",
                                              groupRoot.ID);
                reference.Title = grp.Title + " - " + groupRoleItem.GroupRole;
                switch(groupRoleItem.GroupRole)
                {
                    case "initiator":
                    case "moderator":
                        roles.ModeratorInGroups.CollectionContent.Add(reference);
                        break;
                    case "collaborator":
                    case "viewer":
                        roles.MemberInGroups.CollectionContent.Add(reference);
                        break;
                }
            }
            StorageSupport.StoreInformation(accountContainer);
        }
    }
}
