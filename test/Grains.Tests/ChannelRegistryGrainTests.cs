using Orleans.TestingHost;
using System.Threading.Tasks;
using Xunit;

namespace Grains.Tests
{
    [Collection(nameof(ClusterCollection))]
    public class ChannelRegistryGrainTests
    {
        public ChannelRegistryGrainTests(ClusterFixture context)
        {
            _cluster = context.Cluster;
        }

        private readonly TestCluster _cluster;

        [Fact]
        public async Task GetsDifferentChatRoomsForDifferentNames()
        {
            var room1 = await _cluster.GrainFactory.GetGrain<IChatRoomRegistryGrain>("ChannelOne").GetOrCreateChatRoomAsync();
            var room2 = await _cluster.GrainFactory.GetGrain<IChatRoomRegistryGrain>("ChannelTwo").GetOrCreateChatRoomAsync();

            Assert.NotNull(room1);
            Assert.NotNull(room2);
            Assert.NotEqual(room1, room2);
        }

        [Fact]
        public async Task GetsSameChatRoomForSameName()
        {
            var room1 = await _cluster.GrainFactory.GetGrain<IChatRoomRegistryGrain>("ChannelThree").GetOrCreateChatRoomAsync();
            var room2 = await _cluster.GrainFactory.GetGrain<IChatRoomRegistryGrain>("ChannelThree").GetOrCreateChatRoomAsync();

            Assert.NotNull(room1);
            Assert.NotNull(room2);
            Assert.Equal(room1, room2);
        }
    }
}
