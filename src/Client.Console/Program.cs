using Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Console
{
    public static class Program
    {
        /// <summary>
        /// For testing only.
        /// </summary>
        public static CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();

        /// <summary>
        /// For testing only.
        /// </summary>
        public static TaskCompletionSource<bool> Started { get; set; } = new TaskCompletionSource<bool>();

        /// <summary>
        /// Exposes the host to test code.
        /// </summary>
        public static IHost Host { get; private set; }

        public static async Task Main(string[] args)
        {
            Host = new HostBuilder()
                .UseSharedConfiguration(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ClusterClientHostedService>();
                    services.AddSingleton<IHostedService>(_ => _.GetService<ClusterClientHostedService>());
                    services.AddSingleton(_ => _.GetService<ClusterClientHostedService>().ClusterClient);

                    services.AddSingleton<ConsoleClientHostedService>();
                    services.AddSingleton<IHostedService>(_ => _.GetService<ConsoleClientHostedService>());
                })
                .UseConsoleLifetime()
                .Build();

            await Host.StartAsync(CancellationTokenSource.Token);

            Started.TrySetResult(true);

            await Host.WaitForShutdownAsync(CancellationTokenSource.Token);
        }
    }
}