using Grains.Models;
using Grains.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grains
{
    public class PlayerGrain : Grain<PlayerState>, IPlayerGrain
    {
        private readonly ILogger<PlayerGrain> _logger;
        private readonly PlayerOptions _options;
        private readonly Queue<TellMessage> _messages = new Queue<TellMessage>();

        private string GrainType => nameof(PlayerGrain);
        private string GrainKey => this.GetPrimaryKeyString();

        public PlayerGrain(ILogger<PlayerGrain> logger, IOptions<PlayerOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task SetInfoAsync(PlayerInfo info)
        {
            // validate input consistency
            if (info.Handle != GrainKey)
            {
                _logger.LogError("{@GrainType} {@GrainKey} refusing request to set info to {@PlayerInfo} because of inconsistent key",
                    GrainType, GrainKey, info);

                throw new InvalidOperationException();
            }

            // apply to grain state
            State.Info = info;
            await WriteStateAsync();

            _logger.LogDebug("{@GrainType} {@GrainKey} updated info to {@PlayerInfo}",
                GrainType, GrainKey, info);
        }

        public Task<PlayerInfo> GetInfoAsync() => Task.FromResult(State.Info);

        public Task TellAsync(TellMessage message)
        {
            _logger.LogDebug("{@GrainType} {@GrainKey} received tell {@Message}",
                GrainType, GrainKey, message);

            _messages.Enqueue(message, _options.MaxCachedMessages);

            return Task.CompletedTask;
        }
    }

    public class PlayerState
    {
        public PlayerInfo Info { get; set; }
    }
}
