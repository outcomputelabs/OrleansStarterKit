using Orleans;
using System.Threading.Tasks;

namespace Grains
{
    public interface IReceiverGrain : IGrainWithStringKey
    {
        Task StartAsync();
    }
}
