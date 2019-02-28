using Silo;
using Xunit;

namespace UnitTests
{
    public class NetworkPortFinderTests
    {
        [Fact]
        public void NetworkHelper_Implements_Interface()
        {
            Assert.IsAssignableFrom<INetworkPortFinder>(new NetworkPortFinder());
        }
    }
}
