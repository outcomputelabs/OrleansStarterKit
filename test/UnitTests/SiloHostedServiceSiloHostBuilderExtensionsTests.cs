using Microsoft.Extensions.Configuration;
using Orleans.Hosting;
using Silo;
using System;
using Xunit;

namespace UnitTests
{
    public class SiloHostedServiceSiloHostBuilderExtensionsTests
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
    }
}
