using Core.Tests.Fakes;
using Microsoft.Extensions.Configuration;
using Orleans;
using System;
using Xunit;

namespace Core.Tests
{
    public class ClientBuilderExtensionsTests
    {
        [Fact]
        public void TryUseLocalhostClustering_RefusesNullBuilder()
        {
            IClientBuilder builder = null;

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryUseLocalhostClustering(new ConfigurationBuilder().Build());
            });
            Assert.Equal("builder", error.ParamName);
        }

        [Fact]
        public void TryUseLocalhostClustering_RefusesNullConfiguration()
        {
            var builder = new FakeClientBuilder();

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryUseLocalhostClustering(null);
            });
            Assert.Equal("configuration", error.ParamName);
        }
    }
}
