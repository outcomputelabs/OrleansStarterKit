using Grains;
using Orleans;
using Orleans.Hosting;
using System;

namespace Client.Tests
{
    public class ClusterFixture : IDisposable
    {
        private readonly ISiloHost _host;

        public ClusterFixture()
        {
            _host = new SiloHostBuilder()
                .ConfigureApplicationParts(_ =>
                {
                    _.AddApplicationPart(typeof(TestGrain).Assembly).WithReferences();
                })
                .UseLocalhostClustering()
                .Build();

            _host.StartAsync().Wait();
        }

        public void Dispose()
        {
            _host.StopAsync().Wait();
        }
    }
}
