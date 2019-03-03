using Silo;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
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
            var task = Program.Main(parameters.ToArray(), source.Token);

            // wait for program to start
            await Program.Started;

            // order shutdown
            source.Cancel(true);

            // wait for program to shutdown
            await task;
        }
    }
}
