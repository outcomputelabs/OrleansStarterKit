using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class PartyMessage : Message
    {
        public PartyMessage(string from, Guid to, string content)
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
