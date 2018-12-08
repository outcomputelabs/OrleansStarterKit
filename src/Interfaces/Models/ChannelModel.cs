using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    /// <summary>
    /// Provides relational information on a channel.
    /// Useful for listing channels on a user interface.
    /// </summary>
    [Immutable]
    public class ChannelModel : IComparable<ChannelModel>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChannelModel"/>
        /// </summary>
        /// <param name="id">Unique identifier of the channel.</param>
        /// <param name="name">Name of the channel.</param>
        /// <param name="creator">User who created the channel.</param>
        /// <param name="timestamp">UTC timestamp for when the channel was created.</param>
        public ChannelModel(Guid id, string name, string creator, DateTime timestamp)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Creator = creator ?? throw new ArgumentNullException(nameof(creator));
            Timestamp = timestamp;
        }

        /// <summary>
        /// Unique identifier of the channel.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Name of the channel.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// User who created the channel.
        /// </summary>
        public string Creator { get; }

        /// <summary>
        /// UTC timestamp for when the channel was created.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Compares two channel models with each other.
        /// </summary>
        /// <returns></returns>
        public int CompareTo(ChannelModel other)
        {
            // two channel models are the same if
            // 1) their unique ids are the same or
            // 2) their names are the same
            // otherwise they will sort by name
            if (Id == other.Id)
            {
                return 0;
            }
            else
            {
                return string.Compare(Name, other.Name, true);
            }
        }
    }
}
