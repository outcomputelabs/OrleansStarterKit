using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grains.Models;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Grains
{
    public class Player : Grain, IPlayer
    {
        private string GrainKey => this.GetPrimaryKeyString();

        private readonly ILogger<Player> _logger;

        private readonly Queue<TellMessage> _received = new Queue<TellMessage>();
        private readonly Queue<TellMessage> _sent = new Queue<TellMessage>();
        private readonly HashSet<Guid> _handled = new HashSet<Guid>();

        private const int MaxMessagesCached = 100;

        public Player(ILogger<Player> logger)
        {
            _logger = logger;
        }

        public Task ReceiveTellAsync(TellMessage message)
        {
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
            _received.Enqueue(message);
            _handled.Add(message.Id);

            // clear overflowing messages from the cache
            while (_received.Count > MaxMessagesCached)
            {
                _handled.Remove(_received.Dequeue().Id);
            }

            return Task.CompletedTask;
        }

        public async Task SendTellAsync(TellMessage message)
        {
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
            _sent.Enqueue(message);

            // clear overflowing messages from the cache
            while (_sent.Count > MaxMessagesCached)
            {
                _handled.Remove(_sent.Dequeue().Id);
            }

            // mark the message as handled
            _handled.Add(message.Id);
        }
    }
}
