using Orleans;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IFeedFollower : IGrainWithStringKey
    {
        Task OnMessageAsync(Message message);
    }
}
