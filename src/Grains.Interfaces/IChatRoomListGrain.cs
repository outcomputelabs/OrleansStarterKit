using Grains.Models;
using Orleans;
using System.Threading.Tasks;

namespace Grains
{
    public interface IChatRoomListGrain : IGrainWithGuidKey
    {
        Task AddOrUpdateAsync(ChatRoomInfo info);
    }
}
