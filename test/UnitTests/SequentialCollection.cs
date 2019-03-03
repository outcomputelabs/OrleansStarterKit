using Xunit;

namespace UnitTests
{
    [CollectionDefinition(nameof(SequentialCollection), DisableParallelization = true)]
    public class SequentialCollection : ICollectionFixture<SequentialFixture>
    {
    }
}
