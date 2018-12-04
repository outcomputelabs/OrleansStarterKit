using Orleans.Concurrency;
using System;

namespace Interfaces.Models
{
    [Immutable]
    public class ChannelInfo
    {
        public ChannelInfo(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }
        public string Name { get; }
    }
}
