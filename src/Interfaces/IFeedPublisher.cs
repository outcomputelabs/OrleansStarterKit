using Orleans;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IFeedPublisher : IGrainWithStringKey
    {
        Task SubscribeAsync(string handle, IFeedSubscriber follower);
        Task UnsubscribeAsync(string handle);
    }
}
