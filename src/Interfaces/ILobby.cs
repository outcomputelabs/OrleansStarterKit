using Grains.Models;
using Orleans;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    /// <summary>
    /// Represents a list of channels.
    /// </summary>
    public interface ILobby : IGrainWithGuidKey
    {
        /// <summary>
        /// Gets information about all the channels in this lobby.
        /// </summary>
        Task<ImmutableList<ChannelModel>> GetChannels();

        /// <summary>
        /// Creates a new channel in this lobby and returns its identifier.
        /// </summary>
        /// <param name="name">The name of the new channel. It must be unique within the lobby.</param>
        Task<Guid> CreateChannel(string name);

        /// <summary>
        /// Removes a channel from the lobby.
        /// It does not delete the channel itself from the system.
        /// </summary>
        /// <param name="id">The id of the channel to remove.</param>
        Task RemoveChannel(Guid id);
    }
}
