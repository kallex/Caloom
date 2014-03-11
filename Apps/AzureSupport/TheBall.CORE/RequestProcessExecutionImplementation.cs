using AaltoGlobalImpact.OIP;

namespace TheBall.CORE
{
    public class RequestProcessExecutionImplementation
    {
        public static string GetTarget_ActiveContainerName()
        {
            return StorageSupport.CurrActiveContainer.Name;
        }

        public static QueueEnvelope GetTarget_RequestEnvelope(string processId, IContainerOwner owner, string activeContainerName)
        {
            var envelope = new QueueEnvelope
                {
                    OwnerPrefix = owner.ToFolderName(),
                    ActiveContainerName = activeContainerName,
                    SingleOperation = new OperationRequest
                        {
                            ProcessIDToExecute = processId
                        }
                };
            return envelope;
        }

        public static void ExecuteMethod_PutEnvelopeToDefaultQueue(QueueEnvelope requestEnvelope)
        {
            QueueSupport.PutToDefaultQueue(requestEnvelope);
        }
    }
}