using Client.Console.Tests.Fakes;
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
            var service = new ConsoleClientHostedService(new FakeClusterClient());

            await service.StartAsync(default(CancellationToken));
            await service.StopAsync(default(CancellationToken));
        }
    }
}
