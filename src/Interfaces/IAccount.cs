using Grains.Models;
using Orleans;
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
    }
}
