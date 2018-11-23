using Grains;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Net;
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

            // configure orleans
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = Configuration["Orleans:ClusterId"];
                    options.ServiceId = Configuration["Orleans:ServiceId"];
                })
                .Configure<EndpointOptions>(options =>
                {
                    options.AdvertisedIPAddress = IPAddress.Loopback;
                })
                .ConfigureApplicationParts(config =>
                {
                    config.AddApplicationPart(typeof(User).Assembly).WithReferences();
                })
                .ConfigureLogging(config =>
                {
                    config.AddProvider(services.GetRequiredService<ILoggerProvider>());
                });

            // start orleans
            using (var host = builder.Build())
            {
                await host.StartAsync();
                await host.Stopped;
            }
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
