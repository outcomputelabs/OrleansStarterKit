using Microsoft.Extensions.Hosting;
using Orleans;

namespace Silo.Services
{
    public interface ISiloHostedService : IHostedService
    {
        IClusterClient ClusterClient { get; }
    }
}
