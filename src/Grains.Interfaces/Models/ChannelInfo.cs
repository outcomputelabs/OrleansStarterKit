using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class ChannelInfo
    {
        public ChannelInfo(Guid id, string handle, string description)
        {
            Id = id;
            Handle = handle;
            Description = description;
        }

        public Guid Id { get; }
        public string Handle { get; }
        public string Description { get; }
    }
}
