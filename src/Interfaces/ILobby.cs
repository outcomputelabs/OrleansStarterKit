using Grains.Models;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grains
{
    /// <summary>
    /// Represents a list of channels.
    /// </summary>
    public interface ILobby : IGrainWithGuidKey
    {
        /// <summary>
        /// Returns the list of channels owned by this lobby.
        /// </summary>
        Task<IEnumerable<ChannelInfo>> GetChannelsAsync();

        /// <summary>
        /// Creates a new channel.
        /// </summary>
        /// <param name="info">The properties of the new channel.</param>
        /// <exception cref="InvalidChannelNameException">The key in <paramref name="info"/> does not match the grain key.</exception>
        Task CreateChannelAsync(ChannelInfo info);
    }
}
