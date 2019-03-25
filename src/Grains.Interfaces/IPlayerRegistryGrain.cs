using Grains.Models;
using Orleans;
using System.Threading.Tasks;

namespace Grains
{
    public interface IPlayerRegistryGrain : IGrainWithIntegerKey
    {
        /// <summary>
        /// Registers or updates a player with the given info.
        /// </summary>
        Task RegisterAsync(PlayerInfo info);
    }
}
