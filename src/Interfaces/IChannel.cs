using Orleans;

namespace Grains
{
    /// <summary>
    /// Represents a chat channel.
    /// </summary>
    public interface IChannel : IGrainWithStringKey
    {
    }
}
