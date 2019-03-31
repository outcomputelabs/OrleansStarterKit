using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class Message
    {
        public Message(Guid id, Guid senderId, string senderHandle, string senderName, Guid receiverId, string content, DateTime timestamp)
        {
            Id = id;
            SenderId = senderId;
            SenderHandle = senderHandle;
            SenderName = senderName;
            ReceiverId = receiverId;
            Content = content;
            Timestamp = timestamp;
        }

        public Guid Id { get; }
        public Guid SenderId { get; }
        public string SenderHandle { get; }
        public string SenderName { get; }
        public Guid ReceiverId { get; }
        public string Content { get; }
        public DateTime Timestamp { get; }
    }
}
