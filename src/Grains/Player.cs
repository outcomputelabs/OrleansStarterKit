using Grains.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public class Player : Grain, IPlayer
    {
        private string GrainKey => this.GetPrimaryKeyString();

        private readonly ILogger<Player> _logger;

        private readonly Queue<Message> _messages = new Queue<Message>();
        private readonly HashSet<Guid> _handled = new HashSet<Guid>();

        private readonly int MaxMessagesCached = 100;

        public Player(ILogger<Player> logger)
        {
            _logger = logger;
        }

        public Task HelloWorldAsync()
        {
            _logger.LogInformation("{@GrainKey} says hello world!", GrainKey);
            return Task.CompletedTask;
        }

        public Task MessageAsync(Message message)
        {
            _logger.LogInformation("{@GrainKey} received {@Message}", message);
            _messages.Enqueue(message, MaxMessagesCached);
            return Task.CompletedTask;
        }

        public Task<ImmutableList<Message>> GetMessagesAsync()
        {
            return Task.FromResult(_messages.ToImmutableList());
        }
    }
}