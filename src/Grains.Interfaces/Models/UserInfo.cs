using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class UserInfo : IEquatable<UserInfo>
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

        public bool Equals(UserInfo other)
        {
            return Id == other.Id
                && Handle == other.Handle
                && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(UserInfo)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals((UserInfo)obj);
        }

        public override int GetHashCode() => HashCode.Combine(Id, Handle, Name);

        public override string ToString() => $"Id = {Id}, Handle = '{Handle}', Name = '{Name}'";
    }
}
