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
using System.Reflection;
using UnitTests.Fakes;
using Xunit;

namespace UnitTests
{
    public class SupportApiHostedServiceTests
    {
        [Fact]
        public void SupportApiHostedService_Starts_And_Stops()
        {
            // arrange
            var options = new FakeSupportApiOptions();
            var loggerProvider = new FakeLoggerProvider();
            var client = new FakeClusterClient();
            var portFinder = new FakeNetworkPortFinder();

            // act
            var api = new SupportApiHostedService(options, loggerProvider, client, portFinder);

            // assert - white box
            var host = api.GetType().GetField("_host", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(api) as IWebHost;
            Assert.NotNull(host);

            // assert kestrel is there
            Assert.NotNull(host.Services.GetService<IOptions<KestrelServerOptions>>());

            // assert mvc is there
            Assert.NotNull(host.Services.GetService<IOptions<MvcOptions>>());

            // assert api versioning is there
            Assert.NotNull(host.Services.GetService<IOptions<ApiVersioningOptions>>());

            // assert the api explorer is there
            Assert.NotNull(host.Services.GetService<IOptions<ApiExplorerOptions>>());

            // assert the swagger generator is there
            Assert.NotNull(host.Services.GetService<IOptions<SwaggerGenOptions>>());

            // assert swagger is there
            Assert.NotNull(host.Services.GetService<IOptions<SwaggerOptions>>());

            // assert swagger ui is there
            Assert.NotNull(host.Services.GetService<IOptions<SwaggerUIOptions>>());

            // assert logger provider is there
            Assert.Same(loggerProvider, host.Services.GetService<ILoggerProvider>());

            // assert the cluster client is there
            Assert.Same(client, host.Services.GetService<IClusterClient>());
        }
    }
}
