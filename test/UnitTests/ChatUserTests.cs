using Grains;
using Grains.Models;
using Orleans.TestingHost;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    [Collection(nameof(ClusterCollection))]
    public class ChatUserTests
    {
        public ChatUserTests(ClusterFixture context)
        {
            _cluster = context.Cluster;
        }

        private readonly TestCluster _cluster;

        [Fact]
        public async Task ChatUser_Stores_Message()
        {
            // arrange
            var grain = _cluster.GrainFactory.GetGrain<IChatUser>("A");
            var message = new ChatMessage("B", "A", "xpto");

            // act
            await grain.MessageAsync(message);
            var messages = await grain.GetMessagesAsync();

            // assert
            Assert.Single(messages);
            Assert.Same(message, messages[0]);
        }
    }
}
