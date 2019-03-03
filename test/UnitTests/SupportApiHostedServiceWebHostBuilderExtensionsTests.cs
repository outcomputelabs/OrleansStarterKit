using Microsoft.Extensions.DependencyInjection;
using Silo;
using System;
using Xunit;

namespace UnitTests
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