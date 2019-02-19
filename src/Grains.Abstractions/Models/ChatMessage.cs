using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class ChatMessage
    {
        public ChatMessage(Guid id, string fromUserId, string toUserId, string content, DateTime timestamp)
        {
            Id = id;
            FromUserId = fromUserId;
            ToUserId = toUserId;
            Content = content;
            Timestamp = timestamp;
        }

        public Guid Id { get; }
        public string FromUserId { get; }
        public string ToUserId { get; }
        public string Content { get; }
        public DateTime Timestamp { get; }
    }
}
