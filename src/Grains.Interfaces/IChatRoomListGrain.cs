using Grains.Models;
using Orleans;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public interface IChatRoomListGrain : IGrainWithGuidKey
    {
        Task<int> SetAsync(ChatRoomInfo info);
        Task<ImmutableList<ChatRoomInfo>> GetAsync();
        Task<ChatRoomListPollResult> PollAsync(int version);
    }
}
