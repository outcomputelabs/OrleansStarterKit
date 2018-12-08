using Grains.Models;
using Orleans;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    /// <summary>
    /// The persisted state of a lobby.
    /// </summary>
    public class LobbyState
    {
        /// <summary>
        /// The group of channels that this lobby indexes.
        /// </summary>
        public ImmutableSortedSet<ChannelInfo> Channels { get; set; } = ImmutableSortedSet<ChannelInfo>.Empty;
    }

    /// <summary>
    /// Indexes a group a channels for quick listing.
    /// </summary>
    public class Lobby : Grain<LobbyState>, ILobby
    {
        public override Task OnActivateAsync()
        {
            // add dummy data for now
            State.Channels = State.Channels.Add(new ChannelInfo(Guid.NewGuid(), "orleans", "johndoe", DateTime.UtcNow));

            return base.OnActivateAsync();
        }

        public Task<IEnumerable<ChannelInfo>> GetChannelsAsync()
        {
            return Task.FromResult<IEnumerable<ChannelInfo>>(State.Channels);
        }
    }
}
