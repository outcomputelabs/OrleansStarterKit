using Xunit;

namespace Silo.Tests
{
    [CollectionDefinition(nameof(SequentialCollection), DisableParallelization = true)]
    public class SequentialCollection : ICollectionFixture<SequentialFixture>
    {
    }
}
