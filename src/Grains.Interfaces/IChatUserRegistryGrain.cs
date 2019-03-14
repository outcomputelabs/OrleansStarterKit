using Orleans;
using System.Threading.Tasks;

namespace Grains
{
    /// <summary>
    /// Maps a mutable user name to that user's immutable key.
    /// This allows the user to change name over its lifetime while references to it remain consistent.
    /// Provide the user name as the grain string key.
    /// </summary>
    public interface IChatUserRegistryGrain : IGrainWithStringKey
    {
        /// <summary>
        /// Gets the user with the name passed in the grain key.
        /// If the user does not yet have a key, a new one is created.
        /// </summary>
        /// <returns>The user with the given name.</returns>
        Task<IChatUserGrain> GetOrCreateChatRoomAsync();

        /// <summary>
        /// Checks whether the user name given in the grain key has an immutable key.
        /// In other words, it checks if the given user has been created yet.
        /// </summary>
        /// <returns>True if the user has been created, otherwise false.</returns>
        Task<bool> ExistsAsync();
    }
}
