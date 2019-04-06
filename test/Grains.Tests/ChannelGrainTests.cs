using Grains.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

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

        [Fact]
        public async Task GetLatestMessagesAsync_Returns_Cached_Messages()
        {
            // arrange
            var key = Guid.NewGuid();
            var message1 = new Message(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), key, Guid.NewGuid().ToString(), DateTime.Now);
            var message2 = new Message(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), key, Guid.NewGuid().ToString(), DateTime.Now);
            var message3 = new Message(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), key, Guid.NewGuid().ToString(), DateTime.Now);
            var grain = _fixture.Cluster.GrainFactory.GetGrain<IChannelGrain>(key);
            await grain.SetInfoAsync(new ChannelInfo(key, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            // act
            await grain.TellAsync(message1);
            await grain.TellAsync(message2);
            await grain.TellAsync(message3);

            // assert
            var state = await grain.GetLatestMessagesAsync();
            Assert.NotNull(state);
            Assert.Collection(state,
                m => Assert.Equal(message1, m),
                m => Assert.Equal(message2, m),
                m => Assert.Equal(message3, m));
        }

        [Fact]
        public async Task AddUserAsync_Adds_Members()
        {
            // arrange
            var key = Guid.NewGuid();
            var member1 = new ChannelUser(key, Guid.NewGuid());
            var member2 = new ChannelUser(key, Guid.NewGuid());
            var member3 = new ChannelUser(key, Guid.NewGuid());
            var grain = _fixture.Cluster.GrainFactory.GetGrain<IChannelGrain>(key);

            // act
            await grain.AddUserAsync(member1);
            await grain.AddUserAsync(member2);
            await grain.AddUserAsync(member3);
            var state = await grain.GetUsersAsync();

            // assert
            Assert.Collection(state,
                m => Assert.Equal(member1, m),
                m => Assert.Equal(member2, m),
                m => Assert.Equal(member3, m));
        }

        [Fact]
        public async Task GetInfoAsync_Requires_Initialization()
        {
            // arrange
            var grain = _fixture.Cluster.GrainFactory.GetGrain<IChannelGrain>(Guid.NewGuid());

            // act and assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await grain.GetInfoAsync();
            });
        }

        [Fact]
        public async Task SetInfoAsync_Validates_Input()
        {
            // arrange
            var grain = _fixture.Cluster.GrainFactory.GetGrain<IChannelGrain>(Guid.NewGuid());
            var info = new ChannelInfo(Guid.NewGuid(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            // act and assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await grain.SetInfoAsync(info);
            });
        }
    }
}
