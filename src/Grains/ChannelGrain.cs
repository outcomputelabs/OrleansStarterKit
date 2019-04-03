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
    public class ChannelGrain : Grain, IChannelGrain
    {
        private readonly ILogger<ChannelGrain> _logger;
        private readonly ChannelOptions _options;
        private readonly Queue<Message> _messages = new Queue<Message>();

        private IStorageRegistryGrain _registry;

        private ChannelInfo _info;

        private string GrainType => nameof(ChannelGrain);
        private Guid GrainKey => this.GetPrimaryKey();

        public ChannelGrain(ILogger<ChannelGrain> logger, IOptions<ChannelOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public override async Task OnActivateAsync()
        {
            // keep the registry proxy for comfort
            _registry = GrainFactory.GetGrain<IStorageRegistryGrain>(Guid.Empty);

            // cache any existing channel info from the registry
            _info = await _registry.GetChannelAsync(GrainKey);

            // cache latest messages
            _messages.Enqueue(await _registry.GetLatestMessagesByReceiverIdAsync(GrainKey, _options.MaxCachedMessages), _options.MaxCachedMessages);
        }

        /// <summary>
        /// Set the channel information for this channel.
        /// </summary>
        public async Task SetInfoAsync(ChannelInfo info)
        {
            ValidateChannelInfo(info);

            // save info state to registry
            await _registry.RegisterChannelAsync(info);

            // keep it cached
            _info = info;

            LogChannelInfoUpdated(info);
        }

        /// <summary>
        /// Returns the channel information for this channel.
        /// </summary>
        public Task<ChannelInfo> GetInfoAsync()
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

        /// <inheritdoc />
        public Task AddUserAsync(IUserGrain user)
        {
            // todo: add this user to the persisted list of participating users in the channel

            // todo: add it to the cached list as well
            return Task.CompletedTask;
        }

        #region Helpers

        /// <summary>
        /// This method checks if the given message is directed at the current channel and throws an <see cref="InvalidOperationException"/> if not.
        /// It also throws a <see cref="ArgumentNullException"/> if the message is null.
        /// Either will only ever happen to some accidental design bug, but better to be defensive about it.
        /// </summary>
        /// <exception cref="ArgumentNullException">The message is null.</exception>
        /// <exception cref="InvalidOperationException">The receiver id of the message does not match the current channel.</exception>
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
        /// This method checks if the channel information has been initialized and throws an <see cref="InvalidOperationException"/> if not.
        /// It also logs an error that contains the name of the method that called this one.
        /// Use this to protect grain calls that require channel information to be initialized.
        /// This event will only even happen due to some accidental design bug but better to be defensive about it.
        /// </summary>
        /// <exception cref="InvalidOperationException">The channel is not initialized yet.</exception>
        private void EnsureInitialized([CallerMemberName] string method = null)
        {
            if (_info == null)
            {
                LogCannotExecuteMethod(method);
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Checks if the given channel info is consistent with the current channel.
        /// </summary>
        /// <exception cref="InvalidOperationException">The id of the given channel info does not match the current grain key.</exception>
        private void ValidateChannelInfo(ChannelInfo info)
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
        /// Logs an error regarding inability to execute a certain method due to channel information not yet being initialzed.
        /// </summary>
        private void LogCannotExecuteMethod(string method)
        {
            _logger.LogError(
                "{@GrainType} {@GrainKey} cannot execute {@MethodName} because the channel information is not yet initialized",
                GrainType, GrainKey, method);
        }

        /// <summary>
        /// Logs an error regarding an attempt to set the channel info with an inconsistent key.
        /// </summary>
        private void LogInconsistentKey(ChannelInfo info)
        {
            _logger.LogError("{@GrainType} {@GrainKey} refusing request to set info to {@ChannelInfo} because of inconsistent key",
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
        /// Logs a debug entry regarding this grain updating the channel info.
        /// </summary>
        private void LogChannelInfoUpdated(ChannelInfo info)
        {
            _logger.LogDebug("{@GrainType} {@GrainKey} updated info to {@ChannelInfo}",
                GrainType, GrainKey, info);
        }

        #endregion
    }
}
