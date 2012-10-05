namespace AaltoGlobalImpact.OIP
{
    partial class TBCollaboratorRole
    {
        private const string InitiatorRoleValue = "Initiator";
        private const string ModeratorRoleValue = "Moderator";
        private const string CollaboratorRoleValue = "Collaborator";
        public const string ViewerRoleValue = "Viewer";


        private const string RoleStatusInvitedValue = "Invited";
        public const string RoleStatusMemberValue = "Member";

        public static bool HasInitiatorRights(string role)
        {
            return role == InitiatorRoleValue;
        }

        public static bool HasModeratorRights(string role)
        {
            return role == ModeratorRoleValue || role.ToLower() == InitiatorRoleValue.ToLower(); 
        }

        public static bool HasCollaboratorRights(string role)
        {
            return role == ModeratorRoleValue || role == InitiatorRoleValue || role == CollaboratorRoleValue;
        }

        public static bool HasViewerRights(string role)
        {
            return role == ModeratorRoleValue || role == InitiatorRoleValue || role == CollaboratorRoleValue ||
                   role == ViewerRoleValue;

        }

        public static bool IsRoleStatusValidMember(string roleStatus)
        {
            return roleStatus == RoleStatusMemberValue;
        }

        public void SetRoleAsInvited()
        {
            RoleStatus = RoleStatusInvitedValue;
        }

        public void SetRoleAsMember()
        {
            RoleStatus = RoleStatusMemberValue;
        }
    }
}