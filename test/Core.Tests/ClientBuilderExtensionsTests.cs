﻿using Core.Tests.Fakes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Configuration;
using System;
using System.Collections.Generic;
using Xunit;

namespace Core.Tests
{
    public class ClientBuilderExtensionsTests
    {
        [Fact]
        public void TryUseLocalhostClustering_RefusesNullBuilder()
        {
            IClientBuilder builder = null;

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryUseLocalhostClustering(new ConfigurationBuilder().Build());
            });
            Assert.Equal("builder", error.ParamName);
        }

        [Fact]
        public void TryUseLocalhostClustering_RefusesNullConfiguration()
        {
            var builder = new FakeClientBuilder();

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryUseLocalhostClustering(null);
            });
            Assert.Equal("configuration", error.ParamName);
        }

        [Fact]
        public void TryUseLocalhostClustering_AppliesConfiguration()
        {
            // arrange
            var builder = new ClientBuilder();
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Orleans:Providers:Clustering:Provider", "Localhost" },
                    { "Orleans:Ports:Gateway:Start", "22222" },
                    { "Orleans:ServiceId", "SomeServiceId" },
                    { "Orleans:ClusterId", "SomeClusterId" }
                })
                .Build();

            // act
            builder.TryUseLocalhostClustering(config);

            // assert the cluster options are there
            var client = builder.Build();
            var clusterOptions = client.ServiceProvider.GetService<IOptions<ClusterOptions>>();
            Assert.NotNull(clusterOptions);
            Assert.Equal("SomeClusterId", clusterOptions.Value.ClusterId);
            Assert.Equal("SomeServiceId", clusterOptions.Value.ServiceId);

            // assert the static gateway options are there
            var providerOptions = client.ServiceProvider.GetService<IOptions<StaticGatewayListProviderOptions>>();
            Assert.Single(providerOptions.Value.Gateways);
            Assert.Equal(22222, providerOptions.Value.Gateways[0].Port);
        }

        [Fact]
        public void TryUseAdoNetClustering_RefusesNullBuilder()
        {
            IClientBuilder builder = null;

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryUseAdoNetClustering(new ConfigurationBuilder().Build());
            });
            Assert.Equal("builder", error.ParamName);
        }

        [Fact]
        public void TryUseAdoNetClustering_RefusesNullConfiguration()
        {
            var builder = new FakeClientBuilder();

            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                builder.TryUseAdoNetClustering(null);
            });
            Assert.Equal("configuration", error.ParamName);
        }
    }
}