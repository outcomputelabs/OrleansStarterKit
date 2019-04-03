using Grains.Models;
using Orleans;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public interface IStorageRegistryGrain : IGrainWithGuidKey
    {
        #region Users

        /// <summary>
        /// Registers a user with the registry.
        /// If the user does not yet exist, it will be added.
        /// If the user already exists, it will be updated.
        /// This method is for use by <see cref="IUserGrain"/> activations only.
        /// Do not call this method from client code.
        /// </summary>
        Task RegisterUserAsync(UserInfo entity);

        /// <summary>
        /// Unregisters a user from the registry.
        /// This method is for use by <see cref="IUserGrain"/> activations only.
        /// Do not call this method from client code.
        /// </summary>
        Task UnregisterUserAsync(UserInfo entity);

        /// <summary>
        /// Returns the user with the given identifier or null if no corresponding entity exists.
        /// </summary>
        Task<UserInfo> GetUserAsync(Guid id);

        /// <summary>
        /// Returns the user with the given handle or null if no corresponding entity exists.
        /// </summary>
        Task<UserInfo> GetUserByHandleAsync(string handle);

        #endregion

        #region Channels

        /// <summary>
        /// Registers a channel with the registry.
        /// If the channel does not yet exist, it will be added.
        /// If the channel already exists, it will be updated.
        /// This method is for use by <see cref="IChannelGrain"/> activations only.
        /// Do not call this method from client code.
        /// </summary>
        Task RegisterChannelAsync(ChannelInfo entity);

        /// <summary>
        /// Unregisters a channel from the registry.
        /// This method is for use by <see cref="IChannelGrain"/> activations only.
        /// Do not call this method from client code.
        /// </summary>
        Task UnregisterChannelAsync(ChannelInfo entity);

        /// <summary>
        /// Returns the channel with the given identifier or null if no corresponding entity exists.
        /// </summary>
        Task<ChannelInfo> GetChannelAsync(Guid id);

        /// <summary>
        /// Returns the channel with the given handle or null if no corresponding entity exists.
        /// </summary>
        Task<ChannelInfo> GetChannelByHandleAsync(string handle);

        #endregion

        #region Messages

        /// <summary>
        /// Registers a message with the registry.
        /// If the message does not yet exist, it will be added.
        /// If the message already exists, it will be updated.
        /// This method is for use by <see cref="IUserGrain"/> activations only.
        /// Do not call this method from client code.
        /// </summary>
        Task RegisterMessageAsync(Message entity);

        /// <summary>
        /// Unregisters a message from the registry.
        /// This method is for use by <see cref="IUserGrain"/> activations only.
        /// Do not call this method from client code.
        /// </summary>
        Task UnregisterMessageAsync(Message entity);

        /// <summary>
        /// Returns the message with the given identifier or null if no corresponding entity exists.
        /// </summary>
        Task<Message> GetMessageAsync(Guid id);

        /// <summary>
        /// Returns the latest messages for a given receiver id, up to the given count.
        /// </summary>
        Task<ImmutableList<Message>> GetLatestMessagesByReceiverIdAsync(Guid receiverId, int count);

        #endregion
    }
}
