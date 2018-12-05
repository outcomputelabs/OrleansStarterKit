using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IFeed : IFeedPublisher, IFeedSubscriber
    {
        /// <summary>
        /// Returns the latest messages for this feed.
        /// </summary>
        Task<ImmutableList<Message>> GetLatestMessagesAsync();

        /// <summary>
        /// Publishes a message on this feed.
        /// </summary>
        Task PublishMessageAsync(string content);
    }
}
