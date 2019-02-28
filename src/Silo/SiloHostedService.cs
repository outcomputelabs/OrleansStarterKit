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
            // get desired port configuration
            var siloPort = networkHelper.GetAvailablePortFrom(
                configuration.GetValue<int>("Orleans:Ports:Silo:Start"),
                configuration.GetValue<int>("Orleans:Ports:Silo:Count"));
            var gatewayPort = networkHelper.GetAvailablePortFrom(
                configuration.GetValue<int>("Orleans:Ports:Gateway:Start"),
                configuration.GetValue<int>("Orleans:Ports:Gateway:Count"));
            var dashboardPort = networkHelper.GetAvailablePortFrom(
                configuration.GetValue<int>("Orleans:Ports:Dashboard:Start"),
                configuration.GetValue<int>("Orleans:Ports:Dashboard:Count"));

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
                .UseLocalhostClustering()
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