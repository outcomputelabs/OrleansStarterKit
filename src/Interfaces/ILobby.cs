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
    }
}
