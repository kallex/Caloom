using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecuritySupport
{
    public interface INegotiationProtocolMember
    {
        bool WaitForOtherParty { get; }
        bool IsDoneWithProtocol { get; }
        byte[] LatestMessageFromOtherParty { set; }
        void PerformNextAction();
        Task PerformNextActionAsync();
        Action<byte[]> SendMessageToOtherParty { get; set; }
        Func<byte[], Task> SendMessageToOtherPartyAsync { get; set; }
        List<byte[]> NegotiationResults { get; }
    }
}