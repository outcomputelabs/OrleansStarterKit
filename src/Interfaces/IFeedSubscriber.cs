using Orleans;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IFeedSubscriber : IGrainWithStringKey
    {
        Task OnMessageAsync(Message message);
    }
}
