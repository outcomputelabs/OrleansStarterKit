using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orleans.Configuration;
using Orleans.Hosting;
using Silo;
using System.Reflection;
using UnitTests.Fakes;
using Xunit;

namespace UnitTests
{
    public class SiloHostedServiceTests
    {
        [Fact]
        public void Has_SiloHost()
        {
            // arrange
            var options = new FakeSiloHostedServiceOptions();
            options.Value.AdoNetConnectionString = "SomeConnectionString";
            options.Value.AdoNetInvariant = "SomeInvariant";
            options.Value.SiloPortRange.Start = 11111;
            options.Value.ClusterId = "SomeClusterId";
            options.Value.ServiceId = "SomeServiceId";

            var environment = new FakeHostingEnvironment
            {
                EnvironmentName = "SomeEnvironment"
            };

            // act
            var service = new SiloHostedService(
                options,
                new FakeLoggerProvider(),
                new FakeNetworkPortFinder(),
                environment);

            // assert - white box
            var host = service.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(service) as ISiloHost;
            Assert.NotNull(host);
        }

        [Fact]
        public void Has_Endpoints()
        {
            // arrange
            var options = new FakeSiloHostedServiceOptions();
            options.Value.AdoNetConnectionString = "SomeConnectionString";
            options.Value.AdoNetInvariant = "SomeInvariant";
            options.Value.SiloPortRange.Start = 11111;
            options.Value.ClusterId = "SomeClusterId";
            options.Value.ServiceId = "SomeServiceId";

            var environment = new FakeHostingEnvironment
            {
                EnvironmentName = "SomeEnvironment"
            };

            // act
            var service = new SiloHostedService(
                options,
                new FakeLoggerProvider(),
                new FakeNetworkPortFinder(),
                environment);

            // white box
            var host = service.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(service) as ISiloHost;

            // assert the endpoints are there
            Assert.NotNull(host.Services.GetService<IOptions<EndpointOptions>>());
        }
    }
}
