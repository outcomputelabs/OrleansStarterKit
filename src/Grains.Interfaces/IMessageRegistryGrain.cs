using Grains.Models;
using Orleans;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public interface IMessageRegistryGrain : IGrainWithGuidKey
    {
        /// <summary>
        /// Registers an entity with the registry.
        /// If the entity does not yet exist, it will be added.
        /// If the entity already exists, it will be updated.
        /// This method is for use by <see cref="IUserGrain"/> activations only.
        /// Do not call this method from client code.
        /// </summary>
        Task RegisterAsync(Message entity);

        /// <summary>
        /// Unregisters an entity from the registry.
        /// This method is for use by <see cref="IUserGrain"/> activations only.
        /// Do not call this method from client code.
        /// </summary>
        Task UnregisterAsync(Message entity);

        /// <summary>
        /// Returns the entity with the given identifier or null if no corresponding entity exists.
        /// </summary>
        Task<Message> GetAsync(Guid id);

        /// <summary>
        /// Returns the latest messages for a given receiver id, up to the given count.
        /// </summary>
        Task<ImmutableList<Message>> GetLatestByReceiverIdAsync(Guid receiverId, int count);
    }
}
