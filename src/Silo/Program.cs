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
            // configure services
            var provider = ConfigureServiceProvider();

            // configure orleans
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "MyCluster";
                    options.ServiceId = "MyService";
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
                    config.AddProvider(provider.GetRequiredService<ILoggerProvider>());
                });

            // start orleans
            using (var host = builder.Build())
            {
                await host.StartAsync();
                await host.Stopped;
            }
        }

        private static IServiceProvider ConfigureServiceProvider()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            return services.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // configure serilog
            services.AddLogging(config => config.AddSerilog(new LoggerConfiguration()
                .WriteTo.Console(LogEventLevel.Information)
                .WriteTo.MSSqlServer("Logging", "Logs", LogEventLevel.Information)
                .CreateLogger()));
        }
    }
}
