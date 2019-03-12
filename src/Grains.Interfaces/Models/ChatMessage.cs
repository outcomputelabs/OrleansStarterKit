using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class ChatMessage
    {
        public ChatMessage(Guid id, string publisherId, string content, DateTime timestamp)
        {
            Id = id;
            PublisherId = publisherId;
            Content = content;
            Timestamp = timestamp;
        }

        public Guid Id { get; }
        public string PublisherId { get; }
        public string Content { get; }
        public DateTime Timestamp { get; }
    }
}
