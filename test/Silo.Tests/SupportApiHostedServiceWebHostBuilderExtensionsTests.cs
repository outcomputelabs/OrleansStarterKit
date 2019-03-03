using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Silo.Tests
{
    public class SupportApiHostedServiceWebHostBuilderExtensionsTests
    {
        [Fact]
        public void Refuses_Null_Services()
        {
            // arrange
            IServiceCollection services = null;

            // act
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                services.AddSupportApiInfo("SomeTitle");
            });

            // assert
            Assert.Equal("services", error.ParamName);
        }
    }
}