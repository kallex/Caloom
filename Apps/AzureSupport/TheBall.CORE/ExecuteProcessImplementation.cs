using System;
using System.Reflection;
using AzureSupport;

namespace TheBall.CORE
{
    public class ExecuteProcessImplementation
    {
        static IContainerOwner Owner { get { return InformationContext.CurrentOwner; }}

        public static Process GetTarget_Process(string processId)
        {
            return Process.RetrieveFromOwnerContent(Owner, processId);
        }

        public static string GetTarget_ProcessLockLocation(Process process)
        {
            return process.RelativeLocation + ".lock";
        }

        public static void ExecuteMethod_ExecuteAndStoreProcessWithLock(string processLockLocation, Process process)
        {
            string lockEtag;
            bool obtainLock = StorageSupport.AcquireLogicalLockByCreatingBlob(processLockLocation, out lockEtag);
            if (obtainLock == false)
                return;
            try
            {
                string operationTypeName = process.ExecutingOperation.ItemFullType;
                string parameterTypeName = operationTypeName + "Parameters";
                Type operationType = TypeSupport.GetTypeByName(operationTypeName);
                Type parameterType = TypeSupport.GetTypeByName(parameterTypeName);
                dynamic parameters = Activator.CreateInstance(parameterType);
                parameters.Process = process;
                var method = operationType.GetMethod("Execute", BindingFlags.Public | BindingFlags.Static);
                method.Invoke(null, new object[] { parameters});
                process.StoreInformation();
            }
            finally
            {
                StorageSupport.ReleaseLogicalLockByDeletingBlob(processLockLocation, lockEtag);
            }
        }
    }
}