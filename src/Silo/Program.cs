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
        static Program()
        {
            // setup the configuration provider
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        private static IConfiguration Configuration { get; }

        public static void Main()
        {
            MainAsync().Wait();
        }

        public static async Task MainAsync()
        {
            // configure services
            var services = ConfigureServices();

            // configure the silo
            var host = new SiloHostBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = Configuration["Orleans:ClusterId"];
                    options.ServiceId = Configuration["Orleans:ServiceId"];
                })
                .ConfigureEndpoints(
                    Configuration.GetValue<int>("Orleans:Endpoints:SiloPort"),
                    Configuration.GetValue<int>("Orleans:Endpoints:GatewayPort")
                )
                .ConfigureApplicationParts(config =>
                {
                    config.AddApplicationPart(typeof(Lobby).Assembly).WithReferences();
                })
                .ConfigureLogging(config =>
                {
                    config.AddProvider(services.GetRequiredService<ILoggerProvider>());
                })
                .UseAdoNetClustering(options =>
                {
                    options.ConnectionString = Configuration.GetConnectionString("Orleans");
                    options.Invariant = Configuration["Orleans:AdoNet:Invariant"];
                })
                .UseAdoNetReminderService(options =>
                {
                    options.ConnectionString = Configuration.GetConnectionString("Orleans");
                    options.Invariant = Configuration["Orleans:AdoNet:Invariant"];
                })
                .AddAdoNetGrainStorageAsDefault(options =>
                {
                    options.ConnectionString = Configuration.GetConnectionString("Orleans");
                    options.Invariant = Configuration["Orleans:AdoNet:Invariant"];
                    options.UseJsonFormat = true;
                    options.TypeNameHandling = TypeNameHandling.None;
                })
                .AddSimpleMessageStreamProvider("SimpleMessageStreamProvider")
                .AddAdoNetGrainStorage("PubSubStore", options =>
                {
                    options.ConnectionString = Configuration.GetConnectionString("Orleans");
                    options.Invariant = Configuration["Orleans:AdoNet:Invariant"];
                })
                .EnableDirectClient()
                .Build();

            // start the silo
            await host.StartAsync();

            // wait until gracefully stopped
            await host.Stopped;
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // configure serilog
            services.AddLogging(config => config.AddSerilog(new LoggerConfiguration()
                .WriteTo.Console(
                    restrictedToMinimumLevel: Configuration.GetValue<LogEventLevel>("Serilog:Console:RestrictedToMinimumLevel"))
                .WriteTo.MSSqlServer(
                    connectionString: Configuration.GetConnectionString("Logging"),
                    schemaName: Configuration["Serilog:MSSqlServer:SchemaName"],
                    tableName: Configuration["Serilog:MSSqlServer:TableName"],
                    restrictedToMinimumLevel: Configuration.GetValue<LogEventLevel>("Serilog:MSSqlServer:RestrictedToMinimumLevel"))
                .CreateLogger()));

            // all done
            return services.BuildServiceProvider();
        }
    }
}
