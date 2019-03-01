using Grains;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private readonly ISiloHost _host;

        public SiloHostedService(ILoggerProvider loggerProvider, INetworkPortFinder portFinder, IConfiguration configuration, IHostingEnvironment environment)
        {
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
                .ConfigureEndpoints(SiloPort, GatewayPort)
                .ConfigureApplicationParts(configure =>
                {
                    configure.AddApplicationPart(typeof(ChatUser).Assembly).WithReferences();
                })
                .ConfigureLogging(configure =>
                {
                    configure.AddProvider(loggerProvider);
                })
                .UseAdoNetClustering(options =>
                {
                    options.ConnectionString = configuration.GetConnectionString("Orleans");
                    options.Invariant = configuration["Orleans:AdoNet:Invariant"];
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = configuration["Orleans:ClusterId"];
                    options.ServiceId = configuration["Orleans:ServiceId"];
                })
                .Configure<ClusterMembershipOptions>(options =>
                {
                    if (environment.IsDevelopment())
                    {
                        options.ValidateInitialConnectivity = false;
                    }
                })
                .UseAdoNetReminderService(options =>
                {
                    options.ConnectionString = configuration.GetConnectionString("Orleans");
                    options.Invariant = configuration["Orleans:AdoNet:Invariant"];
                })
                .AddAdoNetGrainStorageAsDefault(options =>
                {
                    options.ConnectionString = configuration.GetConnectionString("Orleans");
                    options.Invariant = configuration["Orleans:AdoNet:Invariant"];
                    options.UseJsonFormat = true;
                    options.TypeNameHandling = TypeNameHandling.None;
                })
                .AddSimpleMessageStreamProvider("SMS")
                .AddAdoNetGrainStorage("PubSubStore", options =>
                {
                    options.ConnectionString = configuration.GetConnectionString("Orleans");
                    options.Invariant = configuration["Orleans:AdoNet:Invariant"];
                    options.UseJsonFormat = true;
                })
                .UseDashboard(options =>
                {
                    options.HostSelf = true;
                    options.Port = DashboardPort;
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