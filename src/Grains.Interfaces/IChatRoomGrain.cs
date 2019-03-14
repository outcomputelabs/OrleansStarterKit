using Orleans;

namespace Grains
{
    /// <summary>
    /// Represents a chat room.
    /// A chat room is a well-known conversation with a unique mutable name and unique immutable key.
    /// This allows the chat room to change name over its lifetime while references to it remain consistent.
    /// The grain key is the unique immutable key.
    /// Use the <see cref="IChatRoomRegistryGrain"/> to lookup the immutable key based on the chat room name.
    /// </summary>
    public interface IChatRoomGrain : IGrainWithGuidKey
    {
    }
}
