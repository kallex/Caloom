 

using System;

		namespace AaltoGlobalImpact.OIP { 
				public class InviteMemberToGroupParameters 
		{
				public string MemberEmailAddress ;
				public string GroupID ;
				}
		
		public class InviteMemberToGroup 
		{
				private static void PrepareParameters(InviteMemberToGroupParameters parameters)
		{
					}
				public static void Execute(InviteMemberToGroupParameters parameters)
		{
						PrepareParameters(parameters);
					TBRGroupRoot GroupRoot = InviteMemberToGroupImplementation.GetTarget_GroupRoot(parameters.GroupID);	
				TBEmailValidation EmailValidation = InviteMemberToGroupImplementation.GetTarget_EmailValidation(parameters.MemberEmailAddress, parameters.GroupID);	
				string AccountID = InviteMemberToGroupImplementation.GetTarget_AccountID(parameters.MemberEmailAddress);	
				InviteMemberToGroupImplementation.ExecuteMethod_AddAsPendingInvitationToGroupRoot(parameters.MemberEmailAddress, GroupRoot);		
				InviteMemberToGroupImplementation.ExecuteMethod_StoreObjects(GroupRoot, EmailValidation);		
				InviteMemberToGroupImplementation.ExecuteMethod_SendEmailConfirmation(EmailValidation, GroupRoot);		
				
		{ // Local block to allow local naming
			RefreshAccountGroupMembershipsParameters operationParameters = InviteMemberToGroupImplementation.RefreshAccountAndGroupContainers_GetParameters(GroupRoot, AccountID);
			RefreshAccountGroupMemberships.Execute(operationParameters);
									
		} // Local block closing
				}
				}
				public class RemoveMemberFromGroupParameters 
		{
				public string EmailAddress ;
				public string AccountID ;
				public string GroupID ;
				}
		
		public class RemoveMemberFromGroup 
		{
				private static void PrepareParameters(RemoveMemberFromGroupParameters parameters)
		{
					}
				public static void Execute(RemoveMemberFromGroupParameters parameters)
		{
						PrepareParameters(parameters);
					TBRGroupRoot GroupRoot = RemoveMemberFromGroupImplementation.GetTarget_GroupRoot(parameters.GroupID);	
				string AccountID = RemoveMemberFromGroupImplementation.GetTarget_AccountID(parameters.EmailAddress, parameters.AccountID);	
				TBRAccountRoot AccountRoot = RemoveMemberFromGroupImplementation.GetTarget_AccountRoot(AccountID);	
				string MemberEmailAddress = RemoveMemberFromGroupImplementation.GetTarget_MemberEmailAddress(parameters.EmailAddress, AccountRoot, GroupRoot);	
				RemoveMemberFromGroupImplementation.ExecuteMethod_RemoveMemberFromGroup(MemberEmailAddress, GroupRoot);		
				RemoveMemberFromGroupImplementation.ExecuteMethod_StoreObjects(GroupRoot);		
				
		{ // Local block to allow local naming
			RefreshAccountGroupMembershipsParameters operationParameters = RemoveMemberFromGroupImplementation.RefreshAccountAndGroupContainers_GetParameters(GroupRoot, AccountID);
			RefreshAccountGroupMemberships.Execute(operationParameters);
									
		} // Local block closing
				}
				}
				public class ConfirmInviteToJoinGroupParameters 
		{
				public string MemberEmailAddress ;
				public string GroupID ;
				}
		
		public class ConfirmInviteToJoinGroup 
		{
				private static void PrepareParameters(ConfirmInviteToJoinGroupParameters parameters)
		{
					}
				public static void Execute(ConfirmInviteToJoinGroupParameters parameters)
		{
						PrepareParameters(parameters);
					TBRGroupRoot GroupRoot = ConfirmInviteToJoinGroupImplementation.GetTarget_GroupRoot(parameters.GroupID);	
				string AccountID = ConfirmInviteToJoinGroupImplementation.GetTarget_AccountID(parameters.MemberEmailAddress);	
				ConfirmInviteToJoinGroupImplementation.ExecuteMethod_ConfirmPendingInvitationToGroupRoot(parameters.MemberEmailAddress, GroupRoot);		
				ConfirmInviteToJoinGroupImplementation.ExecuteMethod_StoreObjects(GroupRoot);		
				
		{ // Local block to allow local naming
			RefreshAccountGroupMembershipsParameters operationParameters = ConfirmInviteToJoinGroupImplementation.RefreshAccountAndGroupContainers_GetParameters(GroupRoot, AccountID);
			RefreshAccountGroupMemberships.Execute(operationParameters);
									
		} // Local block closing
				}
				}
				public class RefreshAccountGroupMembershipsParameters 
		{
				public TBRGroupRoot GroupRoot ;
				public string AccountID ;
				}
		
		public class RefreshAccountGroupMemberships 
		{
				private static void PrepareParameters(RefreshAccountGroupMembershipsParameters parameters)
		{
					}
				public static void Execute(RefreshAccountGroupMembershipsParameters parameters)
		{
						PrepareParameters(parameters);
					
		{ // Local block to allow local naming
			UpdateAccountRootGroupMembershipParameters operationParameters = RefreshAccountGroupMembershipsImplementation.UpdateAccountRoot_GetParameters(parameters.GroupRoot, parameters.AccountID);
			UpdateAccountRootGroupMembership.Execute(operationParameters);
									
		} // Local block closing
				
		{ // Local block to allow local naming
			UpdateLoginGroupPermissionsParameters operationParameters = RefreshAccountGroupMembershipsImplementation.UpdateAccountLoginGroups_GetParameters(parameters.AccountID);
			UpdateLoginGroupPermissions.Execute(operationParameters);
									
		} // Local block closing
				
		{ // Local block to allow local naming
			UpdateGroupContainersGroupMembershipParameters operationParameters = RefreshAccountGroupMembershipsImplementation.UpdateGroupContainers_GetParameters(parameters.GroupRoot);
			UpdateGroupContainersGroupMembership.Execute(operationParameters);
									
		} // Local block closing
				
		{ // Local block to allow local naming
			UpdateAccountContainersGroupMembershipParameters operationParameters = RefreshAccountGroupMembershipsImplementation.UpdateAccountContainers_GetParameters(parameters.GroupRoot, parameters.AccountID);
			UpdateAccountContainersGroupMembership.Execute(operationParameters);
									
		} // Local block closing
				}
				}
				public class UpdateGroupContainersGroupMembershipParameters 
		{
				public TBRGroupRoot GroupRoot ;
				}
		
		public class UpdateGroupContainersGroupMembership 
		{
				private static void PrepareParameters(UpdateGroupContainersGroupMembershipParameters parameters)
		{
					}
				public static void Execute(UpdateGroupContainersGroupMembershipParameters parameters)
		{
						PrepareParameters(parameters);
					AccountRootAndContainer[] AccountRootsAndContainers = UpdateGroupContainersGroupMembershipImplementation.GetTarget_AccountRootsAndContainers(parameters.GroupRoot);	
				GroupContainer GroupContainer = UpdateGroupContainersGroupMembershipImplementation.GetTarget_GroupContainer(parameters.GroupRoot);	
				UpdateGroupContainersGroupMembershipImplementation.ExecuteMethod_UpdateGroupContainerMembership(parameters.GroupRoot, AccountRootsAndContainers, GroupContainer);		
				UpdateGroupContainersGroupMembershipImplementation.ExecuteMethod_StoreObjects(GroupContainer);		
				}
				}
				public class UpdateLoginGroupPermissionsParameters 
		{
				public string AccountID ;
				}
		
		public class UpdateLoginGroupPermissions 
		{
				private static void PrepareParameters(UpdateLoginGroupPermissionsParameters parameters)
		{
					}
				public static void Execute(UpdateLoginGroupPermissionsParameters parameters)
		{
						PrepareParameters(parameters);
					TBRAccountRoot AccountRoot = UpdateLoginGroupPermissionsImplementation.GetTarget_AccountRoot(parameters.AccountID);	
				TBRLoginGroupRoot[] LoginGroupRoots = UpdateLoginGroupPermissionsImplementation.GetTarget_LoginGroupRoots(AccountRoot);	
				UpdateLoginGroupPermissionsImplementation.ExecuteMethod_SynchronizeLoginGroupRoots(AccountRoot, LoginGroupRoots);		
				}
				}
				public class UpdateAccountRootGroupMembershipParameters 
		{
				public TBRGroupRoot GroupRoot ;
				public string AccountID ;
				}
		
		public class UpdateAccountRootGroupMembership 
		{
				private static void PrepareParameters(UpdateAccountRootGroupMembershipParameters parameters)
		{
					}
				public static void Execute(UpdateAccountRootGroupMembershipParameters parameters)
		{
						PrepareParameters(parameters);
					TBRAccountRoot AccountRoot = UpdateAccountRootGroupMembershipImplementation.GetTarget_AccountRoot(parameters.AccountID);	
				UpdateAccountRootGroupMembershipImplementation.ExecuteMethod_UpdateAccountRootGroupMemberships(parameters.GroupRoot, AccountRoot);		
				UpdateAccountRootGroupMembershipImplementation.ExecuteMethod_StoreObjects(AccountRoot);		
				}
				}
				public class UpdateAccountContainersGroupMembershipParameters 
		{
				public TBRGroupRoot GroupRoot ;
				public string AccountID ;
				}
		
		public class UpdateAccountContainersGroupMembership 
		{
				private static void PrepareParameters(UpdateAccountContainersGroupMembershipParameters parameters)
		{
					}
				public static void Execute(UpdateAccountContainersGroupMembershipParameters parameters)
		{
						PrepareParameters(parameters);
					GroupContainer GroupContainer = UpdateAccountContainersGroupMembershipImplementation.GetTarget_GroupContainer(parameters.GroupRoot);	
				Group Group = UpdateAccountContainersGroupMembershipImplementation.GetTarget_Group(GroupContainer);	
				TBRAccountRoot AccountRoot = UpdateAccountContainersGroupMembershipImplementation.GetTarget_AccountRoot(parameters.AccountID);	
				AccountContainer AccountContainer = UpdateAccountContainersGroupMembershipImplementation.GetTarget_AccountContainer(parameters.AccountID);	
				GroupSummaryContainer GroupSummaryContainer = UpdateAccountContainersGroupMembershipImplementation.GetTarget_GroupSummaryContainer(parameters.AccountID);	
				UpdateAccountContainersGroupMembershipImplementation.ExecuteMethod_UpdateGroupSummaryContainerMemberships(parameters.GroupRoot, Group, AccountRoot, GroupSummaryContainer);		
				UpdateAccountContainersGroupMembershipImplementation.ExecuteMethod_UpdateAccountContainerMemberships(parameters.GroupRoot, Group, GroupSummaryContainer, AccountRoot, AccountContainer);		
				UpdateAccountContainersGroupMembershipImplementation.ExecuteMethod_StoreObjects(AccountContainer, GroupSummaryContainer);		
				}
				}
				public class PerformWebActionParameters 
		{
				public string TargetObjectID ;
				public string CommandName ;
				public IContainerOwner Owner ;
				public InformationSourceCollection InformationSources ;
				public string[] FormSourceNames ;
				}
		
		public class PerformWebAction 
		{
				private static void PrepareParameters(PerformWebActionParameters parameters)
		{
					}
				public static PerformWebActionReturnValue Execute(PerformWebActionParameters parameters)
		{
						PrepareParameters(parameters);
					bool ExecuteActualOperationOutput = PerformWebActionImplementation.ExecuteMethod_ExecuteActualOperation(parameters.TargetObjectID, parameters.CommandName, parameters.Owner, parameters.InformationSources, parameters.FormSourceNames);		
				PerformWebActionReturnValue returnValue = PerformWebActionImplementation.Get_ReturnValue(ExecuteActualOperationOutput);
		return returnValue;
				}
				}
				public class PerformWebActionReturnValue 
		{
				public bool RenderPageAfterOperation ;
				}
				public class UpdatePageContentParameters 
		{
				public string changedInformation ;
				}
		
		public class UpdatePageContent 
		{
				private static void PrepareParameters(UpdatePageContentParameters parameters)
		{
					}
				public static void Execute(UpdatePageContentParameters parameters)
		{
						PrepareParameters(parameters);
					UpdatePageContentImplementation.ExecuteMethod_UpdatePage();		
				}
				}
		 } 