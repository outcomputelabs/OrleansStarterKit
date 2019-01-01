using System;

namespace Grains.Models
{
    public interface IMessage
    {
        Guid Id { get; }
        DateTime Timestamp { get; }
    }
}
