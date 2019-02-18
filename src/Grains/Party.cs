using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grains
{
    public class Party : Grain, IParty
    {
        private readonly ILogger<Party> _logger;

        private readonly HashSet<IPlayer> _players = new HashSet<IPlayer>();

        private IPlayer _leader;

        private Guid GrainKey => this.GetPrimaryKey();

        private readonly int MaxCapacity = 4;
        private readonly TimeSpan InviteExpiry = TimeSpan.FromMinutes(5);

        public Party(ILogger<Party> logger)
        {
            _logger = logger;
        }

        public Task SetLeaderAsync(IPlayer player)
        {
            if (_leader == null)
            {
                _leader = player;
                _logger.LogInformation("Party {@GrainKey} leader is now {@Player}",
                    GrainKey, player);
            }
            else
            {
                throw new InvalidOperationException();
            }
            return Task.CompletedTask;
        }

        public Task<IPlayer> GetLeaderAsync()
        {
            return Task.FromResult(_leader);
        }

        public Task<bool> IsActiveAsync()
        {
            return Task.FromResult(_players.Count >= 2);
        }
    }
}