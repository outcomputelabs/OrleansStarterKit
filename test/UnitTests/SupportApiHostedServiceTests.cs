using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using Silo;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnitTests.Fakes;
using Xunit;

namespace UnitTests
{
    public class SupportApiHostedServiceTests
    {
        [Fact]
        public void UsesKestrel()
        {
            // act
            var api = new SupportApiHostedService(
                new FakeSupportApiOptions(),
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
            // act
            var api = new SupportApiHostedService(
                new FakeSupportApiOptions(),
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
            // act
            var api = new SupportApiHostedService(
                new FakeSupportApiOptions(),
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
            // act
            var api = new SupportApiHostedService(
                new FakeSupportApiOptions(),
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
            // act
            var api = new SupportApiHostedService(
                new FakeSupportApiOptions(),
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
            // act
            var api = new SupportApiHostedService(
                new FakeSupportApiOptions(),
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
            // act
            var api = new SupportApiHostedService(
                new FakeSupportApiOptions(),
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
            // act
            var loggerProvider = new FakeLoggerProvider();
            var api = new SupportApiHostedService(
                new FakeSupportApiOptions(),
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
            // act
            var client = new FakeClusterClient();
            var api = new SupportApiHostedService(
                new FakeSupportApiOptions(),
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
        public async Task Starts_And_Stops()
        {
            // act
            var api = new SupportApiHostedService(
                new FakeSupportApiOptions(),
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
        public void SupportApiHostedService_Refuses_Null_Options()
        {
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                new SupportApiHostedService(null, new FakeLoggerProvider(), new FakeClusterClient(), new FakeNetworkPortFinder());
            });
            Assert.Equal("options", error.ParamName);
        }

        [Fact]
        public void SupportApiHostedService_Refuses_Null_Options_Value()
        {
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                new SupportApiHostedService(new FakeSupportApiOptions()
                {
                    Value = null
                }, new FakeLoggerProvider(), new FakeClusterClient(), new FakeNetworkPortFinder());
            });
            Assert.Equal("options", error.ParamName);
        }

        [Fact]
        public void SupportApiHostedService_Refuses_Null_LoggerProvider()
        {
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                new SupportApiHostedService(new FakeSupportApiOptions(), null, new FakeClusterClient(), new FakeNetworkPortFinder());
            });
            Assert.Equal("loggerProvider", error.ParamName);
        }

        [Fact]
        public void SupportApiHostedService_Refuses_Null_ClusterClient()
        {
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                new SupportApiHostedService(new FakeSupportApiOptions(), new FakeLoggerProvider(), null, new FakeNetworkPortFinder());
            });
            Assert.Equal("client", error.ParamName);
        }

        [Fact]
        public void SupportApiHostedService_Refuses_Null_PortFinder()
        {
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                new SupportApiHostedService(new FakeSupportApiOptions(), new FakeLoggerProvider(), new FakeClusterClient(), null);
            });
            Assert.Equal("portFinder", error.ParamName);
        }
    }
}
