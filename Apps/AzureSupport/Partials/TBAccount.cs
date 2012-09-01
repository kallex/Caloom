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
            foreach(var loginItem in this.Logins.CollectionContent)
            {
                string loginRootID = TBLoginInfo.GetLoginIDFromLoginURL(loginItem.OpenIDUrl);
                TBRLoginRoot loginRoot = TBRLoginRoot.RetrieveFromDefaultLocation(loginRootID);
                loginRoot.Account = this;
                StorageSupport.StoreInformation(loginRoot);
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
        }
    }
}
