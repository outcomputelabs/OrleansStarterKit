using Grains.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public class ChatUser : Grain, IChatUser
    {
        private readonly ILogger<ChatUser> _logger;

        private readonly Queue<ChatMessage> _messages = new Queue<ChatMessage>();

        private readonly int MaxMessagesCached = 100;

        public ChatUser(ILogger<ChatUser> logger)
        {
            _logger = logger;
        }

        public override Task OnActivateAsync()
        {
            _grainKey = this.GetPrimaryKeyString();

            return base.OnActivateAsync();
        }

        private string _grainKey;

        public Task MessageAsync(ChatMessage message)
        {
            _logger.LogInformation("{@GrainType} {@GrainKey} received {@Message}",
                nameof(ChatUser), _grainKey, message);

            _messages.Enqueue(message, MaxMessagesCached);

            return Task.CompletedTask;
        }

        public Task<ImmutableList<ChatMessage>> GetMessagesAsync()
        {
            return Task.FromResult(_messages.ToImmutableList());
        }
    }
}