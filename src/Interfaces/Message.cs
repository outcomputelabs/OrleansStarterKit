using Orleans.Concurrency;
using System;

namespace Interfaces
{
    [Immutable]
    public class Message
    {
        public Message(Guid id, string content, DateTime timestamp, string publisherHandle)
        {
            Id = id;
            Content = content;
            Timestamp = timestamp;
            PublisherHandle = publisherHandle;
        }

        public Guid Id { get; }

        public string Content { get; }

        public DateTime Timestamp { get; }

        public string PublisherHandle { get; }
    }
}
