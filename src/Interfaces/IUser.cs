using Grains.Models;
using Orleans;
using System.Threading.Tasks;

namespace Grains
{
    /// <summary>
    /// Represents an application user.
    /// </summary>
    public interface IUser : IGrainWithStringKey
    {
        /// <summary>
        /// Updates details for this user.
        /// </summary>
        /// <param name="info">The information to update the user with.</param>
        Task UpdateInfoAsync(UserInfo info);

        /// <summary>
        /// Returns the current details for the his user.
        /// </summary>
        Task<UserInfo> GetInfoAsync();
    }
}
