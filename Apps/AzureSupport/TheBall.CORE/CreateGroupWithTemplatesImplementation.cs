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
    }
}