using Grains.Models;
using Orleans.TestingHost;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Grains.Tests
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
        public async Task StoresAndRetrievesMessage()
        {
            // arrange
            var grain = _cluster.GrainFactory.GetGrain<IChatUserGrain>(Guid.NewGuid());
            var message = new ChatMessage(Guid.NewGuid(), Guid.NewGuid(), "xpto", DateTime.UtcNow);

            // act
            await grain.MessageAsync(message);
            var messages = await grain.GetMessagesAsync();

            // assert
            Assert.Single(messages);
            Assert.Equal(message.Id, messages[0].Id);
            Assert.Equal(message.PublisherId, messages[0].PublisherId);
            Assert.Equal(message.Content, messages[0].Content);
            Assert.Equal(message.Timestamp, messages[0].Timestamp);
        }
    }
}
