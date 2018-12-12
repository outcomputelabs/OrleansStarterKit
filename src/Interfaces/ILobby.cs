using Grains.Models;
using Orleans;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    /// <summary>
    /// Represents a list of accounts in the system.
    /// </summary>
    public interface ILobby : IGrainWithGuidKey
    {
        /// <summary>
        /// Adds or updates account information on this lobby.
        /// </summary>
        /// <param name="info">The account information to list.</param>
        Task SetAccountInfoAsync(AccountInfo info);

        /// <summary>
        /// Returns account information to display.
        /// </summary>
        Task<ImmutableList<AccountInfo>> GetAccountInfoListAsync();
    }
}
