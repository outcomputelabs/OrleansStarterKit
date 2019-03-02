using Grains;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Orleans;
using Orleans.ApplicationParts;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Providers;
using Orleans.Providers.Streams.SimpleMessageStream;
using Orleans.Runtime;
using Orleans.Streams;
using OrleansDashboard;
using Silo;
using Silo.Options;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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
            options.Value.ClusteringProvider = SiloHostedServiceClusteringProvider.AdoNet;

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
            options.Value.ReminderProvider = SiloHostedServiceReminderProvider.AdoNet;

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
            options.Value.DefaultStorageProvider = SiloHostedServiceStorageProvider.AdoNet;

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
            options.Value.DashboardPortRange.Start = 33333;
            options.Value.ClusterId = "SomeClusterId";
            options.Value.ServiceId = "SomeServiceId";
            options.Value.PubSubStorageProvider = SiloHostedServiceStorageProvider.AdoNet;

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

        [Fact]
        public void Has_DashboardOptions()
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

            // assert the dashboard options are there
            var actual = host.Services.GetService<IOptions<DashboardOptions>>();
            Assert.True(actual.Value.HostSelf);
            Assert.Equal(options.Value.DashboardPortRange.Start, actual.Value.Port);
        }

        [Fact]
        public void Has_DirectClient()
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

            // assert the cluster client is there
            var client = host.Services.GetService<IClusterClient>();
            Assert.NotNull(client);

            // assert the grain factory is there
            var factory = host.Services.GetService<IGrainFactory>();
            Assert.NotNull(factory);
        }

        [Fact]
        public void Exposes_ClusterClient()
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

            // assert
            Assert.NotNull(service.ClusterClient);
        }

        [Fact]
        public void Refuses_Null_Options()
        {
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                new SiloHostedService(
                    null,
                    new FakeLoggerProvider(),
                    new FakeNetworkPortFinder(),
                    new FakeHostingEnvironment());
            });
            Assert.Equal("options", error.ParamName);
        }

        [Fact]
        public void Refuses_Null_Options_Value()
        {
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                new SiloHostedService(
                    new FakeSiloHostedServiceOptions()
                    {
                        Value = null
                    },
                    new FakeLoggerProvider(),
                    new FakeNetworkPortFinder(),
                    new FakeHostingEnvironment());
            });
            Assert.Equal("options", error.ParamName);
        }

        [Fact]
        public void Refuses_Null_LoggerProvider()
        {
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                new SiloHostedService(
                    new FakeSiloHostedServiceOptions(),
                    null,
                    new FakeNetworkPortFinder(),
                    new FakeHostingEnvironment());
            });
            Assert.Equal("loggerProvider", error.ParamName);
        }

        [Fact]
        public void Refuses_Null_PortFinder()
        {
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                new SiloHostedService(
                    new FakeSiloHostedServiceOptions(),
                    new FakeLoggerProvider(),
                    null,
                    new FakeHostingEnvironment());
            });
            Assert.Equal("portFinder", error.ParamName);
        }

        [Fact]
        public void Refuses_Null_Environment()
        {
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                new SiloHostedService(
                    new FakeSiloHostedServiceOptions(),
                    new FakeLoggerProvider(),
                    new FakeNetworkPortFinder(),
                    null);
            });
            Assert.Equal("environment", error.ParamName);
        }

        [Fact]
        public async Task Starts_And_Stops()
        {
            // arrange
            var options = new FakeSiloHostedServiceOptions();
            options.Value.AdoNetConnectionString = "SomeConnectionString";
            options.Value.AdoNetInvariant = "System.Data.SqlClient";
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

            // assert
            await service.StartAsync(new CancellationToken());
            await service.StopAsync(new CancellationToken());
        }
    }
}
