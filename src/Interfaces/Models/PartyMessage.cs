using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class PartyMessage : IMessage
    {
        public PartyMessage(Guid id, string from, Guid to, string content, DateTime timestamp)
        {
            Id = id;
            From = from;
            To = to;
            Content = content;
            Timestamp = timestamp;
        }

        public Guid Id { get; }
        public string From { get; }
        public Guid To { get; }
        public string Content { get; }
        public DateTime Timestamp { get; }
    }
}
