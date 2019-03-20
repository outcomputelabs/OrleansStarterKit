using Orleans.Concurrency;
using System.Collections.Immutable;

namespace Grains.Models
{
    [Immutable]
    public class ChatRoomListPollResult
    {
        public ChatRoomListPollResult(int version, ImmutableList<ChatRoomInfo> info)
        {
            Version = version;
            Info = info;
        }

        public int Version { get; }
        public ImmutableList<ChatRoomInfo> Info { get; }
    }
}
