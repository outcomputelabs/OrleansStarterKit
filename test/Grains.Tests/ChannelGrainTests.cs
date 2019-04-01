using Grains.Models;
using Orleans.TestingHost;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Grains.Tests
{
    [Collection(nameof(ClusterCollection))]
    public class ChannelGrainTests
    {
        private readonly ClusterFixture _fixture;

        public ChannelGrainTests(ClusterFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task SetInfoAsync_Saves_State()
        {
            // arrange
            var id = Guid.NewGuid();
            var handle = "SomeHandle";
            var description = "SomeDescription";
            var info = new ChannelInfo(id, handle, description);

            // act
            await _fixture.Cluster.GrainFactory
                .GetGrain<IChannelGrain>(id)
                .SetInfoAsync(info);

            // assert the state was saved
            var state = await _fixture.SiloServiceProvider.GetService<RegistryContext>()
                .Channels
                .FindAsync(id);

            Assert.NotNull(state);
            Assert.Equal(info.Id, state.Id);
        }

        [Fact]
        public async Task SetInfoAsync_Caches_State()
        {
            // arrange
            var id = Guid.NewGuid();
            var handle = "SomeHandle";
            var description = "SomeDescription";
            var info = new ChannelInfo(id, handle, description);

            // act
            await _fixture.Cluster.GrainFactory
                .GetGrain<IChannelGrain>(id)
                .SetInfoAsync(info);

            // assert the state is available
            var state = await _fixture.Cluster.GrainFactory
                .GetGrain<IChannelGrain>(id)
                .GetInfoAsync();

            Assert.NotNull(state);
            Assert.Equal(info.Id, state.Id);
        }

        [Fact]
        public async Task GetInfoAsync_Reads_State()
        {
            // arrange
            var id = Guid.NewGuid();
            var handle = "SomeHandle";
            var description = "SomeDescription";
            var info = new ChannelInfo(id, handle, description);

            var context = _fixture.SiloServiceProvider.GetService<RegistryContext>();
            context.Channels.Add(info);
            await context.SaveChangesAsync();

            // act
            var state = await _fixture.Cluster.GrainFactory
                .GetGrain<IChannelGrain>(id)
                .GetInfoAsync();

            // assert the state matches
            Assert.NotNull(state);
            Assert.Equal(info.Id, state.Id);
        }

        [Fact]
        public async Task TellAsync_Saves_Message()
        {
            // arrange
            var id = Guid.NewGuid();
            var senderId = Guid.NewGuid();
            var senderHandle = "SomeSender";
            var senderName = "Some Sender";
            var receiverId = Guid.NewGuid();
            var content = "Some Content";
            var timestamp = DateTime.Now;
            var message = new Message(id, senderId, senderHandle, senderName, receiverId, content, timestamp);

            await _fixture.Cluster.GrainFactory
                .GetGrain<IChannelGrain>(receiverId)
                .SetInfoAsync(new ChannelInfo(receiverId, "SomeReceiver", "Some Receiver"));

            // act
            await _fixture.Cluster.GrainFactory
                .GetGrain<IChannelGrain>(receiverId)
                .TellAsync(message);

            // assert the message was saved
            var state = await _fixture.SiloServiceProvider.GetService<RegistryContext>().Messages.FindAsync(id);
            Assert.NotNull(state);
            Assert.Equal(id, state.Id);
        }

        [Fact]
        public async Task TellAsync_Throws_On_Null_Message()
        {
            // arrage
            var id = Guid.NewGuid();
            var grain = _fixture.Cluster.GrainFactory.GetGrain<IChannelGrain>(id);
            await grain.SetInfoAsync(new ChannelInfo(id, "SomeHandle", "SomeDescription"));

            // act
            var error = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _fixture.Cluster.GrainFactory
                     .GetGrain<IChannelGrain>(id)
                     .TellAsync(null);
            });

            // assert
            Assert.Equal("message", error.ParamName);
        }

        [Fact]
        public async Task TellAsync_Throws_On_Misdirected_Message()
        {
            // arrage
            var id = Guid.NewGuid();
            var grain = _fixture.Cluster.GrainFactory.GetGrain<IChannelGrain>(id);
            await grain.SetInfoAsync(new ChannelInfo(id, "SomeHandle", "SomeDescription"));

            // act and assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _fixture.Cluster.GrainFactory
                     .GetGrain<IChannelGrain>(id)
                     .TellAsync(new Message(Guid.NewGuid(), Guid.NewGuid(), "SenderHandle", "SenderName", Guid.NewGuid(), "Content", DateTime.Now));
            });
        }
    }
}
