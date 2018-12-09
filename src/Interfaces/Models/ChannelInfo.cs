using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    /// <summary>
    /// Provides relational information on a channel.
    /// Useful for listing channels on a user interface.
    /// </summary>
    [Immutable]
    public class ChannelInfo : IComparable<ChannelInfo>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChannelInfo"/>
        /// </summary>
        /// <param name="name">Name of the channel.</param>
        /// <param name="createdBy">User who created the channel.</param>
        /// <param name="timestamp">UTC timestamp for when the channel was created.</param>
        public ChannelInfo(string name, string createdBy, DateTime timestamp)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
            Timestamp = timestamp;
        }

        /// <summary>
        /// Name of the channel.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// User who created the channel.
        /// </summary>
        public string CreatedBy { get; }

        /// <summary>
        /// UTC timestamp for when the channel was created.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Compares two channels with each other.
        /// </summary>
        /// <returns></returns>
        public int CompareTo(ChannelInfo other)
        {
            // two channel are the same if their names are the same
            return string.Compare(Name, other.Name);
        }
    }
}
