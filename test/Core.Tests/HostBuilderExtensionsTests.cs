using Microsoft.Extensions.Hosting;
using System;
using Xunit;

namespace Core.Tests
{
    public class HostBuilderExtensionsTests
    {
        [Fact]
        public void UseSharedConfiguration_RefusesNullBuilder()
        {
            IHostBuilder builder = null;

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.UseSharedConfiguration();
            });
            Assert.Equal("builder", error.ParamName);
        }
    }
}
