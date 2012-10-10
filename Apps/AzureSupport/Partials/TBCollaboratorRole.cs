namespace AaltoGlobalImpact.OIP
{
    partial class TBCollaboratorRole
    {
        private const string InitiatorRoleValue = "Initiator";
        private const string ModeratorRoleValue = "Moderator";
        public const string CollaboratorRoleValue = "Collaborator";
        public const string ViewerRoleValue = "Viewer";


        private const string RoleStatusInvitedValue = "Invited";
        public const string RoleStatusMemberValue = "Member";

        public static bool HasInitiatorRights(string role)
        {
            return role.ToLower() == InitiatorRoleValue.ToLower();
        }

        public static bool HasModeratorRights(string role)
        {
            return role.ToLower() == ModeratorRoleValue.ToLower() || role.ToLower() == InitiatorRoleValue.ToLower(); 
        }

        public static bool HasCollaboratorRights(string role)
        {
            return role.ToLower() == ModeratorRoleValue.ToLower() || role.ToLower() == InitiatorRoleValue.ToLower() || role.ToLower() == CollaboratorRoleValue.ToLower();
        }

        public static bool HasViewerRights(string role)
        {
            return role.ToLower() == ModeratorRoleValue.ToLower() || role.ToLower() == InitiatorRoleValue.ToLower() || role.ToLower() == CollaboratorRoleValue.ToLower() ||
                   role.ToLower() == ViewerRoleValue.ToLower();

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