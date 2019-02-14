using Orleans;
using System.Threading.Tasks;

namespace Grains
{
    public interface ISenderGrain : IGrainWithGuidKey
    {
        Task StartAsync();
    }
}
