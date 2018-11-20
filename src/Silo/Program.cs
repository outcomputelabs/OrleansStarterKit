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
            // configure serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(LogEventLevel.Information)
                .CreateLogger();

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
                    config.AddSerilog();
                });

            // start orleans
            using (var host = builder.Build())
            {
                await host.StartAsync();
                await host.Stopped;
            }
        }
    }
}
