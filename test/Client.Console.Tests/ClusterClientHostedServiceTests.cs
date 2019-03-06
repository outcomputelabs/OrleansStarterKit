using Client.Console.Tests.Fakes;
using Grains;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Client.Console.Tests
{
    [Collection(nameof(ClusterCollection))]
    public class ClusterClientHostedServiceTests
    {
        [Fact]
        public void RefusesNullConfiguration()
        {
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                new ClusterClientHostedService(null, new FakeLoggerProvider());
            });
            Assert.Equal("configuration", error.ParamName);
        }

        [Fact]
        public void RefusesNullLoggerProvider()
        {
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                new ClusterClientHostedService(new FakeConfiguration(), null);
            });
            Assert.Equal("loggerProvider", error.ParamName);
        }

        [Fact]
        public void BuildsClusterClient()
        {
            // arrange
            var id = Guid.NewGuid();
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Orleans:Ports:Gateway:Start", "30000" },
                    { "Orleans:Ports:Gateway:End", "30000" },
                    { "Orleans:ClusterId", "dev" },
                    { "Orleans:ServiceId", "dev" },
                    { "Orleans:Providers:Clustering:Provider", "Localhost" }
                })
                .Build();

            // act
            var service = new ClusterClientHostedService(config, new FakeLoggerProvider());

            // assert
            Assert.NotNull(service.ClusterClient);
            Assert.False(service.ClusterClient.IsInitialized);
        }

        [Fact]
        public async Task ConnectsClusterClient()
        {
            // arrange
            var id = Guid.NewGuid();
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Orleans:Ports:Gateway:Start", "30000" },
                    { "Orleans:Ports:Gateway:End", "30000" },
                    { "Orleans:ClusterId", "dev" },
                    { "Orleans:ServiceId", "dev" },
                    { "Orleans:Providers:Clustering:Provider", "Localhost" }
                })
                .Build();

            // act
            var service = new ClusterClientHostedService(config, new FakeLoggerProvider());
            await service.StartAsync(default(CancellationToken));

            // assert
            Assert.NotNull(service.ClusterClient);
            Assert.True(service.ClusterClient.IsInitialized);
            Assert.Equal(id, await service.ClusterClient.GetGrain<ITestGrain>(id).GetKeyAsync());
        }

        [Fact]
        public async Task DisconnectsClusterClient()
        {
            // arrange
            var id = Guid.NewGuid();
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Orleans:Ports:Gateway:Start", "30000" },
                    { "Orleans:Ports:Gateway:End", "30000" },
                    { "Orleans:ClusterId", "dev" },
                    { "Orleans:ServiceId", "dev" },
                    { "Orleans:Providers:Clustering:Provider", "Localhost" }
                })
                .Build();

            // act
            var service = new ClusterClientHostedService(config, new FakeLoggerProvider());
            await service.StartAsync(default(CancellationToken));
            await service.StopAsync(default(CancellationToken));

            // assert
            Assert.False(service.ClusterClient.IsInitialized);
            await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
            {
                await service.ClusterClient.GetGrain<ITestGrain>(Guid.Empty).GetKeyAsync();
            });
        }
    }
}
