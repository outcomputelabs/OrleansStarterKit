using Grains;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
                    _.ClusterId = configuration.GetValue<string>("Orleans:ClusterId");
                    _.ServiceId = configuration.GetValue<string>("Orleans:ServiceId");
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
            switch (configuration.GetValue<SiloHostedServiceClusteringProvider>("Orleans:Providers:Clustering:Provider"))
            {
                case SiloHostedServiceClusteringProvider.Localhost:
                    builder.UseLocalhostClustering(SiloPort, GatewayPort, null,
                        configuration.GetValue<string>("Orleans:ServiceId"),
                        configuration.GetValue<string>("Orleans:ClusterId"));
                    break;

                case SiloHostedServiceClusteringProvider.AdoNet:
                    builder.UseAdoNetClustering(_ =>
                    {
                        _.ConnectionString = configuration.GetConnectionString(configuration.GetValue<string>("Orleans:Providers:Clustering:AdoNet:ConnectionStringName"));
                        _.Invariant = configuration.GetValue<string>("Orleans:Providers:Clustering:AdoNet:Invariant");
                    });
                    break;
            }

            // configure the reminder service
            switch (configuration.GetValue<SiloHostedServiceReminderProvider>("Orleans:Providers:Reminders:Provider"))
            {
                case SiloHostedServiceReminderProvider.InMemory:
                    builder.UseInMemoryReminderService();
                    break;

                case SiloHostedServiceReminderProvider.AdoNet:
                    builder.UseAdoNetReminderService(_ =>
                    {
                        _.ConnectionString = configuration.GetConnectionString(configuration.GetValue<string>("Orleans:Providers:Reminders:AdoNet:ConnectionStringName"));
                        _.Invariant = configuration.GetValue<string>("Orleans:Providers:Reminders:AdoNet:Invariant");
                    });
                    break;
            }

            // configure the default storage provider
            switch (configuration.GetValue<SiloHostedServiceStorageProvider>("Orleans:Providers:Storage:Default:Provider"))
            {
                case SiloHostedServiceStorageProvider.InMemory:
                    builder.AddMemoryGrainStorageAsDefault();
                    break;

                case SiloHostedServiceStorageProvider.AdoNet:
                    builder.AddAdoNetGrainStorageAsDefault(_ =>
                    {
                        _.ConnectionString = configuration.GetConnectionString(configuration.GetValue<string>("Orleans:Providers:Storage:Default:AdoNet:ConnectionStringName"));
                        _.Invariant = configuration.GetValue<string>("Orleans:Providers:Storage:Default:AdoNet:Invariant");
                        _.UseJsonFormat = configuration.GetValue<bool>("Orleans:Providers:Storage:Default:AdoNet:UseJsonFormat");
                        _.TypeNameHandling = configuration.GetValue<TypeNameHandling>("Orleans:Providers:Storage:Default:AdoNet:TypeNameHandling");
                    });
                    break;
            }

            // configure the storage provider for pubsub
            switch (configuration.GetValue<SiloHostedServiceStorageProvider>("Orleans:Providers:Storage:PubSub:Provider"))
            {
                case SiloHostedServiceStorageProvider.InMemory:
                    builder.AddMemoryGrainStorage(PubSubStorageProviderName);
                    break;

                case SiloHostedServiceStorageProvider.AdoNet:
                    builder.AddAdoNetGrainStorage(PubSubStorageProviderName, _ =>
                    {
                        _.ConnectionString = configuration.GetConnectionString(configuration.GetValue<string>("Orleans:Providers:Storage:PubSub:AdoNet:ConnectionStringName"));
                        _.Invariant = configuration.GetValue<string>("Orleans:Providers:Storage:PubSub:AdoNet:Invariant");
                        _.UseJsonFormat = configuration.GetValue<bool>("Orleans:Providers:Storage:PubSub:AdoNet:UseJsonFormat"); ;
                    });
                    break;
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