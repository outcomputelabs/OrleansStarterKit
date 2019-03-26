using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Orleans;
using Orleans.ApplicationParts;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Silo.Tests
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
            var services = new ServiceCollection();
            var builder = new Mock<ISiloHostBuilder>(MockBehavior.Strict);
            builder.Setup(_ => _.ConfigureServices(It.IsAny<Action<HostBuilderContext, IServiceCollection>>()))
                .Callback((Action<HostBuilderContext, IServiceCollection> action) =>
                {
                    action(null, services);
                })
                .Returns(() => builder.Object);

            var config = Mock.Of<IConfiguration>(_ =>
                _["Orleans:Providers:Reminders:Provider"] == "InMemory");

            // act
            builder.Object.TryUseInMemoryReminderService(config);

            // assert
            var service = services.SingleOrDefault(_ => _.ServiceType == typeof(IReminderTable));
            Assert.NotNull(service);
            Assert.Contains("UseInMemoryReminderService", service.ImplementationFactory.Method.Name);
        }

        [Fact]
        public void TryAddMemoryGrainStorageAsDefault_Applies_StorageService()
        {
            // arrange
            var services = new ServiceCollection();
            var apm = Mock.Of<IApplicationPartManager>();
            var builder = Mock.Of<ISiloHostBuilder>(_ => _.Properties == new Dictionary<object, object>());
            Mock.Get(builder).Setup(_ => _.ConfigureServices(It.IsAny<Action<HostBuilderContext, IServiceCollection>>()))
                .Callback((Action<HostBuilderContext, IServiceCollection> action) =>
                {
                    action(null, services);
                })
                .Returns(() => builder);
            var configuration = Mock.Of<IConfiguration>(_ =>
                _["Orleans:Providers:Storage:Default:Provider"] == "InMemory");

            // act
            builder.TryAddMemoryGrainStorageAsDefault(configuration);

            // assert
            var service = services.SingleOrDefault(_ => _.ServiceType == typeof(IGrainStorage));
            Assert.NotNull(service);
            Assert.Contains("AddMemoryGrainStorage", service.ImplementationFactory.Method.Name);
        }

        [Fact]
        public void TryAddMemoryGrainStorageForPubSub_Applies_StorageService()
        {
            // arrange
            var services = new ServiceCollection();
            var builder = Mock.Of<ISiloHostBuilder>(_ => _.Properties == new Dictionary<object, object>());
            Mock.Get(builder).Setup(_ => _.ConfigureServices(It.IsAny<Action<HostBuilderContext, IServiceCollection>>()))
                .Callback((Action<HostBuilderContext, IServiceCollection> action) =>
                {
                    action(null, services);
                })
                .Returns(() => builder);

            var configuration = Mock.Of<IConfiguration>(_ =>
                _["Orleans:Providers:Storage:PubSub:Provider"] == "InMemory");

            // act
            builder.TryAddMemoryGrainStorageForPubSub(configuration);

            // assert
            var service = services.SingleOrDefault(_ => _.ServiceType == typeof(IKeyedService<string, IGrainStorage>));
            Assert.NotNull(service);
            Assert.Contains("AddSingletonKeyedService", service.ImplementationFactory.Method.Name);
        }
    }
}
