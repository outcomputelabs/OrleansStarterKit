using Grains.Models;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
    public interface IUserRegistryGrain : IGrainWithGuidKey
    {
        /// <summary>
        /// Registers an entity with the registry.
        /// If the entity does not yet exist, it will be added.
        /// If the entity already exists, it will be updated.
        /// This method is for use by <see cref="IUserGrain"/> activations only.
        /// Do not call this method from client code.
        /// </summary>
        Task RegisterAsync(UserInfo entity);

        /// <summary>
        /// Unregisters an entity from the registry.
        /// This method is for use by <see cref="IUserGrain"/> activations only.
        /// Do not call this method from client code.
        /// </summary>
        Task UnregisterAsync(UserInfo entity);

        /// <summary>
        /// Returns the entity with the given identifier or null if no corresponding entity exists.
        /// </summary>
        Task<UserInfo> GetAsync(Guid id);

        /// <summary>
        /// Returns the entity with the given handle or null if no corresponding entity exists.
        /// </summary>
        Task<UserInfo> GetByHandleAsync(string handle);
    }
}
