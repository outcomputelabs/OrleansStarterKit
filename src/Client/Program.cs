using Grains;
using Grains.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Client
{
    internal class Program
    {
        private static string userId;

        private static async Task Main()
        {
            // build the configuration provider
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // set the window title
            Console.Title = configuration.GetValue("Console:Title", nameof(ISiloHost));

            // configure services
            var services = ConfigureServices(configuration);

            // grab a logger
            var logger = services.GetRequiredService<ILogger<Program>>();

            // build the client
            var client = new ClientBuilder()
                .ConfigureHostConfiguration(action =>
                {
                    action.AddJsonFile("appsettings.json", false);
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = configuration.GetValue<string>("Orleans:ClusterId");
                    options.ServiceId = configuration.GetValue<string>("Orleans:ServiceId");
                })
                .UseAdoNetClustering(options =>
                {
                    options.ConnectionString = configuration.GetConnectionString("Orleans");
                    options.Invariant = configuration.GetValue<string>("Orleans:AdoNet:Invariant");
                })
                .ConfigureLogging(builder =>
                {
                    builder.AddProvider(services.GetRequiredService<ILoggerProvider>());
                })
                .Build();

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
                    if ((match = Regex.Match(command, @"^user (?<userid>\w+)$")).Success)
                    {
                        userId = match.Groups["userid"].Value;
                        Console.WriteLine($"Current user is now [{userId}]");
                    }
                    else if ((match = Regex.Match(command, @"^tell (?<other>\w+) (?<content>.+)")).Success)
                    {
                        var other = match.Groups["other"].Value;
                        var content = match.Groups["content"].Value;
                        var message = new ChatMessage(Guid.NewGuid(), userId, other, content, DateTime.UtcNow);

                        await client.GetGrain<IChatUser>(other).MessageAsync(message);
                    }
                    else if ((match = Regex.Match(command, @"^messages$")).Success)
                    {
                        var messages = await client.GetGrain<IChatUser>(userId).GetMessagesAsync();
                        foreach (var m in messages)
                        {
                            Console.WriteLine($"{m.Timestamp:HH:mm} {m.FromUserId}: {m.Content}");
                        }
                    }
                    else if ((match = Regex.Match(command, @"^quit$")).Success)
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