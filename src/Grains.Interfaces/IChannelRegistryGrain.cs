using Grains.Models;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
    public interface IChannelRegistryGrain : IGrainWithGuidKey
    {
        /// <summary>
        /// Registers an entity with the registry.
        /// If the entity does not yet exist, it will be added.
        /// If the entity already exists, it will be updated.
        /// This method is for use by <see cref="IChannelGrain"/> activations only.
        /// Do not call this method from client code.
        /// </summary>
        Task RegisterAsync(ChannelInfo entity);

        /// <summary>
        /// Unregisters an entity from the registry.
        /// This method is for use by <see cref="IChannelGrain"/> activations only.
        /// Do not call this method from client code.
        /// </summary>
        Task UnregisterAsync(ChannelInfo entity);

        /// <summary>
        /// Returns the entity with the given identifier or null if no corresponding entity exists.
        /// </summary>
        Task<ChannelInfo> GetAsync(Guid id);

        /// <summary>
        /// Returns the entity with the given handle or null if no corresponding entity exists.
        /// </summary>
        Task<ChannelInfo> GetByHandleAsync(string handle);
    }
}
