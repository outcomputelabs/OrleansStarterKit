using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class UserInfo
    {
        public UserInfo(Guid id, string handle, string name)
        {
            Id = id;
            Handle = handle;
            Name = name;
        }

        public Guid Id { get; }
        public string Handle { get; }
        public string Name { get; }
    }
}
