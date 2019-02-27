﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using Serilog;
using Serilog.Events;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Silo
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        private const string EnvironmentVariablePrefix = "ORLEANS_";

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
                    services.AddSingleton<INetworkHelper, NetworkHelper>();
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
                .UseHostedService<SiloHostedService>()
                .Build();

            await host.RunAsync();
        }
    }
}