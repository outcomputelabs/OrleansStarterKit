using Orleans;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IFeedPublisher : IGrainWithStringKey
    {
        Task SubscribeAsync(IFeedSubscriber subscriber);
    }
}
