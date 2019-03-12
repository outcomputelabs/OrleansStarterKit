using Moq;
using Orleans;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Client.Console.Tests
{
    public class ConsoleClientHostedServiceTests
    {
        [Fact]
        public void RefusesNullClusterClient()
        {
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                new ConsoleClientHostedService(null);
            });
            Assert.Equal("client", error.ParamName);
        }

        [Fact]
        public async Task StartsAndStops()
        {
            var client = new Mock<IClusterClient>();
            var service = new ConsoleClientHostedService(client.Object);

            await service.StartAsync(default(CancellationToken));
            await service.StopAsync(default(CancellationToken));
        }
    }
}
