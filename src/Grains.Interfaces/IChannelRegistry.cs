using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
    /// <summary>
    /// Maps a transient channel name to the channel permanent identifier.
    /// This allows a channel name to change while its identifier remains the same.
    /// Provide the channel name as the grain string key.
    /// </summary>
    public interface IChannelRegistry : IGrainWithStringKey
    {
        /// <summary>
        /// Get the channel key for the channel name passed in the grain key.
        /// If the channel key does not yet exist, then creates a new key.
        /// </summary>
        /// <returns>The key for the given channel name.</returns>
        Task<Guid> GetOrCreateKeyAsync();
    }
}
