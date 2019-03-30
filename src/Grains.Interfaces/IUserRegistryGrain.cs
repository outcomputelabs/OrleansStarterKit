using Grains.Models;
using Orleans;
using System.Threading.Tasks;

namespace Grains
{
    public interface IUserRegistryGrain : IGrainWithGuidKey
    {
        /// <summary>
        /// Registers an entity with the registry.
        /// If the entity does not yet exist, it will be added.
        /// If the entity already exists, it will be updated.
        /// </summary>
        Task RegisterAsync(UserInfo info);

        /// <summary>
        /// Unregisters an entity from the registry.
        /// </summary>
        Task UnregisterAsync(UserInfo info);
    }
}
