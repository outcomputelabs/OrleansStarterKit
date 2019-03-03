using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace Silo
{
    public static class Program
    {
        private const string EnvironmentVariablePrefix = "ORLEANS_";

        /// <summary>
        /// For unit testing - notifies awaiters than the host has started.
        /// </summary>
        private static TaskCompletionSource<bool> _startedSource = new TaskCompletionSource<bool>();

        /// <summary>
        /// For unit testing - notifies awaiters than the host has started.
        /// </summary>
        public static Task Started => _startedSource.Task;

        /// <summary>
        /// For unit testing - allows access to the host.
        /// </summary>
        public static IHost Host { get; private set; }

        public static async Task Main(string[] args)
        {
            Host = new HostBuilder()
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

                    // add the silo hosted service and the services it makes available
                    services.AddSingleton<SiloHostedService>();
                    services.AddSingleton<IHostedService>(_ => _.GetService<SiloHostedService>());
                    services.AddSingleton(_ => _.GetService<SiloHostedService>().ClusterClient);

                    // add the back-end api service
                    services.AddSingleton<SupportApiHostedService>();
                    services.AddSingleton<IHostedService>(_ => _.GetService<SupportApiHostedService>());
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
            var silo = Host.Services.GetService<SiloHostedService>();
            var api = Host.Services.GetService<SupportApiHostedService>();
            Console.Title = $"{nameof(Silo)}: Silo: {silo.SiloPort}, Gateway: {silo.GatewayPort}, Dashboard: {silo.DashboardPort}, Api: {api.Port}";

            // start the host now
            await Host.StartAsync();

            // notify test code that the host has started
            _startedSource.SetResult(true);

            // wait for any shutdown order including from test code
            await Host.WaitForShutdownAsync();

            // reset the started flag for the next test
            _startedSource = new TaskCompletionSource<bool>();
        }
    }
}