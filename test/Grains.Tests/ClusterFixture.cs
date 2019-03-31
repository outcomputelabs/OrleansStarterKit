using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.TestingHost;
using System;

namespace Grains.Tests
{
    public class ClusterFixture : IDisposable
    {
        public TestCluster Cluster { get; private set; }

        public static IServiceProvider StaticSiloServiceProvider { get; private set; }
        public IServiceProvider SiloServiceProvider => StaticSiloServiceProvider;

        private static readonly InMemoryDatabaseRoot _registryContextRoot = new InMemoryDatabaseRoot();

        public ClusterFixture()
        {
            var builder = new TestClusterBuilder();
            builder.AddSiloBuilderConfigurator<TestSiloBuilderConfigurator>();
            var cluster = builder.Build();
            cluster.Deploy();

            Cluster = cluster;
        }

        public void Dispose()
        {
            Cluster.StopAllSilos();
        }

        public class TestSiloBuilderConfigurator : ISiloBuilderConfigurator
        {
            public void Configure(ISiloHostBuilder hostBuilder)
            {
                hostBuilder
                    .Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = nameof(TestCluster);
                        options.ServiceId = nameof(TestCluster);
                    })
                    .ConfigureApplicationParts(configure =>
                    {
                        configure.AddApplicationPart(typeof(UserGrain).Assembly).WithReferences();
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddDbContext<RegistryContext>(options =>
                        {
                            options.UseInMemoryDatabase(nameof(TestCluster), _registryContextRoot);
                        });
                        services.AddSingleton<Func<RegistryContext>>(_ => () => _.GetService<RegistryContext>());
                    })
                    .UseServiceProviderFactory(services =>
                    {
                        var provider = services.BuildServiceProvider();
                        StaticSiloServiceProvider = provider;
                        return provider;
                    })
                    .AddMemoryGrainStorageAsDefault();
            }
        }
    }
}
