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
                })
                .Build();

            await client.Connect(async error =>
            {
                await Task.Delay(1000);
                return true;
            });

            while (true)
            {
                var command = Console.ReadLine();

                Match match;

                if ((match = Regex.Match(command, @"^/player (?<player>\w+)$")).Success)
                {
                    player = match.Groups["player"].Value;
                    Console.WriteLine($"Current player is now [{player}]");
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
        }
    }
}