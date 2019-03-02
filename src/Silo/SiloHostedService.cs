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
using System.Linq;

namespace Silo
{
    public class SiloHostedService : IHostedService
    {
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
            _host = new SiloHostBuilder()
                .ConfigureEndpoints(SiloPort, GatewayPort)
                .ConfigureApplicationParts(_ =>
                {
                    _.AddApplicationPart(typeof(ChatUser).Assembly).WithReferences();
                })
                .ConfigureLogging(_ =>
                {
                    _.AddProvider(loggerProvider);
                })
                .UseAdoNetClustering(_ =>
                {
                    _.ConnectionString = options.Value.AdoNetConnectionString;
                    _.Invariant = options.Value.AdoNetInvariant;
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
                .UseAdoNetReminderService(_ =>
                {
                    _.ConnectionString = options.Value.AdoNetConnectionString;
                    _.Invariant = options.Value.AdoNetInvariant;
                })
                .AddAdoNetGrainStorageAsDefault(_ =>
                {
                    _.ConnectionString = options.Value.AdoNetConnectionString;
                    _.Invariant = options.Value.AdoNetInvariant;
                    _.UseJsonFormat = true;
                    _.TypeNameHandling = TypeNameHandling.None;
                })
                .AddSimpleMessageStreamProvider("SMS")
                .AddAdoNetGrainStorage("PubSubStore", _ =>
                {
                    _.ConnectionString = options.Value.AdoNetConnectionString;
                    _.Invariant = options.Value.AdoNetInvariant;
                    _.UseJsonFormat = true;
                })
                .UseDashboard(_ =>
                {
                    _.HostSelf = true;
                    _.Port = DashboardPort;
                })
                .EnableDirectClient()
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