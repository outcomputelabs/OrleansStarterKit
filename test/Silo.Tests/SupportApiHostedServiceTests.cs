using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Orleans;
using Silo.Options;
using Silo.Tests.Fakes;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Silo.Tests
{
    public class SupportApiHostedServiceTests
    {
        [Fact]
        public void Uses_Kestrel()
        {
            // arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Api:Port:Start", "55555" },
                    { "Api:Port:End", "55555" }
                })
                .Build();

            // act
            var api = new SupportApiHostedService(
                config,
                new FakeLoggerProvider(),
                new FakeClusterClient(),
                new FakeNetworkPortFinder());

            // assert - white box
            var host = api.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(api) as IWebHost;
            Assert.NotNull(host);

            // assert kestrel is there
            Assert.NotNull(host.Services.GetService<IOptions<KestrelServerOptions>>());
        }

        [Fact]
        public void Uses_Mvc()
        {
            // arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                })
                .Build();

            // act
            var api = new SupportApiHostedService(
                config,
                new FakeLoggerProvider(),
                new FakeClusterClient(),
                new FakeNetworkPortFinder());

            // assert - white box
            var host = api.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(api) as IWebHost;
            Assert.NotNull(host);

            // assert mvc is there
            Assert.NotNull(host.Services.GetService<IOptions<MvcOptions>>());
        }

        [Fact]
        public void Uses_ApiVersioning()
        {
            // arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                })
                .Build();

            // act
            var api = new SupportApiHostedService(
                config,
                new FakeLoggerProvider(),
                new FakeClusterClient(),
                new FakeNetworkPortFinder());

            // assert - white box
            var host = api.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(api) as IWebHost;
            Assert.NotNull(host);

            // assert api versioning is there
            Assert.NotNull(host.Services.GetService<IOptions<ApiVersioningOptions>>());
        }

        [Fact]
        public void Uses_ApiExplorer()
        {
            // arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                })
                .Build();

            // act
            var api = new SupportApiHostedService(
                config,
                new FakeLoggerProvider(),
                new FakeClusterClient(),
                new FakeNetworkPortFinder());

            // assert - white box
            var host = api.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(api) as IWebHost;
            Assert.NotNull(host);

            // assert the api explorer is there
            Assert.NotNull(host.Services.GetService<IOptions<ApiExplorerOptions>>());
        }

        [Fact]
        public void Uses_SwaggerGenerator()
        {
            // arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                })
                .Build();

            // act
            var api = new SupportApiHostedService(
                config,
                new FakeLoggerProvider(),
                new FakeClusterClient(),
                new FakeNetworkPortFinder());

            // assert - white box
            var host = api.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(api) as IWebHost;
            Assert.NotNull(host);

            // assert the swagger generator is there
            Assert.NotNull(host.Services.GetService<IOptions<SwaggerGenOptions>>());
        }

        [Fact]
        public void Uses_Swagger()
        {
            // arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                })
                .Build();

            // act
            var api = new SupportApiHostedService(
                config,
                new FakeLoggerProvider(),
                new FakeClusterClient(),
                new FakeNetworkPortFinder());

            // assert - white box
            var host = api.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(api) as IWebHost;
            Assert.NotNull(host);

            // assert swagger is there
            Assert.NotNull(host.Services.GetService<IOptions<SwaggerOptions>>());
        }

        [Fact]
        public void Uses_SwaggerUI()
        {
            // arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                })
                .Build();

            // act
            var api = new SupportApiHostedService(
                config,
                new FakeLoggerProvider(),
                new FakeClusterClient(),
                new FakeNetworkPortFinder());

            // assert - white box
            var host = api.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(api) as IWebHost;
            Assert.NotNull(host);

            // assert swagger ui is there
            Assert.NotNull(host.Services.GetService<IOptions<SwaggerUIOptions>>());
        }

        [Fact]
        public void Uses_LoggerProvider()
        {
            // arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                })
                .Build();
            var loggerProvider = new FakeLoggerProvider();

            // act
            var api = new SupportApiHostedService(
                config,
                loggerProvider,
                new FakeClusterClient(),
                new FakeNetworkPortFinder());

            // assert - white box
            var host = api.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(api) as IWebHost;
            Assert.NotNull(host);

            // assert logger provider is there
            Assert.Same(loggerProvider, host.Services.GetService<ILoggerProvider>());
        }

        [Fact]
        public void Uses_ClusterClient()
        {
            // arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                })
                .Build();
            var client = new FakeClusterClient();

            // act
            var api = new SupportApiHostedService(
                config,
                new FakeLoggerProvider(),
                client,
                new FakeNetworkPortFinder());

            // assert - white box
            var host = api.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(api) as IWebHost;
            Assert.NotNull(host);

            // assert the cluster client is there
            Assert.Same(client, host.Services.GetService<IClusterClient>());
        }

        [Fact]
        public void Uses_ApiOptions()
        {
            // arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Api:Title", "SomeTitle" }
                })
                .Build();

            // act
            var api = new SupportApiHostedService(
                config,
                new FakeLoggerProvider(),
                new FakeClusterClient(),
                new FakeNetworkPortFinder());

            // assert - white box
            var host = api.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(api) as IWebHost;
            Assert.NotNull(host);

            // assert the cluster client is there
            var options = host.Services.GetService<IOptions<SupportApiOptions>>();
            Assert.Equal("SomeTitle", options.Value.Title);
        }

        [Fact]
        public async Task Starts_And_Stops()
        {
            // arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                })
                .Build();

            // act
            var api = new SupportApiHostedService(
                config,
                new FakeLoggerProvider(),
                new FakeClusterClient(),
                new FakeNetworkPortFinder());

            // assert - white box
            var host = api.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(api) as IWebHost;
            Assert.NotNull(host);

            // assert the service starts
            await api.StartAsync(new CancellationToken());

            // assert the api stops
            await api.StopAsync(new CancellationToken());

            // if it did not crash yet we are good
            Assert.True(true);
        }

        [Fact]
        public void SupportApiHostedService_Refuses_Null_Configuration()
        {
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                new SupportApiHostedService(null, new FakeLoggerProvider(), new FakeClusterClient(), new FakeNetworkPortFinder());
            });
            Assert.Equal("configuration", error.ParamName);
        }

        [Fact]
        public void SupportApiHostedService_Refuses_Null_LoggerProvider()
        {
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                new SupportApiHostedService(new Mock<IConfiguration>().Object, null, new FakeClusterClient(), new FakeNetworkPortFinder());
            });
            Assert.Equal("loggerProvider", error.ParamName);
        }

        [Fact]
        public void SupportApiHostedService_Refuses_Null_ClusterClient()
        {
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                new SupportApiHostedService(new Mock<IConfiguration>().Object, new FakeLoggerProvider(), null, new FakeNetworkPortFinder());
            });
            Assert.Equal("client", error.ParamName);
        }

        [Fact]
        public void SupportApiHostedService_Refuses_Null_PortFinder()
        {
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                new SupportApiHostedService(new Mock<IConfiguration>().Object, new FakeLoggerProvider(), new FakeClusterClient(), null);
            });
            Assert.Equal("portFinder", error.ParamName);
        }
    }
}
