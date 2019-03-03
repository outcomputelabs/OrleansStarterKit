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
        public async Task Program_Runs()
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

            // order shutdown
            source.Cancel(true);

            // wait for program to shutdown
            await task;
        }

        [Fact]
        public async Task Program_Has_Host()
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
    }
}
