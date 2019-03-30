using Grains.Models;
using Grains.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Grains
{
    public class UserGrain : Grain, IUserGrain
    {
        private readonly ILogger<UserGrain> _logger;
        private readonly UserOptions _options;
        private readonly Queue<Message> _messages = new Queue<Message>();

        private IUserRegistryGrain _registry;
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
            _registry = GrainFactory.GetGrain<IUserRegistryGrain>(Guid.Empty);

            // load any existing info from the registry
            _info = await _registry.GetAsync(GrainKey);
        }

        public async Task SetInfoAsync(UserInfo info)
        {
            // validate input consistency
            if (info.Id != GrainKey)
            {
                _logger.LogError("{@GrainType} {@GrainKey} refusing request to set info to {@UserInfo} because of inconsistent key",
                    GrainType, GrainKey, info);

                throw new InvalidOperationException();
            }

            // save info state to registry
            await _registry.RegisterAsync(info);

            _logger.LogDebug("{@GrainType} {@GrainKey} updated info to {@PlayerInfo}",
                GrainType, GrainKey, info);
        }

        public Task<UserInfo> GetInfoAsync() => Task.FromResult(_info);

        public Task TellAsync(Message message)
        {
            ThrowIfNotInitialized();

            _logger.LogDebug("{@GrainType} {@GrainKey} received tell {@Message}",
                GrainType, GrainKey, message);

            _messages.Enqueue(message, _options.MaxCachedMessages);

            return Task.CompletedTask;
        }

        /// <summary>
        /// This method checks if the user information has been initialized and throws an <see cref="InvalidOperationException"/> if not.
        /// It also log an error that contains the name of the method that called this one.
        /// Use this to protect grain calls that require user information to be initialized.
        /// This event will only even happen due to some accidental design bug but better to be defensive about it.
        /// </summary>
        private void ThrowIfNotInitialized([CallerMemberName] string method = null)
        {
            if (_info == null)
            {
                var error = new InvalidOperationException();

                _logger.LogError(error,
                    "{@GrainType} {@GrainKey} cannot execute {@MethodName} because the user information is not yet initialized",
                    GrainType, GrainKey, method);

                throw error;
            }
        }
    }
}
