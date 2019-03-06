using Grains;
using Orleans;
using Orleans.Hosting;
using System;

namespace Client.Console.Tests
{
    public class ClusterFixture : IDisposable
    {
        public ISiloHost Host { get; private set; }

        public ClusterFixture()
        {
            Host = new SiloHostBuilder()
                .ConfigureApplicationParts(_ =>
                {
                    _.AddApplicationPart(typeof(TestGrain).Assembly).WithReferences();
                })
                .UseLocalhostClustering()
                .Build();

            Host.StartAsync().Wait();
        }

        public void Dispose()
        {
            Host.StopAsync().Wait();
        }
    }
}
