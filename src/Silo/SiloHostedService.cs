using Grains;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Silo.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

namespace Silo
{
    public class SiloHostedService : IHostedService
    {
        private const string PubSubStorageProviderName = "PubSubStore";
        private const string SimpleMessageStreamProviderName = "SMS";

        private readonly ISiloHost _host;

        public SiloHostedService(IOptions<SiloHostedServiceOptions> options, ILoggerProvider loggerProvider, INetworkPortFinder portFinder, IHostingEnvironment environment)
        {
            // validate
            if (options?.Value == null) throw new ArgumentNullException(nameof(options));
            if (loggerProvider == null) throw new ArgumentNullException(nameof(loggerProvider));
            if (portFinder == null) throw new ArgumentNullException(nameof(portFinder));
            if (environment == null) throw new ArgumentNullException(nameof(environment));

            // get desired port configuration
            SiloPort = portFinder.GetAvailablePortFrom(options.Value.SiloPortRange.Start, options.Value.SiloPortRange.End);
            GatewayPort = portFinder.GetAvailablePortFrom(options.Value.GatewayPortRange.Start, options.Value.GatewayPortRange.End);
            DashboardPort = portFinder.GetAvailablePortFrom(options.Value.DashboardPortRange.Start, options.Value.DashboardPortRange.End);

            // configure the silo host
            var builder = new SiloHostBuilder();

            // configure shared options
            builder
                .ConfigureEndpoints(SiloPort, GatewayPort)
                .ConfigureApplicationParts(_ =>
                {
                    _.AddApplicationPart(typeof(ChatUser).Assembly).WithReferences();
                })
                .ConfigureLogging(_ =>
                {
                    _.AddProvider(loggerProvider);
                })
                .Configure<ClusterOptions>(_ =>
                {
                    _.ClusterId = options.Value.ClusterId;
                    _.ServiceId = options.Value.ServiceId;
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
                .EnableDirectClient();

            // configure the clustering provider
            switch (options.Value.ClusteringProvider)
            {
                case SiloHostedServiceClusteringProvider.Localhost:
                    builder.UseLocalhostClustering(SiloPort, GatewayPort, null, options.Value.ServiceId, options.Value.ClusterId);
                    break;

                case SiloHostedServiceClusteringProvider.AdoNet:
                    builder.UseAdoNetClustering(_ =>
                    {
                        _.ConnectionString = options.Value.AdoNetClusteringConnectionString;
                        _.Invariant = options.Value.AdoNetClusteringInvariant;
                    });
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(options.Value.ClusteringProvider));
            }

            // configure the reminder service
            switch (options.Value.ReminderProvider)
            {
                case SiloHostedServiceReminderProvider.InMemory:
                    builder.UseInMemoryReminderService();
                    break;

                case SiloHostedServiceReminderProvider.AdoNet:
                    builder.UseAdoNetReminderService(_ =>
                    {
                        _.ConnectionString = options.Value.AdoNetConnectionString;
                        _.Invariant = options.Value.AdoNetInvariant;
                    });
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(options.Value.ReminderProvider));
            }

            // configure the default storage provider
            switch (options.Value.DefaultStorageProvider)
            {
                case SiloHostedServiceStorageProvider.InMemory:
                    builder.AddMemoryGrainStorageAsDefault();
                    break;

                case SiloHostedServiceStorageProvider.AdoNet:
                    builder.AddAdoNetGrainStorageAsDefault(_ =>
                    {
                        _.ConnectionString = options.Value.AdoNetConnectionString;
                        _.Invariant = options.Value.AdoNetInvariant;
                        _.UseJsonFormat = true;
                        _.TypeNameHandling = TypeNameHandling.None;
                    });
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(options.Value.DefaultStorageProvider));
            }

            // configure the storage provider for pubsub
            switch (options.Value.PubSubStorageProvider)
            {
                case SiloHostedServiceStorageProvider.InMemory:
                    builder.AddMemoryGrainStorage(PubSubStorageProviderName);
                    break;

                case SiloHostedServiceStorageProvider.AdoNet:
                    builder.AddAdoNetGrainStorage(PubSubStorageProviderName, _ =>
                    {
                        _.ConnectionString = options.Value.AdoNetConnectionString;
                        _.Invariant = options.Value.AdoNetInvariant;
                        _.UseJsonFormat = true;
                    });
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(options.Value.PubSubStorageProvider));
            }

            // done
            _host = builder.Build();
        }

        public IClusterClient ClusterClient => _host.Services.GetService<IClusterClient>();

        public int SiloPort { get; private set; }

        public int GatewayPort { get; private set; }

        public int DashboardPort { get; private set; }

        public Task StartAsync(CancellationToken cancellationToken) => _host.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) => _host.StopAsync(cancellationToken);
    }
}