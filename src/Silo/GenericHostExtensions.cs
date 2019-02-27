using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Hosting;

namespace Silo
{
    public static class GenericHostExtensions
    {
        public static IHostBuilder UseHostedService<T>(this IHostBuilder hostBuilder)
            where T : class, IHostedService
        {
            return hostBuilder.ConfigureServices(services =>
            {
                services.AddHostedService<T>();
            });
        }
    }
}
