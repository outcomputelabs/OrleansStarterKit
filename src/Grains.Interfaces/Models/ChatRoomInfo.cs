using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class ChatRoomInfo : IEquatable<ChatRoomInfo>
    {
        public ChatRoomInfo(Guid key, string name)
        {
            Key = key;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public Guid Key { get; }
        public string Name { get; }

        public bool Equals(ChatRoomInfo other) => other != null && other.Key == Key;

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals((ChatRoomInfo)obj);
        }

        public override int GetHashCode() => (Key.GetHashCode() * 397) ^ Name.GetHashCode();
    }
}
