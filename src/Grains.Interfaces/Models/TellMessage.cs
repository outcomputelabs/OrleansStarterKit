using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class TellMessage
    {
        public TellMessage(Guid id, string fromHandle, string fromName, string content, DateTime timestamp)
        {
            Id = id;
            FromHandle = fromHandle;
            FromName = fromName;
            Content = content;
            Timestamp = timestamp;
        }

        public Guid Id { get; }
        public string FromHandle { get; }
        public string FromName { get; }
        public string Content { get; }
        public DateTime Timestamp { get; }
    }
}
