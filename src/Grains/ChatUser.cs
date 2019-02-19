using Grains.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public class ChatUser : Grain, IChatUser
    {
        private string GrainKey => this.GetPrimaryKeyString();

        private readonly ILogger<ChatUser> _logger;

        private readonly Queue<ChatMessage> _messages = new Queue<ChatMessage>();
        private readonly HashSet<Guid> _handled = new HashSet<Guid>();

        private readonly int MaxMessagesCached = 100;

        public ChatUser(ILogger<ChatUser> logger)
        {
            _logger = logger;
        }

        public Task MessageAsync(ChatMessage message)
        {
            _logger.LogInformation("{@GrainKey} received {@Message}", message);
            _messages.Enqueue(message, MaxMessagesCached);
            return Task.CompletedTask;
        }

        public Task<ImmutableList<ChatMessage>> GetMessagesAsync()
        {
            return Task.FromResult(_messages.ToImmutableList());
        }
    }
}