using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class Message
    {
        public Message(Guid id, DateTime timestamp)
        {
            Id = id;
            Timestamp = timestamp;
        }

        public Guid Id { get; }
        public DateTime Timestamp { get; }
    }
}
