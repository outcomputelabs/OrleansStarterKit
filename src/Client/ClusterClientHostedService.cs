using Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class ClusterClientHostedService : IHostedService
    {
        public ClusterClientHostedService(IConfiguration configuration, ILoggerProvider loggerProvider, ILogger<ClusterClientHostedService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _loggerProvider = loggerProvider ?? throw new ArgumentNullException(nameof(loggerProvider));
        }

        private readonly IConfiguration _configuration;
        private readonly ILoggerProvider _loggerProvider;
        private readonly ILogger<ClusterClientHostedService> _logger;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            ClusterClient = new ClientBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = _configuration.GetValue<string>("Orleans:ClusterId");
                    options.ServiceId = _configuration.GetValue<string>("Orleans:ServiceId");
                })

                .TryUseLocalhostClustering(_configuration)
                .TryUseAdoNetClustering(_configuration)

                .ConfigureLogging(builder =>
                {
                    builder.AddProvider(_loggerProvider);
                })
                .Build();

            _logger.LogInformation("Connecting...");

            var maxRetries = _configuration.GetValue<int>("Client:Connect:MaxRetries");
            var retryDelay = _configuration.GetValue<TimeSpan>("Client:Connect:RetryDelay");
            await ClusterClient.Connect(async error =>
            {
                _logger.LogError(error, "Error Connecting: {@Message}", error.Message);
                if (--maxRetries < 0) return false;
                await Task.Delay(retryDelay, cancellationToken);
                return true;
            });

            _logger.LogInformation("Connected.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public IClusterClient ClusterClient { get; private set; }
    }
}
