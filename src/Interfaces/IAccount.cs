using Grains.Models;
using Orleans;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    /// <summary>
    /// Represents an application account.
    /// </summary>
    public interface IAccount : IGrainWithStringKey
    {
        /// <summary>
        /// Updates the properties of the account.
        /// </summary>
        /// <param name="info">The properties to update the account with.</param>
        Task SetInfoAsync(AccountInfo info);

        /// <summary>
        /// Returns the properties of the account.
        /// </summary>
        Task<AccountInfo> GetInfoAsync();

        /// <summary>
        /// Asks this account to follow the given account.
        /// </summary>
        Task FollowAsync(AccountInfo info, IAccount account);

        /// <summary>
        /// Tells this account that the given account is going to become a follower.
        /// </summary>
        Task FollowedByAsync(AccountInfo info, IAccount account);

        /// <summary>
        /// Returns the list of accounts that this account is following.
        /// </summary>
        Task<ImmutableList<AccountInfo>> GetFollowingAsync();
    }
}
