using System;
using System.Collections.Specialized;

namespace AaltoGlobalImpact.OIP
{
    public static class PerformWebActionImplementation
    {
        public static bool ExecuteMethod_ExecuteActualOperation(string targetObjectID, string commandName, IContainerOwner owner, InformationSourceCollection informationSources, string[] formSourceNames, NameValueCollection formSubmitContent)
        {
            switch(commandName)
            {
                case "RemoveCollaborator":
                    return CallRemoveGroupMember(targetObjectID, owner);
                case "PublishGroupPublicContent":
                    return CallPublishGroupContentToPublicArea(owner);
                case "PublishGroupWwwContent":
                    return CallPublishGroupContentToWww(owner);
                case "AssignCollaboratorRole":
                    return CallAssignCollaboratorRole(targetObjectID, owner, informationSources.GetDefaultSource(typeof(GroupContainer).FullName) ,formSubmitContent);
                case "UnlinkEmailAddress":
                    return CallUnlinkEmailAddress(targetObjectID, owner,
                                                  informationSources.GetDefaultSource(typeof (AccountContainer).FullName));
                default:
                    throw new NotImplementedException("Operation mapping for command not implemented: " + commandName);
            }
        }

        private static bool CallUnlinkEmailAddress(string targetObjectID, IContainerOwner owner, InformationSource accountContainerSource)
        {
            if(accountContainerSource == null)
                throw new ArgumentNullException("accountContainerSource");
            AccountContainer accountContainer = (AccountContainer) accountContainerSource.RetrieveInformationObject();
            string accountID = owner.LocationPrefix;
            string emailID = targetObjectID;
            UnlinkEmailAddress.Execute(new UnlinkEmailAddressParameters
                                           {
                                               AccountContainerBeforeGroupRemoval = accountContainer,
                                               AccountID = accountID,
                                               EmailAddressID = emailID
                                           });
            return true;
        }

        private static bool CallAssignCollaboratorRole(string targetObjectID, IContainerOwner owner, InformationSource groupContainerSource, NameValueCollection formSubmitContent)
        {
            if(groupContainerSource == null)
                throw new ArgumentNullException("groupContainerSource");
            GroupContainer groupContainer = (GroupContainer) groupContainerSource.RetrieveInformationObject();
            string roleToAssign = formSubmitContent["AssignRoleToCollaborator"];
            string groupID = owner.LocationPrefix;
            string collaboratorID = targetObjectID;
            AssignCollaboratorRole.Execute(new AssignCollaboratorRoleParameters
                                               {
                                                   CollaboratorID = collaboratorID,
                                                   GroupContainer = groupContainer,
                                                   GroupID = groupID,
                                                   RoleToAssign = roleToAssign
                                               });
            return true;
        }

        private static bool CallPublishGroupContentToWww(IContainerOwner owner)
        {
            string groupID = owner.LocationPrefix;
            PublishGroupContentToWww.Execute(new PublishGroupContentToWwwParameters { GroupID = groupID, UseWorker = true });
            return false;
        }

        private static bool CallPublishGroupContentToPublicArea(IContainerOwner owner)
        {
            string groupID = owner.LocationPrefix;
            PublishGroupContentToPublicArea.Execute(new PublishGroupContentToPublicAreaParameters
                                                        {GroupID = groupID, UseWorker = true});
            return false;
        }

        private static bool CallRemoveGroupMember(string targetObjectID, IContainerOwner owner)
        {
            if(String.IsNullOrEmpty(targetObjectID))
                throw new ArgumentException("AccountID must be given for remove group member", "targetObjectID");
            string accountID = targetObjectID;
            string groupID = owner.LocationPrefix;
            RemoveMemberFromGroup.Execute(new RemoveMemberFromGroupParameters {AccountID = accountID, GroupID = groupID});
            return true;
        }

        public static PerformWebActionReturnValue Get_ReturnValue(bool executeActualOperationOutput)
        {
            return new PerformWebActionReturnValue() {RenderPageAfterOperation = executeActualOperationOutput};
        }
    }
}