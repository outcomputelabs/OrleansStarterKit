using Grains.Models;
using Orleans;
using System.Threading.Tasks;

namespace Grains
{
    public interface IUserRegistryGrain : IGrainWithGuidKey
    {
        /// <summary>
        /// Attempts to create a new user with the given key and username.
        /// The username check is case-insensitive.
        /// </summary>
        Task<bool> TryCreateAsync(UserInfo info);
    }
}
