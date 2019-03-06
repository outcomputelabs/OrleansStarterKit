using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using Serilog;
using Serilog.Events;
using System.Threading.Tasks;

namespace Client.Console
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

                    services.AddSingleton<IHostedService, ConsoleClientHostedService>();
                })
                .RunConsoleAsync();

            /*
            // build the client
            

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
    }
}