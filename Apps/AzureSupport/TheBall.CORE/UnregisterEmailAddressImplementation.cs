using System.Linq;
using AaltoGlobalImpact.OIP;

namespace TheBall.CORE
{
    public class UnregisterEmailAddressImplementation
    {
        public static AccountContainer GetTarget_AccountContainerBeforeGroupRemoval(string accountId)
        {
            VirtualOwner owner = VirtualOwner.FigureOwner("acc/" + accountId);
            return AccountContainer.RetrieveFromOwnerContent(owner, "default");
        }

        public static string GetTarget_EmailAddressID(string emailAddress, AccountContainer accountContainerBeforeGroupRemoval)
        {
            return
                accountContainerBeforeGroupRemoval.AccountModule.Security.EmailCollection.CollectionContent.First(
                    email => email.EmailAddress == emailAddress).ID;
        }

        public static void ExecuteMethod_ExecuteUnlinkEmailAddress(string accountId, AccountContainer accountContainerBeforeGroupRemoval, string emailAddressId)
        {
            UnlinkEmailAddressParameters parameters = new UnlinkEmailAddressParameters
                {
                    AccountID = accountId,
                    AccountContainerBeforeGroupRemoval = accountContainerBeforeGroupRemoval,
                    EmailAddressID = emailAddressId
                };
            UnlinkEmailAddress.Execute(parameters);
        }
    }
}