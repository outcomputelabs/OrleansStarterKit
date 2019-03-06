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
    }
}
