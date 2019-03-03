using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Silo.Tests
{
    [Collection(nameof(SequentialCollection))]
    public class SiloProgramTests
    {
        [Fact]
        public async Task Cancels_Startup()
        {
            var args = new[]
            {
                "/Orleans:Providers:Clustering:Provider=Localhost",
                "/Orleans:Providers:Reminders:Provider=InMemory",
                "/Orleans:Providers:Storage:Default:Provider=InMemory",
                "/Orleans:Providers:Storage:PubSub:Provider=InMemory"
            };

            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await Program.MainForTesting(args, new CancellationToken(true));
            });
        }

        [Fact]
        public async Task Starts_And_Stops()
        {
            var args = new[]
            {
                "/Orleans:Providers:Clustering:Provider=Localhost",
                "/Orleans:Providers:Reminders:Provider=InMemory",
                "/Orleans:Providers:Storage:Default:Provider=InMemory",
                "/Orleans:Providers:Storage:PubSub:Provider=InMemory"
            };

            var task = Program.MainForTesting(args, new CancellationToken());
            await Task.Delay(1000);
            await Program.Host.StopAsync();
            await task;
        }
    }
}
