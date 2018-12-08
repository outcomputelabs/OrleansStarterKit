using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    /// <summary>
    /// Represents a single message from a given user on a channel.
    /// </summary>
    [Immutable]
    public class MessageModel : IComparable<MessageModel>
    {
        /// <summary>
        /// Creates a new unique message.
        /// </summary>
        /// <param name="id">Unique identifier of the message.</param>
        /// <param name="userName">User who posted the message.</param>
        /// <param name="content">Text content of the message.</param>
        /// <param name="timestamp">UTC timestamp of the message.</param>
        public MessageModel(Guid id, string userName, string content, DateTime timestamp)
        {
            Id = id;
            UserName = userName;
            Content = content;
            Timestamp = timestamp;
        }

        /// <summary>
        /// unique identifier of the message.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// user who posted the message.
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// text content of the message.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// UTC timestamp of the message.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Compares two message models with each other.
        /// </summary>
        /// <returns></returns>
        public int CompareTo(MessageModel other)
        {
            // two message models are the same if their ids are the same
            // otherwise, they order by timestamp
            // in the rare event where the timestamps are the same, they order by unique id alphabetical order
            if (Id == other.Id)
            {
                return 0;
            }
            else
            {
                var byTimestamp = DateTime.Compare(Timestamp, other.Timestamp);
                if (byTimestamp == 0)
                {
                    return Id.ToString("N").CompareTo(other.Id.ToString("N"));
                }
                else
                {
                    return byTimestamp;
                }
            }
        }
    }
}
