using Core.Tests.Fakes;
using Microsoft.Extensions.Configuration;
using Orleans;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Core.Tests
{
    public class SiloHostBuilderExtensionsTests
    {
        [Fact]
        public void TryUseLocalhostClustering_Refuses_Null_Builder()
        {
            ISiloHostBuilder builder = null;
            IConfiguration configuration = new ConfigurationBuilder().Build();

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryUseLocalhostClustering(configuration, 0, 0);
            });

            Assert.Equal("builder", error.ParamName);
        }

        [Fact]
        public void TryUseLocalhostClustering_Refuses_Null_Configuration()
        {
            ISiloHostBuilder builder = new SiloHostBuilder();
            IConfiguration configuration = null;

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryUseLocalhostClustering(configuration, 0, 0);
            });

            Assert.Equal("configuration", error.ParamName);
        }

        [Fact]
        public void TryUseAdoNetClustering_Refuses_Null_Builder()
        {
            ISiloHostBuilder builder = null;
            IConfiguration configuration = new ConfigurationBuilder().Build();

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryUseAdoNetClustering(configuration, 0, 0);
            });

            Assert.Equal("builder", error.ParamName);
        }

        [Fact]
        public void TryUseAdoNetClustering_Refuses_Null_Configuration()
        {
            ISiloHostBuilder builder = new SiloHostBuilder();
            IConfiguration configuration = null;

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryUseAdoNetClustering(configuration, 0, 0);
            });

            Assert.Equal("configuration", error.ParamName);
        }

        [Fact]
        public void TryUseInMemoryReminderService_Refuses_Null_Builder()
        {
            ISiloHostBuilder builder = null;
            IConfiguration configuration = new ConfigurationBuilder().Build();

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryUseInMemoryReminderService(configuration);
            });

            Assert.Equal("builder", error.ParamName);
        }

        [Fact]
        public void TryUseInMemoryReminderService_Refuses_Null_Configuration()
        {
            ISiloHostBuilder builder = new SiloHostBuilder();
            IConfiguration configuration = null;

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryUseInMemoryReminderService(configuration);
            });

            Assert.Equal("configuration", error.ParamName);
        }

        [Fact]
        public void TryUseAdoNetReminderService_Refuses_Null_Builder()
        {
            ISiloHostBuilder builder = null;
            IConfiguration configuration = new ConfigurationBuilder().Build();

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryUseAdoNetReminderService(configuration);
            });

            Assert.Equal("builder", error.ParamName);
        }

        [Fact]
        public void TryUseAdoNetReminderService_Refuses_Null_Configuration()
        {
            ISiloHostBuilder builder = new SiloHostBuilder();
            IConfiguration configuration = null;

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryUseAdoNetReminderService(configuration);
            });

            Assert.Equal("configuration", error.ParamName);
        }

        [Fact]
        public void TryAddMemoryGrainStorageAsDefault_Refuses_Null_Builder()
        {
            ISiloHostBuilder builder = null;
            IConfiguration configuration = new ConfigurationBuilder().Build();

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryAddMemoryGrainStorageAsDefault(configuration);
            });

            Assert.Equal("builder", error.ParamName);
        }

        [Fact]
        public void TryAddMemoryGrainStorageAsDefault_Refuses_Null_Configuration()
        {
            ISiloHostBuilder builder = new SiloHostBuilder();
            IConfiguration configuration = null;

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryAddMemoryGrainStorageAsDefault(configuration);
            });

            Assert.Equal("configuration", error.ParamName);
        }

        [Fact]
        public void TryAddAdoNetGrainStorageAsDefault_Refuses_Null_Builder()
        {
            ISiloHostBuilder builder = null;
            IConfiguration configuration = new ConfigurationBuilder().Build();

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryAddAdoNetGrainStorageAsDefault(configuration);
            });

            Assert.Equal("builder", error.ParamName);
        }

        [Fact]
        public void TryAddAdoNetGrainStorageAsDefault_Refuses_Null_Configuration()
        {
            ISiloHostBuilder builder = new SiloHostBuilder();
            IConfiguration configuration = null;

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryAddAdoNetGrainStorageAsDefault(configuration);
            });

            Assert.Equal("configuration", error.ParamName);
        }

        [Fact]
        public void TryAddMemoryGrainStorageForPubSub_Refuses_Null_Builder()
        {
            ISiloHostBuilder builder = null;
            IConfiguration configuration = new ConfigurationBuilder().Build();

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryAddMemoryGrainStorageForPubSub(configuration);
            });

            Assert.Equal("builder", error.ParamName);
        }

        [Fact]
        public void TryAddMemoryGrainStorageForPubSub_Refuses_Null_Configuration()
        {
            ISiloHostBuilder builder = new SiloHostBuilder();
            IConfiguration configuration = null;

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryAddMemoryGrainStorageForPubSub(configuration);
            });

            Assert.Equal("configuration", error.ParamName);
        }

        [Fact]
        public void TryAddAdoNetGrainStorageForPubSub_Refuses_Null_Builder()
        {
            ISiloHostBuilder builder = null;
            IConfiguration configuration = new ConfigurationBuilder().Build();

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryAddAdoNetGrainStorageForPubSub(configuration);
            });

            Assert.Equal("builder", error.ParamName);
        }

        [Fact]
        public void TryAddAdoNetGrainStorageForPubSub_Refuses_Null_Configuration()
        {
            ISiloHostBuilder builder = new SiloHostBuilder();
            IConfiguration configuration = null;

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryAddAdoNetGrainStorageForPubSub(configuration);
            });

            Assert.Equal("configuration", error.ParamName);
        }

        [Fact]
        public void TryUseInMemoryReminderService_Applies_ReminderService()
        {
            // arrange
            var builder = new FakeSiloHostBuilder();
            var services = new FakeServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Orleans:Providers:Reminders:Provider", "InMemory" }
                })
                .Build();

            // act
            builder.TryUseInMemoryReminderService(configuration);

            // assert
            var service = builder.ServiceCollection.SingleOrDefault(_ => _.ServiceType == typeof(IReminderTable));
            Assert.NotNull(service);
            Assert.Contains("UseInMemoryReminderService", service.ImplementationFactory.Method.Name);
        }

        [Fact]
        public void TryAddMemoryGrainStorageAsDefault_Applies_StorageService()
        {
            // arrange
            var builder = new FakeSiloHostBuilder();
            var services = new FakeServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Orleans:Providers:Storage:Default:Provider", "InMemory" }
                })
                .Build();

            // act
            builder.TryAddMemoryGrainStorageAsDefault(configuration);

            // assert
            var service = builder.ServiceCollection.SingleOrDefault(_ => _.ServiceType == typeof(IGrainStorage));
            Assert.NotNull(service);
            Assert.Contains("AddMemoryGrainStorage", service.ImplementationFactory.Method.Name);
        }

        [Fact]
        public void TryAddMemoryGrainStorageForPubSub_Applies_StorageService()
        {
            // arrange
            var builder = new FakeSiloHostBuilder();
            var services = new FakeServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Orleans:Providers:Storage:PubSub:Provider", "InMemory" }
                })
                .Build();

            // act
            builder.TryAddMemoryGrainStorageForPubSub(configuration);

            // assert
            var service = builder.ServiceCollection.SingleOrDefault(_ => _.ServiceType == typeof(IKeyedService<string, IGrainStorage>));
            Assert.NotNull(service);
            Assert.Contains("AddSingletonKeyedService", service.ImplementationFactory.Method.Name);
        }
    }
}
