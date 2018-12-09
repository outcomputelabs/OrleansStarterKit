using Grains.Models;
using Orleans;
using System.Threading.Tasks;

namespace Grains
{
    /// <summary>
    /// Represents a chat channel.
    /// </summary>
    public interface IChannel : IGrainWithStringKey
    {
        /// <summary>
        /// Updates the channel information.
        /// </summary>
        /// <param name="info">The channel information.</param>
        Task SetInfoAsync(ChannelInfo info);
    }
}
