using Grains;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Threading;
using System.Threading.Tasks;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

namespace Silo
{
    /// <summary>
    /// Facilitates integration of an Orleans Silo Host as a <see cref="IHostedService"/> via its <see cref="ISiloHostBuilder"/>
    /// </summary>
    public class SiloHostedService : IHostedService
    {
        private readonly ISiloHost _host;

        public SiloHostedService(ILoggerProvider loggerProvider, INetworkHelper networkHelper, IConfiguration configuration, IHostingEnvironment environment)
        {
            // test for open ports
            var ports = networkHelper.GetAvailablePorts(3);

            // get desired port configuration
            var siloPort = configuration.GetValue("Orleans:Ports:Silo", 0).ValueIf(0, ports[0]);
            var gatewayPort = configuration.GetValue("Orleans:Ports:Gateway", 0).ValueIf(0, ports[1]);
            var dashboardPort = configuration.GetValue("Orleans:Ports:Dashboard", 0).ValueIf(0, ports[2]);

            // configure the silo host
            _host = new SiloHostBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = configuration["Orleans:ClusterId"];
                    options.ServiceId = configuration["Orleans:ServiceId"];
                })
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

        public Task StartAsync(CancellationToken cancellationToken) => _host.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) => _host.StopAsync(cancellationToken);
    }
}
