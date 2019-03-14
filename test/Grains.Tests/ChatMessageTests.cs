using Grains.Models;
using System;
using Xunit;

namespace Grains.Tests
{
    public class ChatMessageTests
    {
        [Fact]
        public void HoldsData()
        {
            // arrange
            var publisherId = Guid.NewGuid();
            var messageId = Guid.NewGuid();
            var timestamp = DateTime.UtcNow;

            // act
            var message = new ChatMessage(messageId, publisherId, "SomeContent", timestamp);

            // assert
            Assert.Equal(messageId, message.Id);
            Assert.Equal(publisherId, message.PublisherId);
            Assert.Equal("SomeContent", message.Content);
            Assert.Equal(timestamp, message.Timestamp);
        }
    }
}
