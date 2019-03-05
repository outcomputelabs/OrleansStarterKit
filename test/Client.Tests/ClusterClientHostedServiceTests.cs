using Client.Tests.Fakes;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Xunit;

namespace Client.Tests
{
    public class ClusterClientHostedServiceTests
    {
        [Fact]
        public void RefusesNullConfiguration()
        {
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                new ClusterClientHostedService(null, new FakeLoggerProvider());
            });
            Assert.Equal("configuration", error.ParamName);
        }

        [Fact]
        public void RefusesNullLoggerProvider()
        {
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                new ClusterClientHostedService(new FakeConfiguration(), null);
            });
            Assert.Equal("configuration", error.ParamName);
        }

        [Fact]
        public void StartsAndMakesClusterClientAvailable()
        {
            // arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                })
                .Build();

            // act
            var service = new ClusterClientHostedService(config, new FakeLoggerProvider());

            // assert
            Assert.NotNull(service.ClusterClient);
        }
    }
}
