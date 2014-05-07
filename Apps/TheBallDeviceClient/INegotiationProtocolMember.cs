using System;
using System.Collections.Generic;

#if ASYNC
using System.Threading.Tasks;
#endif

namespace TheBall.Support.DeviceClient
{
    public interface INegotiationProtocolMember
    {
        bool WaitForOtherParty { get; }
        bool IsDoneWithProtocol { get; }
        byte[] LatestMessageFromOtherParty { set; }
        void PerformNextAction();
        Action<byte[]> SendMessageToOtherParty { get; set; }
        List<byte[]> NegotiationResults { get; }
#if ASYNC
        Task PerformNextActionAsync();
        Func<byte[], Task> SendMessageToOtherPartyAsync { get; set; }
#endif
    }
}