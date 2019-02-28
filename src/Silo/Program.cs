using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using Serilog;
using Serilog.Events;
using Silo.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Silo
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        private const string EnvironmentVariablePrefix = "ORLEANS_";

        public static Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(configure =>
                {
                    configure.AddJsonFile("hostsettings.json", true, true);
                    configure.AddEnvironmentVariables(EnvironmentVariablePrefix);
                    configure.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hosting, configure) =>
                {
                    configure
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{hosting.HostingEnvironment.EnvironmentName}.json", true, true)
                        .AddEnvironmentVariables(EnvironmentVariablePrefix)
                        .AddCommandLine(args);
                })
                .ConfigureServices((hosting, services) =>
                {
                    // helps discover free ports
                    services.AddSingleton<INetworkPortFinder, NetworkPortFinder>();

                    // add options for the api hosted service
                    services.Configure<SupportApiOptions>(options =>
                    {
                        options.Title = hosting.Configuration.GetValue<string>("Api:Title");
                        options.Port = hosting.Configuration.GetValue<int>("Api:Port");
                    });

                    // add the silo hosted service and the services it makes available
                    services.AddSingleton<SiloHostedService>();
                    services.AddSingleton<IHostedService>(_ => _.GetService<SiloHostedService>());
                    services.AddSingleton(_ => _.GetService<SiloHostedService>().ClusterClient);

                    // add the back-end api service
                    services.AddSingleton<IHostedService, SupportApiHostedService>();
                })
                .ConfigureLogging((hosting, configure) =>
                {
                    configure.AddSerilog(new LoggerConfiguration()
                        .WriteTo.Console(
                            restrictedToMinimumLevel: hosting.Configuration.GetValue<LogEventLevel>("Serilog:Console:RestrictedToMinimumLevel"))
                        .WriteTo.MSSqlServer(
                            connectionString: hosting.Configuration.GetConnectionString("Orleans"),
                            schemaName: hosting.Configuration["Serilog:MSSqlServer:SchemaName"],
                            tableName: hosting.Configuration["Serilog:MSSqlServer:TableName"],
                            restrictedToMinimumLevel: hosting.Configuration.GetValue<LogEventLevel>("Serilog:MSSqlServer:RestrictedToMinimumLevel"))
                        .CreateLogger());
                })
                .UseConsoleLifetime()
                .Build();

            // write the port configuration on the console title
            var silo = host.Services.GetService<SiloHostedService>();
            Console.Title = $"{nameof(Silo)}: Silo: {silo.SiloPort}, Gateway: {silo.GatewayPort}, Dashboard: {silo.DashboardPort}";

            return host.RunAsync();
        }
    }
}