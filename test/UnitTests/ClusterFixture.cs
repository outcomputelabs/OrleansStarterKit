using Microsoft.Extensions.Configuration.Memory;
using Orleans.TestingHost;
using System;

namespace UnitTests
{
    public class ClusterFixture : IDisposable
    {
        public TestCluster Cluster { get; private set; }

        public ClusterFixture()
        {
            Cluster = new TestCluster(new TestClusterOptions
            {
                ClusterId = nameof(TestCluster),
                ServiceId = nameof(TestCluster),
                BaseSiloPort = 11111,
                BaseGatewayPort = 22222,
                InitializeClientOnDeploy = true,
                UseTestClusterMembership = true,
            },
            new[] { new MemoryConfigurationSource() });
            Cluster.Deploy();
        }

        public void Dispose()
        {
            Cluster.StopAllSilos();
        }
    }
}
