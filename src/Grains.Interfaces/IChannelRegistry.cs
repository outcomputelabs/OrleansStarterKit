using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
    /// <summary>
    /// Maps a transient channel name to its internal identifier.
    /// This allows a channel to change while its identifier remains the same forever.
    /// Provide the channel name as the grain string sharding key.
    /// </summary>
    public interface IChannelRegistry : IGrainWithStringKey
    {
        /// <summary>
        /// Get the unique key for the given channel name.
        /// If the key does not yet exist, then creates a new key.
        /// </summary>
        /// <param name="name">The channel name to get the key for.</param>
        /// <returns>The key for the given channel name.</returns>
        Task<Guid> GetOrCreateKey(string name);
    }
}
