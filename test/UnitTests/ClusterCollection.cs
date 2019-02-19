using Xunit;

namespace UnitTests
{
    [CollectionDefinition(nameof(ClusterCollection))]
    public class ClusterCollection : ICollectionFixture<ClusterFixture>
    {
    }
}
