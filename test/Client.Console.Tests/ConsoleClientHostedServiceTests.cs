using System;
using Xunit;

namespace Client.Console.Tests
{
    public class ConsoleClientHostedServiceTests
    {
        [Fact]
        public void RefusesNullClusterClient()
        {
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                new ConsoleClientHostedService(null);
            });
            Assert.Equal("client", error.ParamName);
        }
    }
}
