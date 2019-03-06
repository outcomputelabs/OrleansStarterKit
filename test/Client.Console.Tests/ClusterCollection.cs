using Xunit;

namespace Client.Console.Tests
{
    [CollectionDefinition(nameof(ClusterCollection))]
    public class ClusterCollection : ICollectionFixture<ClusterFixture>
    {
    }
}
