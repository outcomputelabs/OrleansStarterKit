using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class Message
    {
        public Message(Guid id, string content, Guid senderId, string senderHandle, string senderName, DateTime timestamp)
        {
            Id = id;
            Timestamp = timestamp;
        }

        public Guid Id { get; }
        public string Content { get; }
        public string FromHandle { get; }
        public string FromName { get; }
        public DateTime Timestamp { get; }
    }
}
