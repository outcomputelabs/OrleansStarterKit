using Grains.Exceptions;
using Grains.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grains
{
    public class Party : Grain, IParty
    {
        private readonly ILogger<Party> _logger;

        private readonly HashSet<IPlayer> _players = new HashSet<IPlayer>();
        private readonly HashSet<Invite> _invites = new HashSet<Invite>();

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

        public async Task<InviteResult> InviteAsync(IPlayer sender, IPlayer other)
        {
            // check if the invite sender is the current leader
            if (sender != _leader)
                return InviteResult.SenderIsNotPartyLeader;

            // check if the player is already in the party
            if (_players.Contains(other))
                return InviteResult.PlayerAlreadyInParty;

            // check if the party has reached capacity
            if (_players.Count > MaxCapacity)
                return InviteResult.PartyIsFull;

            // check if there is a already a pending invite to the player for this party
            if (_invites.Count(x => x.To == other) > 0)
                return InviteResult.PlayerAlreadyInvited;

            // send the invite to the target player
            var invite = new Invite(this.AsReference<IParty>(), sender, other, DateTime.UtcNow.Add(InviteExpiry));
            var result = await other.TakeInviteAsync(invite);

            // propagate an immediate rejection
            if (result != InviteResult.Success)
                return result;

            // otherwise keep this as a pending invite
            _invites.Add(invite);

            // signal that invite was successful
            return result;
        }

        public Task<bool> IsActiveAsync()
        {
            return Task.FromResult(_players.Count >= 2);
        }
    }
}