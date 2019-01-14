using Grains;
using Grains.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private static string player;

        static void Main()
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            var provider = services.BuildServiceProvider();

            var logger = provider.GetRequiredService<ILogger<Program>>();

            var client = new ClientBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = config.GetValue<string>("Orleans:ClusterId");
                    options.ServiceId = config.GetValue<string>("Orleans:ServiceId");
                })
                .UseAdoNetClustering(options =>
                {
                    options.ConnectionString = config.GetConnectionString("Orleans");
                    options.Invariant = config.GetValue<string>("Orleans:AdoNet:Invariant");
                })
                .ConfigureLogging(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Warning);
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
                    if ((match = Regex.Match(command, @"^/player (?<player>\w+)$")).Success)
                    {
                        player = match.Groups["player"].Value;
                        Console.WriteLine($"Current player is now [{player}]");
                    }
                    else if ((match = Regex.Match(command, @"^/tell (?<other>\w+) (?<content>.+)")).Success)
                    {
                        var other = match.Groups["other"].Value;
                        var content = match.Groups["content"].Value;
                        var message = new Message(client.GetGrain<IPlayer>(player), content, MessageType.Tell);

                        await client.GetGrain<IPlayer>(other).MessageAsync(message);
                    }
                    else if ((match = Regex.Match(command, @"^/messages$")).Success)
                    {
                        var messages = await client.GetGrain<IPlayer>(player).GetMessagesAsync();
                        foreach (var m in messages)
                        {
                            Console.ForegroundColor =
                                m.Type == MessageType.Tell ? ConsoleColor.White :
                                m.Type == MessageType.Party ? ConsoleColor.Yellow :
                                ConsoleColor.Gray;

                            Console.WriteLine($"{m.Timestamp:HH:mm} {m.From}: {m.Content}");
                            Console.ResetColor();
                        }
                    }
                    else if ((match = Regex.Match(command, @"^/invite (?<other>\w+)")).Success)
                    {
                        var other = match.Groups["other"].Value;
                        var result = await client.GetGrain<IPlayer>(player).InviteAsync(client.GetGrain<IPlayer>(other));

                        //await client.GetGrain<IPlayer>(player).InviteAsync(client.GetGrain<IPlayer>(other));
                        Console.WriteLine($"Invited player [{other}] to the party.");
                    }
                    else if ((match = Regex.Match(command, @"^/quit$")).Success)
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
    }
}