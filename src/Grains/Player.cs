using Grains.Exceptions;
using Grains.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Grains
{
    public class Player : Grain, IPlayer
    {
        private string GrainKey => this.GetPrimaryKeyString();

        private readonly ILogger<Player> _logger;

        private readonly Queue<Message> _messages = new Queue<Message>();
        private readonly HashSet<Guid> _handled = new HashSet<Guid>();
        private readonly HashSet<Invite> _invites = new HashSet<Invite>();

        private readonly int MaxMessagesCached = 100;

        private IParty _party = null;

        public Player(ILogger<Player> logger)
        {
            _logger = logger;
        }

        public Task HelloWorldAsync()
        {
            _logger.LogInformation("{@GrainKey} says hello world!", GrainKey);
            return Task.CompletedTask;
        }

        public Task MessageAsync(Message message)
        {
            _logger.LogInformation("{@GrainKey} received {@Message}", message);
            _messages.Enqueue(message, MaxMessagesCached);
            return Task.CompletedTask;
        }

        public Task<ImmutableList<Message>> GetMessagesAsync()
        {
            return Task.FromResult(_messages.ToImmutableList());
        }

        public async Task<InviteResult> InviteAsync(IPlayer other)
        {
            // check if this player is in a party
            if (_party == null)
            {
                // create a new party
                var party = GrainFactory.GetGrain<IParty>(Guid.NewGuid());

                // set this player as the initial leader
                // in the extraordinary rare case that the new GUID is a duplicate of an existing party key
                // then this call will fail
                await party.SetLeaderAsync(this.AsReference<IPlayer>());

                // keep the party only if the above worked
                // this helps keep this method idempotent
                _party = party;
            }

            // attempt to invite the player via the party
            // this way the party can validate who is the current leader plus the number of players
            // and therefore allow or reject the invite attempt
            return await _party.InviteAsync(this.AsReference<IPlayer>(), other);
        }

        public async Task<InviteResult> TakeInviteAsync(Invite invite)
        {
            // check if the player is already in an active party
            if (_party != null && await _party.IsActiveAsync())
                return InviteResult.PlayerAlreadyInParty;

            // check if the invite is repeated
            if (_invites.Count(x => x.From == invite.From && x.Party == invite.From) > 0)
                return InviteResult.Success;

            // keep the invite as pending
            _invites.Add(invite);
            return InviteResult.Success;
        }
    }
}