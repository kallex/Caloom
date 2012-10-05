using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using AaltoGlobalImpact.OIP;

namespace TheBall
{
    public class InformationContext
    {
        private const string KEYNAME = "INFOCTX";
        private static ConcurrentDictionary<int, InformationContext> _syncStorage = null;
        public string CurrentGroupRole;

        public static void InitializeFunctionality(int concurrentWorkers, bool allowStatic = false)
        {
            if(_syncStorage != null)
                throw new InvalidOperationException("InformationContext already initialized!");
            _syncStorage = new ConcurrentDictionary<int, InformationContext>(concurrentWorkers, concurrentWorkers);
            AllowStatic = allowStatic;
        }

        public InformationContext()
        {
            FinalizingOperationQueue = new List<OperationRequest>();
        }

        public static bool AllowStatic { get; private set; }

        public static InformationContext Current
        {
            get
            {
                var httpContext = HttpContext.Current;
                if(httpContext != null)
                {
                    if (httpContext.Items.Contains(KEYNAME))
                        return (InformationContext) httpContext.Items[KEYNAME];
                    InformationContext informationContext = InformationContext.Create();
                    httpContext.Items.Add(KEYNAME, informationContext);
                    return informationContext;
                }
                var currTaskID = Task.CurrentId;
                if(currTaskID.HasValue || AllowStatic)
                {
                    if(_syncStorage == null)
                        throw new InvalidOperationException("InformationContext not initialized for Task required storage");
                    // Return 0 in case of null, for AllowStatic
                    int currKey = currTaskID.GetValueOrDefault(0);
                    InformationContext informationContext = _syncStorage.GetOrAdd(currKey, CreateWithID);
                    return informationContext;
                }
                throw new NotSupportedException("InformationContext requires either HttpContext.Current, Task.CurrentId or to be available or AllowStatic to be defined");
            }
        }

        public static void ProcessAndClearCurrent()
        {
            Current.PerformFinalizingActions();
            var httpContext = HttpContext.Current;
            if(httpContext != null)
            {
                if(httpContext.Items.Contains(KEYNAME))
                {
                    httpContext.Items.Remove(KEYNAME);
                    return;
                }
            }
            var currTaskID = Task.CurrentId;
            if (currTaskID.HasValue || AllowStatic)
            {
                int currKey = currTaskID.GetValueOrDefault(0);
                InformationContext informationContext;
                bool result = _syncStorage.TryRemove(currKey, out informationContext);
                if (result)
                    return;
            }
            throw new InvalidOperationException("InformationContext ClearCurrent failed - no active context set");
        }

        private static InformationContext CreateWithID(int id)
        {
            InformationContext result = InformationContext.Create();
            result.ID = id;
            return result;
        }

        public int ID { get; private set; }

        private static InformationContext Create()
        {
            return new InformationContext();
        }

        public void AddOperationRequestToFinalizingQueue(OperationRequest operationRequest)
        {
            FinalizingOperationQueue.Add(operationRequest);
        }

        public void PerformFinalizingActions()
        {
            FinalizingOperationQueue.ForEach(oper => QueueSupport.PutToOperationQueue(oper));
        }

        protected List<OperationRequest> FinalizingOperationQueue { get; private set; }
    }
}