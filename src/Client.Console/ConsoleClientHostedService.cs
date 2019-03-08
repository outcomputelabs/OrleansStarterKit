using Grains;
using Microsoft.Extensions.Hosting;
using Orleans;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Console
{
    public class ConsoleClientHostedService : IHostedService
    {
        public ConsoleClientHostedService(IClusterClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        private readonly IClusterClient _client;
        private Task _consoleTask;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _consoleTask = Task.Run(async () =>
            {
                await _client.GetGrain<ITestGrain>(Guid.NewGuid()).GetKeyAsync();
            });
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
