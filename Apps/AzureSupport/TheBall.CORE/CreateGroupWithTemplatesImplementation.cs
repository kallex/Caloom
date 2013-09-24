using System.Web;
using AaltoGlobalImpact.OIP;

namespace TheBall.CORE
{
    public class CreateGroupWithTemplatesImplementation
    {
        public static string ExecuteMethod_ExecuteCreateGroup(string groupName, string accountId)
        {
            CreateGroupParameters parameters = new CreateGroupParameters()
                {
                    AccountID = accountId,
                    GroupName = groupName
                };
            var result = CreateGroup.Execute(parameters);
            return result.GroupID;
        }

        public static void ExecuteMethod_CopyGroupTemplates(string templateNameList, string executeCreateGroupOutput)
        {
            string[] templates = templateNameList.Split(',');
            foreach(var templateName in templates)
                RenderWebSupport.RefreshGroupTemplate(executeCreateGroupOutput, templateName, false);
        }

        public static void ExecuteMethod_RedirectToGivenUrl(string redirectUrlAfterCreation, string executeCreateGroupOutput)
        {
            if (string.IsNullOrEmpty(redirectUrlAfterCreation) == false)
            {
                string redirectTarget = "/auth/grp/" + executeCreateGroupOutput + "/" + redirectUrlAfterCreation;
                HttpContext.Current.Response.Redirect(redirectTarget, true);
            }
        }

        public static IContainerOwner GetTarget_GroupAsOwner(string executeCreateGroupOutput)
        {
            throw new System.NotImplementedException();
        }

        public static SetOwnerWebRedirectParameters SetDefaultRedirect_GetParameters(string groupDefaultRedirect, IContainerOwner groupAsOwner)
        {
            throw new System.NotImplementedException();
        }
    }
}