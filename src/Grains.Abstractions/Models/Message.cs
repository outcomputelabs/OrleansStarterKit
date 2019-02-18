using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class Message
    {
        public Message(string fromUserId, string toUserId, string content)
        {
            FromUserId = fromUserId;
            ToUserId = toUserId;
            Content = content;
        }

        public Guid Id { get; } = Guid.NewGuid();
        public string FromUserId { get; }
        public string ToUserId { get; }
        public string Content { get; }
        public DateTime Timestamp { get; } = DateTime.UtcNow;
    }
}
