using Orleans.TestingHost;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Grains.Tests
{
    [Collection(nameof(ClusterCollection))]
    public class ChannelRegistryTests
    {
        public ChannelRegistryTests(ClusterFixture context)
        {
            _cluster = context.Cluster;
        }

        private readonly TestCluster _cluster;

        [Fact]
        public async Task GetsDifferentKeyForDifferentNames()
        {
            var name1 = await _cluster.GrainFactory.GetGrain<IChannelRegistry>("ChannelOne").GetOrCreateKeyAsync();
            var name2 = await _cluster.GrainFactory.GetGrain<IChannelRegistry>("ChannelTwo").GetOrCreateKeyAsync();

            Assert.NotEqual(Guid.Empty, name1);
            Assert.NotEqual(Guid.Empty, name2);
            Assert.NotEqual(name1, name2);
        }

        [Fact]
        public async Task GetsSameKeyForSameName()
        {
            var name1 = await _cluster.GrainFactory.GetGrain<IChannelRegistry>("ChannelThree").GetOrCreateKeyAsync();
            var name2 = await _cluster.GrainFactory.GetGrain<IChannelRegistry>("ChannelThree").GetOrCreateKeyAsync();

            Assert.NotEqual(Guid.Empty, name1);
            Assert.NotEqual(Guid.Empty, name2);
            Assert.Equal(name1, name2);
        }
    }
}
