using Orleans;
using System.Threading.Tasks;

namespace Grains
{
    /// <summary>
    /// Maps a mutable chat room name to that chat room's immutable key.
    /// This allows the chat room to change name over its lifetime while references to it remain consistent.
    /// Provide the chat room name as the grain string key.
    /// </summary>
    public interface IChatRoomRegistryGrain : IGrainWithStringKey
    {
        /// <summary>
        /// Gets the chat room with the name passed in the grain key.
        /// If the chat room does not yet have a key, a new one is created.
        /// </summary>
        /// <returns>The chat room with the given name.</returns>
        Task<IChatRoomGrain> GetOrCreateChatRoomAsync();

        /// <summary>
        /// Checks whether the chat room name given in the grain key has an immutable key.
        /// In other words, it checks if the given chat room has been created yet.
        /// </summary>
        /// <returns>True if the chat room has been created, otherwise false.</returns>
        Task<bool> ExistsAsync();
    }
}
