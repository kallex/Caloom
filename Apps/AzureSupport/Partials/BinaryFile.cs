using TheBall.CORE;

namespace AaltoGlobalImpact.OIP
{
    partial class BinaryFile
    {
        partial void DoPostDeleteExecute(IContainerOwner owner)
        {
            Data.ClearCurrentContent(owner);
        }
    }
}