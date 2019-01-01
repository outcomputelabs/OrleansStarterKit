using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class TellMessage
    {
        public TellMessage(Guid id, string from, string to, string content, DateTime timestamp)
        {
            Id = id;
            From = from;
            To = to;
            Content = content;
            Timestamp = timestamp;
        }

        public Guid Id { get; }
        public string From { get; }
        public string To { get; }
        public string Content { get; }
        public DateTime Timestamp { get; }
    }
}
