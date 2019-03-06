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

namespace Client.Console
{
    public class ClusterClientHostedService : IHostedService
    {
        public ClusterClientHostedService(IConfiguration configuration, ILoggerProvider loggerProvider)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _loggerProvider = loggerProvider ?? throw new ArgumentNullException(nameof(loggerProvider));
            _logger = _loggerProvider.CreateLogger(GetType().FullName);

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
        }

        private readonly IConfiguration _configuration;
        private readonly ILoggerProvider _loggerProvider;
        private readonly ILogger _logger;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Connecting...");

            var maxRetries = _configuration.GetValue<int>("Orleans:Client:Connect:MaxRetries");
            var retryDelay = _configuration.GetValue<TimeSpan>("Orleans:Client:Connect:RetryDelay");
            await ClusterClient.Connect(async error =>
            {
                _logger.LogError(error, "Error Connecting: {@Message}", error.Message);
                if (--maxRetries < 0) return false;
                await Task.Delay(retryDelay, cancellationToken);
                return true;
            });

            _logger.LogInformation("Connected.");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Disconnecting...");

            await ClusterClient.Close();

            _logger.LogInformation("Disconnected.");
        }

        public IClusterClient ClusterClient { get; private set; }
    }
}
