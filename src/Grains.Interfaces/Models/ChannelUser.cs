using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class ChannelUser : IEquatable<ChannelUser>
    {
        public ChannelUser(Guid channelId, Guid userId)
        {
            ChannelId = channelId;
            UserId = userId;
        }

        public Guid ChannelId { get; }
        public Guid UserId { get; }

        #region Equatable

        public bool Equals(ChannelUser other)
        {
            return ChannelId == other.ChannelId
                && UserId == other.UserId;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(ChannelUser)) return false;
            return Equals((ChannelUser)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ChannelId, UserId);
        }

        #endregion
    }
}
