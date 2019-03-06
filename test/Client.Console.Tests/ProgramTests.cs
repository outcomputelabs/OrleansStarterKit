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
            Program.CancellationToken = new CancellationToken();

            // act
            await Program.Main(null);


        }
    }
}
