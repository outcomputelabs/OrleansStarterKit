using Microsoft.Extensions.Hosting;
using Orleans;

namespace Silo
{
    public interface ISiloHostedService : IHostedService
    {
        IClusterClient ClusterClient { get; }
    }
}
