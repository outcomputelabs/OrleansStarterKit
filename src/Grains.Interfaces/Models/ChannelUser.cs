using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class ChannelUser
    {
        public ChannelUser(Guid channelId, Guid userId)
        {
            ChannelId = channelId;
            UserId = userId;
        }

        public Guid ChannelId { get; }
        public Guid UserId { get; }
    }
}
