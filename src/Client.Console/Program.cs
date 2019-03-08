using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using Serilog;
using Serilog.Events;
using System.Threading;
using System.Threading.Tasks;
using EnvironmentName = Microsoft.Extensions.Hosting.EnvironmentName;

namespace Client.Console
{
    public static class Program
    {
        private const string EnvironmentVariablePrefix = "ORLEANS_";

        /// <summary>
        /// For testing only.
        /// </summary>
        public static CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();

        /// <summary>
        /// For testing only.
        /// </summary>
        public static TaskCompletionSource<bool> Started { get; set; } = new TaskCompletionSource<bool>();

        /// <summary>
        /// Exposes the host to test code.
        /// </summary>
        public static IHost Host { get; private set; }

        public static async Task Main(string[] args)
        {
            Host = new HostBuilder()
                .UseEnvironment(EnvironmentName.Development)
                .ConfigureHostConfiguration(configure =>
                {
                    configure.AddJsonFile("hostsettings.json", true, true);
                    configure.AddEnvironmentVariables(EnvironmentVariablePrefix);
                    configure.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hosting, configure) =>
                {
                    configure
                        .AddJsonFile("appsettings.shared.json", true, true)
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{hosting.HostingEnvironment.EnvironmentName}.json", true, true)
                        .AddEnvironmentVariables(EnvironmentVariablePrefix)
                        .AddCommandLine(args);
                })
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddSerilog(new LoggerConfiguration()
                        .WriteTo.Console(
                            restrictedToMinimumLevel: context.Configuration.GetValue<LogEventLevel>("Serilog:Console:RestrictedToMinimumLevel"))
                        .WriteTo.MSSqlServer(
                            connectionString: context.Configuration.GetConnectionString("Orleans"),
                            schemaName: context.Configuration["Serilog:MSSqlServer:SchemaName"],
                            tableName: context.Configuration["Serilog:MSSqlServer:TableName"],
                            restrictedToMinimumLevel: context.Configuration.GetValue<LogEventLevel>("Serilog:MSSqlServer:RestrictedToMinimumLevel"))
                        .CreateLogger());
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ClusterClientHostedService>();
                    services.AddSingleton<IHostedService>(_ => _.GetService<ClusterClientHostedService>());
                    services.AddSingleton(_ => _.GetService<ClusterClientHostedService>().ClusterClient);

                    services.AddSingleton<ConsoleClientHostedService>();
                    services.AddSingleton<IHostedService>(_ => _.GetService<ConsoleClientHostedService>());
                })
                .UseConsoleLifetime()
                .Build();

            await Host.StartAsync(CancellationTokenSource.Token);

            Started.TrySetResult(true);

            await Host.WaitForShutdownAsync(CancellationTokenSource.Token);
        }
    }
}