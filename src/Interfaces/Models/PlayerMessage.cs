using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class PlayerMessage : Message
    {
        public PlayerMessage(Guid id, DateTime timestamp, string from, string to, string content)
            : base(id, timestamp)
        {
            From = from;
            To = to;
            Content = content;
        }

        public string From { get; }
        public string To { get; }
        public string Content { get; }
    }
}
