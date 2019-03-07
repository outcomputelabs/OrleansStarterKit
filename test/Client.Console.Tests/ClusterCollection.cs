using Xunit;

namespace Client.Console.Tests
{
    [CollectionDefinition(nameof(ClusterCollection), DisableParallelization = true)]
    public class ClusterCollection : ICollectionFixture<ClusterFixture>
    {
    }
}
