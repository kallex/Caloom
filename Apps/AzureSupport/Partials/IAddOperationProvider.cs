using System.Web;

namespace AaltoGlobalImpact.OIP
{
    public interface IAddOperationProvider
    {
        bool PerformAddOperation(string commandName, InformationSourceCollection sources, string requesterLocation, HttpFileCollection files);
    }
}