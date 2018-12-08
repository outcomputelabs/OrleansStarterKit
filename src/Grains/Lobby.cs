using Grains.Models;
using Orleans;
using System.Collections.Immutable;

namespace Grains
{
    /// <summary>
    /// The persisted state of a lobby.
    /// </summary>
    public class LobbyState
    {
        /// <summary>
        /// The group of channels that this lobby indexes.
        /// This immutable sorted set sacrifices insertion speed in favour of fast ordered retrieval for user interface listing.
        /// </summary>
        public ImmutableSortedSet<ChannelModel> Channels { get; set; } = ImmutableSortedSet<ChannelModel>.Empty;
    }

    /// <summary>
    /// Indexes a group a channels for quick listing.
    /// </summary>
    public class Lobby : Grain<LobbyState>, ILobby
    {
    }
}
