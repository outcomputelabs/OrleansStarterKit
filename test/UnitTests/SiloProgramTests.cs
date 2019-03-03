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
        public async Task Program_Runs_And_Stops_Via_Host_Command()
        {
            // arrange
            var parameters = new List<string>
            {
                "/Orleans:Providers:Clustering:Provider=Localhost",
                "/Orleans:Providers:Reminders:Provider=InMemory",
                "/Orleans:Providers:Storage:Default:Provider=InMemory",
                "/Orleans:Providers:Storage:PubSub:Provider=InMemory"
            };

            // act
            var task = Program.Main(parameters.ToArray());

            // wait for program to start
            await Program.Started;

            // assert the host is there
            Assert.NotNull(Program.Host);

            // order the host to shutdown
            await Program.Host.StopAsync();

            // wait for program to shutdown
            await task;
        }

        [Fact]
        public async Task Program_Runs_And_Stops_Via_Token()
        {
            // arrange
            var parameters = new List<string>
            {
                "/Orleans:Providers:Clustering:Provider=Localhost",
                "/Orleans:Providers:Reminders:Provider=InMemory",
                "/Orleans:Providers:Storage:Default:Provider=InMemory",
                "/Orleans:Providers:Storage:PubSub:Provider=InMemory"
            };
            var source = new CancellationTokenSource(TimeSpan.FromMinutes(1));

            // act
            var task = Program.MainForTesting(parameters.ToArray(), source.Token);

            // wait for program to start
            await Program.Started;

            // assert the host is there
            Assert.NotNull(Program.Host);

            // order shutdown
            source.Cancel(true);

            // wait for program to shutdown
            await task;
        }

        [Fact]
        public async Task Program_Cancels_Start()
        {
            // arrange
            var parameters = new List<string>
            {
                "/Orleans:Providers:Clustering:Provider=Localhost",
                "/Orleans:Providers:Reminders:Provider=InMemory",
                "/Orleans:Providers:Storage:Default:Provider=InMemory",
                "/Orleans:Providers:Storage:PubSub:Provider=InMemory"
            };

            // act
            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await Program.MainForTesting(parameters.ToArray(), new CancellationToken(true));
            });
        }
    }
}
