using Grains.Models;
using Orleans;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public interface IChatRoomListGrain : IGrainWithGuidKey
    {
        Task<Guid> AddOrUpdateAsync(ChatRoomInfo info);
        Task<ImmutableList<ChatRoomInfo>> GetAsync();
    }
}
