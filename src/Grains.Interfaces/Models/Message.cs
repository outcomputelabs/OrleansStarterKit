using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class Message : IEquatable<Message>
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

        public bool Equals(Message other)
        {
            return Id == other.Id
                && SenderId == other.SenderId
                && SenderHandle == other.SenderHandle
                && SenderName == other.SenderName
                && ReceiverId == other.ReceiverId
                && Content == other.Content
                && Timestamp == other.Timestamp;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Message)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals((Message)obj);
        }

        public override int GetHashCode() => HashCode.Combine(Id, SenderId, SenderHandle, SenderName, ReceiverId, Content, Timestamp);

        public override string ToString() => $"Id = {Id}, SenderId = {SenderId}, Content = '{Content}'";
    }
}
