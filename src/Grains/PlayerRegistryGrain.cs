using Grains.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grains
{
    public class PlayerRegistryGrain : Grain<PlayerRegistryState>, IPlayerRegistryGrain
    {
        private readonly ILogger<PlayerRegistryGrain> _logger;

        public PlayerRegistryGrain(ILogger<PlayerRegistryGrain> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public Task RegisterAsync(PlayerInfo info)
        {
            State.Players.Remove(info);
            State.Players.Add(info);
            return WriteStateAsync();
        }
    }

    public class PlayerRegistryState
    {
        public HashSet<PlayerInfo> Players { get; set; }
    }
}
