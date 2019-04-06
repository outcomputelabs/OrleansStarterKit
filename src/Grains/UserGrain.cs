using Grains.Models;
using Grains.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Grains
{
    public class UserGrain : Grain, IUserGrain
    {
        private readonly ILogger<UserGrain> _logger;
        private readonly UserOptions _options;
        private readonly Queue<Message> _messages = new Queue<Message>();
        private readonly HashSet<ChannelUser> _channels = new HashSet<ChannelUser>();

        private IStorageRegistryGrain _registry;

        private UserInfo _info;

        private string GrainType => nameof(UserGrain);
        private Guid GrainKey => this.GetPrimaryKey();

        public UserGrain(ILogger<UserGrain> logger, IOptions<UserOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public override async Task OnActivateAsync()
        {
            // keep the registry proxy for comfort
            _registry = GrainFactory.GetGrain<IStorageRegistryGrain>(Guid.Empty);

            // cache any existing user info from the registry
            _info = await _registry.GetUserAsync(GrainKey);

            // cache latest messages
            _messages.Enqueue(await _registry.GetLatestMessagesByReceiverIdAsync(GrainKey, _options.MaxCachedMessages), _options.MaxCachedMessages);

            // cache joined channels
            _channels.UnionWith(await _registry.GetChannelsByUserAsync(GrainKey));
        }

        /// <summary>
        /// Set the user information for this user.
        /// </summary>
        public async Task SetInfoAsync(UserInfo info)
        {
            ValidateUserInfo(info);

            // save info state to registry
            await _registry.RegisterUserAsync(info);

            // keep it cached to fulfill queries
            _info = info;

            LogUserInfoUpdated(info);
        }

        /// <summary>
        /// Returns the user information for this user.
        /// </summary>
        public Task<UserInfo> GetInfoAsync()
        {
            EnsureInitialized();
            return Task.FromResult(_info);
        }

        /// <summary>
        /// Receives a tell message and saves it to the message registry.
        /// </summary>
        public async Task TellAsync(Message message)
        {
            EnsureInitialized();
            ValidateMessage(message);
            LogTell(message);

            // save this message to the registry
            await _registry.RegisterMessageAsync(message);

            // cache this message in memory
            // this helps fulfill requests for the latest messages without touching storage
            _messages.Enqueue(message, _options.MaxCachedMessages);
        }

        /// <summary>
        /// Returns the latest messages as cached by this grain.
        /// </summary>
        public Task<ImmutableList<Message>> GetLatestMessagesAsync()
        {
            return Task.FromResult(_messages.ToImmutableList());
        }

        public async Task JoinChannelAsync(IChannelGrain channel)
        {
            // register this participation with the registry
            // this ensures both this user and the channel can recover from the registry upon activation
            var member = new ChannelUser(channel.GetPrimaryKey(), GrainKey);
            await _registry.RegisterChannelUserAsync(member);

            // tell the channel to add this user now
            // in case this call fails the channel can now recover
            await channel.AddUserAsync(member);

            // keep this membership cached to facilitate queries on it
            _channels.Add(member);
        }

        #region Helpers

        /// <summary>
        /// This method checks if the given message is directed at the current user and throws an <see cref="InvalidOperationException"/> if not.
        /// It also throws a <see cref="ArgumentNullException"/> if the message is null.
        /// Either will only ever happen to some accidental design bug, but better to be defensive about it.
        /// </summary>
        /// <exception cref="ArgumentNullException">The message is null.</exception>
        /// <exception cref="InvalidOperationException">The receiver id of the message does not match the current user.</exception>
        private void ValidateMessage(Message message)
        {
            if (message == null)
            {
                LogNullMessage();
                throw new ArgumentNullException(nameof(message));
            }

            if (message.ReceiverId != GrainKey) throw new InvalidOperationException();
        }

        /// <summary>
        /// This method checks if the user information has been initialized and throws an <see cref="InvalidOperationException"/> if not.
        /// It also logs an error that contains the name of the method that called this one.
        /// Use this to protect grain calls that require user information to be initialized.
        /// This event will only even happen due to some accidental design bug but better to be defensive about it.
        /// </summary>
        /// <exception cref="InvalidOperationException">The user is not initialized yet.</exception>
        private void EnsureInitialized([CallerMemberName] string method = null)
        {
            if (_info == null)
            {
                LogCannotExecuteMethod(method);
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Checks if the given user info is consistent with the current user.
        /// </summary>
        /// <exception cref="InvalidOperationException">The id of the given user info does not match the current grain key.</exception>
        private void ValidateUserInfo(UserInfo info)
        {
            if (info.Id != GrainKey)
            {
                LogInconsistentKey(info);
                throw new InvalidOperationException();
            }
        }

        #endregion

        #region Logging

        /// <summary>
        /// Logs a debug entry regarding a tell message being received.
        /// </summary>
        /// <param name="message"></param>
        private void LogTell(Message message)
        {
            _logger.LogDebug("{@GrainType} {@GrainKey} received tell {@Message}",
                GrainType, GrainKey, message);
        }

        /// <summary>
        /// Logs an error regarding inability to execute a certain method due to user information not yet being initialzed.
        /// </summary>
        private void LogCannotExecuteMethod(string method)
        {
            _logger.LogError(
                "{@GrainType} {@GrainKey} cannot execute {@MethodName} because the user information is not yet initialized",
                GrainType, GrainKey, method);
        }

        /// <summary>
        /// Logs an error regarding an attempt to set the user info with an inconsistent key.
        /// </summary>
        private void LogInconsistentKey(UserInfo info)
        {
            _logger.LogError("{@GrainType} {@GrainKey} refusing request to set info to {@UserInfo} because of inconsistent key",
                GrainType, GrainKey, info);
        }

        /// <summary>
        /// Logs an error regarding receiving a null message.
        /// </summary>
        private void LogNullMessage()
        {
            _logger.LogError(
                "{@GrainType} {@GrainKey} received a null message.",
                GrainType, GrainKey);
        }

        /// <summary>
        /// Logs a debug entry regarding this grain updating the user info.
        /// </summary>
        private void LogUserInfoUpdated(UserInfo info)
        {
            _logger.LogDebug("{@GrainType} {@GrainKey} updated info to {@UserInfo}",
                GrainType, GrainKey, info);
        }

        #endregion
    }
}
