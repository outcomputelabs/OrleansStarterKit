using Grains.Models;
using Orleans;

namespace Grains
{
    /// <summary>
    /// Represents an observer to a channel.
    /// This is useful for temporarily subscribing to real-time channel updates,
    /// for example, from a SignalR connection on a stateless web app.
    /// </summary>
    public interface IChannelObserver : IGrainObserver
    {
        void OnMessage(MessageModel message);
    }
}
