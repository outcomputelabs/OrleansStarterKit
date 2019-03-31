using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class Message
    {
        public Message(Guid id, Guid publisherId, string publisherHandle, string publisherName, string content, DateTime timestamp)
        {
            Id = id;
            PublisherId = publisherId;
            PublisherHandle = publisherHandle;
            PublisherName = publisherName;
            Content = content;
            Timestamp = timestamp;
        }

        public Guid Id { get; }
        public Guid PublisherId { get; }
        public string PublisherHandle { get; }
        public string PublisherName { get; }
        public string Content { get; }
        public DateTime Timestamp { get; }
    }
}
