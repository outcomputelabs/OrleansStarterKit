using Grains;
using Grains.Models;
using Orleans.TestingHost;
using System;
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
        public async Task ChatUser_Stores_And_Retrieves_Message()
        {
            // arrange
            var grain = _cluster.GrainFactory.GetGrain<IChatUser>("A");
            var message = new ChatMessage(Guid.NewGuid(), "B", "A", "xpto", DateTime.UtcNow);

            // act
            await grain.MessageAsync(message);
            var messages = await grain.GetMessagesAsync();

            // assert
            Assert.Single(messages);
            Assert.Equal(message.Id, messages[0].Id);
            Assert.Equal(message.FromUserId, messages[0].FromUserId);
            Assert.Equal(message.ToUserId, messages[0].ToUserId);
            Assert.Equal(message.Content, messages[0].Content);
            Assert.Equal(message.Timestamp, messages[0].Timestamp);
        }
    }
}
