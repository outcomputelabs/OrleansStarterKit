using Grains;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans.ApplicationParts;
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
            var actual = host.Services.GetService<IOptions<EndpointOptions>>();
            Assert.NotNull(actual);
            Assert.Equal(options.Value.SiloPortRange.Start, actual.Value.SiloPort);
            Assert.Equal(options.Value.GatewayPortRange.Start, actual.Value.GatewayPort);
        }

        [Fact]
        public void Has_ApplicationParts()
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

            // assert the application part is there
            var parts = host.Services.GetService<IApplicationPartManager>();
            Assert.Contains(parts.ApplicationParts, _ => (_ as AssemblyPart)?.Assembly == typeof(ChatUser).Assembly);
        }

        [Fact]
        public void Has_LoggerProvider()
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

            var loggerProvider = new FakeLoggerProvider();

            // act
            var service = new SiloHostedService(
                options,
                loggerProvider,
                new FakeNetworkPortFinder(),
                environment);

            // white box
            var host = service.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(service) as ISiloHost;

            // assert the logger provider is there
            var actual = host.Services.GetServices<ILoggerProvider>();
            Assert.Contains(loggerProvider, actual);
        }

        [Fact]
        public void Has_AdoNetClustering()
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

            // assert the ado net clustering options are there
            var actual = host.Services.GetService<IOptions<AdoNetClusteringSiloOptions>>();
            Assert.NotNull(actual);
            Assert.Equal(options.Value.AdoNetConnectionString, actual.Value.ConnectionString);
            Assert.Equal(options.Value.AdoNetInvariant, actual.Value.Invariant);
        }
    }
}
