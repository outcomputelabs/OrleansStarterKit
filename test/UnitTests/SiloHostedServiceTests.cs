using Grains;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
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
using System.Collections.Generic;
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
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Orleans:Ports:Silo:Start", "11111" },
                    { "Orleans:Ports:Silo:End", "11111" },
                    { "Orleans:ClusterId", "SomeClusterId" },
                    { "Orleans:ServiceId", "SomeServiceId" },
                    { "Orleans:Providers:Clustering:Provider", "Localhost" }
                })
                .Build();

            // act
            var service = new SiloHostedService(
                config,
                new FakeLoggerProvider(),
                new FakeNetworkPortFinder(),
                new FakeHostingEnvironment());

            // assert - white box
            var host = service.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(service) as ISiloHost;
            Assert.NotNull(host);
        }

        [Fact]
        public void Has_Endpoints()
        {
            // arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Orleans:Ports:Silo:Start", "11111" },
                    { "Orleans:Ports:Silo:End", "11111" },
                    { "Orleans:Ports:Gateway:Start", "22222" },
                    { "Orleans:Ports:Gateway:End", "22222" },
                    { "Orleans:ClusterId", "SomeClusterId" },
                    { "Orleans:ServiceId", "SomeServiceId" },
                    { "Orleans:Providers:Clustering:Provider", "Localhost" }
                })
                .Build();

            // act
            var service = new SiloHostedService(
                config,
                new FakeLoggerProvider(),
                new FakeNetworkPortFinder(),
                new FakeHostingEnvironment());

            // white box
            var host = service.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(service) as ISiloHost;

            // assert the endpoints are there
            var actual = host.Services.GetService<IOptions<EndpointOptions>>();
            Assert.NotNull(actual);
            Assert.Equal(11111, actual.Value.SiloPort);
            Assert.Equal(22222, actual.Value.GatewayPort);
        }

        [Fact]
        public void Has_ApplicationParts()
        {
            // arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Orleans:Ports:Silo:Start", "11111" },
                    { "Orleans:Ports:Silo:End", "11111" },
                    { "Orleans:Ports:Gateway:Start", "22222" },
                    { "Orleans:Ports:Gateway:End", "22222" },
                    { "Orleans:ClusterId", "SomeClusterId" },
                    { "Orleans:ServiceId", "SomeServiceId" },
                    { "Orleans:Providers:Clustering:Provider", "Localhost" }
                })
                .Build();

            // act
            var service = new SiloHostedService(
                config,
                new FakeLoggerProvider(),
                new FakeNetworkPortFinder(),
                new FakeHostingEnvironment());

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
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Orleans:Ports:Silo:Start", "11111" },
                    { "Orleans:Ports:Silo:End", "11111" },
                    { "Orleans:Ports:Gateway:Start", "22222" },
                    { "Orleans:Ports:Gateway:End", "22222" },
                    { "Orleans:ClusterId", "SomeClusterId" },
                    { "Orleans:ServiceId", "SomeServiceId" },
                    { "Orleans:Providers:Clustering:Provider", "Localhost" }
                })
                .Build();

            var loggerProvider = new FakeLoggerProvider();

            // act
            var service = new SiloHostedService(
                config,
                loggerProvider,
                new FakeNetworkPortFinder(),
                new FakeHostingEnvironment());

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
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Orleans:Ports:Silo:Start", "11111" },
                    { "Orleans:Ports:Silo:End", "11111" },
                    { "Orleans:Ports:Gateway:Start", "22222" },
                    { "Orleans:Ports:Gateway:End", "22222" },
                    { "Orleans:ClusterId", "SomeClusterId" },
                    { "Orleans:ServiceId", "SomeServiceId" },
                    { "Orleans:Providers:Clustering:Provider", "AdoNet" },
                    { "Orleans:Providers:Clustering:AdoNet:ConnectionStringName", "SomeConnectionStringName" },
                    { "Orleans:Providers:Clustering:AdoNet:Invariant", "SomeInvariant" },
                    { "ConnectionStrings:SomeConnectionStringName", "SomeConnectionString" }
                })
                .Build();

            // act
            var service = new SiloHostedService(
                config,
                new FakeLoggerProvider(),
                new FakeNetworkPortFinder(),
                new FakeHostingEnvironment());

            // white box
            var host = service.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(service) as ISiloHost;

            // assert the ado net clustering options are there
            var actual = host.Services.GetService<IOptions<AdoNetClusteringSiloOptions>>();
            Assert.NotNull(actual);
            Assert.Equal("SomeConnectionString", actual.Value.ConnectionString);
            Assert.Equal("SomeInvariant", actual.Value.Invariant);
        }

        [Fact]
        public void Has_ClusteringOptions()
        {
            // arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Orleans:Ports:Silo:Start", "11111" },
                    { "Orleans:Ports:Silo:End", "11111" },
                    { "Orleans:Ports:Gateway:Start", "22222" },
                    { "Orleans:Ports:Gateway:End", "22222" },
                    { "Orleans:ClusterId", "SomeClusterId" },
                    { "Orleans:ServiceId", "SomeServiceId" },
                    { "Orleans:Providers:Clustering:Provider", "Localhost" }
                })
                .Build();

            // act
            var service = new SiloHostedService(
                config,
                new FakeLoggerProvider(),
                new FakeNetworkPortFinder(),
                new FakeHostingEnvironment());

            // white box
            var host = service.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(service) as ISiloHost;

            // assert the silo clustering options are there
            var actual = host.Services.GetService<IOptions<ClusterOptions>>();
            Assert.NotNull(actual);
            Assert.Equal("SomeClusterId", actual.Value.ClusterId);
            Assert.Equal("SomeServiceId", actual.Value.ServiceId);
        }

        [Fact]
        public void Has_ClusterMembershipOptions()
        {
            // arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Orleans:Ports:Silo:Start", "11111" },
                    { "Orleans:Ports:Silo:End", "11111" },
                    { "Orleans:Ports:Gateway:Start", "22222" },
                    { "Orleans:Ports:Gateway:End", "22222" },
                    { "Orleans:ClusterId", "SomeClusterId" },
                    { "Orleans:ServiceId", "SomeServiceId" },
                    { "Orleans:Providers:Clustering:Provider", "Localhost" }
                })
                .Build();

            // act
            var service = new SiloHostedService(
                config,
                new FakeLoggerProvider(),
                new FakeNetworkPortFinder(),
                new FakeHostingEnvironment());

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
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Orleans:Ports:Silo:Start", "11111" },
                    { "Orleans:Ports:Silo:End", "11111" },
                    { "Orleans:Ports:Gateway:Start", "22222" },
                    { "Orleans:Ports:Gateway:End", "22222" },
                    { "Orleans:ClusterId", "SomeClusterId" },
                    { "Orleans:ServiceId", "SomeServiceId" },
                    { "Orleans:Providers:Clustering:Provider", "Localhost" }
                })
                .Build();

            // act
            var service = new SiloHostedService(
                config,
                new FakeLoggerProvider(),
                new FakeNetworkPortFinder(),
                new FakeHostingEnvironment()
                {
                    EnvironmentName = EnvironmentName.Development
                });

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
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Orleans:Ports:Silo:Start", "11111" },
                    { "Orleans:Ports:Silo:End", "11111" },
                    { "Orleans:Ports:Gateway:Start", "22222" },
                    { "Orleans:Ports:Gateway:End", "22222" },
                    { "Orleans:ClusterId", "SomeClusterId" },
                    { "Orleans:ServiceId", "SomeServiceId" },
                    { "Orleans:Providers:Clustering:Provider", "Localhost" },
                    { "Orleans:Providers:Reminders:Provider", "AdoNet" },
                    { "Orleans:Providers:Reminders:AdoNet:ConnectionStringName", "SomeConnectionStringName" },
                    { "Orleans:Providers:Reminders:AdoNet:Invariant", "SomeInvariant" },
                    { "ConnectionStrings:SomeConnectionStringName", "SomeConnectionString" }
                })
                .Build();

            // act
            var service = new SiloHostedService(
                config,
                new FakeLoggerProvider(),
                new FakeNetworkPortFinder(),
                new FakeHostingEnvironment());

            // white box
            var host = service.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(service) as ISiloHost;

            // assert the reminder table options are there
            var actual = host.Services.GetService<IOptions<AdoNetReminderTableOptions>>();
            Assert.Equal("SomeConnectionString", actual.Value.ConnectionString);
            Assert.Equal("SomeInvariant", actual.Value.Invariant);
        }

        [Fact]
        public void Has_AdoNetGrainStorageOptions_As_Default()
        {
            // arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Orleans:Ports:Silo:Start", "11111" },
                    { "Orleans:Ports:Silo:End", "11111" },
                    { "Orleans:Ports:Gateway:Start", "22222" },
                    { "Orleans:Ports:Gateway:End", "22222" },
                    { "Orleans:ClusterId", "SomeClusterId" },
                    { "Orleans:ServiceId", "SomeServiceId" },
                    { "Orleans:Providers:Clustering:Provider", "Localhost" },
                    { "Orleans:Providers:Storage:Default:Provider", "AdoNet" },
                    { "Orleans:Providers:Storage:Default:AdoNet:ConnectionStringName", "SomeConnectionStringName" },
                    { "Orleans:Providers:Storage:Default:AdoNet:Invariant", "SomeInvariant" },
                    { "Orleans:Providers:Storage:Default:AdoNet:UseJsonFormat", "true" },
                    { "Orleans:Providers:Storage:Default:AdoNet:TypeNameHandling", "None" },
                    { "ConnectionStrings:SomeConnectionStringName", "SomeConnectionString" },
                })
                .Build();

            // act
            var service = new SiloHostedService(
                config,
                new FakeLoggerProvider(),
                new FakeNetworkPortFinder(),
                new FakeHostingEnvironment());

            // white box
            var host = service.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(service) as ISiloHost;

            // assert the grain storage options are there
            var actual = host.Services.GetService<IOptionsSnapshot<AdoNetGrainStorageOptions>>().Get(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME);
            Assert.Equal("SomeConnectionString", actual.ConnectionString);
            Assert.Equal("SomeInvariant", actual.Invariant);
            Assert.True(actual.UseJsonFormat);
            Assert.Equal(TypeNameHandling.None, actual.TypeNameHandling);
        }

        [Fact]
        public void Has_SimpleMessageStreamProvider()
        {
            // arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Orleans:Ports:Silo:Start", "11111" },
                    { "Orleans:Ports:Silo:End", "11111" },
                    { "Orleans:Ports:Gateway:Start", "22222" },
                    { "Orleans:Ports:Gateway:End", "22222" },
                    { "Orleans:ClusterId", "SomeClusterId" },
                    { "Orleans:ServiceId", "SomeServiceId" },
                    { "Orleans:Providers:Clustering:Provider", "Localhost" }
                })
                .Build();

            // act
            var service = new SiloHostedService(
                config,
                new FakeLoggerProvider(),
                new FakeNetworkPortFinder(),
                new FakeHostingEnvironment());

            // white box
            var host = service.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(service) as ISiloHost;

            // assert the stream provider is there
            var actual = host.Services.GetServices<IKeyedService<string, IStreamProvider>>().SingleOrDefault(_ => _.Key == "SMS");
            Assert.NotNull(actual);
            Assert.IsType<SimpleMessageStreamProvider>(actual.GetService(host.Services));
        }

        /*
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

        [Fact]
        public void Refuses_Invalid_ClusteringProvider()
        {
            var options = new FakeSiloHostedServiceOptions();
            options.Value.ClusteringProvider = SiloHostedServiceClusteringProvider.None;

            var error = Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new SiloHostedService(
                    options,
                    new FakeLoggerProvider(),
                    new FakeNetworkPortFinder(),
                    new FakeHostingEnvironment());
            });
            Assert.Equal(nameof(options.Value.ClusteringProvider), error.ParamName);
        }

        [Fact]
        public void Refuses_Invalid_ReminderProvider()
        {
            var options = new FakeSiloHostedServiceOptions();
            options.Value.ReminderProvider = SiloHostedServiceReminderProvider.None;

            var error = Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new SiloHostedService(
                    options,
                    new FakeLoggerProvider(),
                    new FakeNetworkPortFinder(),
                    new FakeHostingEnvironment());
            });
            Assert.Equal(nameof(options.Value.ReminderProvider), error.ParamName);
        }

        [Fact]
        public void Refuses_Invalid_DefaultStorageProvider()
        {
            var options = new FakeSiloHostedServiceOptions();
            options.Value.DefaultStorageProvider = SiloHostedServiceStorageProvider.None;

            var error = Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new SiloHostedService(
                    options,
                    new FakeLoggerProvider(),
                    new FakeNetworkPortFinder(),
                    new FakeHostingEnvironment());
            });
            Assert.Equal(nameof(options.Value.DefaultStorageProvider), error.ParamName);
        }

        [Fact]
        public void Refuses_Invalid_PubSubStorageProvider()
        {
            var options = new FakeSiloHostedServiceOptions();
            options.Value.PubSubStorageProvider = SiloHostedServiceStorageProvider.None;

            var error = Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new SiloHostedService(
                    options,
                    new FakeLoggerProvider(),
                    new FakeNetworkPortFinder(),
                    new FakeHostingEnvironment());
            });
            Assert.Equal(nameof(options.Value.PubSubStorageProvider), error.ParamName);
        }
        */
    }
}
