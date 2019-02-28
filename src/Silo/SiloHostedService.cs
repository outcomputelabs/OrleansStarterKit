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

        public SiloHostedService(ILoggerProvider loggerProvider, INetworkPortFinder networkHelper, IConfiguration configuration, IHostingEnvironment environment)
        {
            // test for open ports
            var ports = networkHelper.GetAvailablePorts(3);

            // get desired port configuration
            var siloPort = configuration.GetValue("Orleans:Ports:Silo", 0).ValueIf(0, ports[0]);
            var gatewayPort = configuration.GetValue("Orleans:Ports:Gateway", 0).ValueIf(0, ports[1]);
            var dashboardPort = configuration.GetValue("Orleans:Ports:Dashboard", 0).ValueIf(0, ports[2]);

            // configure the silo host
            _host = new SiloHostBuilder()
                .ConfigureEndpoints(siloPort, gatewayPort)
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
                    // enable aggressive dead silo removal for development environments
                    if (environment.IsDevelopment())
                    {
                        options.ExpectedClusterSize = 1;
                        options.NumMissedProbesLimit = 1;
                        options.NumVotesForDeathDeclaration = 1;
                        options.ProbeTimeout = TimeSpan.FromSeconds(1);
                        options.TableRefreshTimeout = TimeSpan.FromSeconds(1);
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
                    options.Port = dashboardPort;
                })
                .EnableDirectClient()
                .Build();
        }

        public IClusterClient ClusterClient => _host.Services.GetService<IClusterClient>();

        public Task StartAsync(CancellationToken cancellationToken) => _host.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) => _host.StopAsync(cancellationToken);
    }
}