using Silo;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    [Collection(nameof(SequentialCollection))]
    public class SiloProgramTests
    {
        [Fact]
        public async Task Starts_And_Stops()
        {
            var parameters = new List<string>
            {
                "/Orleans:Providers:Clustering:Provider=Localhost",
                "/Orleans:Providers:Reminders:Provider=InMemory",
                "/Orleans:Providers:Storage:Default:Provider=InMemory",
                "/Orleans:Providers:Storage:PubSub:Provider=InMemory"
            };

            await Program.StartAsync(parameters.ToArray());
            await Program.StopAsync();
        }

        [Fact]
        public async Task Main_Starts_And_Waits_For_Stop()
        {
            var parameters = new List<string>
            {
                "/Orleans:Providers:Clustering:Provider=Localhost",
                "/Orleans:Providers:Reminders:Provider=InMemory",
                "/Orleans:Providers:Storage:Default:Provider=InMemory",
                "/Orleans:Providers:Storage:PubSub:Provider=InMemory"
            };

            var main = Program.Main(parameters.ToArray());
            await Program.Started;
            await Program.StopAsync();
            await main;
        }

        [Fact]
        public async Task Cancels_Startup()
        {
            var parameters = new List<string>
            {
                "/Orleans:Providers:Clustering:Provider=Localhost",
                "/Orleans:Providers:Reminders:Provider=InMemory",
                "/Orleans:Providers:Storage:Default:Provider=InMemory",
                "/Orleans:Providers:Storage:PubSub:Provider=InMemory"
            };

            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await Program.StartAsync(parameters.ToArray(), new CancellationToken(true));
            });
        }

        [Fact]
        public async Task Cancels_Stop()
        {
            var parameters = new List<string>
            {
                "/Orleans:Providers:Clustering:Provider=Localhost",
                "/Orleans:Providers:Reminders:Provider=InMemory",
                "/Orleans:Providers:Storage:Default:Provider=InMemory",
                "/Orleans:Providers:Storage:PubSub:Provider=InMemory"
            };

            await Program.StartAsync(parameters.ToArray());
            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await Program.StopAsync(new CancellationToken(true));
            });
        }
    }
}
