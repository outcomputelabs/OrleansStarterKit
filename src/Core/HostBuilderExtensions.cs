using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;

namespace Core
{
    public static class HostBuilderExtensions
    {
        private const string EnvironmentVariablePrefix = "ORLEANS_";

        public static IHostBuilder UseSharedConfiguration(this IHostBuilder builder, string[] args = null)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            return builder
                .UseEnvironment(EnvironmentName.Development)
                .ConfigureHostConfiguration(configure =>
                {
                    configure.AddJsonFile("hostsettings.json", true, true);
                    configure.AddEnvironmentVariables(EnvironmentVariablePrefix);
                    if (args?.Length > 0)
                    {
                        configure.AddCommandLine(args);
                    }
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
                .ConfigureLogging((context, configure) =>
                {
                    configure.AddSerilog(new LoggerConfiguration()
                        .WriteTo.Console(
                            restrictedToMinimumLevel: context.Configuration.GetValue<LogEventLevel>("Serilog:Console:RestrictedToMinimumLevel"))
                        .WriteTo.MSSqlServer(
                            connectionString: context.Configuration.GetConnectionString("Orleans"),
                            schemaName: context.Configuration["Serilog:MSSqlServer:SchemaName"],
                            tableName: context.Configuration["Serilog:MSSqlServer:TableName"],
                            restrictedToMinimumLevel: context.Configuration.GetValue<LogEventLevel>("Serilog:MSSqlServer:RestrictedToMinimumLevel"))
                        .CreateLogger());
                });
        }
    }
}
