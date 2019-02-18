using Grains;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace Silo
{
    public class Program
    {
        public static async Task Main()
        {
            // build the configuration provider
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // set the window title
            Console.Title = configuration.GetValue("Console:Title", nameof(ISiloHost));

            // configure services
            var services = ConfigureServices(configuration);

            // grab the network helper
            var network = services.GetRequiredService<INetworkHelper>();

            // test for open ports
            var ports = network.GetAvailablePorts(2);

            // get desired port configuration
            var siloPort = configuration.GetValue("Orleans:Endpoints:SiloPort", 0);
            if (siloPort == 0)
            {
                siloPort = ports[0];
            }
            var gatewayPort = configuration.GetValue("Orleans:Endpoints:GatewayPort", 0);
            if (gatewayPort == 0)
            {
                gatewayPort = ports[1];
            }

            // build the silo
            var host = new SiloHostBuilder()
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
                    configure.AddProvider(services.GetRequiredService<ILoggerProvider>());
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
                .EnableDirectClient()
                .Build();

            // start the silo
            await host.StartAsync();

            // wait until gracefully stopped
            await host.Stopped;
        }

        private static IServiceProvider ConfigureServices(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            // configure serilog
            services.AddLogging(config => config.AddSerilog(new LoggerConfiguration()
                .WriteTo.Console(
                    restrictedToMinimumLevel: configuration.GetValue<LogEventLevel>("Serilog:Console:RestrictedToMinimumLevel"))
                .WriteTo.MSSqlServer(
                    connectionString: configuration.GetConnectionString("Orleans"),
                    schemaName: configuration["Serilog:MSSqlServer:SchemaName"],
                    tableName: configuration["Serilog:MSSqlServer:TableName"],
                    restrictedToMinimumLevel: configuration.GetValue<LogEventLevel>("Serilog:MSSqlServer:RestrictedToMinimumLevel"))
                .CreateLogger()));

            // configure the network helper
            services.AddSingleton<INetworkHelper, NetworkHelper>();

            // all done
            return services.BuildServiceProvider();
        }
    }
}
