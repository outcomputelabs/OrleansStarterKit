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
            var id = Guid.NewGuid();
            var timestamp = DateTime.UtcNow;

            // act
            var message = new ChatMessage(id, "User1", "SomeContent", timestamp);

            // assert
            Assert.Equal(id, message.Id);
            Assert.Equal("User1", message.PublisherId);
            Assert.Equal("SomeContent", message.Content);
            Assert.Equal(timestamp, message.Timestamp);
        }
    }
}
