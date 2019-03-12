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

        public ClusterFixture()
        {
            var builder = new TestClusterBuilder(3);
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
                        configure.AddApplicationPart(typeof(ChatUserGrain).Assembly).WithReferences();
                    })
                    .AddMemoryGrainStorageAsDefault();
            }
        }
    }
}
