using Grains.Models;
using Orleans;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Linq;

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
        public ImmutableSortedSet<ChannelModel> Channels { get; set; } = ImmutableSortedSet<ChannelModel>.Empty;
    }

    /// <summary>
    /// Indexes a group a channels for quick listing.
    /// </summary>
    public class Lobby : Grain<LobbyState>, ILobby
    {
        public Task<IEnumerable<ChannelModel>> GetChannelsAsync()
        {
            return Task.FromResult<IEnumerable<ChannelModel>>(State.Channels);
        }
    }
}
