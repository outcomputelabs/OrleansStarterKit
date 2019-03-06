using Core;
using Grains;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

namespace Silo
{
    public class SiloHostedService : IHostedService
    {
        private const string SimpleMessageStreamProviderName = "SMS";

        private readonly ISiloHost _host;

        public SiloHostedService(IConfiguration configuration, ILoggerProvider loggerProvider, INetworkPortFinder portFinder, IHostingEnvironment environment)
        {
            // validate
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (loggerProvider == null) throw new ArgumentNullException(nameof(loggerProvider));
            if (portFinder == null) throw new ArgumentNullException(nameof(portFinder));
            if (environment == null) throw new ArgumentNullException(nameof(environment));

            // get desired port configuration
            SiloPort = portFinder.GetAvailablePortFrom(
                configuration.GetValue<int>("Orleans:Ports:Silo:Start"),
                configuration.GetValue<int>("Orleans:Ports:Silo:End"));

            GatewayPort = portFinder.GetAvailablePortFrom(
                configuration.GetValue<int>("Orleans:Ports:Gateway:Start"),
                configuration.GetValue<int>("Orleans:Ports:Gateway:End"));

            DashboardPort = portFinder.GetAvailablePortFrom(
                configuration.GetValue<int>("Orleans:Ports:Dashboard:Start"),
                configuration.GetValue<int>("Orleans:Ports:Dashboard:End"));

            // configure the silo host
            _host = new SiloHostBuilder()

                .ConfigureApplicationParts(_ =>
                {
                    _.AddApplicationPart(typeof(ChatUser).Assembly).WithReferences();
                })
                .ConfigureLogging(_ =>
                {
                    _.AddProvider(loggerProvider);
                })
                .Configure<ClusterMembershipOptions>(_ =>
                {
                    if (environment.IsDevelopment())
                    {
                        _.ValidateInitialConnectivity = false;
                    }
                })
                .AddSimpleMessageStreamProvider(SimpleMessageStreamProviderName)
                .UseDashboard(_ =>
                {
                    _.HostSelf = true;
                    _.Port = DashboardPort;
                })
                .EnableDirectClient()

                // configure the clustering provider
                .TryUseLocalhostClustering(configuration, SiloPort, GatewayPort)
                .TryUseAdoNetClustering(configuration, SiloPort, GatewayPort)

                // configure the reminder service
                .TryUseInMemoryReminderService(configuration)
                .TryUseAdoNetReminderService(configuration)

                // configure the default storage provider
                .TryAddMemoryGrainStorageAsDefault(configuration)
                .TryAddAdoNetGrainStorageAsDefault(configuration)

                // configure the storage provider for pubsub
                .TryAddMemoryGrainStorageForPubSub(configuration)
                .TryAddAdoNetGrainStorageForPubSub(configuration)

                // done
                .Build();
        }

        public IClusterClient ClusterClient => _host.Services.GetService<IClusterClient>();

        public int SiloPort { get; private set; }

        public int GatewayPort { get; private set; }

        public int DashboardPort { get; private set; }

        public Task StartAsync(CancellationToken cancellationToken) => _host.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) => _host.StopAsync(cancellationToken);
    }
}