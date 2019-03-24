using Grains.Models;
using Orleans;
using System.Threading.Tasks;

namespace Grains
{
    public interface IPlayerGrain : IGrainWithStringKey
    {
        Task SetInfoAsync(PlayerInfo info);
        Task<PlayerInfo> GetInfoAsync();
    }
}