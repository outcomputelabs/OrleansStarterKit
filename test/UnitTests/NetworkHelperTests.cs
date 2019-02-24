using Silo;
using System;
using System.Net.Sockets;
using Xunit;

namespace UnitTests
{
    public class NetworkHelperTests
    {
        [Fact]
        public void NetworkHelper_Implements_Interface()
        {
            // assert
            Assert.IsAssignableFrom<INetworkHelper>(new NetworkHelper());
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void NetworkHelper_Refuses_Low_Count(int count)
        {
            // arrange
            var helper = new NetworkHelper();

            // act and assert
            var error = Assert.Throws<ArgumentOutOfRangeException>(() => helper.GetAvailablePorts(count));
            Assert.Equal(count, error.ActualValue);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void NetworkHelper_Returns_Available_Ports(int count)
        {
            // arrange
            var helper = new NetworkHelper();

            // act
            var ports = helper.GetAvailablePorts(count);

            // assert
            Assert.NotNull(ports);
            Assert.Equal(count, ports.Length);
            Assert.All(ports, p => Assert.True(p > 0));
        }
    }
}
