using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class PartyMessage : Message
    {
        public PartyMessage(Guid id, DateTime timestamp, string from, Guid to, string content)
            : base(id, timestamp)
        {
            From = from;
            To = to;
            Content = content;
        }

        public string From { get; }
        public Guid To { get; }
        public string Content { get; }
    }
}
