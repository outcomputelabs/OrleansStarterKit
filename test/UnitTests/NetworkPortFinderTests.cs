using Silo;
using System.Net;
using System.Net.Sockets;
using Xunit;

namespace UnitTests
{
    public class NetworkPortFinderTests
    {
        [Fact]
        public void GetAvailablePortFrom_Returns_Open_Port()
        {
            // arrange
            var finder = new NetworkPortFinder();

            // find an open port
            var listener = TcpListener.Create(0);
            listener.ExclusiveAddressUse = true;
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            // act
            var available = finder.GetAvailablePortFrom(port, port + 10);

            // assert
            Assert.Equal(port, available);
        }
    }
}
