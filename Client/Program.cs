using Microsoft.Extensions.Configuration;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main()
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

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
                .Build();

            await client.Connect(async error =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error.Message);
                await Task.Delay(1000);
                return true;
            });

            Console.WriteLine("Connected!");
        }
    }
}
