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

        private readonly Queue<IMessage> _messages = new Queue<IMessage>();

        private readonly HashSet<Guid> _handled = new HashSet<Guid>();

        private readonly int MaxMessagesCached = 100;

        public Player(ILogger<Player> logger)
        {
            _logger = logger;
        }

        public Task ReceiveTellAsync(TellMessage message)
        {
            // log reception
            _logger.LogInformation("{@GrainKey} receiving {@Message}", GrainKey, message);

            // validate target player
            if (message.To != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // skip repeated requests
            if (_handled.Contains(message.Id))
            {
                return Task.CompletedTask;
            }

            // add the message as received
            _messages.Enqueue(message);
            _handled.Add(message.Id);

            // clear overflowing messages from the cache
            while (_messages.Count > MaxMessagesCached)
            {
                _handled.Remove(_messages.Dequeue().Id);
            }

            // log confirmation
            _logger.LogInformation("{@GrainKey} received {@Message}", GrainKey, message);

            return Task.CompletedTask;
        }

        public async Task SendTellAsync(TellMessage message)
        {
            // log reception
            _logger.LogInformation("{@GrainKey} sending {@Message}", GrainKey, message);

            // validate source player
            if (message.From != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // skip repeated messages
            if (_handled.Contains(message.Id))
            {
                return;
            }

            // send the message
            await GrainFactory.GetGrain<IPlayer>(message.To).ReceiveTellAsync(message);

            // add the message as sent
            _messages.Enqueue(message);

            // clear overflowing messages from the cache
            while (_messages.Count > MaxMessagesCached)
            {
                _handled.Remove(_messages.Dequeue().Id);
            }

            // mark the message as handled
            _handled.Add(message.Id);

            // log confirmation
            _logger.LogInformation("{@GrainKey} sent {@Message}", GrainKey, message);
        }

        public Task<ImmutableList<IMessage>> GetMessagesAsync()
        {
            return Task.FromResult(_messages.ToImmutableList());
        }
    }
}