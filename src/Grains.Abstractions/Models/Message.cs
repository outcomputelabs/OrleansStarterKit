using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class Message
    {
        public Message(IPlayer from, string content, MessageType type)
        {
            From = from;
            Content = content;
            Type = type;
        }

        public IPlayer From { get; }
        public string Content { get; }
        public MessageType Type { get; }

        public DateTime Timestamp { get; } = DateTime.UtcNow;
    }

    public enum MessageType
    {
        Tell,
        Party
    }
}
