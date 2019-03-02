using Grains;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Orleans.ApplicationParts;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Providers;
using Orleans.Providers.Streams.SimpleMessageStream;
using Orleans.Runtime;
using Orleans.Streams;
using Silo;
using System.Linq;
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

        [Fact]
        public void Has_ClusteringOptions()
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

            // assert the silo clustering options are there
            var actual = host.Services.GetService<IOptions<ClusterOptions>>();
            Assert.NotNull(actual);
            Assert.Equal(options.Value.ClusterId, actual.Value.ClusterId);
            Assert.Equal(options.Value.ServiceId, actual.Value.ServiceId);
        }

        [Fact]
        public void Has_ClusterMembershipOptions()
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

            // assert the cluster membership options are there
            var actual = host.Services.GetService<IOptions<ClusterMembershipOptions>>();
            Assert.NotNull(actual);
            Assert.True(actual.Value.ValidateInitialConnectivity);
        }

        [Fact]
        public void Has_ClusterMembershipOptions_And_ValidateInitialConnectivity_On_Development()
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
                EnvironmentName = EnvironmentName.Development
            };

            // act
            var service = new SiloHostedService(
                options,
                new FakeLoggerProvider(),
                new FakeNetworkPortFinder(),
                environment);

            // white box
            var host = service.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(service) as ISiloHost;

            // assert the cluster membership options are there
            var actual = host.Services.GetService<IOptions<ClusterMembershipOptions>>();
            Assert.NotNull(actual);
            Assert.False(actual.Value.ValidateInitialConnectivity);
        }

        [Fact]
        public void Has_AdoNetReminderTableOptions()
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

            // assert the reminder table options are there
            var actual = host.Services.GetService<IOptions<AdoNetReminderTableOptions>>();
            Assert.Equal(options.Value.AdoNetConnectionString, actual.Value.ConnectionString);
            Assert.Equal(options.Value.AdoNetInvariant, actual.Value.Invariant);
        }

        [Fact]
        public void Has_AdoNetGrainStorageOptions_As_Default()
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

            // assert the grain storage options are there
            var actual = host.Services.GetService<IOptionsSnapshot<AdoNetGrainStorageOptions>>().Get(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME);
            Assert.Equal(options.Value.AdoNetConnectionString, actual.ConnectionString);
            Assert.Equal(options.Value.AdoNetInvariant, actual.Invariant);
            Assert.True(actual.UseJsonFormat);
            Assert.Equal(TypeNameHandling.None, actual.TypeNameHandling);
        }

        [Fact]
        public void Has_SimpleMessageStreamProvider()
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

            // assert the stream provider is there
            var actual = host.Services.GetServices<IKeyedService<string, IStreamProvider>>().SingleOrDefault(_ => _.Key == "SMS");
            Assert.NotNull(actual);
            Assert.IsType<SimpleMessageStreamProvider>(actual.GetService(host.Services));
        }

        [Fact]
        public void Has_AdoNetGrainStorageOptions_For_PubSubStore()
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

            // assert the grain storage options are there for pubsub
            var actual = host.Services.GetService<IOptionsSnapshot<AdoNetGrainStorageOptions>>().Get("PubSubStore");
            Assert.Equal(options.Value.AdoNetConnectionString, actual.ConnectionString);
            Assert.Equal(options.Value.AdoNetInvariant, actual.Invariant);
            Assert.True(actual.UseJsonFormat);
        }
    }
}
