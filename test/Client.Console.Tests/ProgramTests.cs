using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Client.Console.Tests
{
    [Collection(nameof(ClusterCollection))]
    public class ProgramTests
    {
        public ProgramTests(ClusterFixture context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        private readonly ClusterFixture _context;

        [Fact]
        public async Task RunsAndConnectsToCluster()
        {
            // arrange
            Program.CancellationTokenSource = new CancellationTokenSource();
            Program.Started = new TaskCompletionSource<bool>();
            var args = new[]
            {
                "/Orleans:Providers:Clustering:Provider=Localhost",
                "/Orleans:Providers:Reminders:Provider=InMemory",
                "/Orleans:Providers:Storage:Default:Provider=InMemory",
                "/Orleans:Providers:Storage:PubSub:Provider=InMemory"
            };

            // act
            var execution = Program.Main(args);
            await Program.Started.Task;

            // assert
            Assert.NotNull(Program.Host);
            Assert.NotNull(Program.Host.Services.GetService<ClusterClientHostedService>());
            Assert.NotNull(Program.Host.Services.GetService<ConsoleClientHostedService>());

            // stop execution
            Program.CancellationTokenSource.Cancel();
            try
            {
                await execution;
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
