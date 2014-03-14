using System;
using System.Collections.Generic;
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
                OperationSupport.ExecuteOperation(operationTypeName, new Tuple<string, object>("Process", process));
                process.StoreInformation();
            }
            finally
            {
                StorageSupport.ReleaseLogicalLockByDeletingBlob(processLockLocation, lockEtag);
            }
        }
    }
}