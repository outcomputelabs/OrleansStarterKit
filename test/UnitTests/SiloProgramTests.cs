using Microsoft.Extensions.DependencyInjection;
using Silo;
using System.Collections.Generic;
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
        public async Task Program_Host_Has_SiloHostedService()
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

            // assert
            Assert.NotNull(Program.Host);
            Assert.NotNull(Program.Host.Services.GetService<SiloHostedService>());

            // order the host to shutdown
            await Program.Host.StopAsync();

            // wait for program to shutdown
            await task;
        }

        [Fact]
        public async Task Program_Host_Has_SupportApiHostedService()
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

            // assert
            Assert.NotNull(Program.Host);
            Assert.NotNull(Program.Host.Services.GetService<SupportApiHostedService>());

            // order the host to shutdown
            await Program.Host.StopAsync();

            // wait for program to shutdown
            await task;
        }
    }
}
