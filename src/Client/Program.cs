using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace Client
{
    public static class Program
    {
        private static string userId;

        private const string EnvironmentVariablePrefix = "ORLEANS_";

        private static Task Main(string[] args)
        {
            return new HostBuilder()
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
                .ConfigureServices(services =>
                {
                    services.AddHostedService<ClusterClientHostedService>();
                    services.AddSingleton(provider => provider.GetService<ClusterClientHostedService>().ClusterClient);

                    services.AddHostedService<ConsoleClientHostedService>();
                })
                .RunConsoleAsync();

            /*
            // build the client

            Console.WriteLine("Connecting...");

            await client.Connect(async error =>
            {
                await Task.Delay(1000);
                return true;
            });

            Console.WriteLine("Connected.");

            while (true)
            {
                Console.Write("> ");
                var command = Console.ReadLine();

                try
                {
                    Match match;

                    match = Regex.Match(command, @"^user (?<userid>\w+)$");
                    if (match.Success)
                    {
                        userId = match.Groups["userid"].Value;
                        Console.WriteLine($"Current user is now [{userId}]");
                        continue;
                    }

                    match = Regex.Match(command, @"^tell (?<other>\w+) (?<content>.+)");
                    if (match.Success)
                    {
                        var other = match.Groups["other"].Value;
                        var content = match.Groups["content"].Value;
                        var message = new ChatMessage(Guid.NewGuid(), userId, other, content, DateTime.UtcNow);
                        await client.GetGrain<IChatUser>(other).MessageAsync(message);
                        continue;
                    }

                    match = Regex.Match(command, @"^messages$");
                    if (match.Success)
                    {
                        var messages = await client.GetGrain<IChatUser>(userId).GetMessagesAsync();
                        foreach (var m in messages)
                        {
                            Console.WriteLine($"{m.Timestamp:HH:mm} {m.FromUserId}: {m.Content}");
                        }
                        continue;
                    }

                    match = Regex.Match(command, @"^quit$");
                    if (match.Success)
                    {
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid command.");
                    }
                }
                catch (Exception error)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(error.Message);
                    Console.ResetColor();
                }
            }
            */
        }

        private static IServiceProvider ConfigureServices(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            // configure serilog
            services.AddLogging(config => config.AddSerilog(new LoggerConfiguration()
                .WriteTo.Console(
                    restrictedToMinimumLevel: configuration.GetValue<LogEventLevel>("Serilog:Console:RestrictedToMinimumLevel"))
                .WriteTo.MSSqlServer(
                    connectionString: configuration.GetConnectionString("Orleans"),
                    schemaName: configuration["Serilog:MSSqlServer:SchemaName"],
                    tableName: configuration["Serilog:MSSqlServer:TableName"],
                    restrictedToMinimumLevel: configuration.GetValue<LogEventLevel>("Serilog:MSSqlServer:RestrictedToMinimumLevel"))
                .CreateLogger()));

            // all done
            return services.BuildServiceProvider();
        }
    }
}