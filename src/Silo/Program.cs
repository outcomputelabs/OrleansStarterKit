using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Silo
{
    public static class Program
    {
        private const string EnvironmentVariablePrefix = "ORLEANS_";

        /// <summary>
        /// For unit testing - allows test code to stop the program.
        /// </summary>
        private static CancellationToken _cancellationToken = default;

        /// <summary>
        /// For unit testing - notifies awaiters than the host has started.
        /// </summary>
        private static TaskCompletionSource<bool> _startedSource = new TaskCompletionSource<bool>();

        /// <summary>
        /// For unit testing - notifies awaiters than the host has started.
        /// </summary>
        public static Task Started { get; } = _startedSource.Task;

        /// <summary>
        /// Facilitates unit testing by allowing test code to stop execution.
        /// </summary>
        public static Task Main(string[] args, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            return Main(args);
        }

        public static async Task Main(string[] args)
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
            var silo = host.Services.GetService<SiloHostedService>();
            var api = host.Services.GetService<SupportApiHostedService>();
            Console.Title = $"{nameof(IHost)}: Silo: {silo.SiloPort}, Gateway: {silo.GatewayPort}, Dashboard: {silo.DashboardPort}, Api: {api.Port}";

            // start the host now
            await host.StartAsync(_cancellationToken);

            // notify test code that the host has started
            _startedSource.SetResult(true);

            // wait for any shutdown order including from test code
            await host.WaitForShutdownAsync(_cancellationToken);
        }
    }
}