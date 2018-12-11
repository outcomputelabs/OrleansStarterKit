using Grains.Models;
using Orleans;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    /// <summary>
    /// Represents a list of users to follow.
    /// </summary>
    public interface ILobby : IGrainWithGuidKey
    {
        /// <summary>
        /// Adds or updates user information with this lobby.
        /// </summary>
        /// <param name="info">The user information to list.</param>
        Task SetUserInfoAsync(AccountInfo info);

        /// <summary>
        /// Returns user information for display.
        /// </summary>
        Task<ImmutableList<AccountInfo>> GetUserInfoListAsync();
    }
}
