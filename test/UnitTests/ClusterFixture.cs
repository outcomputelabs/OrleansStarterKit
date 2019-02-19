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
            Cluster = new TestClusterBuilder(3).Build();
            Cluster.Deploy();
        }

        public void Dispose()
        {
            Cluster.StopAllSilos();
        }
    }
}
