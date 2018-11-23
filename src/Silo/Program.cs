using Grains;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net;
using Orleans;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Silo
{
    public class Program
    {
        public static void Main()
        {
            MainAsync().Wait();
        }

        public static async Task MainAsync()
        {
            // setup the configuration provider
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // configure services
            var services = ConfigureServices(configuration);

            // configure orleans
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = configuration["Orleans:ClusterId"];
                    options.ServiceId = configuration["Orleans:ServiceId"];
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

        private static IServiceProvider ConfigureServices(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            // configure serilog
            services.AddLogging(config => config.AddSerilog(new LoggerConfiguration()
                .WriteTo.Console(
                    restrictedToMinimumLevel: configuration.GetValue<LogEventLevel>("Serilog:Console:RestrictedToMinimumLevel"))
                .WriteTo.MSSqlServer(
                    connectionString: configuration.GetConnectionString("Logging"),
                    schemaName: configuration.GetValue<string>("Serilog:MSSqlServer:SchemaName"),
                    tableName: configuration.GetValue<string>("Serilog:MSSqlServer:TableName"),
                    restrictedToMinimumLevel: configuration.GetValue<LogEventLevel>("Serilog:MSSqlServer:RestrictedToMinimumLevel"))
                .CreateLogger()));

            // all done
            return services.BuildServiceProvider();
        }
    }
}
